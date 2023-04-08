using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace SZRiverSys.SQL
{
    public class User_Role_Sql
    {
        DataTable dt = new DataTable();
        string tmpSql = "";
        #region 角色管理
        public DataTable GetRuleList(string Search)
        {
            try
            {
                tmpSql = @"SELECT
                            r.*
                        FROM
	                        Sys_Accounts_Roles r
                        WHERE
	                        1 = 1";
                if (Search != "")
                {
                    tmpSql += " and r.Description like '%" + Search + "%'";
                }
                dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataTable GetRuleList()
        {
            try
            {
                tmpSql = @"SELECT
                            *
                        FROM
	                        Sys_Accounts_Roles r
                        WHERE
	                        1 = 1";
                dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public int GetRuleParentID(int id)
        {
            try
            {
                int RoleGrade = 0;
                tmpSql = "select RoleGrade from Sys_Accounts_Roles where RoleID='" + id + "'";
                dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (dt.Rows.Count > 0)
                {
                    RoleGrade = int.Parse(dt.Rows[0]["RoleGrade"].ToString()) + 1;
                }
                return RoleGrade;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataTable GetRuleListByid(int ID)
        {
            try
            {
                tmpSql = "select * from Sys_Accounts_Roles where RoleID='" + ID + "'";
                dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool AddRule(Model.Role Role)
        {
            try
            {
                tmpSql = $@"INSERT INTO [dbo].[Sys_Accounts_Roles]
                           ([Description]
                           ,[RoleGrade]
                           ,[RolePid]
                           ,[Remark])
                     VALUES
                           ('{Role.Description}'
                           ,'{Role.RoleGrade}'
                           ,'{Role.RolePid}'
                           ,'{Role.Remark}')";
                int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql);
                if (count > 0)
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
        public bool EditRule(Model.Role Role)
        {
            try
            {
                tmpSql = $@"UPDATE [dbo].[Sys_Accounts_Roles]
                                   SET [Description] = '{Role.Description}'
                                      ,[RoleGrade] = '{Role.RoleGrade}'
                                      ,[RolePid] = '{Role.RolePid}'
                                      ,[Remark] = '{Role.Remark}'
                                 WHERE [RoleID]='{Role.RolePid}'";
                int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql);
                if (count > 0)
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
        public bool DelRule(string ArrayId)
        {
            try
            {
                tmpSql = string.Format("delete Sys_Accounts_Roles where RoleID in ('{0}')", ArrayId.Replace(",", "','"));
                int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql);
                if (count > 0)
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
        #endregion

        #region 用户角色管理


        /// <summary>
        /// 指派用户角色
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="roleid">角色id</param>
        /// <returns></returns>
        public bool SetUserRole(Model.USER_REF_ROLE urr)
        {
            int result = 0;
            try
            {
                tmpSql = "select * from user_ref_role where RoleID='" + urr.RoleID + "' or UserID='" + urr.UserID + "'";
                DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    tmpSql = "update user_ref_role set RoleID='" + urr.RoleID + "',ModifyUserID='" + urr.ModifyUserID + "',ModifyDate='" + urr.ModifyDate + "' where UserID='" + urr.UserID + "'";
                }
                else
                {
                    tmpSql = "insert into user_ref_role(RoleID,UserID,CreateUserID,CreateDate) values('" + urr.RoleID + "','" + urr.UserID + "','" + urr.CreateUserID + "','" + urr.CreateDate + "')";
                }
                result = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql);
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
        /// 指派用户菜单权限
        /// </summary>
        /// <returns></returns>
        public bool SetMenuRole(List<string> menuids, string roleid)
        {
            int result = 0;
            StringBuilder sbuDel = new StringBuilder();
            StringBuilder sbu = new StringBuilder();
            try
            {
                for (int i = 0; i < menuids.Count; i++)
                {
                    sbuDel.Append("DELETE New_ROLE_Menu where MenuID='" + menuids[i] + "' AND roleid='" + roleid + "';");
                    sbu.Append("INSERT INTO New_ROLE_Menu (MenuID, roleid) VALUES('" + menuids[i] + "','" + roleid + "');");
                }
                DBHelper.DBHelperMsSql.ExecuteNonQuery(sbuDel.ToString());
                result = DBHelper.DBHelperMsSql.ExecuteNonQuery(sbu.ToString());
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
        /// 删除所有权限
        /// </summary>
        /// <returns></returns>
        public bool DelAllMenu(int roleid)
        {
            int result = 0;
            try
            {
                tmpSql = $"DELETE [dbo].[New_ROLE_Menu] where roleid={roleid}";
                result = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql);
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
        /// 删除所有按钮权限
        /// </summary>
        /// <returns></returns>
        public bool DelAllOpera(int roleid)
        {
            int result = 0;
            try
            {
                tmpSql = $"DELETE role_operate where roleid={roleid}";
                result = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql);
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

        public bool SetRoleOperate(int roleid, string[] ArrayOperate)
        {
            int result = 0;
            StringBuilder sbuDel = new StringBuilder();
            StringBuilder sbu = new StringBuilder();
            try
            {
                for (int i = 0; i < ArrayOperate.Length; i++)
                {
                    sbuDel.Append("DELETE role_operate where operateid='" + ArrayOperate[i] + "' AND roleid='" + roleid + "';");
                    sbu.Append("INSERT INTO role_operate (operateid,roleid) VALUES('" + ArrayOperate[i] + "','" + roleid + "');");
                }
                DBHelper.DBHelperMsSql.ExecuteNonQuery(sbuDel.ToString());
                result = DBHelper.DBHelperMsSql.ExecuteNonQuery(sbu.ToString());
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
        #endregion

        #region 更新缓存信息
        #endregion
    }
}