﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using SZRiverSys.Model;
using SZRiverSys.Comm;

namespace SZRiverSys.ServiceHandler
{
    /// <summary>
    /// UserHandler 的摘要说明
    /// </summary>
    public class UserHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {            
            string msg = "{\"code\":1,\"success\":false,\"msg\":\"请求参数不对\"}";
            SQL.UserSql Sql = new SQL.UserSql();
            SQL.BusinessSql sqlb = new SQL.BusinessSql();
            JSON.JsonHelper tojson = new JSON.JsonHelper();
            Model.UserModel User = new Model.UserModel();
            string method = context.Request["method"];
            string json = "";

            #region 实例化权限模块
            User1001HTML user1001HTML = new User1001HTML();
            RolePowerHelper _ROLEPOWER = new RolePowerHelper();
            #endregion
            #region 实例化实体类
            List<UserModel> UserModels = new List<UserModel>();
            UserModel UserModel = new UserModel();
            #endregion
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            if (method != null)
            {
                string token = context.Request.Params["token"] == null ? "" : context.Request.Params["token"];
                Model.UserModel tmpUser = CacheHelper.CacheHelper.Get(token) as Model.UserModel;
                if (method == "login")
                {
                    string tmpTelNo = context.Request["usercode"].Trim();
                    string tmpPwd = context.Request["pwd"].Trim();
                    string type = context.Request["type"];
                    int logintype = 0;
                    if (type == "web")
                    {
                        logintype = 1;
                    }
                    if (tmpTelNo != "guest")
                    {
                        if (CacheHelper.CacheHelper.IsContain(tmpTelNo))
                        {
                            string lastUidToken = CacheHelper.CacheHelper.Get(tmpTelNo).ToString(); 
                            CacheHelper.CacheHelper.Remove(lastUidToken);
                            CacheHelper.CacheHelper.Remove(tmpTelNo);
                        }
                    }
                    if (Sql.ValidateUser(tmpTelNo, tmpPwd))
                    {
                        //验证通过将用户信息缓存
                        int roleid = 0;
                        string resultToken = "";
                        if (CacheHelper.CacheHelper.IsContain(tmpTelNo))
                        {
                            if (tmpTelNo == "guest")
                            {
                                resultToken = CacheHelper.CacheHelper.Get(tmpTelNo).ToString();
                            }
                            else
                            {
                                resultToken = CacheHelper.CacheHelper.CacheUserInfo(tmpTelNo, out roleid, logintype);
                            }
                        }
                        else
                        {
                            resultToken = CacheHelper.CacheHelper.CacheUserInfo(tmpTelNo, out roleid, logintype);
                        }
                        msg = "{\"code\":0,\"success\":true,\"msg\":\"登录成功\",\"token\":\""+ resultToken + "\",\"roleid\":\"" + roleid + "\"}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"账号或密码错误\"}";
                    }
                }
                else if (method == "getuserlistbypage")//获取用户列表
                {
                    int pageindex = int.Parse(context.Request["pageindex"]);
                    int pagesize = int.Parse(context.Request["pagesize"]);
                    string search = context.Request["search"];
                    int total = 0;
                    DataTable dt = Sql.GetUserList2(pageindex, pagesize, search, out total);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        user1001HTML = _ROLEPOWER.RULE_User1001HTML(tmpUser.RoleID);
                        UserModels = DataTableToEntity<UserModel>.ConvertToModel(dt);
                        for (int i = 0; i < UserModels.Count; i++)
                        {
                            UserModels[i].Operate = user1001HTML;
                        }
                        msg = "{\"code\":0,\"success\":true,\"data\":" + js.Serialize(UserModels) + ",\"total\":" + total + ",\"msg\":" + js.Serialize(user1001HTML) + "}";
                    }
                    else
                    {
                        msg = "{\"code\":0,\"success\":false,\"data\":[]}";
                    }
                }
                else if (method == "createuser")//创建用户
                {
                    Model.UserModel m = new UserModel();
                    m.usercode = context.Request["usercode"];
                    m.Password = context.Request["Password"];
                    m.EmailAddress = context.Request["EmailAddress"];
                    m.userName = context.Request["userName"];
                    m.Company = context.Request["Company"];
                    m.UserSex = context.Request["UserSex"];
                    m.Age = context.Request["Age"];
                    m.remark = context.Request["remark"];
                    m.tel = context.Request["tel"];
                    m.RoleID =int.Parse(context.Request["RoleID"]);
                    m.projectid = "";
                    m.refIMEI = context.Request["refIMEI"];
                    if (m.refIMEI == null)
                        m.refIMEI = "";
                    if (Sql.CreateUser(m))
                    {
                        msg = "{\"code\":0,\"success\":true,\"msg\":\"添加成功\"}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"添加失败\"}";
                    }

                }
                else if (method == "getuserbyid")//根据ID获取用户信息
                {
                    string ID = context.Request["ID"];
                    DataTable dt = Sql.GetUserByID(ID);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        json = tojson.DataTableToJsonForLayUi(dt);
                        msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":0,\"success\":false,\"data\":[]}";
                    }

                }
                else if (method == "edituserinfo")//修改具体用户信息和用户角色
                {
                    Model.UserModel m = new UserModel();
                    m.UserID =int.Parse(context.Request["ID"]);
                    m.EmailAddress = context.Request["EmailAddress"];
                    m.userName = context.Request["userName"];
                    m.Company = context.Request["Company"];
                    m.UserSex = context.Request["UserSex"];
                    m.Age = context.Request["Age"];
                    m.remark = context.Request["remark"];
                    m.tel = context.Request["tel"];
                    m.RoleID =int.Parse(context.Request["RoleID"]);
                    m.refIMEI = context.Request["refIMEI"];
                    if (m.refIMEI == null)
                        m.refIMEI = "";
                    if (Sql.EditUser(m))
                    {
                        Sql.EditUserPower(m.UserID.ToString(), m.RoleID.ToString());
                        msg = "{\"code\":0,\"success\":true,\"msg\":\"修改成功\"}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"修改失败\"}";
                    }
                }



