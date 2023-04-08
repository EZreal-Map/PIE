﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using SZRiverSys.Model;

namespace SZRiverSys.ServiceHandler
{
    /// <summary>
    /// XPatrolHandler 的摘要说明
    /// </summary>
    public class XPatrolHandler : IHttpHandler
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

            #region 开始巡河
            if (method == "patrolstart")// 开始巡河
            {
                string tmpPrjID = context.Request.Params["projectid"];
                if (tmpPrjID == null || tmpPrjID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                ProjectInfoClass tmpPrjInfo = ProjectInfoClass.GetProjectInfo(tmpPrjID);
                if (tmpPrjInfo != null)
                {
                    string tmpSysID = Guid.NewGuid().ToString("N");
                    string tmpSql = string.Format("Insert Into X_PatrolInfo (SYSID,BelongProjectID,UserName ,StartTime, EndTime ,TotalPatrolTime ,TotalPatrolDistance) "
                            + "Values('{0}','{1}','{2}','{3}','{4}',{5},{6})",
                            tmpSysID, tmpPrjID, tmpUser.usercode,
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "0","0");
                    if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql) > 0)
                    {
                        XProjectHandler.AddLogInfo(tmpPrjID, tmpUser.usercode, "巡河_开始", tmpSysID);
                        string msg = "{\"code\":0,\"success\":true,\"msg\":\"" + tmpSysID + "\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"开始巡河失败.\"}";
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
            #region 更新巡河信息
            if (method == "patrolupdate")// 更新巡河信息
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
                string[] tmpFields = new string[] { "patroltime", "patroldistance" };
                string[] tmpField2s = new string[] { "TotalPatrolTime", "TotalPatrolDistance" };
                int tmpFieldsLen = tmpFields.Length;
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
                    tmpWhereSQL += ",EndTime='"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    string tmpSql = "Update X_PatrolInfo Set " + tmpWhereSQL + "  Where SYSID='" + tmpSYSID + "'";
                    if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql) > 0)
                    {                       
                        string msg = "{\"code\":0,\"success\":true,\"msg\":\"" + tmpSYSID + "\"}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"更新信息失败\"}";
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
            #region 删除巡河信息
            if (method == "patroldelete")// 删除巡河信息
            {
                string tmpSYSID = context.Request.Params["sysid"];
                if (tmpSYSID == null || tmpSYSID.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"sysid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSql = "Select ID From X_PatrolInfo Where UserName='" + tmpUser.usercode + "' AND SYSID='" + tmpSYSID + "'";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    tmpSql = "Delete From X_PatrolInfo Where ID = "+ table.Rows[0].ItemArray[0].ToString();
                    DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql);
                    string msg = "{\"code\":0,\"success\":true,\"data\":\"删除巡河记录成功.\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该巡河记录不存在\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion
            #region 查询巡河信息
            else if (method == "querypatrolinfobytime")
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
                    string tmpSql = "Select SYSID,BelongProjectID,UserName,UserName AS NickName,StartTime, EndTime ,TotalPatrolTime ,TotalPatrolDistance"
                        + " From X_PatrolInfo Where UserName='" + tmpUser.usercode + "' AND BelongProjectID like '" + tmprojectid + "%' AND StartTime>'" + tmpStartTime + "' AND EndTime<'" + tmpEndTime + "'";
                    DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                    if (table != null && table.Rows.Count > 0)
                    {
                        SZRiverSys.Comm.CommonClass.ReplaceTableColumnsName(table, new string[] { "NickName" });//替换用户名为昵称
                        JSON.JsonHelper Json = new JSON.JsonHelper();
                        string json = Json.DataTableToJsonForLayUi(table);
                        string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                        sendResponse(context, strFun, msg);
                        return;
                    }
                    else
                    {
                        string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该项目指定时间范围内没有需要查询的巡河信息\"}";
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
            else if (method == "querymypatrolinfobytime")
            {
                string tmpStartTime = context.Request.Params["starttime"];
                string tmpEndTime = context.Request.Params["endtime"];
                if (XJobHandler.CheckIsExistNullParam(new string[] { tmpStartTime, tmpEndTime }))
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"必要字段缺失\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSql = "Select SYSID,BelongProjectID,UserName,UserName AS NickName,StartTime, EndTime ,TotalPatrolTime ,TotalPatrolDistance"
                        + " From X_PatrolInfo Where UserName='" + tmpUser.usercode + "' AND StartTime>='" + tmpStartTime + "' AND EndTime<='" + tmpEndTime + "'";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    SZRiverSys.Comm.CommonClass.ReplaceTableColumnsName(table, new string[] { "NickName" });//替换用户名为昵称
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                else
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"该项目指定时间范围内没有需要查询的巡河信息\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion


            #region 统计巡河信息
            else if (method == "statisticsbytime")// 统计指定时间内的所有数据
            {
                string msg = "";
                string tmprojectid = context.Request.Params["projectid"];
                if (tmprojectid == null || tmprojectid.Trim() == "")
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpStartTime = context.Request.Params["starttime"];
                string tmpEndTime = context.Request.Params["endtime"];
                string jobType = context.Request.Params["jobtype"];
                string sql = "";
                if (jobType == "" || jobType == null)
                {
                    sql = " AND 1=1";
                }
                else
                {
                    sql = "AND JobType = '" + jobType + "'";
                }

                if (XJobHandler.CheckIsExistNullParam(new string[] { tmpStartTime, tmpEndTime}))
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"必要字段缺失\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }

                //指定时间内所有以及指定的工单数量
                string tmpSql = "Select SYSID,BelongSectionID,JobName,JobType,Creator,Creator as CreatorName,CreateTime,AssignUser,AssignUser as AssignUserName,AssociateUsers,PlanStartTime,PlanEndTime,Content,PlanWorkloadValue,FinishTime,FinishContent,FinishTimeRecords,CurrentProgress,TrueWorkloadValue,FilesInfo,Score,Comment,CommentTime,CommentFilesInfo,Remark"
                     + " From X_Jobs Where  BelongSectionID like '" + tmprojectid + "%' AND PlanStartTime>'" + tmpStartTime + "' AND PlanStartTime<'" + tmpEndTime + "' " + sql + "";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                int jobCount = jobCount = table.Rows.Count;

                //统计指定时间内的所有工作人数
                int workPersonNum = 0;
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    string person = table.Rows[i]["AssociateUsers"].ToString();
                    if (person != "")
                    {
                        if (person.Contains(","))
                        {
                            person = (person.TrimStart(',')).TrimEnd(',');
                            workPersonNum += person.Split(',').Length;
                        }
                        else
                        {
                            workPersonNum += 1;
                        }
                    }
                    else
                    {
                        workPersonNum += 0;
                    }
                }


                //指定时间内以及指定类型未完成的工单数量
                tmpSql = "Select SYSID,BelongSectionID,JobName,JobType,Creator,Creator as CreatorName,CreateTime,AssignUser,AssignUser as AssignUserName,AssociateUsers,PlanStartTime,PlanEndTime,Content,PlanWorkloadValue,FinishTime,FinishContent,FinishTimeRecords,CurrentProgress,TrueWorkloadValue,FilesInfo,Score,Comment,CommentTime,CommentFilesInfo,Remark"
                     + " From X_Jobs Where BelongSectionID like '" + tmprojectid + "%' AND PlanStartTime>'" + tmpStartTime + "' AND PlanStartTime<'" + tmpEndTime + "' AND CurrentProgress < 100  " + sql + " ";
                DataTable jobtable = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                int jobUNCount = jobtable.Rows.Count;

                //统计指定时间内的巡河长度
                tmpSql = "Select SYSID,BelongProjectID,UserName,UserName AS NickName,StartTime, EndTime ,TotalPatrolTime ,TotalPatrolDistance"
                + " From X_PatrolInfo Where  BelongProjectID like '" + tmprojectid + "%' AND StartTime>'" + tmpStartTime + "' AND EndTime<'" + tmpEndTime + "'";
                DataTable Patroltable = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                float TotalPatrolDistance = 0;//巡河长度
                workPersonNum += Patroltable.Rows.Count;
                if (Patroltable != null && Patroltable.Rows.Count > 0)
                {
                    for (int i = 0; i < Patroltable.Rows.Count; i++)
                    {
                        TotalPatrolDistance += Convert.ToSingle(Patroltable.Rows[i]["TotalPatrolDistance"]);
                    }
                }

                //统计指定时间内的所有事件
                tmpSql = "Select SYSID,BelongSectionID,EventName,EventType,UploadUser,UploadUser as UploadUserName,UploadTime,AssignUser,AssignUser as AssignUserName,EmergencyType,Content,FilesInfo,X,Y,Z,IsRead,ReadTime"
                + " From X_EventsInfo Where  BelongSectionID like '" + tmprojectid + "%' AND UploadTime>'" + tmpStartTime + "' AND UploadTime<'" + tmpEndTime + "'";
                DataTable eventtable = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                workPersonNum += eventtable.Rows.Count;
                int eventCount = eventtable.Rows.Count;

                //统计指定时间内的未读事件
                tmpSql = "Select SYSID,BelongSectionID,EventName,EventType,UploadUser,UploadUser as UploadUserName,UploadTime,AssignUser,AssignUser as AssignUserName,EmergencyType,Content,FilesInfo,X,Y,Z,IsRead,ReadTime"
                + " From X_EventsInfo Where BelongSectionID like '" + tmprojectid + "%' AND UploadTime>'" + tmpStartTime + "' AND UploadTime<'" + tmpEndTime + "' AND IsRead = 0";
                DataTable eventUntable = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                int eventUNCount = eventUntable.Rows.Count;

                //返回数据
                msg = "{\"code\":0,\"success\":true,\"workPersonNum\":" + workPersonNum + ",\"TotalPatrolDistance\":" + TotalPatrolDistance + ",\"jobCount\":" + jobCount + ",\"jobUNCount\":" + jobUNCount + ",\"eventCount\":" + eventCount + ",\"eventUNCount\":" + eventUNCount + "}";
                sendResponse(context, strFun, msg);
                return;

                
            }
            #endregion
           
            #region 统计巡河信息时的工单类型下拉框
            else if (method == "jobtypelist")
            {
                string tmpSql = "Select distinct JobType From X_Jobs";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                JSON.JsonHelper Json = new JSON.JsonHelper();
                //转换为json格式
                string json = Json.DataTableToJsonForLayUi(table);
                string msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                sendResponse(context, strFun, msg);
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