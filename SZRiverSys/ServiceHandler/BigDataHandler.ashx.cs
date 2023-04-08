﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using SZRiverSys.Model;

namespace SZRiverSys.ServiceHandler
{
    /// <summary>
    /// BigDataHandler 的摘要说明
    /// </summary>
    public class BigDataHandler : IHttpHandler
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
            context.Response.ContentType = "text/plain";
            bool result = false;
            string msg = "";
            string strFun = context.Request["jsonpcallback"];//传递参数
            string method = context.Request["method"];
            if (method == "getlistcount")//获取工单数量
            {
                string tmpTimeStr01 = context.Request.Params["starttime"];
                string tmpTimeStr02 = context.Request.Params["endtime"];
                int tmpCount01 = 0, tmpCount02 = 0;
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable("Select Count(ID) From Business_flow Where StartTime >='" + tmpTimeStr01 + "' And EndTime<'"+ tmpTimeStr02 + "'");
                if (table != null && table.Rows.Count > 0)
                    tmpCount01 = int.Parse(table.Rows[0].ItemArray[0].ToString());
                table = DBHelper.DBHelperMsSql.ExecuteDataTable("Select Count(ID) From Business_flow Where StartTime >='" + tmpTimeStr01 + "' And EndTime<'" + tmpTimeStr02 + "' AND FinishTime is Not NULL");
                if (table != null && table.Rows.Count > 0)
                    tmpCount02 = int.Parse(table.Rows[0].ItemArray[0].ToString());

                string tmpContent = "{ \"success\":true,\"msg\": {\"totalcount\":" + tmpCount01.ToString()+ ",\"undocount\":" + tmpCount02.ToString()+ "}}";
                sendResponse(context, strFun, tmpContent);
                return;
            }
            else if (method == "getlistcount2")//获取工单数量
            {
                string tmpTimeStr01 = context.Request.Params["starttime"];
                string tmpTimeStr02 = context.Request.Params["endtime"];
               
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable("Select Business_Type, Count(ID) From Business_flow Where StartTime >='" + tmpTimeStr01 + "' And EndTime<'" + tmpTimeStr02 + "' GROUP BY Business_Type");
                if (table != null && table.Rows.Count > 0)
                {
                    Dictionary<string, string[]> tmpDict = new Dictionary<string, string[]>();
                    foreach (DataRow tmpRow in table.Rows)
                    {
                        string[] tmpStrs01 = new string[2];
                        tmpStrs01[0] = tmpRow.ItemArray[1].ToString();
                        tmpStrs01[1] = "0";
                        tmpDict.Add(tmpRow.ItemArray[0].ToString(), tmpStrs01);
                    }
                    DataTable table2 = DBHelper.DBHelperMsSql.ExecuteDataTable("Select Business_Type, Count(ID) From Business_flow Where StartTime >='" + tmpTimeStr01 + "' And EndTime<'" + tmpTimeStr02 + "' AND FinishTime is Not NULL GROUP BY Business_Type");
                    if (table2 != null && table2.Rows.Count > 0)
                    {
                        foreach (DataRow tmpRow in table2.Rows)
                        {
                            if(tmpDict.ContainsKey(tmpRow.ItemArray[0].ToString()))
                            {
                                string[] tmpStrs01 = tmpDict[tmpRow.ItemArray[0].ToString()];
                                tmpStrs01[1] = tmpRow.ItemArray[1].ToString();
                            }
                        }
                    }
                    string tmpContent2 = "";
                    foreach(KeyValuePair<string,string[]> tmpPiar in tmpDict)
                    {
                        if (tmpContent2.Length > 0)
                            tmpContent2 += ",";
                        tmpContent2 += "{\"name\":\""+ tmpPiar.Key + "\",\"totalcount\":" + tmpPiar.Value[0] + ",\"undocount\":" + tmpPiar.Value[1] + "}";
                    }
                    string tmpContent = "{ \"success\":true,\"msg\": ["+ tmpContent2 + "]}";
                    sendResponse(context, strFun, tmpContent);
                    return;
                }
            }
            else if (method == "geteventcount")//获取事件数量
            {
                string tmpTimeStr01 = context.Request.Params["starttime"];
                string tmpTimeStr02 = context.Request.Params["endtime"];
                int tmpCount01 = 0, tmpCount02 = 0;
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable("Select Count(ID) From Business_WorkForm Where CreateTime >='" + tmpTimeStr01 + "' And CreateTime<='" + tmpTimeStr02 + "'");
                if (table != null && table.Rows.Count > 0)
                    tmpCount01 = int.Parse(table.Rows[0].ItemArray[0].ToString());
                table = DBHelper.DBHelperMsSql.ExecuteDataTable("Select Count(ID) From Business_WorkForm Where CreateTime >='" + tmpTimeStr01 + "' And CreateTime<='" + tmpTimeStr02 + "' AND FinishTime is Not NULL");
                if (table != null && table.Rows.Count > 0)
                    tmpCount02 = int.Parse(table.Rows[0].ItemArray[0].ToString());

                string tmpContent = "{\"success\":true,\"msg\": {\"totalcount\":" + tmpCount01.ToString() + ",\"undocount\":" + tmpCount02.ToString() + "}}";
                sendResponse(context, strFun, tmpContent);
                return;
            }
            else if (method == "geteventcount2")//获取工单数量
            {
                string tmpTimeStr01 = context.Request.Params["starttime"];
                string tmpTimeStr02 = context.Request.Params["endtime"];

                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable("Select Type, Count(ID) From Business_WorkForm Where CreateTime >='" + tmpTimeStr01 + "' And CreateTime<'" + tmpTimeStr02 + "' GROUP BY Type");
                if (table != null && table.Rows.Count > 0)
                {
                    Dictionary<string, string[]> tmpDict = new Dictionary<string, string[]>();
                    foreach (DataRow tmpRow in table.Rows)
                    {
                        string[] tmpStrs01 = new string[2];
                        tmpStrs01[0] = tmpRow.ItemArray[1].ToString();
                        tmpStrs01[1] = "0";
                        tmpDict.Add(tmpRow.ItemArray[0].ToString(), tmpStrs01);
                    }
                    DataTable table2 = DBHelper.DBHelperMsSql.ExecuteDataTable("Select Type, Count(ID) From Business_WorkForm Where CreateTime >='" + tmpTimeStr01 + "' And CreateTime<'" + tmpTimeStr02 + "' AND FinishTime is Not NULL GROUP BY Type");
                    if (table2 != null && table2.Rows.Count > 0)
                    {
                        foreach (DataRow tmpRow in table2.Rows)
                        {
                            if (tmpDict.ContainsKey(tmpRow.ItemArray[0].ToString()))
                            {
                                string[] tmpStrs01 = tmpDict[tmpRow.ItemArray[0].ToString()];
                                tmpStrs01[1] = tmpRow.ItemArray[1].ToString();
                            }
                        }
                    }
                    string tmpContent2 = "";
                    foreach (KeyValuePair<string, string[]> tmpPiar in tmpDict)
                    {
                        if (tmpContent2.Length > 0)
                            tmpContent2 += ",";
                        tmpContent2 += "{\"name\":\"" + tmpPiar.Key + "\",\"totalcount\":" + tmpPiar.Value[0] + ",\"undocount\":" + tmpPiar.Value[1] + "}";
                    }
                    string tmpContent = "{ \"success\":true,\"msg\": [" + tmpContent2 + "]}";
                    sendResponse(context, strFun, tmpContent);
                    return;
                }
            }
            else if (method == "geteventsinfo")//获取指定时间段范围内的事件信息
            {
                string tmpTimeStr01 = context.Request.Params["starttime"];
                string tmpTimeStr02 = context.Request.Params["endtime"];
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable("Select WorkName,Type,Content,Assignor,Emergencylevel,PlanFinishTime,CreateTime,Creater,state,FinishTime,FinishExplain,lng,lat From Business_WorkForm Where CreateTime >='" + tmpTimeStr01 + "' And CreateTime<='" + tmpTimeStr02 + "'  Order By CreateTime DESC");
                if (table != null && table.Rows.Count > 0)
                {
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpContent = "{\"code\":-1,\"success\":false}";
                sendResponse(context, strFun, tmpContent);
                return;
            }
            else if (method == "getlasteventsinfo")//获取指定时间段范围内的事件信息
            {
                string tmpCount = context.Request.Params["count"];
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable("Select Top "+ tmpCount + " WorkName,Type,Content,Assignor,Emergencylevel,PlanFinishTime,CreateTime,Creater,state,FinishTime,FinishExplain,lng,lat From Business_WorkForm Order By CreateTime DESC");
                if (table != null && table.Rows.Count > 0)
                {
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpContent = "{\"code\":-1,\"success\":false}";
                sendResponse(context, strFun, tmpContent);
                return;
            }
            else if (method == "getuserfinalgps")//获取所有用户的最后定位信息
            {
                string tmpPrjID = context.Request.Params["projectid"];
                ProjectInfoClass tmpProjectInfoClass = ProjectInfoClass.GetProjectInfo(tmpPrjID);
                string tmpUsersList = tmpProjectInfoClass.AllUsersListStr;

                string tmpsql = @"select a.*,b.userName as RealName,case when datediff(minute,gpsTime,getdate())>5 then '2'else '1' end as state  from [dbo].[UserGPSFinal] a
                                   left join Sys_Accounts_Users b on a.username COLLATE Chinese_PRC_CI_AS =b.usercode";
                if (tmpUsersList.Length > 0)
                    tmpsql += "  where a.username in ("+ tmpUsersList + ") ";

                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql); 
                if (table != null && table.Rows.Count > 0)
                {
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpContent = "{\"code\":-1,\"success\":false}";
                sendResponse(context, strFun, tmpContent);
                return;
            }
            else if (method == "getusergpsbytime")//查询用户轨迹
            {
                SQL.GPSSql sql = new SQL.GPSSql();
                JSON.JsonHelper tojson = new JSON.JsonHelper();
                string username = context.Request["username"];
                string starttime = context.Request["starttime"];
                string endtime = context.Request["endtime"];
                string tmpAccuracy = context.Request["accuracy"];
                if (tmpAccuracy == null)
                    tmpAccuracy = "";
                if (starttime != null && endtime != null)
                {
                    starttime = starttime.Replace('_', ' ');
                    endtime = endtime.Replace('_', ' ');
                    DataTable dt = sql.GetUserGPSTrack(username, starttime, endtime);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        string json = tojson.DataTableToJsonForLayUi(dt);
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
                sendResponse(context, strFun, msg);
                return;
            }
            else if (method == "gettyphoondata")//获取台风数据
            {
                DateTime tmpTime = DateTime.Now;
                string tmpContent = LoadData(@"http://tf.121.com.cn/nodeService/GetTyphoonByYear/"+ tmpTime.Year.ToString()+ "?"+ tmpTime.ToString("yyyyMMdd"));
                sendResponse(context, strFun, tmpContent);
                return;
            }
            else if (method == "gettyphoongisdata")//获取台风数据
            {
                string tmpID = context.Request.Params["id"];
                DateTime tmpTime = DateTime.Now;
                string tmpContent = LoadData(@"http://tf.121.com.cn/nodeService/GetTyRealRoute/" + tmpID + "?" + tmpTime.ToString("yyyyMMdd"));
                sendResponse(context, strFun, tmpContent);
                return;
            }
            else if (method == "geturldata")//获取URL数据
            {
                string tmpURL = context.Request.Params["url"];
                string tmpContent = LoadData(tmpURL);
                sendResponse(context, strFun, tmpContent);
                return;
            }
            else if (method == "getlastworksinfo")//获取最后指定数量的工单信息
            {
                string tmpCount = context.Request.Params["count"];
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable("Select Top " + tmpCount + " Business_Type,Creater,StartTime,EndTime,Assignor,FinishTime,FinishExplain,Score,Comment From Business_flow Order By StartTime DESC");
                if (table != null && table.Rows.Count > 0)
                {
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpContent = "{\"code\":-1,\"success\":false}";
                sendResponse(context, strFun, tmpContent);
                return;
            }

            #region 查询指定时间范围内巡河记录信息
            else if (method == "getpatrolstatisticdata")//查询指定时间范围内巡河记录信息
            {
                string tmprojectid = context.Request.Params["projectid"];
                if (tmprojectid == null || tmprojectid.Trim() == "")
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpStartTime = context.Request.Params["starttime"];
                string tmpEndTime = context.Request.Params["endtime"];
                if (XJobHandler.CheckIsExistNullParam(new string[] { tmpStartTime, tmpEndTime }))
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"必要字段缺失\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }

                string tmpSql;
                tmpSql = "Select UserName,UserName as NickName,SUM(TotalPatrolDistance) As TotalPatrolDistance,SUM(TotalPatrolTime) as TotalPatrolTime From X_PatrolInfo Where BelongProjectID='"
                    + tmprojectid + "' AND StartTime>'"+ tmpStartTime + "' AND EndTime<'"+ tmpEndTime + "' Group By UserName";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    SZRiverSys.Comm.CommonClass.ReplaceTableColumnsName(table, new string[] { "NickName" });//替换用户名为昵称
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpContent = "{\"code\":-1,\"success\":false}";
                sendResponse(context, strFun, tmpContent);
                return;
            }
            #endregion
            #region 查询指定时间范围内已完成工单统计信息
            else if (method == "getjobfinishstatisticdata")//查询指定时间范围内已完成工单统计信息
            {
                string tmprojectid = context.Request.Params["projectid"];
                if (tmprojectid == null || tmprojectid.Trim() == "")
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpStartTime = context.Request.Params["starttime"];
                string tmpEndTime = context.Request.Params["endtime"];
                if (XJobHandler.CheckIsExistNullParam(new string[] { tmpStartTime, tmpEndTime }))
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"必要字段缺失\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }

                string tmpSql;
                tmpSql = "Select AssignUser,AssignUser as NickName,COUNT(*) As TotalCount From X_Jobs Where BelongSectionID like '"
                    + tmprojectid + "%' AND CurrentProgress>=100 AND PlanStartTime>='" + tmpStartTime + "' AND PlanEndTime<='" + tmpEndTime + "' Group By AssignUser";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    SZRiverSys.Comm.CommonClass.ReplaceTableColumnsName(table, new string[] { "NickName" });//替换用户名为昵称
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpContent = "{\"code\":-1,\"success\":false}";
                sendResponse(context, strFun, tmpContent);
                return;
            }
            #endregion
            #region 查询指定时间范围内未完成工单统计信息
            else if (method == "getjobnofinishstatisticdata")//查询指定时间范围内未完成工单统计信息
            {
                string tmprojectid = context.Request.Params["projectid"];
                if (tmprojectid == null || tmprojectid.Trim() == "")
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpStartTime = context.Request.Params["starttime"];
                string tmpEndTime = context.Request.Params["endtime"];
                if (XJobHandler.CheckIsExistNullParam(new string[] { tmpStartTime, tmpEndTime }))
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"必要字段缺失\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }

                string tmpSql;
                tmpSql = "Select AssignUser,AssignUser as NickName,COUNT(*) As TotalCount From X_Jobs Where BelongSectionID like '"
                    + tmprojectid + "%' AND CurrentProgress<100 AND PlanStartTime>='" + tmpStartTime + "' AND PlanEndTime<='" + tmpEndTime + "' Group By AssignUser";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    SZRiverSys.Comm.CommonClass.ReplaceTableColumnsName(table, new string[] { "NickName" });//替换用户名为昵称
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpContent = "{\"code\":-1,\"success\":false}";
                sendResponse(context, strFun, tmpContent);
                return;
            }
            #endregion
            #region 查询指定时间范围内工单统计信息
            else if (method == "getjobstatisticdata")//查询指定时间范围内工单统计信息
            {
                string tmprojectid = context.Request.Params["projectid"];
                if (tmprojectid == null || tmprojectid.Trim() == "")
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpStartTime = context.Request.Params["starttime"];
                string tmpEndTime = context.Request.Params["endtime"];
                if (XJobHandler.CheckIsExistNullParam(new string[] { tmpStartTime, tmpEndTime }))
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"必要字段缺失\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSql;
                tmpSql = " select   a.*,b.*   from (Select AssignUser,AssignUser as NickName,COUNT(*) as TotalFinishCount From X_Jobs Where BelongSectionID like '"+ tmprojectid + "%' AND CurrentProgress>=100 and PlanStartTime>='"
                    + tmpStartTime + "' AND PlanEndTime<='"+ tmpEndTime + "' Group By AssignUser) as a full join (Select AssignUser as AssignUser2,AssignUser as NickName2,COUNT(*) as TotalNoFinishCount From X_Jobs Where BelongSectionID like '"+ tmprojectid + "%' AND CurrentProgress<100 and PlanStartTime>='"
                    +tmpStartTime+"' AND PlanEndTime<='"+ tmpEndTime + "' Group By AssignUser) as b on a.AssignUser=b.AssignUser2";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    SZRiverSys.Comm.CommonClass.ReplaceTableColumnsName(table, new string[] { "NickName", "NickName2" });//替换用户名为昵称
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpContent = "{\"code\":-1,\"success\":false}";
                sendResponse(context, strFun, tmpContent);
                return;
            }
            #endregion

            #region 查询指定时间范围内已完成工单类型统计信息
            else if (method == "getjobtypefinishstatisticdata")//查询指定时间范围内已完成工单类型统计信息
            {
                string tmprojectid = context.Request.Params["projectid"];
                if (tmprojectid == null || tmprojectid.Trim() == "")
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpStartTime = context.Request.Params["starttime"];
                string tmpEndTime = context.Request.Params["endtime"];
                if (XJobHandler.CheckIsExistNullParam(new string[] { tmpStartTime, tmpEndTime }))
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"必要字段缺失\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }

                string tmpSql;
                tmpSql = "Select JobType,COUNT(*) As TotalCount From X_Jobs Where BelongSectionID like '"
                    + tmprojectid + "%' AND CurrentProgress>=100 AND PlanStartTime>='" + tmpStartTime + "' AND PlanEndTime<='" + tmpEndTime + "' Group By JobType";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpContent = "{\"code\":-1,\"success\":false}";
                sendResponse(context, strFun, tmpContent);
                return;
            }
            #endregion
            #region 查询指定时间范围内未完成工单类型统计信息
            else if (method == "getjobtypenofinishstatisticdata")//查询指定时间范围内未完成工单类型统计信息
            {
                string tmprojectid = context.Request.Params["projectid"];
                if (tmprojectid == null || tmprojectid.Trim() == "")
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpStartTime = context.Request.Params["starttime"];
                string tmpEndTime = context.Request.Params["endtime"];
                if (XJobHandler.CheckIsExistNullParam(new string[] { tmpStartTime, tmpEndTime }))
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"必要字段缺失\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }

                string tmpSql;
                tmpSql = "Select JobType,COUNT(*) As TotalCount From X_Jobs Where BelongSectionID like '"
                    + tmprojectid + "%' AND CurrentProgress<100 AND PlanStartTime>='" + tmpStartTime + "' AND PlanEndTime<='" + tmpEndTime + "' Group By JobType";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpContent = "{\"code\":-1,\"success\":false}";
                sendResponse(context, strFun, tmpContent);
                return;
            }
            #endregion
            #region 查询指定时间范围内工单类型统计信息
            else if (method == "getjobtypestatisticdata")//查询指定时间范围内工单类型统计信息
            {
                string tmprojectid = context.Request.Params["projectid"];
                if (tmprojectid == null || tmprojectid.Trim() == "")
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpStartTime = context.Request.Params["starttime"];
                string tmpEndTime = context.Request.Params["endtime"];
                if (XJobHandler.CheckIsExistNullParam(new string[] { tmpStartTime, tmpEndTime }))
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"必要字段缺失\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }

                string tmpSql;
                tmpSql = "Select JobType,COUNT(*) As TotalCount From X_Jobs Where BelongSectionID like '"
                    + tmprojectid + "%' AND PlanStartTime>='" + tmpStartTime + "' AND PlanEndTime<='" + tmpEndTime + "' Group By JobType";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpContent = "{\"code\":-1,\"success\":false}";
                sendResponse(context, strFun, tmpContent);
                return;
            }
            #endregion

            #region 获取指定事件范围内工单数量
            else if (method == "getjobscount")//获取工单数量
            {
                string tmprojectid = context.Request.Params["projectid"];
                if (tmprojectid == null || tmprojectid.Trim() == "")
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpTimeStr01 = context.Request.Params["starttime"];
                string tmpTimeStr02 = context.Request.Params["endtime"];
                int tmpCount01 = 0, tmpCount02 = 0;
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable("Select Count(ID) From X_Jobs Where BelongSectionID like '"
                    + tmprojectid + "%' AND PlanStartTime >='" + tmpTimeStr01 + "' And PlanEndTime<'" + tmpTimeStr02 + "'");
                if (table != null && table.Rows.Count > 0)
                    tmpCount01 = int.Parse(table.Rows[0].ItemArray[0].ToString());
                table = DBHelper.DBHelperMsSql.ExecuteDataTable("Select Count(ID) From X_Jobs Where BelongSectionID like '"
                    + tmprojectid + "%' AND PlanStartTime >='" + tmpTimeStr01 + "' And PlanEndTime<'" + tmpTimeStr02 + "' AND CurrentProgress>=100");
                if (table != null && table.Rows.Count > 0)
                    tmpCount02 = int.Parse(table.Rows[0].ItemArray[0].ToString());

                string tmpContent = "{ \"success\":true,\"msg\": {\"totalcount\":" + tmpCount01.ToString() + ",\"undocount\":" + tmpCount02.ToString() + "}}";
                sendResponse(context, strFun, tmpContent);
                return;
            }
            #endregion

            #region 获取指定事件范围内各种工单类型的数量
            else if (method == "getjobscountwithtype")// 获取指定事件范围内各种工单类型的数量
            {
                string tmprojectid = context.Request.Params["projectid"];
                if (tmprojectid == null || tmprojectid.Trim() == "")
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpTimeStr01 = context.Request.Params["starttime"];
                string tmpTimeStr02 = context.Request.Params["endtime"];

                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable("Select JobType, Count(ID) From X_Jobs Where BelongSectionID like '"
                    + tmprojectid + "%' AND PlanStartTime >='" + tmpTimeStr01 + "' And PlanEndTime<'" + tmpTimeStr02 + "' GROUP BY JobType");
                if (table != null && table.Rows.Count > 0)
                {
                    Dictionary<string, string[]> tmpDict = new Dictionary<string, string[]>();
                    foreach (DataRow tmpRow in table.Rows)
                    {
                        string[] tmpStrs01 = new string[2];
                        tmpStrs01[0] = tmpRow.ItemArray[1].ToString();
                        tmpStrs01[1] = "0";
                        tmpDict.Add(tmpRow.ItemArray[0].ToString(), tmpStrs01);
                    }
                    DataTable table2 = DBHelper.DBHelperMsSql.ExecuteDataTable("Select JobType, Count(ID) From X_Jobs Where BelongSectionID like '"
                    + tmprojectid + "%' AND PlanStartTime >='" + tmpTimeStr01 + "' And PlanEndTime<'" + tmpTimeStr02 + "' AND CurrentProgress>=100 GROUP BY JobType");
                    if (table2 != null && table2.Rows.Count > 0)
                    {
                        foreach (DataRow tmpRow in table2.Rows)
                        {
                            if (tmpDict.ContainsKey(tmpRow.ItemArray[0].ToString()))
                            {
                                string[] tmpStrs01 = tmpDict[tmpRow.ItemArray[0].ToString()];
                                tmpStrs01[1] = tmpRow.ItemArray[1].ToString();
                            }
                        }
                    }
                    string tmpContent2 = "";
                    foreach (KeyValuePair<string, string[]> tmpPiar in tmpDict)
                    {
                        if (tmpContent2.Length > 0)
                            tmpContent2 += ",";
                        tmpContent2 += "{\"name\":\"" + tmpPiar.Key + "\",\"totalcount\":" + tmpPiar.Value[0] + ",\"undocount\":" + tmpPiar.Value[1] + "}";
                    }
                    string tmpContent = "{ \"success\":true,\"msg\": [" + tmpContent2 + "]}";
                    sendResponse(context, strFun, tmpContent);
                    return;
                }
            }
            #endregion
            #region 获取最后指定数量的工单信息
            else if (method == "getlastjobsinfo")//获取最后指定数量的工单信息
            {
                string tmpCount = context.Request.Params["count"];
                string tmpEndTime = context.Request.Params["endtime"];
                string tmprojectid = context.Request.Params["projectid"];
                if (tmprojectid == null || tmprojectid.Trim() == "")
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable("Select Top " + tmpCount 
                    + " SYSID,BelongSectionID,JobName,JobType,Creator,Creator as CreatorName,CreateTime,AssignUser,AssignUser as AssignUserName,AssociateUsers,PlanStartTime,PlanEndTime,Content,PlanWorkloadValue,FinishTime,FinishContent,FinishTimeRecords,CurrentProgress,TrueWorkloadValue,FilesInfo,Score,Comment,CommentTime,CommentFilesInfo,Remark From X_Jobs Where BelongSectionID like '"
                    + tmprojectid + "%' AND PlanEndTime<='"+ tmpEndTime + "' Order By PlanStartTime DESC");
                if (table != null && table.Rows.Count > 0)
                {
                    SZRiverSys.Comm.CommonClass.ReplaceTableColumnsName(table, new string[] { "CreatorName", "AssignUserName" });//替换用户名为昵称
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpContent = "{\"code\":-1,\"success\":false}";
                sendResponse(context, strFun, tmpContent);
                return;
            }
            #endregion
            #region 获取上报的事件数量
            else if (method == "geteventscount")//获取上报的事件数量
            {
                string tmprojectid = context.Request.Params["projectid"];
                if (tmprojectid == null || tmprojectid.Trim() == "")
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpTimeStr01 = context.Request.Params["starttime"];
                string tmpTimeStr02 = context.Request.Params["endtime"];
                int tmpCount01 = 0;
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable("Select Count(ID) From X_EventsInfo Where BelongSectionID like '"
                    + tmprojectid + "%' AND UploadTime >='" + tmpTimeStr01 + "' And UploadTime<='" + tmpTimeStr02 + "'");
                if (table != null && table.Rows.Count > 0)
                    tmpCount01 = int.Parse(table.Rows[0].ItemArray[0].ToString());

                string tmpContent = "{\"success\":true,\"msg\": {\"totalcount\":" + tmpCount01.ToString() + "}}";
                sendResponse(context, strFun, tmpContent);
                return;
            }
            #endregion
            #region 查询指定时间段范围内上传的事件类型数量
            else if (method == "geteventscountwithtype")//查询指定时间段范围内上传的事件类型数量
            {
                string tmprojectid = context.Request.Params["projectid"];
                if (tmprojectid == null || tmprojectid.Trim() == "")
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpTimeStr01 = context.Request.Params["starttime"];
                string tmpTimeStr02 = context.Request.Params["endtime"];
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable("Select EventType,Count(ID) as TotalCount From X_EventsInfo Where BelongSectionID like '"
                    + tmprojectid + "%' AND UploadTime >='" + tmpTimeStr01 + "' And UploadTime<='" + tmpTimeStr02 + "' Group By EventType");
                if (table != null && table.Rows.Count > 0)
                {
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpContent = "{\"code\":-1,\"success\":false}";
                sendResponse(context, strFun, tmpContent);
                return;
            }
            #endregion

            #region 获取指定时间段范围内的事件信息
            else if (method == "geteventsinfobytime")//获取指定时间段范围内的事件信息
            {
                string tmprojectid = context.Request.Params["projectid"];
                if (tmprojectid == null || tmprojectid.Trim() == "")
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpTimeStr01 = context.Request.Params["starttime"];
                string tmpTimeStr02 = context.Request.Params["endtime"];
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable("Select EventName,EventType,UploadUser,UploadTime,UploadUser as UploadUserNickName,AssignUser,X,Y,Z,EmergencyType,Content From X_EventsInfo Where BelongSectionID like '"
                    + tmprojectid + "%' AND UploadTime >='" + tmpTimeStr01 + "' And UploadTime<='" + tmpTimeStr02 + "'  Order By UploadTime DESC");
                if (table != null && table.Rows.Count > 0)
                {
                    SZRiverSys.Comm.CommonClass.ReplaceTableColumnsName(table, new string[] { "UploadUserNickName" });//替换用户名为昵称
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpContent = "{\"code\":-1,\"success\":false}";
                sendResponse(context, strFun, tmpContent);
                return;
            }
            #endregion
            #region 获取最近指定数量的事件信息
            else if (method == "geteventslastinfo")//获取最近指定数量的事件信息
            {
                string tmpCount = context.Request.Params["count"];
                string tmprojectid = context.Request.Params["projectid"];
                if (tmprojectid == null || tmprojectid.Trim() == "")
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable("Select Top "+ tmpCount + " EventName,EventType,UploadUser,UploadUser as UploadUserNickName,UploadTime,AssignUser,X,Y,Z,EmergencyType,Content From X_EventsInfo Where BelongSectionID like '"
                    + tmprojectid + "%' Order By UploadTime DESC");
                if (table != null && table.Rows.Count > 0)
                {
                    SZRiverSys.Comm.CommonClass.ReplaceTableColumnsName(table, new string[] { "UploadUserNickName" });//替换用户名为昵称
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpContent = "{\"code\":-1,\"success\":false}";
                sendResponse(context, strFun, tmpContent);
                return;
            }
            #endregion

            #region 获取河道设施
            else if (method == "getfacilitiesinfobyprojectid")//获取河道设施
            {
                string tmpPrjID = context.Request.Params["projectid"];
                if (tmpPrjID == null || tmpPrjID.Trim() == "")
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSearch = context.Request.Params["search"];
                if (tmpSearch == null)
                    tmpSearch = " 1=1 ";
                else
                {
                    tmpSearch = "( Name like '%" + tmpSearch + "%' OR  FacilitiesType like '%" + tmpSearch + "%')";
                }
                string tmpSql = "";
                tmpSql = "Select SYSID,BelongSectionID,Name,FacilitiesType,X,Y,Z,Info,Remark From X_FacilitiesInfo Where BelongSectionID like '" + tmpPrjID + "%' AND " + tmpSearch;
                string tmpType = context.Request.Params["type"];
                if (tmpType != null)
                    tmpSql += " AND FacilitiesType='" + tmpType + "'";
                DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (table != null && table.Rows.Count > 0)
                {
                    JSON.JsonHelper Json = new JSON.JsonHelper();
                    string json = Json.DataTableToJsonForLayUi(table);
                    msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                else
                {
                    msg = "{\"code\":-1,\"success\":false,\"msg\":\"该项目内没有定义河道设施\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
            }
            #endregion
            
            if (msg == "")
                msg = "\"\"";
            string tmpContent01 = "{success:" + result.ToString().ToLower() + ",msg:" + msg + "}";
            sendResponse(context, strFun, tmpContent01);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public static string LoadData(string url)
        {
            //创建请求
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //GET请求
            request.Method = "GET";
            request.ReadWriteTimeout = 5000;
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            //返回内容
            string retString = myStreamReader.ReadToEnd();
            return retString;
        }
    }
}