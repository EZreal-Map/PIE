using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SZRiverSys.SQL
{
    public class MenuSql
    {

        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <returns></returns>
        public string GetMenu(Model.UserModel User)
        {
            string result = "";
            string tmpsql = "";
            try
            {
                tmpsql = @"select * from(SELECT m.id,m.MenuID,m.MenuName,m.MenuPath,m.ParentMenuID,m.orders,case m.Status when 1 then '启用' when 2 then '停用' end as Status,m.target,m.icon 
                                FROM dbo.New_Menu m 
                                inner JOIN (SELECT * FROM dbo.New_ROLE_Menu WHERE ID IN(
												                                SELECT
													                                min(id)
												                                FROM
													                                dbo.New_ROLE_Menu
												                                GROUP BY
													                                roleid,
													                                MenuID
												                                )
                                                            ) rm on rm.MenuID=m.MenuID
                                INNER JOIN dbo.Sys_Accounts_Roles  ur on ur.RoleID=rm.roleid
                                INNER JOIN dbo.Sys_Accounts_UserRoles urr on urr.RoleID=ur.RoleID
                                inner JOIN dbo.Sys_Accounts_Users u on u.UserID=urr.UserID
                                where u.usercode='" + User.usercode + "' and  m.Status=1) t ORDER BY t.ParentMenuID,t.orders,t.id";

                DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<object> obj = new List<object>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        obj.Add(new
                        {
                            id = dr["id"].ToString(),
                            MenuID = dr["MenuID"].ToString(),
                            MenuName = dr["MenuName"].ToString(),
                            MenuPath = dr["MenuPath"].ToString(),
                            Icon = dr["icon"].ToString(),
                            Target = dr["target"].ToString(),
                            ParentMenuID = dr["ParentMenuID"].ToString()
                        });
                    }
                    result = JsonConvert.SerializeObject(new { success = true, msg = obj });
                }
                else
                {
                    result = JsonConvert.SerializeObject(new { success = false, msg = "没查到相关数据" });
                }
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }

        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <returns></returns>
        public bool AddMenu(Model.MenuModel Menu)
        {
            int result = 0;
            try
            {
                string tmpsql = string.Format(@"insert into dbo.New_Menu(MenuID,MenuName,MenuPath,ParentMenuID,orders,Status,target,Description) 
                                            values({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}')", Menu.MenuID, Menu.MenuName, Menu.MenuPath, Menu.ParentMenuID, Menu.orders, Menu.Status, Menu.Target, Menu.Description);
                result = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);

                //默认新增菜单权限分配管理员
                tmpsql = "insert into dbo.New_ROLE_Menu(MenuID,roleid) values(" + Menu.MenuID + ",1)";
                DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
                if (result > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 修改菜单
        /// </summary>
        /// <returns></returns>
        public bool UpdateMenu(Model.MenuModel Menu)
        {
            int result = 0;
            try
            {
                string tmpSql = string.Format(@"UPDATE dbo.New_Menu SET 
                                                    MenuName='{0}', 
                                                    MenuPath='{1}',
                                                    ParentMenuID='{2}', 
                                                    orders='{3}', 
                                                    Status='{4}',  
                                                    icon='{5}',
                                                    target='{6}',
                                                    Description='{7}' 
                                                    WHERE (MenuID='{8}');",
                                                   Menu.MenuName,
                                                   Menu.MenuPath,
                                                   Menu.ParentMenuID,
                                                   Menu.orders,
                                                   Menu.Status,
                                                   Menu.Icon,
                                                   Menu.Target,
                                                   Menu.Description,
                                                   Menu.MenuID);
                result = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql);
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 查询下级菜单
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public DataTable SearchdownMenu_ByID(string menuId)
        {
            string tmpSql = "select * from dbo.New_Menu where ParentMenuID='" + menuId + "'";
            try
            {
                DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// 查询单个菜单信息（用于修改绑值）
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public DataTable SearchMenu_ByID(int menuId)
        {
            string tmpSql = "select * from dbo.New_Menu where MenuId='" + menuId + "'";
            try
            {
                DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql) ;
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <returns></returns>
        public bool DelMenu(string menuId)
        {
            int result = 0;
            string tmpSql = "";
            try
            {
                tmpSql = string.Format("Delete dbo.New_Menu where MenuID in ('{0}')", menuId.Replace(",", "','"));
                
                //tmpSql = "Delete from Menu where MenuID='" + menuId + "'";
                result = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql);
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}