                else if (method == "updateuserinfo")//修改部分用户信息
                {
                    Model.UserModel m = new UserModel();
                    if (CacheHelper.CacheHelper.IsContain(token))
                    {
                        tmpUser = CacheHelper.CacheHelper.Get(token) as Model.UserModel;
                        m.UserID = tmpUser.UserID;
                        m.userName = context.Request["userName"];
                        m.tel = context.Request["tel"];
                        m.UserSex = context.Request["UserSex"] == "0"?"男":"女";
                        m.EmailAddress = context.Request["EmailAddress"];
                        if (Sql.UpdateUser(m))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"修改成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"修改失败\"}";
                        }
                    }
                }
                else if (method == "updatepassword")//修改用户密码
                {
                    if (CacheHelper.CacheHelper.IsContain(token))
                    {
                        tmpUser = CacheHelper.CacheHelper.Get(token) as Model.UserModel;
                        if (tmpUser.Password == context.Request["oldpass"])
                        {
                            string newpass = context.Request["newpass"];
                            if (Sql.UpdateUserPass(newpass, tmpUser.UserID))
                            {
                                msg = "{\"code\":0,\"success\":true,\"msg\":\"修改成功\"}";
                            }
                            else
                            {
                                msg = "{\"code\":1,\"success\":false,\"msg\":\"修改失败\"}";
                            }
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"原密码错误\"}";
                        }
                        
                    }
                }



                else if (method == "deluser")//删除用户
                {
                    string ID = context.Request["ID"];
                    if (Sql.DelUser(ID))
                    {
                        msg = "{\"code\":0,\"success\":true,\"msg\":\"删除成功\"}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"删除失败\"}";
                    }
                }
                else if (method == "edituserpower")//修改用户权限
                {
                    string UserId = context.Request["UserId"];
                    string RoleId = context.Request["RoleId"];
                    if (Sql.EditUserPower(UserId, RoleId))
                    {
                        msg = "{\"code\":0,\"success\":true,\"msg\":\"修改成功\"}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"修改失败\"}";
                    }
                }
                else if(method=="loginout")
                {
                    if (CacheHelper.CacheHelper.IsContain(token))
                    {
                        CacheHelper.CacheHelper.Remove(token);
                        msg = "{\"code\":0,\"success\":true,\"msg\":\"退出成功\"}";
                    }
                    else
                    {
                        msg = "{\"code\":-1,\"success\":false,\"msg\":\"用户登录失效,请重新登录\"}";
                    }
                }
                #region 根据token获取用户信息
                else if (method == "getuserinfo")
                {
                    if (CacheHelper.CacheHelper.IsContain(token))
                    {
                        tmpUser = CacheHelper.CacheHelper.Get(token) as Model.UserModel;
                        DataTable dt = Sql.GetEditUserInfo(tmpUser);
                        if (dt.Rows.Count > 0)
                        {
                            json = tojson.DataTableToJsonForLayUi(dt);
                            msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                        }

                    }
                }
                #endregion
                else if (method == "getuserlist") //用户下拉框
                {
                    DataTable dt = Sql.GetUserList();
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        json = tojson.DataTableToJsonForLayUi(dt);
                        msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":0,\"success\":false,\"data\":[]}";
                    }
                }
                else if (method == "getuserbycode")
                {
                    string code = context.Request["code"];
                    DataTable dt = Sql.GetUserInfoByUserCode(code);
                    if (dt.Rows.Count > 0)
                    {
                        json = tojson.DataTableToJsonForLayUi(dt);
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
                #region 个人中心业务接口
                else if (method == "getusermessge")//获取用户消息通知
                {
                    tmpUser = CacheHelper.CacheHelper.Get(token) as Model.UserModel;
                    DataTable dt = sqlb.GetUserWorkFormList(tmpUser.usercode);
                    DataTable dt1 = sqlb.GetUserBusiness(tmpUser.usercode);
                    DataTable dt2 = sqlb.UserNotFinishPlan(tmpUser);
                    msg = "{\"code\":0,\"success\":true,\"BusinessCount\":" + dt1.Rows.Count + ",\"WorkFormCount\":" + dt.Rows.Count + ",\"PlanCount\":" + dt2.Rows.Count + "}";
                }
                #endregion
                #region 个人中心业务接口
                else if (method == "getuseroperatebymenuid")//获取用户消息通知
                {
                    tmpUser = CacheHelper.CacheHelper.Get(token) as Model.UserModel;
                    string tmpMenuID = context.Request["menuid"];
                    string tmpData001 = "";
                    {
                        try
                        {
                            //string tmpSql = "SELECT m.OperateCode FROM sys_operate m,New_ROLE_Menu n,role_operate r where m.MenuID=n.MenuID AND r.roleid= AND m.id=r.operateid AND r.roleid=" + tmpUser.RoleID.ToString()
                            //    + " AND  m.MenuID='" + tmpMenuID + "'";
                            string tmpSql = "SELECT OperateCode FROM (SELECT id,OperateName,OperateCode FROM sys_operate Where MenuID = '"+ tmpMenuID 
                                + "') A, (SELECT operateid FROM role_operate Where roleid="+ tmpUser.RoleID.ToString() + ") B Where A.id=B.operateid";
                            DataTable dt01 = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                            if (dt01 != null && dt01.Rows.Count > 0)
                            {
                                string tmpStr001 = "";
                                foreach (DataRow tmpRow in dt01.Rows)
                                {
                                    if (tmpStr001.Length > 0)
                                        tmpStr001 += ",";
                                    tmpStr001 += "\"" + tmpRow.ItemArray[0].ToString() + "\":true";
                                }
                                tmpData001 = "{\"operate\":{" + tmpStr001 + "}}";
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    if(tmpData001!="")
                        msg = "{\"code\":0,\"success\":true,\"data\":" + tmpData001 + "}";
                    else
                        msg = "{\"code\":0,\"success\":false,\"data\":null}";
                }
                #endregion
            }
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.Write(msg);//构造json数据格式
            context.Response.End();
            context.Response.Close();
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