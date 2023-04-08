using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using SZRiverSys.Model;

namespace SZRiverSys.ServiceHandler
{
    /// <summary>
    /// XJobHandler 的摘要说明
    /// </summary>
    public class XJobHandler : IHttpHandler
    {

        private void sendResponse(HttpContext context, string callbackFun, string result)
        {
            if (callbackFun != null)
                result = callbackFun + "(" + result + ")";
            context.Response.Write(result);
            context.Response.End();
            context.Response.Close();
        }

        /// <summary>
        /// 检查是否有null参数
        /// </summary>
        /// <param name="Params"></param>
        /// <returns></returns>
       public static bool CheckIsExistNullParam(string[] Params)
        {
            foreach (string tmpP in Params)
                if (tmpP == null)
                    return true;
            return false;
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

            #region 添加工单
            if (method == "addjob")// 添加工单
            {
                string tmpsectionID = context.Request.Params["sectionid"];
                if (tmpsectionID == null || tmpsectionID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"sectionid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpName = context.Request.Params["name"];
                string tmpType = context.Request.Params["jobtype"];
                string tmpmanager = context.Request.Params["assignuser"];//指定工头                
                string tmpStartTime = context.Request.Params["starttime"];
                string tmpEndTime = context.Request.Params["endtime"];

                string tmpPlanWorkload = context.Request.Params["planworkload"]; //计划工作量
                string tmpContent = context.Request.Params["content"];
                ///
                string tmpUsersList = context.Request.Params["associateusers"];//参与人员

                string jobfrom = context.Request.Params["jobfrom"];//工单来源

                //文件,照片
                HttpFileCollection hfc = context.Request.Files;
                HttpPostedFile hpf = null;
                //if (hfc.Count > 0)
                //{
                //    hpf = context.Request.Files[0];
                //}                       
                string FileNames = "";
                bool errormsg = true;
                string errormsgstr = "";
                if (hfc.Count > 0)
                {
                    //string tmpMonthStr = DateTime.Now.ToString("yyyyMM");
                    string tmpSaveFolder = HttpContext.Current.Server.MapPath("~/Upfile/Jobs");
                    //if (!Directory.Exists(tmpSaveFolder))
                    //    Directory.CreateDirectory(tmpSaveFolder);

                    for (int i = 0; i < hfc.Count; i++)
                    {
                        hpf = context.Request.Files[i];
                        string date = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        var filetype = hpf.FileName.Split('.')[1];
                        //var filename = tmpSectionID+"_"+tmpUser.usercode + "-" + date + "." + filetype;
                        var filename = tmpsectionID + "_" + tmpUser.usercode + "-" + date + "." + filetype;
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


                if (tmpUsersList.Length > 0)
                {
                    if (tmpUsersList[0] != ',')
                        tmpUsersList = "," + tmpUsersList;
                    if (!tmpUsersList.EndsWith("'"))
                        tmpUsersList = tmpUsersList + ",";
                }
                if (CheckIsExistNullParam(new string[] { tmpName, tmpType, tmpmanager , tmpStartTime , tmpEndTime}))
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"必要字段缺失\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }

                //SectionClass tmpSectionClass =  Model.SectionClass.GetSection(tmpsectionID);
                //if(tmpSectionClass != null)
                {
                    string tmpPrjID = tmpsectionID.Substring(0, 12);
                    ProjectInfoClass tmpPrjInfo = ProjectInfoClass.GetProjectInfo(tmpPrjID);
                    if(tmpPrjInfo!=null)
                    {
                        if (tmpPrjInfo.ProjectManager == tmpUser.usercode || tmpPrjInfo.ManagerUserList.Contains(","+ tmpUser.usercode+","))//只有项目负责人与管理层才能创建工单
                        {
                            string tmpSYSID = "";
                            if (CreateJob(tmpsectionID, tmpName, tmpType, tmpUser.usercode, tmpmanager,
                                tmpUsersList, tmpStartTime, tmpEndTime, tmpContent, tmpPlanWorkload, out tmpSYSID, FileNames,jobfrom))
                            {
                                XProjectHandler.AddLogInfo(tmpsectionID, tmpUser.usercode, "工单_添加", tmpSYSID);
                                string msg = "{\"code\":0,\"success\":true,\"msg\":\"" + tmpSYSID + "\"}";
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
                            string msg = "{\"code\":-1,\"success\":false,\"msg\":\"当前用户不是该项目的负责人或管理任务,不能创建工单.\"}";
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
                //else{
                //    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该项目工作范围不存在\"}";
                //    sendResponse(context, strFun, msg);
                //    return;
                //}
            }
            #endregion
            #region 查询工单
            else if (method == "queryalljobsbytime1")// 查询本人所有的工单
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

                if (CheckIsExistNullParam(new string[] { tmpStartTime, tmpEndTime }))
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"必要字段缺失\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                ProjectInfoClass tmpprjInfo = Model.ProjectInfoClass.GetProjectInfo(tmprojectid);
                if (tmpprjInfo != null)
                {
                    //SYSID,BelongSectionID,JobName,JobType,Creator,CreateTime,AssignUser,AssociateUsers,PlanStartTime,PlanEndTime,Content,PlanWorkloadValue,FinishTime,FinishContent,CurrentProgress,TrueWorkloadValue,FilesInfo,Score,Comment,CommentTime,Remark
                    string tmpSql = "Select SYSID,BelongSectionID,JobName,JobType,Creator,Creator as CreatorName,CreateTime,AssignUser,AssignUser as AssignUserName,AssociateUsers,PlanStartTime,PlanEndTime,Content,PlanWorkloadValue,FinishTime,FinishContent,FinishTimeRecords,CurrentProgress,TrueWorkloadValue,FilesInfo,Score,Comment,CommentTime,CommentFilesInfo,Remark"
                        + " From X_Jobs Where (AssignUser='" + tmpUser.usercode + "' OR AssociateUsers like '%," + tmpUser.usercode + ",%') AND BelongSectionID like '" + tmprojectid + "%' AND PlanStartTime>'" + tmpStartTime + "' AND PlanStartTime<'" + tmpEndTime + "'";
                    DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                    if (table != null && table.Rows.Count > 0)
                    {
                        SZRiverSys.Comm.CommonClass.ReplaceTableColumnsName(table, new string[] { "CreatorName", "AssignUserName" });//替换用户名为昵称
                        JSON.JsonHelper Json = new JSON.JsonHelper();
                        string json = Json.DataTableToJsonForLayUi(table);
                        string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该项目指定时间范围内没有需要查询的工单\"}";
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
            else if (method == "queryalljobsbytime")// 查询本人所有的工单
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
                //int pageIndex = Convert.ToInt32(context.Request.Params["pageindex"]);
                //int pageSize = Convert.ToInt32(context.Request.Params["pagesize"]);

                if (CheckIsExistNullParam(new string[] { tmpStartTime, tmpEndTime }))
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"必要字段缺失\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                ProjectInfoClass tmpprjInfo = Model.ProjectInfoClass.GetProjectInfo(tmprojectid);
                if (tmpprjInfo != null)
                {
                    string tmpUserCondition = "";
                    string tmpSql = "SELECT ID FROM [dbo].[X_PartsInfo] Where charindex(PartID,'"+ tmprojectid + "')>0 AND (Manager='" + tmpUser.usercode + "' OR UsersList like '\"" + tmpUser.usercode + "\"')";
                    DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                    if (table != null && table.Rows.Count > 0)
                        tmpUserCondition = " 1=1 ";
                    else
                        tmpUserCondition = "(Creator='" + tmpUser.usercode + "' OR AssignUser='" + tmpUser.usercode + "' OR AssociateUsers like '%," + tmpUser.usercode + ",%')";
                    //SYSID,BelongSectionID,JobName,JobType,Creator,CreateTime,AssignUser,AssociateUsers,PlanStartTime,PlanEndTime,Content,PlanWorkloadValue,FinishTime,FinishContent,CurrentProgress,TrueWorkloadValue,FilesInfo,Score,Comment,CommentTime,Remark
                    //tmpSql = "Select TOP "+ pageSize + " SYSID,BelongSectionID,JobName,JobType,Creator,Creator as CreatorName,CreateTime,AssignUser,AssignUser as AssignUserName,AssociateUsers,PlanStartTime,PlanEndTime,Content,PlanWorkloadValue,FinishTime,FinishContent,FinishTimeRecords,CurrentProgress,TrueWorkloadValue,FilesInfo,Score,Comment,CommentTime,CommentFilesInfo,Remark"
                    //    + " From X_Jobs Where "+ tmpUserCondition + " AND BelongSectionID like '" + tmprojectid + "%' AND PlanStartTime>'" + tmpStartTime + "' AND PlanStartTime<'" + tmpEndTime + "' AND ID not in (select top (("+ pageIndex + "-1)*"+ pageSize + ") ID from X_Jobs WHERE PlanStartTime>'" + tmpStartTime + "' AND PlanStartTime<'" + tmpEndTime + "')";
                    tmpSql = "Select SYSID,BelongSectionID,JobName,JobType,Creator,Creator as CreatorName,CreateTime,AssignUser,AssignUser as AssignUserName,AssociateUsers,PlanStartTime,PlanEndTime,Content,PlanWorkloadValue,FinishTime,FinishContent,FinishTimeRecords,CurrentProgress,TrueWorkloadValue,FilesInfo,Score,Comment,CommentTime,CommentFilesInfo,Remark,jobfrom"
                        + " From X_Jobs Where " + tmpUserCondition + " AND BelongSectionID like '" + tmprojectid + "%' AND PlanStartTime>'" + tmpStartTime + "' AND PlanStartTime<'" + tmpEndTime + "' order by id desc";
                    table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                    if (table != null && table.Rows.Count > 0)
                    {
                        SZRiverSys.Comm.CommonClass.ReplaceTableColumnsName(table, new string[] { "CreatorName", "AssignUserName" });//替换用户名为昵称
                        JSON.JsonHelper Json = new JSON.JsonHelper();
                        string json = Json.DataTableToJsonForLayUi(table);
                        string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该项目指定时间范围内没有需要查询的工单\"}";
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
            else if (method == "queryundonejobsbytime")// 查询本人没有完成的工单
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

                if (CheckIsExistNullParam(new string[] { tmpStartTime, tmpEndTime }))
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"必要字段缺失\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                ProjectInfoClass tmpprjInfo = Model.ProjectInfoClass.GetProjectInfo(tmprojectid);
                if (tmpprjInfo != null)
                {
                    //SYSID,BelongSectionID,JobName,JobType,Creator,CreateTime,AssignUser,AssociateUsers,PlanStartTime,PlanEndTime,Content,PlanWorkloadValue,FinishTime,FinishContent,CurrentProgress,TrueWorkloadValue,FilesInfo,Score,Comment,CommentTime,Remark
                    string tmpSql = "Select SYSID,BelongSectionID,JobName,JobType,Creator,Creator As CreatorName,CreateTime,AssignUser,AssignUser As AssignUserName,AssociateUsers,PlanStartTime,PlanEndTime,Content,PlanWorkloadValue,FinishTime,FinishTimeRecords,FinishContent,CurrentProgress,TrueWorkloadValue,FilesInfo,Score,Comment,CommentTime,CommentFilesInfo,Remark"
                        + " From X_Jobs Where (AssignUser='" + tmpUser.usercode + "' OR AssociateUsers like '%,"+ tmpUser.usercode + ",%') AND BelongSectionID like '" + tmprojectid + "%' AND PlanStartTime>'" + tmpStartTime+ "' AND PlanEndTime<'"+ tmpEndTime + "' AND CurrentProgress < 100";
                    DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                    if (table != null && table.Rows.Count > 0)
                    {
                        SZRiverSys.Comm.CommonClass.ReplaceTableColumnsName(table, new string[] { "CreatorName", "AssignUserName" });//替换用户名为昵称
                        JSON.JsonHelper Json = new JSON.JsonHelper();
                        string json = Json.DataTableToJsonForLayUi(table);
                        string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该项目指定时间范围内没有需要查询的工单\"}";
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
            else if (method == "queryprogressjobsbytime")// 查询本人进度的工单
            {
                string tmprojectid = context.Request.Params["projectid"];
                if (tmprojectid == null || tmprojectid.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpStartProgressStr = context.Request.Params["startprogress"];
                if (tmpStartProgressStr == null || tmpStartProgressStr.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"startprogress不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpEndProgressStr = context.Request.Params["endprogress"];
                if (tmpEndProgressStr == null || tmpEndProgressStr.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"endprogress不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                int tmpProgressInt1 = 0, tmpProgressInt2 = 0;
                if(!int.TryParse(tmpStartProgressStr, out tmpProgressInt1)
                    || !int.TryParse(tmpEndProgressStr, out tmpProgressInt2)
                    || tmpProgressInt1 > tmpProgressInt2)
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"progress参数值错误\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpStartTime = context.Request.Params["starttime"];
                string tmpEndTime = context.Request.Params["endtime"];

                if (CheckIsExistNullParam(new string[] { tmpStartTime, tmpEndTime }))
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"必要字段缺失\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                ProjectInfoClass tmpprjInfo = Model.ProjectInfoClass.GetProjectInfo(tmprojectid);
                if (tmpprjInfo != null)
                {
                    //SYSID,BelongSectionID,JobName,JobType,Creator,CreateTime,AssignUser,AssociateUsers,PlanStartTime,PlanEndTime,Content,PlanWorkloadValue,FinishTime,FinishContent,CurrentProgress,TrueWorkloadValue,FilesInfo,Score,Comment,CommentTime,Remark
                    string tmpSql = "Select SYSID,BelongSectionID,JobName,JobType,Creator,Creator As CreatorName,CreateTime,AssignUser,AssignUser As AssignUserName,AssociateUsers,PlanStartTime,PlanEndTime,Content,PlanWorkloadValue,FinishTime,FinishContent,FinishTimeRecords,CurrentProgress,TrueWorkloadValue,FilesInfo,Score,Comment,CommentTime,CommentFilesInfo,Remark"
                        + " From X_Jobs Where (AssignUser='" + tmpUser.usercode + "' OR AssociateUsers like '%," + tmpUser.usercode + ",%') AND BelongSectionID like '" + tmprojectid + "%' AND PlanStartTime>'" + tmpStartTime + "' AND PlanEndTime<'" + tmpEndTime + "' AND CurrentProgress>=" + tmpProgressInt1.ToString() + " AND CurrentProgress<="+ tmpProgressInt2.ToString();
                    DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                    if (table != null && table.Rows.Count > 0)
                    {
                        SZRiverSys.Comm.CommonClass.ReplaceTableColumnsName(table, new string[] { "CreatorName", "AssignUserName" });//替换用户名为昵称
                        JSON.JsonHelper Json = new JSON.JsonHelper();
                        string json = Json.DataTableToJsonForLayUi(table);
                        string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该项目指定时间范围内没有需要查询的工单\"}";
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
            else if (method == "queryjobbyid")// 查询指定工单信息
            {
                string tmpSYSID = context.Request.Params["sysid"];
                if (tmpSYSID == null || tmpSYSID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"sysid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                //SYSID,BelongSectionID,JobName,JobType,Creator,CreateTime,AssignUser,AssociateUsers,PlanStartTime,PlanEndTime,Content,PlanWorkloadValue,FinishTime,FinishContent,CurrentProgress,TrueWorkloadValue,FilesInfo,Score,Comment,CommentTime,Remark
                string tmpSql = "Select SYSID,BelongSectionID,JobName,JobType,Creator,Creator As CreatorName,CreateTime,AssignUser,AssignUser As AssignUserName,AssociateUsers, AssociateUsers As AssociateUsersName,PlanStartTime,PlanEndTime,Content,PlanWorkloadValue,FinishTime,FinishContent,FinishTimeRecords,CurrentProgress,TrueWorkloadValue,FilesInfo,Score,Comment,CommentTime,CommentFilesInfo,Remark,image,jobfrom"
                    + " From X_Jobs Where  SYSID='" + tmpSYSID + "'";//(AssignUser='" + tmpUser.usercode + "' OR AssociateUsers like '%," + tmpUser.usercode + ",%') AND
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    SZRiverSys.Comm.CommonClass.ReplaceTableColumnsName(table, new string[] { "CreatorName", "AssignUserName", "AssociateUsersName" });//替换用户名为昵称
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该项目内没有需要查询的工单\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }


            //改
            else if (method == "openjobinfo")
            {
                string tmpSYSID = context.Request.Params["sysid"];
                if (tmpSYSID == null || tmpSYSID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"sysid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                //SYSID,BelongSectionID,JobName,JobType,Creator,CreateTime,AssignUser,AssociateUsers,PlanStartTime,PlanEndTime,Content,PlanWorkloadValue,FinishTime,FinishContent,CurrentProgress,TrueWorkloadValue,FilesInfo,Score,Comment,CommentTime,Remark
                string tmpSql = "Select SYSID,BelongSectionID,JobName,JobType,Creator,Creator As CreatorName,CreateTime,AssignUser,AssignUser As AssignUserName,AssociateUsers, AssociateUsers As AssociateUsersName,PlanStartTime,PlanEndTime,Content,PlanWorkloadValue,FinishTime,FinishContent,FinishTimeRecords,CurrentProgress,TrueWorkloadValue,FilesInfo,Score,Comment,CommentTime,CommentFilesInfo,Remark,image,jobfrom"
                    + " From X_Jobs Where  jobfrom ='" + tmpSYSID + "'";//(AssignUser='" + tmpUser.usercode + "' OR AssociateUsers like '%," + tmpUser.usercode + ",%') AND
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    SZRiverSys.Comm.CommonClass.ReplaceTableColumnsName(table, new string[] { "CreatorName", "AssignUserName", "AssociateUsersName" });//替换用户名为昵称
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该项目内没有需要查询的工单\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }



            #endregion
            #region 修改工单
            else if (method == "editjob")// 修改工单
            {
                string tmpSYSID = context.Request.Params["sysid"];
                if (tmpSYSID == null || tmpSYSID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"sysid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSql = "";
                tmpSql = "Select BelongSectionID From X_Jobs Where SYSID='"+ tmpSYSID + "'";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if(table!=null && table.Rows.Count>0)
                {
                    string tmpSectionID = table.Rows[0].ItemArray[0].ToString();
                    //SectionClass tmpSectionClass = Model.SectionClass.GetSection(tmpSectionID);
                    //if (tmpSectionClass != null)
                    {
                        string tmpPrjID = tmpSectionID.Substring(0, 12);
                        ProjectInfoClass tmpPrjInfo = ProjectInfoClass.GetProjectInfo(tmpPrjID);
                        if (tmpPrjInfo != null)
                        {
                            if (tmpPrjInfo.ProjectManager == tmpUser.usercode || tmpPrjInfo.ManagerUserList.Contains("," + tmpUser.usercode + ","))//只有项目负责人与管理层才能修改工单
                            {
                                string tmpWhereSQL = "";
                                Dictionary<string, string> tmpDict = new Dictionary<string, string>();
                                string[] tmpFields = new string[] { "assignuser", "associateusers", "starttime", "endtime", "content" };
                                string[] tmpField2s = new string[] { "AssignUser", "AssociateUsers", "PlanStartTime", "PlanEndTime", "Content" };
                                int tmpFieldsLen = tmpFields.Length;
                                for (int i = 0; i < tmpFieldsLen; i++)
                                {
                                    string tmpField = tmpFields[i];
                                    string tmpParaValue = context.Request.Params[tmpField];
                                    if (tmpParaValue != null)
                                    {
                                        if (tmpWhereSQL.Length > 0)
                                            tmpWhereSQL += ",";
                                        tmpWhereSQL += tmpField2s[i] + "='" + tmpParaValue + "'";
                                    }
                                }
                                tmpFields = new string[] { "planworkload" };
                                tmpField2s = new string[] { "PlanWorkloadValue" };
                                tmpFieldsLen = tmpFields.Length;
                                for (int i = 0; i < tmpFieldsLen; i++)
                                {
                                    string tmpField = tmpFields[i];
                                    string tmpParaValue = context.Request.Params[tmpField];
                                    if (tmpParaValue != null)
                                    {
                                        if (tmpWhereSQL.Length > 0)
                                            tmpWhereSQL += ",";
                                        tmpWhereSQL += tmpField2s[i] + "=" + tmpParaValue + "";
                                    }
                                }
                                if (tmpWhereSQL.Length > 0)
                                {
                                    tmpSql = "Update X_Jobs Set " + tmpWhereSQL + "  Where SYSID='" + tmpSYSID + "'";
                                    if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql) > 0)
                                    {
                                        XProjectHandler.AddLogInfo(tmpSectionID, tmpUser.usercode, "工单_编辑", tmpSYSID+ " " +tmpWhereSQL.Replace("'", ""));
                                        string msg = "{\"code\":0,\"success\":true,\"msg\":\"" + tmpSYSID + "\"}";
                                        sendResponse(context, strFun, msg);
                                        return;
                                    }
                                    else
                                    {
                                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"修改部门信息失败\"}";
                                        sendResponse(context, strFun, msg);
                                        return;
                                    }
                                }
                                else
                                {
                                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"无任何修改参数\"}";
                                    sendResponse(context, strFun, msg);
                                    return;
                                }
                            }
                            else
                            {
                                string msg = "{\"code\":-1,\"success\":false,\"msg\":\"当前用户不是该项目的负责人或管理人员,不能编辑工单.\"}";
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
                    //else
                    //{
                    //    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该项目工作范围不存在\"}";
                    //    sendResponse(context, strFun, msg);
                    //    return;
                    //}
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该工单不存在\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion
            #region 删除工单
            else if (method == "deletejob")// 删除工单
            {
                string tmpSYSID = context.Request.Params["sysid"];
                if (tmpSYSID == null || tmpSYSID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"sysid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSql = "";
                tmpSql = "Select BelongSectionID From X_Jobs Where SYSID='" + tmpSYSID + "'";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    string tmpSectionID = table.Rows[0].ItemArray[0].ToString();
                    //SectionClass tmpSectionClass = Model.SectionClass.GetSection(tmpSectionID);
                    //if (tmpSectionClass != null)
                    {
                        string tmpPrjID = tmpSectionID.Substring(0, 12);
                        ProjectInfoClass tmpPrjInfo = ProjectInfoClass.GetProjectInfo(tmpPrjID);
                        if (tmpPrjInfo != null)
                        {
                            if (tmpPrjInfo.ProjectManager == tmpUser.usercode || tmpPrjInfo.ManagerUserList.Contains("," + tmpUser.usercode + ","))//只有项目负责人与管理层才能删除工单
                            {
                                tmpSql = "Delete From X_Jobs Where SYSID='" + tmpSYSID + "'";
                                if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql) > 0)
                                {
                                    XProjectHandler.AddLogInfo(tmpSectionID, tmpUser.usercode, "工单_删除", "原ID为" + tmpSYSID);
                                    string msg = "{\"code\":0,\"success\":true,\"msg\":\"" + tmpPrjID + "\"}";
                                    sendResponse(context, strFun, msg);
                                    return;
                                }
                                else
                                {
                                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"删除工单失败\"}";
                                    sendResponse(context, strFun, msg);
                                    return;
                                }
                            }
                            else
                            {
                                string msg = "{\"code\":-1,\"success\":false,\"msg\":\"当前用户不是该项目的负责人或管理人员,不能删除工单.\"}";
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
                    //else
                    //{
                    //    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该项目工作范围不存在\"}";
                    //    sendResponse(context, strFun, msg);
                    //    return;
                    //}
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该工单不存在\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion
            #region 处理工单
            else if (method == "processjob")// 处理工单
            {
                string tmpSYSID = context.Request.Params["sysid"];
                if (tmpSYSID == null || tmpSYSID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"sysid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSql = "";
                tmpSql = "Select BelongSectionID,AssignUser,PlanStartTime,PlanEndTime,CurrentProgress From X_Jobs Where SYSID='" + tmpSYSID + "'";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    string tmpSectionID = table.Rows[0].ItemArray[0].ToString();
                    string tmpAssignUser = table.Rows[0].ItemArray[1].ToString();
                    if(tmpAssignUser!=tmpUser.usercode)//不是工单接收人不能处理工单
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"当前用户不是工单受理人,不能处理工单\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                    //比较工单时间
                    DateTime tmpStart = Convert.ToDateTime(table.Rows[0].ItemArray[2].ToString());
                    DateTime tmpEnd = Convert.ToDateTime(table.Rows[0].ItemArray[3].ToString());
                    if(tmpStart > DateTime.Now)
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"当前工单还未到计划处理时间\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                    //if (tmpEnd < DateTime.Now)//已经超期
                    //{
                    //    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"当前工单已过期\"}";
                    //    sendResponse(context, strFun, msg);
                    //    return;
                    //}
                    int tmpCurrentProgress = Convert.ToInt32(table.Rows[0].ItemArray[4].ToString());
                    if (tmpCurrentProgress >= 100)
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"当前工单已经处理完成,不能重复处理\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                    
                    string tmpWhereSQL = "";
                    Dictionary<string, string> tmpDict = new Dictionary<string, string>();
                    string[] tmpFields = new string[] { "finishcontent", "filesinfo" };
                    string[] tmpField2s = new string[] { "FinishContent", "FilesInfo" };
                    int tmpFieldsLen = tmpFields.Length;
                    for (int i = 0; i < tmpFieldsLen; i++)
                    {
                        string tmpField = tmpFields[i];
                        string tmpParaValue = context.Request.Params[tmpField];
                        if (tmpParaValue != null)
                        {
                            if (tmpWhereSQL.Length > 0)
                                tmpWhereSQL += ",";
                            tmpWhereSQL += tmpField2s[i] + "="+ tmpField2s[i] + "+'|" + tmpParaValue + "'";
                        }
                    }
                    string tmpassociateusers = context.Request.Params["associateusers"];
                    if (tmpassociateusers != null)
                    {
                        if (tmpWhereSQL.Length > 0)
                            tmpWhereSQL += ",";
                        tmpWhereSQL += "AssociateUsers='" + tmpassociateusers + "'";
                    }

                    tmpFields = new string[] { "currentprogress", "trueworkload" };
                    tmpField2s = new string[] { "CurrentProgress", "TrueWorkloadValue" };
                    tmpFieldsLen = tmpFields.Length;
                    for (int i = 0; i < tmpFieldsLen; i++)
                    {
                        string tmpField = tmpFields[i];
                        string tmpParaValue = context.Request.Params[tmpField];
                        if (tmpParaValue != null)
                        {
                            if (tmpWhereSQL.Length > 0)
                                tmpWhereSQL += ",";
                            tmpWhereSQL += tmpField2s[i] + "=" + tmpParaValue + "";
                        }
                    }
                    if (tmpWhereSQL.Length > 0)
                    {
                        string tmpFinishTimeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        tmpWhereSQL += ",FinishTime='"+ tmpFinishTimeStr + "',FinishTimeRecords=FinishTimeRecords+'|"+tmpFinishTimeStr+"'";
                        tmpSql = "Update X_Jobs Set " + tmpWhereSQL + "  Where SYSID='" + tmpSYSID + "'";
                        if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql) > 0)
                        {
                            XProjectHandler.AddLogInfo(tmpSectionID, tmpUser.usercode, "工单_处理", tmpSYSID + " " + tmpWhereSQL.Replace("'", ""));
                            string msg = "{\"code\":0,\"success\":true,\"msg\":\"" + tmpSYSID + "\"}";
                            sendResponse(context, strFun, msg);
                            return;
                        }
                        else
                        {
                            string msg = "{\"code\":-1,\"success\":false,\"msg\":\"处理工单失败\"}";
                            sendResponse(context, strFun, msg);
                            return;
                        }
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"无任何修改参数\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该工单不存在\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            else if (method == "judgejob")// 评价工单
            {
                string tmpSYSID = context.Request.Params["sysid"];
                if (tmpSYSID == null || tmpSYSID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"sysid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSql = "";
                tmpSql = "Select BelongSectionID From X_Jobs Where SYSID='" + tmpSYSID + "'";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    string tmpSectionID = table.Rows[0].ItemArray[0].ToString();
                    string tmpPrjID = tmpSectionID.Substring(0, 12);
                    ProjectInfoClass tmpPrjInfo = ProjectInfoClass.GetProjectInfo(tmpPrjID);
                    if (tmpPrjInfo != null)
                    {
                        if (tmpPrjInfo.ProjectManager == tmpUser.usercode || tmpPrjInfo.ManagerUserList.Contains("," + tmpUser.usercode + ","))//只有项目负责人与管理层才能创建工单
                        {
                            string tmpWhereSQL = "";
                            Dictionary<string, string> tmpDict = new Dictionary<string, string>();
                            string[] tmpFields = new string[] { "comment", "commentfilesinfo" };
                            string[] tmpField2s = new string[] { "Comment", "CommentFilesInfo" };
                            int tmpFieldsLen = tmpFields.Length;
                            for (int i = 0; i < tmpFieldsLen; i++)
                            {
                                string tmpField = tmpFields[i];
                                string tmpParaValue = context.Request.Params[tmpField];
                                if (tmpParaValue != null)
                                {
                                    if (tmpWhereSQL.Length > 0)
                                        tmpWhereSQL += ",";
                                    tmpWhereSQL += tmpField2s[i] + "='" + tmpParaValue + "'";
                                }
                            }
                            tmpFields = new string[] { "score"};
                            tmpField2s = new string[] { "Score"};
                            tmpFieldsLen = tmpFields.Length;
                            for (int i = 0; i < tmpFieldsLen; i++)
                            {
                                string tmpField = tmpFields[i];
                                string tmpParaValue = context.Request.Params[tmpField];
                                if (tmpParaValue != null)
                                {
                                    if (tmpWhereSQL.Length > 0)
                                        tmpWhereSQL += ",";
                                    tmpWhereSQL += tmpField2s[i] + "=" + tmpParaValue + "";
                                }
                            }
                            if (tmpWhereSQL.Length > 0)
                            {
                                tmpWhereSQL += ",CommentTime='"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"'";
                                tmpSql = "Update X_Jobs Set " + tmpWhereSQL + "  Where SYSID='" + tmpSYSID + "'";
                                if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql) > 0)
                                {
                                    XProjectHandler.AddLogInfo(tmpSectionID, tmpUser.usercode, "工单_评价", tmpSYSID + " " + tmpWhereSQL.Replace("'", ""));
                                    string msg = "{\"code\":0,\"success\":true,\"msg\":\"" + tmpSYSID + "\"}";
                                    sendResponse(context, strFun, msg);
                                    return;
                                }
                                else
                                {
                                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"评价工单失败\"}";
                                    sendResponse(context, strFun, msg);
                                    return;
                                }
                            }
                            else
                            {
                                string msg = "{\"code\":-1,\"success\":false,\"msg\":\"无任何有效参数\"}";
                                sendResponse(context, strFun, msg);
                                return;
                            }
                        }
                        else
                        {
                            string msg = "{\"code\":-1,\"success\":false,\"msg\":\"当前用户不是该项目的负责人或管理人员,不能编辑工单.\"}";
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
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该工单不存在\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion
            #region 文件处理
            else if (method == "uploadfile")// 文件上传
            {
                //string tmpSYSID = context.Request.Params["sysid"];
                //if (tmpSYSID == null || tmpSYSID.Trim() == "")
                //{
                //    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"sysid不能为空\"}";
                //    sendResponse(context, strFun, msg);
                //    return;
                //}
                //string tmpSql = "";
                //tmpSql = "Select BelongSectionID From X_Jobs Where SYSID='" + tmpSYSID + "'";
                //DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                //if (table != null && table.Rows.Count > 0)
                string tmpprojectID = context.Request.Params["projectid"];
                if (tmpprojectID == null || tmpprojectID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                {
                    //string tmpSectionID = table.Rows[0].ItemArray[0].ToString();
                    HttpFileCollection hfc = context.Request.Files;
                    HttpPostedFile hpf = null;
                    //if (hfc.Count > 0)
                    //{
                    //    hpf = context.Request.Files[0];
                    //}                       
                    string FileNames = "";
                    bool errormsg = true;
                    string errormsgstr = "";
                    if (hfc.Count > 0)
                    {
                        //string tmpMonthStr = DateTime.Now.ToString("yyyyMM");
                        string tmpSaveFolder = HttpContext.Current.Server.MapPath("~/Upfile/Jobs");
                        //if (!Directory.Exists(tmpSaveFolder))
                        //    Directory.CreateDirectory(tmpSaveFolder);

                        for (int i = 0; i < hfc.Count; i++)
                        {
                            hpf = context.Request.Files[i];
                            string date = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                            var filetype = hpf.FileName.Split('.')[1];
                            //var filename = tmpSectionID+"_"+tmpUser.usercode + "-" + date + "." + filetype;
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
                //string tmpSaveFolder = HttpContext.Current.Server.MapPath("~/Upfile/Jobs");
                List<string> filepath = new List<string>();
                for (int i = 0; i < filelist.Length; i++)
                {
                    string tmpFileID = filelist[i];
                    if(tmpFileID.Trim() != "")
                        filepath.Add("Upfile/Jobs/" + tmpFileID);
                }
                //string filepath=HttpContext.Current.Server.MapPath("~/Upfile/RiverManageFile/") +filename;
                //if (File.Exists(filepath))
                //{
                //    DownFileList(filepath);
                //}
                string msg = "{\"code\":0,\"success\":true,\"filepath\":\"" + string.Join(";", filepath) + "\"}";
                sendResponse(context, strFun, msg);
                return;
            }
            #endregion



            #region 根据sysid查询工单事件的整改
            else if (method == "abarbeitungjob")
            {
                string sysid = context.Request.Params["sysid"];
                string sql = string.Format("select * from x_jobs where jobfrom like '%" + sysid + "%'");
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(sql);
                if (table != null && table.Rows.Count>0)
                {
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该项目指定时间范围内没有需要查询的工单\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                
            }
            #endregion
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 创建一个工单
        /// </summary>
        /// <param name="BelongSectionID"></param>
        /// <param name="JobName"></param>
        /// <param name="JobType"></param>
        /// <param name="Creator"></param>
        /// <param name="AssignUser"></param>
        /// <param name="AssociateUsers"></param>
        /// <param name="PlanStartTime"></param>
        /// <param name="PlanEndTime"></param>
        /// <param name="Content"></param>
        /// <param name="PlanWorkloadValue"></param>
        /// <returns></returns>
        public static bool CreateJob(string BelongSectionID, string JobName, string JobType, string Creator, 
            string AssignUser, string AssociateUsers,string PlanStartTime,string PlanEndTime,string Content,string PlanWorkloadValue, out string jobSYSID, string FileNames,string jobfrom)
        {
            string tmpSysID = Guid.NewGuid().ToString("N");
            jobSYSID = "";
            if (AssociateUsers == null) AssociateUsers = "";
            if (Content == null) Content = "";
            if (PlanWorkloadValue == null) PlanWorkloadValue = "0";

            DataTable table = null;
            //改
            if (jobfrom != "1")
            {
                jobfrom = jobfrom.Substring(0, tmpSysID.Length);
                string sql = string.Format("select top 1 jobfrom from X_Jobs where jobfrom like '%" + jobfrom + "%' order by createtime desc");
                table = DBHelper.DBHelperMsSql.ExecuteDataTable(sql);
            }
           
            if (table != null && table.Rows.Count > 0)
            {
                string jobfromid = table.Rows[0]["jobfrom"].ToString();
                int num = Convert.ToInt32( jobfromid.Substring(tmpSysID.Length + 2));
                num = num + 1;
                jobfrom = jobfrom == "1" ? tmpSysID + "00" + num : jobfrom.Substring(0, tmpSysID.Length) + "00" + num;
            }
            else
            {
                jobfrom = jobfrom == "1" ? tmpSysID + "001" : jobfrom + "001";
            }

            //SYSID,BelongSectionID,JobName,JobType,Creator,CreateTime,AssignUser,AssociateUsers,PlanStartTime,PlanEndTime,Content,PlanWorkloadValue
            string tmpSql = string.Format("Insert Into X_Jobs(SYSID,BelongSectionID,JobName,JobType,Creator,CreateTime,AssignUser,AssociateUsers,PlanStartTime,PlanEndTime,Content,PlanWorkloadValue,image,jobfrom) "
                    + "Values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}',{11},'{12}','{13}')",
                    tmpSysID, BelongSectionID, JobName, JobType, Creator,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), AssignUser, AssociateUsers, PlanStartTime, PlanEndTime, Content, PlanWorkloadValue, FileNames,jobfrom);
            int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql);
            if (count > 0)
            {
                jobSYSID = tmpSysID;
                return true;
            }
            return false;
        }


        /// <summary>
        /// 创建计划工单
        /// </summary>
        /// <param name="BelongSectionID"></param>
        /// <param name="JobName"></param>
        /// <param name="JobType"></param>
        /// <param name="Creator"></param>
        /// <param name="AssignUser"></param>
        /// <param name="AssociateUsers"></param>
        /// <param name="PlanStartTime"></param>
        /// <param name="PlanEndTime"></param>
        /// <param name="Content"></param>
        /// <param name="PlanWorkloadValue"></param>
        /// <param name="planID"></param>
        /// <param name="jobSYSID"></param>
        /// <returns></returns>
        public static bool CreatePlanJob(string BelongSectionID, string JobName, string JobType, string Creator,
            string AssignUser, string AssociateUsers, string PlanStartTime, string PlanEndTime, string Content, string PlanWorkloadValue,string planID, out string jobSYSID)
        {
            string tmpSysID = Guid.NewGuid().ToString("N");
            jobSYSID = "";
            if (AssociateUsers == null) AssociateUsers = "";
            if (Content == null) Content = "";
            if (PlanWorkloadValue == null) PlanWorkloadValue = "0";

            //SYSID,BelongSectionID,JobName,JobType,Creator,CreateTime,AssignUser,AssociateUsers,PlanStartTime,PlanEndTime,Content,PlanWorkloadValue
            string tmpSql = string.Format("Insert Into X_Jobs(SYSID,BelongSectionID,JobName,JobType,Creator,CreateTime,AssignUser,AssociateUsers,PlanStartTime,PlanEndTime,Content,PlanID,PlanWorkloadValue) "
                    + "Values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}',{12})",
                    tmpSysID, BelongSectionID, JobName, JobType, Creator,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), AssignUser, AssociateUsers, PlanStartTime, PlanEndTime, Content, planID, PlanWorkloadValue);
            if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql) > 0)
            {
                jobSYSID = tmpSysID;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 根据项目计划生成工单
        /// </summary>
        /// <param name="planID"></param>
        /// <returns></returns>
        public static bool CreateJobByPlanID(string planID)
        {
            string tmpSql;
            tmpSql = "Select BelongProjectID,PlanName,PlanType,Creator,AssignUser,StartTime,EndTime,PlanCount,WorkloadValue,Content From X_ProjectPlanInfo Where SYSID='"+ planID + "'";
            DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
            if (table != null && table.Rows.Count > 0)
            {
                DataRow tmpRow = table.Rows[0];
                DateTime tmpStartTime = Convert.ToDateTime(tmpRow.ItemArray[5].ToString());
                DateTime tmpEndTime = Convert.ToDateTime(tmpRow.ItemArray[6].ToString());
                int tmpPlanCount = Convert.ToInt32(tmpRow.ItemArray[7].ToString());
                double tmpWorkloadValue = Convert.ToDouble(tmpRow.ItemArray[8].ToString());
                tmpWorkloadValue = tmpWorkloadValue / tmpPlanCount;

                double tmpTotalDays = (tmpEndTime - tmpStartTime).TotalDays;
                double tmpSplitDays = tmpTotalDays / tmpPlanCount;

                DateTime tmpStartTime0 = tmpStartTime, tmpEndTime0;
                for (int i=0;i< tmpPlanCount;i++)
                {
                    tmpEndTime0 = tmpStartTime0.AddDays(tmpSplitDays);
                    string tmpOutSYSID = "";
                    CreatePlanJob(tmpRow.ItemArray[0].ToString(), tmpRow.ItemArray[1].ToString(), tmpRow.ItemArray[2].ToString(),
                        tmpRow.ItemArray[3].ToString(), tmpRow.ItemArray[4].ToString(), ""
                        , tmpStartTime0.ToString("yyyy-MM-dd HH:mm:ss"), tmpEndTime0.ToString("yyyy-MM-dd HH:mm:ss")
                        , tmpRow.ItemArray[9].ToString(), tmpWorkloadValue.ToString(), planID, out tmpOutSYSID);
                    tmpStartTime0 = tmpEndTime0;
                }
                return true;
            }
                return false;
        }
    }
}