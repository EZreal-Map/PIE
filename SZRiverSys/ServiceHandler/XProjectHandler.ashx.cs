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
    /// XProjectHandler 的摘要说明
    /// </summary>
    public class XProjectHandler : IHttpHandler
    {
        private void sendResponse(HttpContext context, string callbackFun, string result)
        {
            if (callbackFun != null)
                result = callbackFun + "(" + result + ")";
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.Write(result);
            context.Response.End();
            context.Response.Close();
        }

        public void ProcessRequest(HttpContext context)
        {
            string strtokens = context.Request.Params["token"];
            string strFun = context.Request["jsonpcallback"];//传递参数
            
            if (strtokens==null || strtokens.Trim()=="" || !CacheHelper.CacheHelper.IsContain(strtokens))
            {
                string msg = "{\"code\":-1,\"success\":false,\"msg\":\"用户登录失效,请重新登录\"}";
                sendResponse(context, strFun, msg);
                return;
            }
            string method = context.Request.Params["method"];

            if(method== null || method.Trim() == "")
            {
                string msg = "{\"code\":-1,\"success\":false,\"msg\":\"method不能为空\"}";
                sendResponse(context, strFun, msg);
                return;
            }
            Model.UserModel tmpUser = CacheHelper.CacheHelper.Get(strtokens) as Model.UserModel;
            method = method.ToLower();
            

            #region 添加部门
            if (method == "addpart")// 添加部门
            {
                string tmppartID = context.Request.Params["partid"];
                string tmpName = context.Request.Params["name"];
                string tmpparentid = context.Request.Params["parentid"];
                string tmpmanager = context.Request.Params["manager"];
                string tmpUsersList = context.Request.Params["users"];
                string tmpInfo = context.Request.Params["info"];
                string tmpReamrk = context.Request.Params["remark"];
                if (tmpReamrk == null) tmpReamrk = "";
                if (tmpInfo == null) tmpInfo = "";
                if (tmpUsersList == null) tmpUsersList = "";

                string tmpSql = string.Format("Insert Into X_PartsInfo(PartID,Name,Manager,Creator,CreateTime,UsersList,Info,Remark,ParentID) "
                    + "Values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", tmppartID, tmpName, tmpmanager, tmpUser.usercode,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), tmpUsersList, tmpInfo, tmpReamrk, tmpparentid);
                if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql) > 0)
                {
                    AddLogInfo(tmppartID, tmpUser.usercode, "部门_添加", "");
                    tmpSql = "Select Max(ID) From X_PartsInfo Where PartID='" + tmppartID + "'";
                    DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                    if (table != null && table.Rows.Count > 0)
                        tmppartID = table.Rows[0].ItemArray[0].ToString();
                    string msg = "{\"code\":0,\"success\":true,\"msg\":\"" + tmppartID + "\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"添加部门失败\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion
            #region 编辑部门
            else if (method == "editpart")// 编辑部门
            {
                string tmpID = context.Request.Params["partid"];
                if (tmpID == null || tmpID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"partid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpWhereSQL = "";
                Dictionary<string, string> tmpDict = new Dictionary<string, string>();
                string[] tmpFields = new string[] { "name", "manager", "users", "info", "remark", "parentid" };
                string[] tmpField2s = new string[] { "Name", "Manager", "UsersList", "Info", "Remark", "ParentID" };
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
                if (tmpWhereSQL.Length > 0)
                {
                    string tmpSql = "Select Manager,Creator From X_PartsInfo Where PartID='" + tmpID + "'";
                    DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                    if (table != null && table.Rows.Count > 0)
                    {
                        string tmpStr = table.Rows[0].ItemArray[0].ToString();
                        if (tmpStr == tmpUser.usercode || table.Rows[0].ItemArray[1].ToString() == tmpUser.usercode)//必须是项目经理才能删除部门
                        {
                            tmpSql = "Update X_PartsInfo Set " + tmpWhereSQL + "  Where PartID='" + tmpID + "'";
                            if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql) > 0)
                            {
                                AddLogInfo(tmpID, tmpUser.usercode, "部门_编辑", tmpWhereSQL.Replace("'", ""));
                                string msg = "{\"code\":0,\"success\":true,\"msg\":\"" + tmpID + "\"}";
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
                            string msg = "{\"code\":-1,\"success\":false,\"msg\":\"编辑部门失败,当前用户不是该部门的负责人.\"}";
                            sendResponse(context, strFun, msg);
                            return;
                        }
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该部门不存在\"}";
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
            #endregion
            #region 删除部门
            else if (method == "deletepart")//删除部门
            {
                string tmppartid = context.Request.Params["partid"];
                if (tmppartid == null || tmppartid.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"partid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }

                string tmpSql = "";
                tmpSql = "Select Manager From X_PartsInfo Where  PartID='" + tmppartid + "'";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    string tmpStr = table.Rows[0].ItemArray[0].ToString();
                    if (tmpStr == tmpUser.usercode)//必须是项目经理才能删除部门
                    {
                        tmpSql = "Delete From X_PartsInfo Where PartID like '" + tmppartid + "%'";
                        if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql) > 0)
                        {
                            AddLogInfo(tmppartid, tmpUser.usercode, "部门_删除", "原ID为" + tmppartid);
                            string msg = "{\"code\":0,\"success\":true,\"msg\":\"" + tmppartid + "\"}";
                            sendResponse(context, strFun, msg);
                            return;
                        }
                        else
                        {
                            string msg = "{\"code\":-1,\"success\":false,\"msg\":\"删除部门失败\"}";
                            sendResponse(context, strFun, msg);
                            return;
                        }
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"删除部门失败,当前用户不是该部门的负责人.\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该部门不存在\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion
            #region 查询部门
            else if (method == "getpartsinfobyuser")//根据用户查询所在所有部门信息
            {
                string tmpSql = "";
                string tmpMinPartID = "";
                DataTable table = null;
                tmpSql = "Select Min(PartID) From X_PartsInfo Where Manager='" + tmpUser.usercode + "' OR UsersList like '%\"" + tmpUser.usercode + "\"%'";
                table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    tmpMinPartID = table.Rows[0].ItemArray[0].ToString();
                    tmpSql = "Select ID,PartID,name,Manager,Manager As ManagerName,Creator,CreateTime,Info,Remark,ParentID From X_PartsInfo Where PartID like '" + tmpMinPartID + "%'";//  AND Manager='" + tmpUser.usercode + "' OR UsersList like '%;" + tmpUser.usercode + ";%'";
                    table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                    if (table != null && table.Rows.Count > 0)
                    {
                        SZRiverSys.Comm.CommonClass.ReplaceTableColumnName(table, "ManagerName");//替换用户名为昵称
                        JSON.JsonHelper Json = new JSON.JsonHelper();
                        string json = Json.DataTableToJsonForLayUi(table);
                        string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"无法获取该用户所在部门信息\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"无法获取该用户所在部门信息\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            else if (method == "getpartinfobyid")//根据用户查询所在所有部门信息
            {
                string msg = "";
                string tmpID = context.Request.Params["partid"];
                if (tmpID == null || tmpID.Trim() == "")
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"partid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSql = "";
                string tmpMinPartID = "";
                tmpSql = "Select Min(PartID) From X_PartsInfo Where Manager='" + tmpUser.usercode + "' OR UsersList like '%\"" + tmpUser.usercode + "\"%'";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    tmpMinPartID = table.Rows[0].ItemArray[0].ToString();
                    if (tmpID.Contains(tmpMinPartID))
                    {
                        tmpSql = "Select ID,PartID,name,Manager,Manager As ManagerName,Creator,CreateTime,Info,Remark,ParentID From X_PartsInfo Where PartID='" + tmpID + "'";// AND Manager='" + tmpUser.usercode + "' OR UsersList like '%;" + tmpUser.usercode + ";%'";
                        table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                        if (table != null && table.Rows.Count > 0)
                        {
                            SZRiverSys.Comm.CommonClass.ReplaceTableColumnName(table, "ManagerName");//替换用户名为昵称

                            JSON.JsonHelper Json = new JSON.JsonHelper();
                            string json = Json.DataTableToJsonForLayUi(table);
                            msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                            sendResponse(context, strFun, msg);
                            return;
                        }
                        else
                        {
                            msg = "{\"code\":-1,\"success\":false,\"msg\":\"无法获取该用户所在部门信息\"}";
                            sendResponse(context, strFun, msg);
                            return;
                        }
                    }
                }
                msg = "{\"code\":-1,\"success\":false,\"msg\":\"无法获取该用户所在部门信息\"}";
                sendResponse(context, strFun, msg);
                return;
            }
            else if (method == "getpartuserslistinfobyuser")//查詢部門的用戶信息
            {
                string tmpID = context.Request.Params["partid"];
                if (tmpID == null || tmpID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"partid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSql = "";
                tmpSql = "Select UsersList From X_PartsInfo Where PartID='" + tmpID + "' AND (Manager='" + tmpUser.usercode + "' OR UsersList like '%\"" + tmpUser.usercode + "\"%')";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    string json = table.Rows[0].ItemArray[0].ToString();
                    if (json.Trim() == "")
                        json = "\"\"";
                    string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"无法获取该用户所在部门信息\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion

            #region 添加项目
            else if (method == "addproject")//添加项目
            {
                string tmpPrjID = context.Request.Params["projectid"];
                string tmpPrjName = context.Request.Params["projectname"];
                string tmpPrjManager = context.Request.Params["projectmanager"];//项目经理
                //string tmpSections = context.Request.Params["sections"];
                string tmpStartTime = context.Request.Params["starttime"];
                string tmpEndTime = context.Request.Params["endtime"];
                string tmpUsersList = context.Request.Params["users"];
                string tmpProjectInfo = context.Request.Params["projectinfo"];
                string tmpReamrk = context.Request.Params["remark"];
                if (tmpReamrk == null) tmpReamrk = "";
                if (tmpProjectInfo == null) tmpProjectInfo = "";
                if (tmpUsersList == null) tmpUsersList = "";

                string tmpSql = string.Format("Insert Into X_ProjectInfo(ProjectID,ProjectName,ProjectManager,StartTime,EndTime,Creator,CreateTime,UsersList,ProjectInfo,Remark) "
                    + "Values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')", tmpPrjID, tmpPrjName, tmpPrjManager, tmpStartTime, tmpEndTime, tmpUser.usercode,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), tmpUsersList, tmpProjectInfo, tmpReamrk);
                if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql) > 0)
                {
                    AddLogInfo(tmpPrjID, tmpUser.usercode, "项目_添加", "");
                    string msg = "{\"code\":0,\"success\":true,\"msg\":\"" + tmpPrjID + "\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"添加项目失败\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion
            #region 编辑项目
            else if (method == "editproject")//编辑项目
            {
                string tmpPrjID = context.Request.Params["projectid"];
                if (tmpPrjID == null || tmpPrjID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpWhereSQL = "";
                //ProjectID,ProjectName,ProjectManager,StartTime,EndTime,Creator,CreateTime,UsersList,ProjectInfo,Remark
                Dictionary<string, string> tmpDict = new Dictionary<string, string>();
                string[] tmpFields = new string[] { "projectname", "projectmanager", "starttime", "endtime", "users", "projectinfo", "remark" };
                string[] tmpField2s = new string[] { "ProjectName", "ProjectManager", "StartTime", "EndTime", "UsersList", "ProjectInfo", "Remark" };
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
                if (tmpWhereSQL.Length > 0)
                {
                    string tmpSql = "Update X_ProjectInfo Set " + tmpWhereSQL + "  Where ProjectID='" + tmpPrjID + "'";
                    if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql) > 0)
                    {
                        ProjectInfoClass tmpProjectInfoClass = ProjectInfoClass.GetProjectInfoCache(tmpPrjID);
                        if (tmpProjectInfoClass != null)
                            tmpProjectInfoClass.RefreshProjectInfo();
                        AddLogInfo(tmpPrjID, tmpUser.usercode, "项目_编辑", tmpWhereSQL.Replace("'", ""));
                        string msg = "{\"code\":0,\"success\":true,\"msg\":\"" + tmpPrjID + "\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"修改项目信息失败\"}";
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
            #endregion
            #region 删除项目
            else if (method == "deleteproject")//删除项目
            {
                string tmpPrjID = context.Request.Params["projectid"];
                if (tmpPrjID == null || tmpPrjID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }

                string tmpSql = "";
                tmpSql = "Select ID,projectmanager From X_ProjectInfo Where ProjectID = '" + tmpPrjID + "'";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    string tmpStr = table.Rows[0].ItemArray[0].ToString();
                    string tmpStr2 = table.Rows[0].ItemArray[1].ToString();
                    if (tmpStr2 == tmpUser.usercode)//必须是项目经理才能删除项目
                    {
                        //tmpSql = "Delete From X_ProjectInfo Where ID = " + tmpStr;
                        tmpSql = "Update X_ProjectInfo Set StatusID = 0  Where ID = " + tmpStr;
                        if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql) > 0)
                        {
                            ProjectInfoClass.RemoveProjectInfo(tmpPrjID);
                            AddLogInfo(tmpPrjID, tmpUser.usercode, "项目_删除", "原ID为" + tmpStr);
                            string msg = "{\"code\":0,\"success\":true,\"msg\":\"" + tmpPrjID + "\"}";
                            sendResponse(context, strFun, msg);
                            return;
                        }
                        else
                        {
                            string msg = "{\"code\":-1,\"success\":false,\"msg\":\"删除项目失败\"}";
                            sendResponse(context, strFun, msg);
                            return;
                        }
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"删除项目失败,当前用户不是该项目的项目经理.\"}";
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
            #region 查询项目信息
            else if (method == "getprojectsinfobypartid")//获取项目
            {
                string tmpPrjID = context.Request.Params["partid"];
                if (tmpPrjID == null || tmpPrjID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"partid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSql = "";
                tmpSql = "Select ProjectID,ProjectName,ProjectManager,ProjectManager As ProjectManagerName,StartTime,EndTime,Creator,CreateTime,ProjectInfo,Remark From X_ProjectInfo Where StatusID = 1 AND ProjectID like '" + tmpPrjID + "%'";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    SZRiverSys.Comm.CommonClass.ReplaceTableColumnName(table, "ProjectManagerName");//替换用户名为昵称
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该部门内没有项目\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            else if (method == "getprojectinfobyprojectid")
            {
                string tmpPrjID = context.Request.Params["projectid"];
                if (tmpPrjID == null || tmpPrjID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSql = "";
                tmpSql = "Select ProjectID,ProjectName,ProjectManager,ProjectManager As ProjectManagerName,StartTime,EndTime,Creator,CreateTime,ProjectInfo,Remark From X_ProjectInfo Where StatusID = 1 AND ProjectID like '"
                    + tmpPrjID + "%' AND (ProjectManager = '" + tmpUser.usercode + "' OR UsersList like '%\"" + tmpUser.usercode + "\"%' ";
                string tmpMinPartID = GetMinPartIDByUserName(tmpUser.usercode);
                if (tmpMinPartID != "")
                {
                    tmpSql += " OR ProjectID like '" + tmpMinPartID + "%' ";
                }
                tmpSql += ")";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    int tmpColIndex = table.Columns.Add("ManagerUserList").Ordinal;
                    foreach (DataRow tmpRow in table.Rows)
                    {
                        string tmpPrjID1 = tmpRow.ItemArray[0].ToString();
                        ProjectInfoClass tmpProjectInfoClass = ProjectInfoClass.GetProjectInfo(tmpPrjID1);
                        if (tmpProjectInfoClass != null)
                        {
                            tmpRow[tmpColIndex] = tmpProjectInfoClass.ManagerUserList;
                        }
                    }
                    SZRiverSys.Comm.CommonClass.ReplaceTableColumnName(table, "ProjectManagerName");//替换用户名为昵称
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该项目不存在或当前用户没有相关权限查看该项目信息\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            else if (method == "getprojectsinfobyuser")
            {
                string tmpSql = "";
                string tmpSearch = context.Request.Params["search"];
                if (tmpSearch == null)
                    tmpSearch = " 1=1 ";
                else
                {
                    tmpSearch = " ProjectName like '%" + tmpSearch + "%'";
                }
                tmpSql = "Select ProjectID,ProjectName,ProjectManager,ProjectManager As ProjectManagerName,StartTime,EndTime,Creator,CreateTime,ProjectInfo,Remark From X_ProjectInfo Where "
                    + "StatusID = 1 AND (ProjectManager='" + tmpUser.usercode + "' OR UsersList like '%\"" + tmpUser.usercode + "\"%' ";
                string tmpMinPartID = GetMinPartIDByUserName(tmpUser.usercode);
                if (tmpMinPartID != "")
                {
                    tmpSql += " OR ProjectID like '" + tmpMinPartID + "%' ";
                }
                tmpSql += ") AND " + tmpSearch;

                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    int tmpColIndex = table.Columns.Add("ManagerUserList").Ordinal;
                    foreach (DataRow tmpRow in table.Rows)
                    {
                        string tmpPrjID1 = tmpRow.ItemArray[0].ToString();
                        ProjectInfoClass tmpProjectInfoClass = ProjectInfoClass.GetProjectInfo(tmpPrjID1);
                        if (tmpProjectInfoClass != null)
                        {
                            tmpRow[tmpColIndex] = tmpProjectInfoClass.ManagerUserList;
                        }
                    }
                    SZRiverSys.Comm.CommonClass.ReplaceTableColumnName(table, "ProjectManagerName");//替换用户名为昵称
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该用户没有参与任何项目\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            else if (method == "getprojectuserslistinfobyprojectid")//查询项目内用户信息
            {
                string tmpPrjID = context.Request.Params["projectid"];
                if (tmpPrjID == null || tmpPrjID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSql = "";
                tmpSql = "Select UsersList From X_ProjectInfo Where ProjectID like '"
                    + tmpPrjID + "%' AND (ProjectManager = '" + tmpUser.usercode + "' OR UsersList like '%\"" + tmpUser.usercode + "\"%')";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    string json = table.Rows[0].ItemArray[0].ToString();
                    if (json.Trim() == "")
                        json = "\"\"";
                    string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"无法获取该用户所在项目内用户信息\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion

            #region 添加项目中河道分段
            else if (method == "addsection")//添加项目中河道分段
            {
                string tmpPrjID = context.Request.Params["projectid"];
                string tmpSecName = context.Request.Params["sectionname"];
                string tmpXminStr = context.Request.Params["xmin"];
                string tmpXmaxStr = context.Request.Params["xmax"];
                string tmpYminStr = context.Request.Params["ymin"];
                string tmpYmaxStr = context.Request.Params["ymax"];
                string tmpBoundary = context.Request.Params["boundary"];
                string tmpSectionInfo = context.Request.Params["sectioninfo"];
                string tmpReamrk = context.Request.Params["remark"];
                if (tmpReamrk == null) tmpReamrk = "";
                if (tmpXminStr == null || tmpXminStr.Trim() == "") tmpXminStr = "0";
                if (tmpXmaxStr == null || tmpXmaxStr.Trim() == "") tmpXmaxStr = "0";
                if (tmpYminStr == null || tmpYminStr.Trim() == "") tmpYminStr = "0";
                if (tmpYmaxStr == null || tmpYmaxStr.Trim() == "") tmpYmaxStr = "0";

                string tmpSql = "", tmpSectionID = "";
                tmpSql = "Select Max(SectionID) From X_ProjectSectionInfo Where SectionID like '" + tmpPrjID + "%'";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    string tmpStr = table.Rows[0].ItemArray[0].ToString();
                    if (tmpStr.Length > 12)
                    {
                        string tmpStr02 = tmpStr.Substring(12);
                        int tmpI = 0;
                        if (int.TryParse(tmpStr02, out tmpI))
                        {
                            tmpI++;
                            tmpSectionID = tmpPrjID + string.Format("{0:d3}", tmpI);
                        }
                    }
                }
                if (tmpSectionID == "")
                    tmpSectionID = tmpPrjID + "001";
                tmpSql = string.Format("Insert Into X_ProjectSectionInfo(SectionID,SectionName,XMin,XMax,YMin,YMax,Boundary,SectionInfo,Remark) "
                    + "Values('{0}','{1}',{2},{3},{4},{5},'{6}','{7}','{8}')", tmpSectionID, tmpSecName, tmpXminStr, tmpXmaxStr, tmpYminStr, tmpYmaxStr, tmpBoundary, tmpSectionInfo, tmpReamrk);
                if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql) > 0)
                {
                    ProjectInfoClass tmpPrjInfo = ProjectInfoClass.GetProjectInfo(tmpPrjID);
                    if (tmpPrjInfo != null)
                        tmpPrjInfo.RefreshSectionInfo();
                    AddLogInfo(tmpSectionID, tmpUser.usercode, "河道范围_添加", "");
                    string msg = "{\"code\":0,\"success\":true,\"msg\":\"" + tmpSectionID + "\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"添加河道工作范围失败\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion
            #region 编辑项目中河道分段
            else if (method == "editsection")//编辑项目中河道分段信息
            {
                string tmpSectionID = context.Request.Params["sectionid"];
                if (tmpSectionID == null || tmpSectionID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"sectionid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpWhereSQL = "";
                Dictionary<string, string> tmpDict = new Dictionary<string, string>();
                string[] tmpFields = new string[] { "sectionname", "boundary", "sectioninfo", "remark" };
                foreach (string tmpField in tmpFields)
                {
                    string tmpParaValue = context.Request.Params[tmpField];
                    if (tmpParaValue != null)
                    {
                        if (tmpWhereSQL.Length > 0)
                            tmpWhereSQL += ",";
                        tmpWhereSQL += tmpField + "='" + tmpParaValue + "'";
                    }
                }
                tmpFields = new string[] { "XMin", "XMax", "YMin", "YMax" };
                foreach (string tmpField in tmpFields)
                {
                    string tmpParaValue = context.Request.Params[tmpField];
                    if (tmpParaValue != null)
                    {
                        if (tmpWhereSQL.Length > 0)
                            tmpWhereSQL += ",";
                        tmpWhereSQL += tmpField + "=" + tmpParaValue + "";
                    }
                }
                if (tmpWhereSQL.Length > 0)
                {
                    string tmpSql = "Update X_ProjectSectionInfo Set " + tmpWhereSQL + "  Where SectionID='" + tmpSectionID + "'";
                    if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql) > 0)
                    {
                        string tmpPrjID = tmpSectionID.Substring(0, 12);
                        ProjectInfoClass tmpPrjInfo = ProjectInfoClass.GetProjectInfo(tmpPrjID);
                        if (tmpPrjInfo != null)
                            tmpPrjInfo.RefreshSectionInfo();
                        AddLogInfo(tmpSectionID, tmpUser.usercode, "河道范围_编辑", tmpWhereSQL.Replace("'", ""));
                        string msg = "{\"code\":0,\"success\":true,\"msg\":\"" + tmpSectionID + "\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"修改河道工作范围失败\"}";
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
            #endregion
            #region 删除项目中河道分段
            else if (method == "deletesection")//删除项目中河道分段信息
            {
                string tmpSectionID = context.Request.Params["sectionid"];
                if (tmpSectionID == null || tmpSectionID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"sectionid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSql = "";
                tmpSql = "Select ID From X_ProjectSectionInfo Where SectionID = '" + tmpSectionID + "'";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    string tmpStr = table.Rows[0].ItemArray[0].ToString();
                    tmpSql = "Delete From X_ProjectSectionInfo Where ID = " + tmpStr;
                    if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql) > 0)
                    {
                        string tmpPrjID = tmpSectionID.Substring(0, 12);
                        ProjectInfoClass tmpPrjInfo = ProjectInfoClass.GetProjectInfo(tmpPrjID);
                        if (tmpPrjInfo != null)
                            tmpPrjInfo.RefreshSectionInfo();
                        AddLogInfo(tmpSectionID, tmpUser.usercode, "河道范围_删除", "原ID为" + tmpStr);
                        string msg = "{\"code\":0,\"success\":true,\"msg\":\"" + tmpSectionID + "\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"删除河道工作范围失败\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该河道工作范围不存在\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion
            #region 查询项目中河道分段
            else if (method == "getsectionsinfobyprojectid")//获取项目中河道分段信息
            {
                string tmpPrjID = context.Request.Params["projectid"];
                if (tmpPrjID == null || tmpPrjID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSearch = context.Request.Params["search"];
                if (tmpSearch == null)
                    tmpSearch = " 1=1 ";
                else
                {
                    tmpSearch = " SectionName like '%" + tmpSearch + "%'";
                }
                string tmpSql = "";
                tmpSql = "Select SectionID,SectionName,XMin,XMax,YMin,YMax,Boundary,SectionInfo,Remark From X_ProjectSectionInfo Where SectionID like '" + tmpPrjID + "%' AND " + tmpSearch;
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该项目内没有定义河道工作范围\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            else if (method == "getsectioninfobyid")//根据河道编号获取河道分段信息
            {
                string tmpSectionID = context.Request.Params["sectionid"];
                if (tmpSectionID == null || tmpSectionID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"sectionid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSql = "";
                tmpSql = "Select SectionID,SectionName,XMin,XMax,YMin,YMax,Boundary,SectionInfo,Remark From X_ProjectSectionInfo Where SectionID ='" + tmpSectionID + "'";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"系统中没有定义该河道工作范围\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion

            #region 添加项目中设施
            else if (method == "addfacility")//添加项目中设施
            {
                string tmpSYSID = Guid.NewGuid().ToString("N").Replace("-", "").Trim();
                string tmpSectionID = context.Request.Params["sectionid"];
                string tmpName = context.Request.Params["name"];
                string tmpFacType = context.Request.Params["type"];

                string tmpXStr = context.Request.Params["x"];
                string tmpYStr = context.Request.Params["y"];
                string tmpZStr = context.Request.Params["z"];
                string tmpInfo = context.Request.Params["info"];
                string tmpReamrk = context.Request.Params["remark"];
                if (tmpReamrk == null) tmpReamrk = "";
                if (tmpInfo == null) tmpInfo = "";
                string tmpSql = "";
                tmpSql = string.Format("Insert Into X_FacilitiesInfo (SYSID,BelongSectionID,Name,FacilitiesType,X,Y,Z,Info,Remark) "
                    + "Values('{0}','{1}','{2}','{3}',{4},{5},{6},'{7}','{8}')", tmpSYSID,
                    tmpSectionID, tmpName, tmpFacType, tmpXStr, tmpYStr, tmpZStr, tmpInfo, tmpReamrk);
                if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql) > 0)
                {
                    AddLogInfo(tmpSectionID, tmpUser.usercode, "设施_添加", tmpSYSID);
                    string msg = "{\"code\":0,\"success\":true,\"msg\":\"" + tmpSYSID + "\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"添加设施失败\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion
            #region 编辑项目中设施
            else if (method == "editfacility")//编辑项目中设施
            {
                string tmpSYSID = context.Request.Params["sysid"];
                if (tmpSYSID == null || tmpSYSID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"sysid\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSectionID = context.Request.Params["sectionid"];
                if (tmpSectionID == null || tmpSectionID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"sectionid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpWhereSQL = "";
                Dictionary<string, string> tmpDict = new Dictionary<string, string>();
                string[] tmpFields = new string[] { "sectionid", "name", "type", "info", "remark" };
                string[] tmpField2s = new string[] { "BelongSectionID", "Name", "FacilitiesType", "Info", "Remark" };
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
                tmpFields = new string[] { "x", "y", "z" };
                foreach (string tmpField in tmpFields)
                {
                    string tmpParaValue = context.Request.Params[tmpField];
                    if (tmpParaValue != null)
                    {
                        if (tmpWhereSQL.Length > 0)
                            tmpWhereSQL += ",";
                        tmpWhereSQL += tmpField + "=" + tmpParaValue + "";
                    }
                }
                if (tmpWhereSQL.Length > 0)
                {
                    string tmpSql = "Update X_FacilitiesInfo Set " + tmpWhereSQL + "  Where sysid='" + tmpSYSID + "'";
                    if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql) > 0)
                    {
                        AddLogInfo(tmpSectionID, tmpUser.usercode, "设施_编辑", tmpWhereSQL.Replace("'", ""));
                        string msg = "{\"code\":0,\"success\":true,\"msg\":\"" + tmpSYSID + "\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"修改设施信息失败\"}";
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
            #endregion
            #region 删除项目中河道设施
            else if (method == "deletefacility")//删除项目中河道设施
            {
                string tmpSYSID = context.Request.Params["sysid"];
                if (tmpSYSID == null || tmpSYSID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"sysid\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSql = "";
                tmpSql = "Select ID,BelongSectionID From X_FacilitiesInfo Where sysid = '" + tmpSYSID + "'";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    string tmpStr = table.Rows[0].ItemArray[0].ToString();
                    string tmpSectionID = table.Rows[0].ItemArray[1].ToString();
                    tmpSql = "Delete From X_FacilitiesInfo Where ID = " + tmpStr;
                    if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql) > 0)
                    {
                        AddLogInfo(tmpSectionID, tmpUser.usercode, "设施_删除", "原SYSID为" + tmpSYSID);
                        string msg = "{\"code\":0,\"success\":true,\"msg\":\"" + tmpSYSID + "\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"删除设施失败\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该设施不存在\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion
            #region 查询项目中河道设施
            else if (method == "getfacilitiesinfobyprojectid")//获取项目中河道设施信息
            {
                string tmpPrjID = context.Request.Params["projectid"];

                string facilitesType = context.Request.Params["facilitiestype"];

                string SectionName = context.Request.Params["SectionName"];

               
               

                string sql = "";
                if(facilitesType == "" || facilitesType == null)
                {
                    sql = "1=1";
                }
                else
                {
                    sql = "FacilitiesType ='" + facilitesType + "' ";
                }

                if (tmpPrjID == null || tmpPrjID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                //if (tmpPrjID.Length > 12)
                //{
                //    tmpPrjID = tmpPrjID.Substring(0, 12);
                //}
                string tmpSearch = context.Request.Params["search"];
                if (tmpSearch == null)
                    tmpSearch = " 1=1 ";
                else
                {
                    tmpSearch = "( Name like '%" + tmpSearch + "%' OR  FacilitiesType like '%" + tmpSearch + "%')";
                }
                string tmpSql = "";

                if (SectionName == "" || SectionName == null)
                {
                    tmpSql = "Select SYSID,BelongSectionID,BelongSectionID As BelongSectionName,Name,FacilitiesType,X,Y,Z,Info,Remark From X_FacilitiesInfo Where BelongSectionID like '" + tmpPrjID + "%' AND " + tmpSearch + "  AND " + sql + "";
                }
                else
                {
                    tmpSql = "Select SYSID,BelongSectionID,BelongSectionID As BelongSectionName,Name,FacilitiesType,X,Y,Z,Info,Remark From X_FacilitiesInfo Where BelongSectionID like '" + SectionName + "%' AND " + tmpSearch + "  AND " + sql + "";
                }

                //tmpSql = "Select SYSID,BelongSectionID,BelongSectionID As BelongSectionName,Name,FacilitiesType,X,Y,Z,Info,Remark From X_FacilitiesInfo Where BelongSectionID like '" + tmpPrjID + "%' AND " + tmpSearch +"  AND "+ sql +" AND";
                string tmpType = context.Request.Params["type"];
                if (tmpType != null)
                    tmpSql += " AND FacilitiesType='" + tmpType + "'";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    ProjectInfoClass tmpPrjInfo = ProjectInfoClass.GetProjectInfo(tmpPrjID);
                    if (tmpPrjInfo != null)
                        tmpPrjInfo.ConvertTableColumnWithSection(table, "BelongSectionName");
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该项目内没有定义河道设施\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            else if (method == "getfacilityinfobyid")//根据河道编号获取河道设施信息
            {
                string tmpSysID = context.Request.Params["sysid"];
                if (tmpSysID == null || tmpSysID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"sysid\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSql = "";
                tmpSql = "Select SYSID,BelongSectionID,Name,FacilitiesType,X,Y,Z,Info,Remark From X_FacilitiesInfo Where SYSID ='" + tmpSysID + "'";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    string tmpPrjID = table.Rows[0].ItemArray[1].ToString().Substring(0, 12);
                    ProjectInfoClass tmpPrjInfo = ProjectInfoClass.GetProjectInfo(tmpPrjID);
                    if (tmpPrjInfo != null)
                        tmpPrjInfo.ConvertTableColumnWithSection(table, "BelongSectionName");
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"系统中没有定义该河道设施\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion

            #region 根据设施类型查询项目中河道设施中下拉框
            else if (method == "facilitiestypelist")
            {
                string tmpSql = "Select distinct FacilitiesType From X_FacilitiesInfo";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                JSON.JsonHelper Json = new JSON.JsonHelper();
                //转换为json格式
                string json = Json.DataTableToJsonForLayUi(table);
                string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                sendResponse(context, strFun, msg);
            }
            #endregion
            #region 根据项目获取对应河道下拉框
            else if (method == "riverwaylist")
            {
                string msg = "";
                string tmpPrjID = context.Request.Params["projectid"];
                if (tmpPrjID == null || tmpPrjID.Trim() == "")
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"sysid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSql = "Select SectionID ,SectionName From X_ProjectSectionInfo where SectionID like '"+ tmpPrjID + "%'";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                string json = "";
                if (table.Rows.Count < 1)
                {
                    msg = "{\"code\":0,\"success\":true,\"data\":0}";
                }
                else
                {
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    //转换为json格式
                    json = Json.DataTableToJsonForLayUi(table);
                    msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                }
                sendResponse(context, strFun, msg);
            }
            #endregion

            #region 添加项目计划
            else if (method == "addplan")//添加项目计划
            {
                string tmpSYSID = Guid.NewGuid().ToString("N").Replace("-", "").Trim();
                string tmpProjectID = context.Request.Params["projectid"];
                string tmpBelongProjectID = context.Request.Params["sectionid"];
                string tmpName = context.Request.Params["name"];
                string tmpType = context.Request.Params["type"];

                string tmpAssignName = context.Request.Params["assignuser"];
                string tmpStartTime = context.Request.Params["starttime"];
                string tmpEndTime = context.Request.Params["endtime"];

                string tmpPlanCount = context.Request.Params["plancount"];
                string tmpWorkloadValue = context.Request.Params["workload"];

                string tmpContent = context.Request.Params["content"];
                if (tmpContent == null) tmpContent = "";
                string tmpRemark = context.Request.Params["remark"];
                if (tmpRemark == null) tmpRemark = "";
                if (XJobHandler.CheckIsExistNullParam(new string[] { tmpProjectID, tmpBelongProjectID, tmpName, tmpType, tmpAssignName, tmpStartTime, tmpEndTime, tmpPlanCount }))
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"必要字段缺失\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSql = "";
                tmpSql = "Select ProjectManager From X_ProjectInfo Where ProjectID = '" + tmpProjectID + "'";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    string tmpProjectManager = table.Rows[0].ItemArray[0].ToString();
                    if (tmpProjectManager == tmpUser.usercode)
                    {
                        tmpSql = string.Format("Insert Into X_ProjectPlanInfo (SYSID,BelongProjectID,PlanName,PlanType,Creator,CreateTime,AssignUser,StartTime,EndTime,Content,PlanCount,WorkloadValue,Remark) "
                     + "Values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',{10},{11},'{12}')",
                     tmpSYSID, tmpBelongProjectID, tmpName, tmpType, tmpUser.usercode, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                     tmpAssignName, tmpStartTime, tmpEndTime, tmpContent, tmpPlanCount, tmpWorkloadValue, tmpRemark);
                        if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql) > 0)
                        {
                            AddLogInfo(tmpBelongProjectID, tmpUser.usercode, "项目计划_添加", tmpSYSID);
                            string msg = "{\"code\":0,\"success\":true,\"msg\":\"" + tmpSYSID + "\"}";
                            sendResponse(context, strFun, msg);
                            return;
                        }
                        else
                        {
                            string msg = "{\"code\":-1,\"success\":false,\"msg\":\"添加项目计划失败\"}";
                            sendResponse(context, strFun, msg);
                            return;
                        }
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"当前用户不是项目经理,不能新建项目计划\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"系统中没有该项目\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion
            #region 编辑项目计划
            else if (method == "editplan")//编辑项目计划
            {
                string tmpSYSID = context.Request.Params["sysid"];
                if (tmpSYSID == null || tmpSYSID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"sysid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }

                string tmpWhereSQL = "";
                Dictionary<string, string> tmpDict = new Dictionary<string, string>();
                string[] tmpFields = new string[] { "sectionid", "name", "type", "assignuser", "starttime", "endtime", "content", "remark" };
                string[] tmpField2s = new string[] { "BelongProjectID", "PlanName", "PlanType", "AssignUser", "StartTime", "EndTime", "Content", "Remark" };
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
                tmpFields = new string[] { "plancount", "workload" };
                tmpField2s = new string[] { "PlanCount", "WorkloadValue" };
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
                    string tmpSql;
                    tmpSql = "Select BelongProjectID,Creator From X_ProjectPlanInfo Where sysid='" + tmpSYSID + "' AND IsDone = 0";
                    DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                    if (table != null && table.Rows.Count > 0)
                    {
                        string tmpBelongProjectID = table.Rows[0].ItemArray[0].ToString();
                        string tmpManager = table.Rows[0].ItemArray[1].ToString();
                        if (tmpManager == tmpUser.usercode)
                        {
                            tmpSql = "Update X_ProjectPlanInfo Set " + tmpWhereSQL + "  Where sysid='" + tmpSYSID + "'";
                            if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql) > 0)
                            {
                                XJobHandler.CreateJobByPlanID(tmpSYSID);
                                AddLogInfo(tmpBelongProjectID, tmpUser.usercode, "项目计划_编辑", tmpSYSID);
                                string msg = "{\"code\":0,\"success\":true,\"msg\":\"" + tmpSYSID + "\"}";
                                sendResponse(context, strFun, msg);
                                return;
                            }
                            else
                            {
                                string msg = "{\"code\":-1,\"success\":false,\"msg\":\"编辑项目计划失败\"}";
                                sendResponse(context, strFun, msg);
                                return;
                            }
                        }
                        else
                        {
                            string msg = "{\"code\":-1,\"success\":false,\"msg\":\"当前用户不是项目经理,不能编辑项目计划\"}";
                            sendResponse(context, strFun, msg);
                            return;
                        }
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"系统中没有该项目或计划已经执行\"}";
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
            #endregion
            #region 执行项目计划
            else if (method == "exeplan")//执行项目计划
            {
                string tmpSYSID = context.Request.Params["sysid"];
                if (tmpSYSID == null || tmpSYSID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"sysid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }

                string tmpSql = "";
                tmpSql = "Select BelongProjectID,Creator From X_ProjectPlanInfo Where SYSID = '" + tmpSYSID + "' AND IsDone = 0";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    string tmpBelongProjectID = table.Rows[0].ItemArray[0].ToString();
                    string tmpManager = table.Rows[0].ItemArray[1].ToString();
                    if (tmpManager == tmpUser.usercode)
                    {
                        tmpSql = "Update X_ProjectPlanInfo Set IsDone = 1 Where SYSID='" + tmpSYSID + "'";
                        if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql) > 0)
                        {
                            XJobHandler.CreateJobByPlanID(tmpSYSID);
                            AddLogInfo(tmpBelongProjectID, tmpUser.usercode, "项目计划_执行", tmpSYSID);
                            string msg = "{\"code\":0,\"success\":true,\"msg\":\"" + tmpSYSID + "\"}";
                            sendResponse(context, strFun, msg);
                            return;
                        }
                        else
                        {
                            string msg = "{\"code\":-1,\"success\":false,\"msg\":\"执行项目计划失败\"}";
                            sendResponse(context, strFun, msg);
                            return;
                        }
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"当前用户不是项目经理,不能执行项目计划\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"系统中没有该项目或计划已经执行\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion
            #region 项目计划取消
            else if (method == "cancelplan")//项目计划取消
            {
                string tmpSYSID = context.Request.Params["sysid"];
                if (tmpSYSID == null || tmpSYSID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"sysid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }

                string tmpSql = "";
                tmpSql = "Select BelongProjectID,Creator From X_ProjectPlanInfo Where SYSID = '" + tmpSYSID + "' AND IsDone = 1";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    string tmpBelongProjectID = table.Rows[0].ItemArray[0].ToString();
                    string tmpManager = table.Rows[0].ItemArray[1].ToString();
                    if (tmpManager == tmpUser.usercode)
                    {
                        tmpSql = "Update X_ProjectPlanInfo Set IsDone = 0 Where SYSID='" + tmpSYSID + "'";
                        if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql) > 0)
                        {
                            //同时删除所有的工单
                            tmpSql = "Delete From X_Jobs Where PlanID = '" + tmpSYSID + "'";
                            DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql);

                            AddLogInfo(tmpBelongProjectID, tmpUser.usercode, "项目计划_取消", tmpSYSID);
                            string msg = "{\"code\":0,\"success\":true,\"msg\":\"" + tmpSYSID + "\"}";
                            sendResponse(context, strFun, msg);
                            return;
                        }
                        else
                        {
                            string msg = "{\"code\":-1,\"success\":false,\"msg\":\"执行项目计划失败\"}";
                            sendResponse(context, strFun, msg);
                            return;
                        }
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"当前用户不是项目经理,不能执行项目计划\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"系统中没有该项目或计划未执行\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion
            #region 删除项目计划
            else if (method == "deleteplan")//删除项目计划
            {
                string tmpSYSID = context.Request.Params["sysid"];
                if (tmpSYSID == null || tmpSYSID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"sysid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSql = "";
                tmpSql = "Select BelongProjectID,Creator From X_ProjectPlanInfo Where SYSID = '" + tmpSYSID + "'";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    string tmpBelongProjectID = table.Rows[0].ItemArray[0].ToString();
                    string tmpManager = table.Rows[0].ItemArray[1].ToString();
                    if (tmpManager == tmpUser.usercode)
                    {
                        tmpSql = "Delete From X_ProjectPlanInfo Where SYSID='" + tmpSYSID + "'";
                        if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql) > 0)
                        {
                            //同时删除所有的工单
                            tmpSql = "Delete From X_Jobs Where PlanID = '" + tmpSYSID + "'";
                            DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql);

                            AddLogInfo(tmpBelongProjectID, tmpUser.usercode, "项目计划_删除", tmpSYSID);
                            string msg = "{\"code\":0,\"success\":true,\"msg\":\"" + tmpSYSID + "\"}";
                            sendResponse(context, strFun, msg);
                            return;
                        }
                        else
                        {
                            string msg = "{\"code\":-1,\"success\":false,\"msg\":\"删除项目计划失败\"}";
                            sendResponse(context, strFun, msg);
                            return;
                        }
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"当前用户不是项目经理,不能删除项目计划\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"系统中没有该项目\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion
            #region 查询项目计划
            else if (method == "queryplanbysysid")//根据项目编码查询项目计划
            {
                string tmpSYSID = context.Request.Params["sysid"];
                if (tmpSYSID == null || tmpSYSID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"sysid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSql = "";
                tmpSql = "Select SYSID,BelongProjectID,PlanName,PlanType,Creator,Creator as CreatorName,AssignUser,AssignUser as AssignUserName,StartTime,EndTime,PlanCount,WorkloadValue,IsDone,Content From X_ProjectPlanInfo Where SYSID = '" + tmpSYSID + "'";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    string tmpBelongProjectID = table.Rows[0].ItemArray[1].ToString();
                    //string tmpManager = table.Rows[0].ItemArray[4].ToString();
                    //if (tmpManager == tmpUser.usercode)
                    {
                        SZRiverSys.Comm.CommonClass.ReplaceTableColumnsName(table, new string[] { "CreatorName", "AssignUserName" });//替换用户名为昵称
                        JSON.JsonHelper Json = new JSON.JsonHelper();
                        string json = Json.DataTableToJsonForLayUi(table);
                        string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                    //else
                    //{
                    //    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"当前用户不是项目经理,不能查看项目计划\"}";
                    //    sendResponse(context, strFun, msg);
                    //    return;
                    //}
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"系统中没有该项目\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            else if (method == "queryplansbyprojectid")//根据项目编码查询项目计划
            {
                string tmpprojectid = context.Request.Params["projectid"];
                if (tmpprojectid == null || tmpprojectid.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSearch = context.Request.Params["search"];
                if (tmpSearch == null)
                    tmpSearch = " 1=1 ";
                else
                {
                    tmpSearch = "( PlanName like '%" + tmpSearch + "%' OR  PlanType like '%" + tmpSearch + "%')";
                }

                string tmpUserCondition = "";
                string tmpSql = "SELECT ID FROM [dbo].[X_PartsInfo] Where charindex(PartID,'" + tmpprojectid + "')>0 AND (Manager='" + tmpUser.usercode + "' OR UsersList like '\"" + tmpUser.usercode + "\"')";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                    tmpUserCondition = " 1=1 ";
                else
                    tmpUserCondition = "(Creator='" + tmpUser.usercode + "' OR AssignUser='" + tmpUser.usercode + "') ";

                tmpSql = "Select SYSID,BelongProjectID,PlanName,PlanType,Creator,Creator as CreatorName,AssignUser, AssignUser as AssignUserName, StartTime,EndTime,PlanCount,WorkloadValue,IsDone,Content From X_ProjectPlanInfo Where BelongProjectID like '"
                    + tmpprojectid + "%' AND " + tmpUserCondition + " AND " + tmpSearch;
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
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"系统中没有该项目\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            else if (method == "queryplansbyprojectidwithtime")//根据项目编码查询项目计划(指定项目计划时间范围)
            {
                string tmpprojectid = context.Request.Params["projectid"];
                if (tmpprojectid == null || tmpprojectid.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpStartTime = context.Request.Params["starttime"];
                string tmpEndTime = context.Request.Params["endtime"];
                if (tmpStartTime == null || tmpStartTime.Trim() == ""
                    || tmpEndTime == null || tmpEndTime.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"starttime,endtime不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }

                string tmpUserCondition = "";
                string tmpSql = "SELECT ID FROM [dbo].[X_PartsInfo] Where charindex(PartID,'" + tmpprojectid + "')>0 AND (Manager='" + tmpUser.usercode + "' OR UsersList like '\"" + tmpUser.usercode + "\"')";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                    tmpUserCondition = " 1=1 ";
                else
                    tmpUserCondition = "(Creator='" + tmpUser.usercode + "' OR AssignUser='" + tmpUser.usercode + "') ";

                tmpSql = "";
                tmpSql = "Select SYSID,BelongProjectID,PlanName,PlanType,Creator,Creator as CreatorName,AssignUser, AssignUser as AssignUserName,StartTime,EndTime,PlanCount,WorkloadValue,IsDone,Content From X_ProjectPlanInfo Where BelongProjectID like '"
                        + tmpprojectid + "%' AND " + tmpUserCondition + " AND StartTime>='" + tmpStartTime + "' AND EndTime<='" + tmpEndTime + "'";
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
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"系统中没有该项目\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion
            #region 查询本人未读信息
            else if (method == "queryunreadmessages")// 查询本人未读信息
            {
                string msg;
                JSON.JsonHelper Json = new JSON.JsonHelper();
                string tmpSql = "";
                string tmpData01 = "", tmpData02 = "[]", tmpData03 = "[]";
                DataTable table;
                //获取事件信息
                tmpSql = "Select SYSID,BelongSectionID,EventName,EventType,UploadTime,EmergencyType,Content"
                        + " From X_EventsInfo Where IsRead = 0 AND AssignUser='" + tmpUser.usercode + "'";
                table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                    tmpData01 = Json.DataTableToJsonForLayUi(table);

                if (tmpData01 == "")
                {
                    tmpData01 = "0";
                }
                string tmpTimeStr = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                //获取工单信息
                tmpSql = "Select SYSID,BelongSectionID,JobName,JobType,PlanStartTime,PlanEndTime,Content"
                         + " From X_Jobs Where AssignUser='" + tmpUser.usercode + "' AND PlanStartTime<'" + tmpTimeStr + "' AND PlanEndTime>'" + tmpTimeStr + "' AND CurrentProgress < 100";
                table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                    tmpData02 = Json.DataTableToJsonForLayUi(table);

                if (tmpData02 == "[]")
                {
                    tmpData02 = "0";
                }
                msg = "{\"code\":0,\"success\":true,\"data\":{\"event\":" + tmpData01 + ",\"job\":" + tmpData02 + "}}";
                //msg = "{\"code\":0,\"success\":true,\"data\":{\"event\":45646,\"job\":112313}}";
                sendResponse(context, strFun, msg);
                return;
            }
            #endregion
            #region 查询项目中设施,工单，事件类型
            else if (method == "getprojecttypeenum")//查询项目中设施,工单，事件类型
            {
                string tmpType = context.Request.Params["type"];
                if (tmpType == null || tmpType.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"tmpType不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSql = "";
                tmpSql = "Select TypeName From X_ProjectTypeEnum Where ClassName='" + tmpType + "'";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"系统中没有定义该类型\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion

            #region 获取河道相应点位
            else if (method == "riverdot")
            {
                string tmpsectionID = context.Request.Params["sectionid"];
                string sql = string.Format("select name from X_FacilitiesInfo where BelongSectionID = '"+ tmpsectionID + "'");
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(sql);
                if (table != null && table.Rows.Count > 0)
                {
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"没有sectionid\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion

            #region 添加河道水质检测照片的存储氨氮照片
            else if (method == "addammonianitrogenfilenames")
            {
                string tmpsectionID = context.Request.Params["sectionid"];
                if (tmpsectionID == null || tmpsectionID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"tmpsectionID不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string ammoniaNitrogenFileNames = "";//氨氮照片
                //文件,照片
                HttpFileCollection hfc = context.Request.Files;
                HttpPostedFile hpf = null;
                bool errormsg = true;
                string errormsgstr = "";
                if (hfc.Count > 0)
                {
                    string tmpSaveFolder = HttpContext.Current.Server.MapPath("~/Upfile/waterQuality");
                    if (!Directory.Exists(tmpSaveFolder))
                        Directory.CreateDirectory(tmpSaveFolder);

                    for (int i = 0; i < hfc.Count; i++)
                    {
                        hpf = context.Request.Files[i];
                        string date = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        var filetype = hpf.FileName.Split('.')[1];
                        var filename = "/Upfile/waterQuality/" + tmpsectionID + "_" + tmpUser.usercode + "-" + date + "." + filetype;
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
                        ammoniaNitrogenFileNames += filename + ";";
                    }
                }
                if (errormsg)
                {
                    string msg = "{\"code\":0,\"success\":true,\"msg\":\"上传成功\",\"ammoniaNitrogenFileNames\":\"" + ammoniaNitrogenFileNames + "\"}";
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
            #endregion
            #region 添加河道水质检测照片的存储总磷照片
            else if (method == "addphosphorusfilenames")
            {
                string tmpsectionID = context.Request.Params["sectionid"];
                if (tmpsectionID == null || tmpsectionID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"tmpsectionID不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string phosphorusFileNames = "";//总磷照片
                //文件,照片
                HttpFileCollection hfc = context.Request.Files;
                HttpPostedFile hpf = null;
                bool errormsg = true;
                string errormsgstr = "";
                if (hfc.Count > 0)
                {
                    string tmpSaveFolder = HttpContext.Current.Server.MapPath("~/Upfile/waterQuality");
                    if (!Directory.Exists(tmpSaveFolder))
                        Directory.CreateDirectory(tmpSaveFolder);

                    for (int i = 0; i < hfc.Count; i++)
                    {
                        hpf = context.Request.Files[i];
                        string date = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        var filetype = hpf.FileName.Split('.')[1];
                        var filename = "/Upfile/waterQuality/" + tmpsectionID + "_" + tmpUser.usercode + "-" + date + "." + filetype;
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
                        phosphorusFileNames += filename + ";";
                    }
                }
                if (errormsg)
                {
                    string msg = "{\"code\":0,\"success\":true,\"msg\":\"上传成功\",\"phosphorusFileNames\":\"" + phosphorusFileNames + "\"}";
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
            #endregion
            #region 添加河道水质检测照片的存储现场照片
            else if (method == "addsitefilenames")
            {
                string tmpsectionID = context.Request.Params["sectionid"];
                if (tmpsectionID == null || tmpsectionID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"tmpsectionID不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string siteFileNames = "";//现场照片
                //文件,照片
                HttpFileCollection hfc = context.Request.Files;
                HttpPostedFile hpf = null;
                bool errormsg = true;
                string errormsgstr = "";
                if (hfc.Count > 0)
                {
                    string tmpSaveFolder = HttpContext.Current.Server.MapPath("~/Upfile/waterQuality");
                    if (!Directory.Exists(tmpSaveFolder))
                        Directory.CreateDirectory(tmpSaveFolder);

                    for (int i = 0; i < hfc.Count; i++)
                    {
                        hpf = context.Request.Files[i];
                        string date = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        var filetype = hpf.FileName.Split('.')[1];
                        var filename = "/Upfile/waterQuality/" + tmpsectionID + "_" + tmpUser.usercode + "-" + date + "." + filetype;
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
                        siteFileNames += filename + ";";
                    }
                }
                if (errormsg)
                {
                    string msg = "{\"code\":0,\"success\":true,\"msg\":\"上传成功\",\"siteFileNames\":\"" + siteFileNames + "\"}";
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
            #endregion
            #region 添加河道水质检测照片的存储数据上传
            else if (method == "addwaterqualityphotos")
            {
                string tmpsectionID = context.Request.Params["sectionid"];
                if (tmpsectionID == null || tmpsectionID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"tmpsectionID不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string ammoniaNitrogenNum = context.Request.Params["ammonianitrogennum"];//氨氮数值
                string PhosphorusNumerical = context.Request.Params["phosphorusnumerical"];//磷数值
                string ammoniaNitrogenFileNames = context.Request.Params["ammoniaNitrogenFileNames"];//氨氮照片
                string phosphorusFileNames = context.Request.Params["phosphorusFileNames"];//总磷照片
                string siteFileNames = context.Request.Params["siteFileNames"];//现场照片
                string comment = context.Request.Params["comment"];//备注
                string dot = context.Request.Params["riverdot"];//河道点位

                string sql = string.Format("insert into X_WaterQualityPictures(SectionID,ammoniaNitrogenNum,phosphorusNumerical,ammoniaNitrogenFileNames" +
                           ",phosphorusFileNames,siteFileNames,comment,uploadTime,dot) values('" + tmpsectionID + "'," + ammoniaNitrogenNum + "," + PhosphorusNumerical + "" +
                           ",'" + ammoniaNitrogenFileNames + "','" + phosphorusFileNames + "','" + siteFileNames + "','" + comment + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "'" +
                           ",'" + dot + "')");
                int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(sql);
                if (count > 0)
                {
                    string msg = "{\"code\":0,\"success\":true,\"msg\":\"上传成功\"}";
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
            #endregion

            #region 查询水质检测照片
            else if (method == "findwaterqualityphotos")
            {
                string tmpsectionID = context.Request.Params["sectionid"];//河道id
                string tmpStartTime = context.Request.Params["uploadtime"];//上传时间

                string sql = string.Format("SELECT dot,ammoniaNitrogenNum,phosphorusNumerical,ammoniaNitrogenFileNames,phosphorusFileNames,siteFileNames," +
                    "comment,uploadTime FROM X_WaterQualityPictures WHERE SectionID = '" + tmpsectionID + "' AND uploadtime = '" + tmpStartTime + "'");
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(sql);
                if (table != null && table.Rows.Count > 0)
                {
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"指定时间内没有数据\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }


            }

            #endregion


            #region 初始化缓存数据
            else if (method == "initialdata")
            {
                ProjectInfoClass.Initial();
                SectionClass.Initial();
            }
            #endregion

        }

        /// <summary>
        /// 添加日志信息
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="username"></param>
        /// <param name="logType"></param>
        /// <param name="logInfo"></param>
        public static void AddLogInfo(string projectID, string username, string logType, string logInfo)
        {
            string tmpSql = string.Format("Insert into X_ProjectLog (ProjectID,ModifyUser,ModifyTime,LogType,LogInfo) Values ('{0}','{1}','{2}','{3}','{4}')", projectID, username,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), logType, logInfo);
            DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql);
        }
        /// <summary>
        /// 根据用户名获取其在所有部门中的最高等级部门ID
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static string GetMinPartIDByUserName(string username)
        {
            string tmpMinPartID = "";
            string tmpSql = "Select Min(PartID) From X_PartsInfo Where Manager='" + username + "' OR UsersList like '%\"" + username + "\"%'";
            DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
            if (table != null && table.Rows.Count > 0)
            {
                tmpMinPartID = table.Rows[0].ItemArray[0].ToString();
            }
            return tmpMinPartID;
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