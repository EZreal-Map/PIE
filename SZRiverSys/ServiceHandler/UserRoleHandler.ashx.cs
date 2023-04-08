﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using SZRiverSys.Comm;
using SZRiverSys.Model;

namespace SZRiverSys.ServiceHandler
{
    /// <summary>
    /// UserRoleHandler 的摘要说明
    /// </summary>
    public class UserRoleHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            JsonMsgModel jsonMsg = new JsonMsgModel();
            try
            {
                #region 实例化数据访问层
                SQL.User_Role_Sql user_Role_Sql = new SQL.User_Role_Sql();
                SQL.ProjectPlanSql projectPlanSql = new SQL.ProjectPlanSql();
                #endregion
                #region 实例化容器
                DataTable dt = new DataTable();
                #endregion
                #region 实例帮助类
                JSON.JsonHelper Json = new JSON.JsonHelper();
                #endregion
                #region 实例化实体类
                List<UserModel> userModels = new List<UserModel>();
                UserModel userModel = new UserModel();//用户实体


                USER_REF_ROLE urr = new USER_REF_ROLE();
                Role role = new Role();
                List<Role> roles = new List<Role>();
                Tool tl = new Tool();
                #endregion
                #region 实例化权限模块
                RoleListHTML _tmpEntity = new RoleListHTML();

                RolePowerHelper _ROLEPOWER = new RolePowerHelper();
                #endregion

                #region 通用变量
                int count = 0;
                int id = 0;
                int num = 0;
                string ArrayID = "";
                string ProjectID = "";
                #endregion
                context.Response.ContentType = "text/plain";
                string method = context.Request["method"];
                if (method != null)
                {
                    #region 缓存信息及安全标识码
                    string token = context.Request.Params["token"] == null ? "" : context.Request.Params["token"];
                    userModel = CacheHelper.CacheHelper.Get(token) as UserModel;
                    #endregion
                    #region 列表分页专用
                    int pageSize = Convert.ToInt32(context.Request.Params["pagesize"]);//每页显示行数
                    int pageIndex = Convert.ToInt32(context.Request.Params["pageindex"]);//当前页索引
                    string search = context.Request.Params["search"] == null ? "" : context.Request.Params["search"].Trim();//筛选条件
                    #endregion
                    if (CacheHelper.CacheHelper.IsContain(token))
                    {

                        switch (method.ToLower())
                        {
                            #region 角色管理
                            case "getrulelist"://查询角色信息
                                List<Role> ListEntity = new List<Role>();
                                userModel = (Model.UserModel)CacheHelper.CacheHelper.Get(token);
                                dt = user_Role_Sql.GetRuleList(search);
                                _tmpEntity = _ROLEPOWER.ROLE_RoleListHTML(userModel.RoleID);
                                if (dt.Rows.Count > 0)
                                {
                                    bool tmpbtnSetMenuRole = _tmpEntity.btnSetMenuRole;
                                    bool tmpbtnSetOperateRole = _tmpEntity.btnSetOperateRole;
                                    roles = DataTableToEntity<Role>.ConvertToModel(dt);
                                    foreach (Role item in roles)
                                    {
                                        _tmpEntity = new RoleListHTML();
                                        _tmpEntity = _ROLEPOWER.ROLE_RoleListHTML(userModel.RoleID);
                                        if (tmpbtnSetMenuRole)
                                        {
                                            if (userModel.RoleList.RoleGrade < item.RoleGrade)
                                            {
                                                _tmpEntity.btnSetMenuRole = true;
                                            }
                                            else
                                            {
                                                _tmpEntity.btnSetMenuRole = false;
                                            }
                                        }
                                        if (tmpbtnSetOperateRole)
                                        {
                                            if (userModel.RoleList.RoleGrade < item.RoleGrade)
                                            {
                                                _tmpEntity.btnSetOperateRole = true;
                                            }
                                            else
                                            {
                                                _tmpEntity.btnSetOperateRole = false;
                                            }
                                        }
                                        item.Operate = _tmpEntity;
                                    }
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = _tmpEntity;
                                    jsonMsg.count = roles.Count;
                                    jsonMsg.data = roles.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                                }
                                break;
                            case "getrulelistbyid"://根据id获取某一条角色信息【修改绑值】
                                id = int.Parse(context.Request["id"]);
                                dt = user_Role_Sql.GetRuleListByid(id);
                                if (dt.Rows.Count > 0)
                                {
                                    roles = DataTableToEntity<Role>.ConvertToModel(dt);
                                }
                                jsonMsg.code = 0;
                                jsonMsg.msg = true;
                                jsonMsg.data = roles;
                                break;
                            case "getrulelistnotpage"://选择上级角色【绑定下拉框】
                                dt = user_Role_Sql.GetRuleList();
                                if (dt.Rows.Count > 0)
                                {
                                    roles = DataTableToEntity<Role>.ConvertToModel(dt);
                                }
                                jsonMsg.code = 0;
                                jsonMsg.msg = true;
                                jsonMsg.data = roles;
                                break;
                            case "addrole"://添加一条角色
                                userModel = CacheHelper.CacheHelper.Get(token) as Model.UserModel;
                                role.Description = context.Request["description"];
                                role.RolePid = int.Parse(context.Request["rolepid"]);
                                role.Remark = context.Request["remark"];
                                //获取父级的角色等级
                                if (role.RolePid == 0)
                                {
                                    role.RoleGrade = 1;
                                }
                                else
                                {
                                    role.RoleGrade = user_Role_Sql.GetRuleParentID(role.RolePid);
                                }
                                //role.RoleGroupID = int.Parse(context.Request["rolegroupid"]);
                                if (user_Role_Sql.AddRule(role))
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = true;
                                    jsonMsg.data = "新增角色成功";
                                }
                                else
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = false;
                                    jsonMsg.data = "新增角色失败";
                                }
                                break;
                            case "editrole"://修改角色信息
                                userModel = CacheHelper.CacheHelper.Get(token) as Model.UserModel;
                                role.RoleID = int.Parse(context.Request["roleid"]);
                                role.Description = context.Request["description"];
                                role.RolePid = int.Parse(context.Request["rolepid"]);
                                role.Remark = context.Request["remark"];
                                //获取父级的角色等级
                                if (role.RolePid == 0)
                                {
                                    role.RoleGrade = 1;
                                }
                                else
                                {
                                    role.RoleGrade = user_Role_Sql.GetRuleParentID(role.RolePid);
                                }
                                //role.RoleGroupID = int.Parse(context.Request["rolegroupid"]);
                                if (user_Role_Sql.EditRule(role))
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = true;
                                    jsonMsg.data = "修改角色成功";
                                }
                                else
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = false;
                                    jsonMsg.data = "修改角色失败";
                                }
                                break;
                            case "delrole"://删除角色信息
                                ArrayID = context.Request["arrayid"];
                                if (user_Role_Sql.DelRule(ArrayID))
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = true;
                                    jsonMsg.data = "删除角色成功";
                                }
                                else
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = false;
                                    jsonMsg.data = "删除角色失败";
                                }
                                break;
                            #endregion
                            #region 用户角色管理
                            case "setuserrole"://设置用户角色
                                userModel = CacheHelper.CacheHelper.Get(token) as Model.UserModel;
                                urr.UserID = int.Parse(context.Request["userid"]);
                                urr.RoleID = int.Parse(context.Request["roleid"]);
                                urr.CreateUserID = userModel.UserID;
                                urr.CreateDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                urr.ModifyUserID = userModel.UserID;
                                urr.ModifyDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                if (user_Role_Sql.SetUserRole(urr))
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = true;
                                    jsonMsg.data = "指派角色成功";
                                }
                                else
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = false;
                                    jsonMsg.data = "指派角色失败";
                                }
                                break;
                            case "setrolemenu"://设置菜单角色权限
                                string roleids_menu = context.Request["roleid_menu"];
                                string menuid = context.Request["menuid"];
                                if (menuid == "")
                                {
                                    if (user_Role_Sql.DelAllMenu(int.Parse(roleids_menu)))
                                    {
                                        jsonMsg.code = 0;
                                        jsonMsg.msg = true;
                                        jsonMsg.data = "指派菜单权限成功";
                                    }
                                    else
                                    {
                                        jsonMsg.code = 0;
                                        jsonMsg.msg = false;
                                        jsonMsg.data = "指派菜单权限失败";
                                    }
                                }
                                else
                                {

                                    string[] menuids = menuid.Split(',');
                                    List<string> tmpStr = tl.GetMenuParent(menuids);

                                    if (user_Role_Sql.SetMenuRole(tmpStr, roleids_menu))
                                    {
                                        jsonMsg.code = 0;
                                        jsonMsg.msg = true;
                                        jsonMsg.data = "指派菜单权限成功";
                                    }
                                    else
                                    {
                                        jsonMsg.code = 0;
                                        jsonMsg.msg = false;
                                        jsonMsg.data = "指派菜单权限失败";
                                    }
                                }
                                break;
                            case "getusermenu"://查询用户已指定的菜单
                                int roleid = int.Parse(context.Request["roleid"]);
                                //msg = tl.GetTreeByMenu(roleid);
                                jsonMsg.code = 0;
                                jsonMsg.msg = true;
                                jsonMsg.data = tl.GetTreeByMenu(roleid);
                                break;
                            case "setroleopetate"://给角色指派功能权限
                                roleid = int.Parse(context.Request["roleid"]);
                                string opetateArray = context.Request["opetatearray"];
                                if (opetateArray == "")
                                {
                                    if (user_Role_Sql.DelAllOpera(roleid))
                                    {
                                        jsonMsg.code = 0;
                                        jsonMsg.msg = true;
                                        jsonMsg.data = "指派功能成功";
                                    }
                                    else
                                    {
                                        jsonMsg.code = 0;
                                        jsonMsg.msg = false;
                                        jsonMsg.data = "指派功能失败";
                                    }
                                }
                                else
                                {
                                    string[] Array = opetateArray.Split(',');
                                    if (user_Role_Sql.SetRoleOperate(roleid, Array))
                                    {
                                        jsonMsg.code = 0;
                                        jsonMsg.msg = true;
                                        jsonMsg.data = "指派功能成功";
                                    }
                                    else
                                    {
                                        jsonMsg.code = 0;
                                        jsonMsg.msg = false;
                                        jsonMsg.data = "指派功能失败";
                                    }
                                }
                                break;
                            case "getuseroperate"://查询用户已指定的按钮
                                roleid = int.Parse(context.Request["roleid"]);
                                //msg = tl.GetTreeBySysOperate(roleid);
                                jsonMsg.code = 0;
                                jsonMsg.msg = true;
                                jsonMsg.data = tl.GetTreeBySysOperate(roleid);
                                break;
                        }
                        #endregion
                    }
                }
            }
            catch (Exception)
            {
                jsonMsg.code = 1;
                jsonMsg.msg = false;
                jsonMsg.data = "服务器程序错误";
            }
            finally
            {
                System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                context.Response.ContentType = "text/plain; charset=utf-8";
                context.Response.Write(js.Serialize(jsonMsg));//构造json数据格式
                context.Response.End();
                context.Response.Close();
            }
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