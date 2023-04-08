using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SZRiverSys.ServiceHandler
{
    /// <summary>
    /// MenuHandler 的摘要说明
    /// </summary>
    public class MenuHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string msg = "{\"code\":1,\"success\":false,\"msg\":\"请求参数不对\"}";
            SQL.MenuSql Sql = new SQL.MenuSql();
            Model.MenuModel Menu = new Model.MenuModel();
            DataTable dt = new DataTable();
            string method = context.Request["method"].ToLower();
            if (method != null)
            {
                string token = context.Request.Params["token"] == null ? "" : context.Request.Params["token"];
                if (CacheHelper.CacheHelper.IsContain(token))
                {
                    switch (method)
                    {
                        case "getmenuhtml": //返回用户角色的菜单栏
                            Model.UserModel User = new Model.UserModel();
                            User = CacheHelper.CacheHelper.Get(token) as Model.UserModel;
                            msg = Sql.GetMenu(User);
                            break;
                        case "editmenu"://修改菜单
                            Menu.MenuID = int.Parse(context.Request["menuid"]);
                            Menu.MenuName = context.Request["menuname"];
                            Menu.MenuPath = context.Request["href"];
                            Menu.orders = int.Parse(context.Request["order"]);
                            Menu.ParentMenuID = int.Parse(context.Request["parentid"]);
                            Menu.Status = int.Parse(context.Request["status"]);
                            Menu.Icon = context.Request["icon"];
                            Menu.Target = context.Request["target"];
                            Menu.Description = context.Request["description"];
                            if (Sql.UpdateMenu(Menu))
                            {
                                msg = "{\"code\":0,\"success\":true,\"msg\":\"修改成功\"}";
                            }
                            else
                            {
                                msg = "{\"code\":1,\"success\":false,\"msg\":\"修改失败\"}";
                            }
                            break;
                        case "delmenu"://删除菜单
                            string menuid = context.Request["menuid"];
                            dt = Sql.SearchdownMenu_ByID(menuid);

                            if (dt != null && dt.Rows.Count > 0)
                            {
                                msg = "{\"code\":1,\"success\":false,\"msg\":\"该菜单含有子菜单，无法删除\"}";
                            }
                            else
                            {
                                if (Sql.DelMenu(menuid))
                                {
                                    msg = "{\"code\":0,\"success\":true,\"msg\":\"删除成功\"}";
                                }
                                else
                                {
                                    msg = "{\"code\":1,\"success\":false,\"msg\":\"删除失败\"}";
                                }
                            }
                            break;
                        case "addmenu"://新增菜单
                            Menu.MenuID = int.Parse(context.Request["menuid"]);
                            Menu.MenuName = context.Request["menuname"];
                            Menu.MenuPath = context.Request["href"];
                            Menu.orders = int.Parse(context.Request["order"]);
                            Menu.ParentMenuID = int.Parse(context.Request["parentid"]);
                            Menu.Status = int.Parse(context.Request["status"]);
                            Menu.Icon = context.Request["icon"];
                            Menu.Target = context.Request["target"];
                            Menu.Description = context.Request["description"];
                            dt = Sql.SearchMenu_ByID(Menu.MenuID);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                msg = "{\"code\":1,\"success\":false,\"msg\":\"菜单编号重复\"}";
                            }
                            else
                            {
                                if (Sql.AddMenu(Menu))
                                {
                                    msg = "{\"code\":0,\"success\":true,\"msg\":\"新增成功\"}";
                                }
                                else
                                {
                                    msg = "{\"code\":1,\"success\":false,\"msg\":\"新增失败\"}";
                                }
                            }
                            break;
                        case "searchmenubyupdate"://查询单个菜单信息
                            int id = int.Parse(context.Request["menuid"]);
                            dt = Sql.SearchMenu_ByID(id);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                JSON.JsonHelper Json = new JSON.JsonHelper();
                                string json = Json.DataTableToJsonForLayUi(dt);
                                msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                            }
                            else
                            {
                                msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                            }
                            break;
                    }
                }
                else
                {
                    msg = "{\"code\":-1,success:false,msg:\"用户登录失效,请重新登录\"}";
                }
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