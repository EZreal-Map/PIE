﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using SZRiverSys.Model;

namespace SZRiverSys.ServiceHandler
{
    /// <summary>
    /// XEventHandler 的摘要说明
    /// </summary>
    public class XEventHandler : IHttpHandler
    {

        private void sendResponse(HttpContext context, string callbackFun, string result)
        {
            if (callbackFun != null)
                result = callbackFun + "(" + result + ")";
            context.Response.Write(result);
            context.Response.End();
            context.Response.Close();
        }

        public void ProcessRequest(HttpContext context)
        {
            string strtokens = context.Request.Params["token"];
            string strFun = context.Request["jsonpcallback"];//传递参数

            if (strtokens == null || strtokens.Trim() == "" || !CacheHelper.CacheHelper.IsContain(strtokens))
            {
                string msg = "{\"code\":-1,\"success\":false,\"msg\":\"用户登录失效,请重新登录\"}";
                sendResponse(context, strFun, msg);
                return;
            }
            string method = context.Request.Params["method"];
            if (method == null || method.Trim() == "")
            {
                string msg = "{\"code\":-1,\"success\":false,\"msg\":\"method不能为空\"}";
                sendResponse(context, strFun, msg);
                return;
            }
            Model.UserModel tmpUser = CacheHelper.CacheHelper.Get(strtokens) as Model.UserModel;
            method = method.ToLower();

            #region 上传事件
            if (method == "uploadevent")// 上传事件
            {
                string tmpPrjID = context.Request.Params["projectid"];
                if (tmpPrjID == null || tmpPrjID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpsectionID = context.Request.Params["sectionid"];
                if (tmpsectionID == null || tmpsectionID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"sectionid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpName = context.Request.Params["name"];
                string tmpType = context.Request.Params["eventtype"];          
                string tmpEmergencyType = context.Request.Params["emergencytype"];
                string tmpContent = context.Request.Params["content"];

                string tmpX = context.Request.Params["x"];
                string tmpY = context.Request.Params["y"];
                string tmpZ = context.Request.Params["z"];
                string tmpfiles = context.Request.Params["filesinfo"];
                if (tmpX == null) tmpX = "0";
                if (tmpY == null) tmpY = "0";
                if (tmpZ == null) tmpZ = "0";
                if (tmpfiles == null) tmpfiles = "";

                if (XJobHandler.CheckIsExistNullParam(new string[] { tmpName, tmpType, tmpEmergencyType, tmpContent }))
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"必要字段缺失\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                ProjectInfoClass tmpPrjInfo = ProjectInfoClass.GetProjectInfo(tmpPrjID);
                if (tmpPrjInfo != null)
                {
                    string tmpSysID = Guid.NewGuid().ToString("N");
                    string tmpSql = string.Format("Insert Into X_EventsInfo(SYSID,BelongSectionID,EventName,EventType,UploadUser,UploadTime,AssignUser,EmergencyType,Content,FilesInfo,X,Y,Z) "
                            + "Values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',{10},{11},{12})",
                            tmpSysID, tmpsectionID, tmpName, tmpType, tmpUser.usercode,
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), tmpPrjInfo.ProjectManager, tmpEmergencyType, tmpContent, tmpfiles, tmpX, tmpY, tmpZ);
                    if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql) > 0)
                    {
                        XProjectHandler.AddLogInfo(tmpsectionID, tmpUser.usercode, "事件_添加", tmpSysID);
                        string msg = "{\"code\":0,\"success\":true,\"msg\":\"" + tmpSysID + "\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"创建工单失败.\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该项目不存在\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion
            #region 查询事件
            else if (method == "queryeventsbytime1")// 查询事件
            {
                string tmprojectid = context.Request.Params["projectid"];
                if (tmprojectid == null || tmprojectid.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpStartTime = context.Request.Params["starttime"];
                string tmpEndTime = context.Request.Params["endtime"];                
                if (XJobHandler.CheckIsExistNullParam(new string[] { tmpStartTime, tmpEndTime }))
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"必要字段缺失\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                ProjectInfoClass tmpprjInfo = Model.ProjectInfoClass.GetProjectInfo(tmprojectid);
                if (tmpprjInfo != null)
                {
                    string tmpSql = "Select SYSID,BelongSectionID,EventName,EventType,UploadUser,UploadUser as UploadUserName,UploadTime,AssignUser,AssignUser as AssignUserName,EmergencyType,Content,FilesInfo,X,Y,Z,IsRead,ReadTime"
                        + " From X_EventsInfo Where (AssignUser='" + tmpUser.usercode + "' OR UploadUser = '" + tmpUser.usercode + "') AND BelongSectionID like '" + tmprojectid + "%' AND UploadTime>'" + tmpStartTime + "' AND UploadTime<'" + tmpEndTime + "'";
                    DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                    if (table != null && table.Rows.Count > 0)
                    {
                        SZRiverSys.Comm.CommonClass.ReplaceTableColumnsName(table, new string[] { "UploadUserName", "AssignUserName" });//替换用户名为昵称
                        Dictionary<string, string> tmpDict = new Dictionary<string, string>();
                        tmpDict.Add("0", "未读");
                        tmpDict.Add("1", "已读");
                        SZRiverSys.Comm.CommonClass.ReplaceTableColumnsValue(table, "IsRead", "IsReadStr", tmpDict);
                        JSON.JsonHelper Json = new JSON.JsonHelper();
                        string json = Json.DataTableToJsonForLayUi(table);
                        string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该项目指定时间范围内没有需要查询的事件\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该项目不存在\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            else if (method == "queryeventsbytime")// 查询事件
            {
                string tmprojectid = context.Request.Params["projectid"];
                if (tmprojectid == null || tmprojectid.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpStartTime = context.Request.Params["starttime"];
                string tmpEndTime = context.Request.Params["endtime"];
                string eventType = context.Request.Params["eventtype"];
                int pageIndex = Convert.ToInt32(context.Request.Params["pageindex"]);
                int pageSize = Convert.ToInt32(context.Request.Params["pagesize"]);
                string riverBelongSectionID = context.Request.Params["riverid"];

                string sql = "";
                if (eventType == "" || eventType == null)
                {
                    sql = "1=1";
                }
                else
                {
                    sql = "EventType = '" + eventType + "'";
                }

                if (XJobHandler.CheckIsExistNullParam(new string[] { tmpStartTime, tmpEndTime }))
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"必要字段缺失\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                ProjectInfoClass tmpprjInfo = Model.ProjectInfoClass.GetProjectInfo(tmprojectid);
                if (tmpprjInfo != null)
                {
                    string tmpUserCondition = "";
                    string tmpSql = "SELECT ID FROM [dbo].[X_PartsInfo] Where charindex(PartID,'" + tmprojectid + "')>0 AND (Manager='" + tmpUser.usercode + "' OR UsersList like '\"" + tmpUser.usercode + "\"')";
                    DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                    if (table != null && table.Rows.Count > 0)
                        tmpUserCondition = " 1=1 ";
                    else
                        tmpUserCondition = "(AssignUser='" + tmpUser.usercode + "' OR UploadUser ='" + tmpUser.usercode + "')";

                    //根据河道查询
                    tmprojectid = riverBelongSectionID == "" || riverBelongSectionID == null ? tmprojectid : riverBelongSectionID;

                    //查询事件总数
                    tmpSql = "Select count(sysid) num"
                       + " From X_EventsInfo Where " + tmpUserCondition + " AND BelongSectionID like '" + tmprojectid + "%' AND UploadTime>'" + tmpStartTime + "' AND UploadTime<'" + tmpEndTime + "' AND " + sql + "";
                    table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                    int count = Convert.ToInt32(table.Rows[0]["num"].ToString());

                    tmpSql = "Select  TOP " + pageSize + " SYSID,BelongSectionID,EventName,EventType,UploadUser,UploadUser as UploadUserName,UploadTime,AssignUser,AssignUser as AssignUserName,EmergencyType,Content,FilesInfo,X,Y,Z,IsRead,ReadTime"
                        + " From X_EventsInfo Where " + tmpUserCondition + " AND BelongSectionID like '" + tmprojectid + "%' AND UploadTime>'" + tmpStartTime + "' AND UploadTime<'" + tmpEndTime + "' AND " + sql + " AND ID not in " +
                        "(select top ((" + pageIndex + "-1)*" + pageSize + ") ID from X_EventsInfo  Where " + tmpUserCondition + " AND BelongSectionID like '" + tmprojectid + "%' AND UploadTime>'" + tmpStartTime + "' AND UploadTime<'" + tmpEndTime + "' AND " + sql + ")";
                    table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                    if (table != null && table.Rows.Count > 0)
                    {
                        SZRiverSys.Comm.CommonClass.ReplaceTableRiverName(table, "BelongSectionID");//替换河道名称
                        SZRiverSys.Comm.CommonClass.ReplaceTableColumnsName(table, new string[] { "UploadUserName", "AssignUserName" });//替换用户名为昵称
                        Dictionary<string, string> tmpDict = new Dictionary<string, string>();
                        tmpDict.Add("0", "未读");
                        tmpDict.Add("1", "已读");
                        SZRiverSys.Comm.CommonClass.ReplaceTableColumnsValue(table, "IsRead", "IsReadStr", tmpDict);
                        JSON.JsonHelper Json = new JSON.JsonHelper();

                        string json = Json.DataTableToJsonForLayUi(table);
                        string msg = "{\"code\":0,\"success\":true,\"count\":" + count + ",\"data\":" + json + "}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该项目指定时间范围内没有需要查询的事件\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该项目不存在\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            else if (method == "openeventinfo")// 查看事件信息
            {
                string tmpSYSID = context.Request.Params["sysid"];
                if (tmpSYSID == null || tmpSYSID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"sysid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSql = "Select SYSID,BelongSectionID,EventName,EventType,UploadUser,UploadUser as UploadUserName,UploadTime,AssignUser,AssignUser as AssignUserName,EmergencyType,Content,FilesInfo,X,Y,Z,IsRead,ReadTime"
                         + " From X_EventsInfo Where SYSID='" + tmpSYSID + "'";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    SZRiverSys.Comm.CommonClass.ReplaceTableRiverName(table, "BelongSectionID");//替换河道名称
                    SZRiverSys.Comm.CommonClass.ReplaceTableColumnsName(table, new string[] { "UploadUserName", "AssignUserName" });//替换用户名为昵称
                    if(table.Rows[0].ItemArray[7].ToString()==tmpUser.usercode)//如果查看用户是指派人就更新读状态
                    {
                        tmpSql = "Update X_EventsInfo Set IsRead = 1,ReadTime='"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' Where SYSID='" + tmpSYSID + "'";
                        DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql);
                    }
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"没有该事件的信息\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            else if (method == "queryunreadeventscount")//查询本人未读事件数量
            {
                string tmprojectid = context.Request.Params["projectid"];
                if (tmprojectid == null || tmprojectid.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                ProjectInfoClass tmpprjInfo = Model.ProjectInfoClass.GetProjectInfo(tmprojectid);
                if (tmpprjInfo != null)
                {
                    string tmpSql = "Select COUNT(*)"
                        + " From X_EventsInfo Where IsRead = 0 AND AssignUser='" + tmpUser.usercode + "' AND BelongSectionID like '" + tmprojectid + "%'";
                    DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                    if (table != null && table.Rows.Count > 0)
                    {
                        string msg = "{\"code\":0,\"success\":true,\"data\":" + table.Rows[0].ItemArray[0].ToString()+ "}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该项目指定时间范围内没有需要查询的事件\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该项目不存在\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            else if (method == "queryunreadeventsinfo")// 查询本人未读事件信息
            {
                string tmprojectid = context.Request.Params["projectid"];
                if (tmprojectid == null || tmprojectid.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                ProjectInfoClass tmpprjInfo = Model.ProjectInfoClass.GetProjectInfo(tmprojectid);
                if (tmpprjInfo != null)
                {
                    string tmpSql = "Select SYSID,BelongSectionID,EventName,EventType,UploadUser,UploadUser as UploadUserName,UploadTime,AssignUser,AssignUser as AssignUserName,EmergencyType,Content,FilesInfo,X,Y,Z,IsRead,ReadTime"
                        + " From X_EventsInfo Where IsRead = 0 AND AssignUser='" + tmpUser.usercode + "' AND BelongSectionID like '" + tmprojectid + "%'";
                    DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                    if (table != null && table.Rows.Count > 0)
                    {
                        SZRiverSys.Comm.CommonClass.ReplaceTableColumnsName(table, new string[] { "UploadUserName", "AssignUserName" });//替换用户名为昵称
                        JSON.JsonHelper Json = new JSON.JsonHelper();
                        string json = Json.DataTableToJsonForLayUi(table);
                        string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该项目指定时间范围内没有需要查询的事件\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该项目不存在\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion
            #region 文件处理
            else if (method == "uploadfile")// 文件上传
            {
                string tmpprojectID = context.Request.Params["projectid"];
                if (tmpprojectID == null || tmpprojectID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }               
                {
                    HttpFileCollection hfc = context.Request.Files;
                    HttpPostedFile hpf = null;
                    string FileNames = "";
                    bool errormsg = true;
                    string errormsgstr = "";
                    if (hfc.Count > 0)
                    {
                        string tmpSaveFolder = HttpContext.Current.Server.MapPath("~/Upfile/Events");
                        if (!Directory.Exists(tmpSaveFolder))
                            Directory.CreateDirectory(tmpSaveFolder);

                        for (int i = 0; i < hfc.Count; i++)
                        {
                            hpf = context.Request.Files[i];
                            string date = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                            var filetype = hpf.FileName.Split('.')[1];
                            var filename = tmpprojectID + "_" + tmpUser.usercode + "-" + date + "." + filetype;
                            var filepath = Path.Combine(tmpSaveFolder, filename);
                            string directoryName = Path.GetDirectoryName(filepath);
                            if (!Directory.Exists(directoryName))
                            {
                                Directory.CreateDirectory(directoryName);
                            }
                            try
                            {
                                hpf.SaveAs(filepath);
                            }
                            catch (Exception ex)
                            {
                                errormsg = false;
                                errormsgstr = ex.Message;
                            }
                            FileNames += filename + ";";
                        }
                    }
                    if (errormsg)
                    {
                        string msg = "{\"code\":0,\"success\":true,\"msg\":\"上传成功\",\"FileName\":\"" + FileNames + "\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"上传失败\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                }
            }
            else if (method == "downloadfile")// 下载文件
            {
                string filename = context.Request["filepath"];
                var filelist = filename.TrimEnd(';').Split(';');
                List<string> filepath = new List<string>();
                for (int i = 0; i < filelist.Length; i++)
                {
                    string tmpFileID = filelist[i];
                    if (tmpFileID.Trim() != "")
                        filepath.Add("Upfile/Events/" + tmpFileID);
                }
                string msg = "{\"code\":0,\"success\":true,\"filepath\":\"" + string.Join(";", filepath) + "\"}";
                sendResponse(context, strFun, msg);
                return;
            }
            #endregion
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}