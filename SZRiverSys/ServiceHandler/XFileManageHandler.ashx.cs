using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SZRiverSys.ServiceHandler
{
    /// <summary>
    /// XFileManageHandler 的摘要说明
    /// </summary>
    public class XFileManageHandler : IHttpHandler
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

            #region 查询文档结构信息
            if (method == "getfilemanageinfobyprojectid")// 根据项目编码获取对应的文档结构信息
            {
                string tmpprojectid = context.Request.Params["projectid"];
                if (tmpprojectid == null || tmpprojectid.Trim() == "")
                {
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"projectid不能为空\"}";
                    sendResponse(context, strFun, msg);
                    return;
                }
                string tmpSql = "";
                tmpSql = "Select SYSID,BelongProjectID,FolderName,ParentID,Remark From X_FilesManager Where BelongProjectID like '" + tmpprojectid + "%'";
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
                    string msg = "{\"code\":-1,\"success\":false,\"msg\":\"系统中没有该项目\"}";
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
    }
}