using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using SZRiverSys.Model;
namespace SZRiverSys.SQL
{
    public class ProjectSql
    {
        #region 公共变量
        DataTable dt = new DataTable();
        string tmpsql = "";
        #endregion

        #region 查询操作

        /// <summary>
        /// 获取项目信息
        /// </summary>
        /// <param name="Search">筛选条件</param>
        /// <returns></returns>
        public DataTable GetProjectInfo(string Search, UserModel userModel)
        {
            try
            {
                tmpsql = $@"select rm_p.* from RM_ProjectInfo rm_p where 1=1";
                if (userModel.RoleID != 1)
                {
                    tmpsql = $@"select rm_p.* from RM_ProjectInfo rm_p
                            inner join Project_ref_User pru on rm_p.ID = pru.ProjectID
                            inner join Sys_Accounts_Users u on u.UserID = pru.UserID
                            where 1 = 1 and pru.UserID='{userModel.UserID}'";
                }
                if (Search != "")
                {
                    tmpsql += $" and rm_p.ProJectNo like '%{Search}%' or rm_p.ProJectName like '%{Search}%'";
                }
                dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 获取项目信息
        /// </summary>
        /// <param name="Search">筛选条件</param>
        /// <returns></returns>
        public DataTable GetProjectInfo(UserModel userModel)
        {
            try
            {
                tmpsql = $@"select * from RM_ProjectInfo where 1=1";
                if (userModel.RoleID != 1)
                {
                    tmpsql += $" and CHARINDEX('{userModel.userName}',ProjectTeamMembers)>0 ";
                }
                dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 获取某一个项目信息
        /// </summary>
        /// <param name="ID">当前选中行ID</param>
        /// <returns></returns>
        public DataTable GetProjectInfoByID(int ID)
        {
            try
            {
                tmpsql = $"select * from RM_ProjectInfo where ID='{ID}'";
                dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 验证该项目是否存在
        /// </summary>
        /// <param name="ProJectNo">项目编号</param>
        /// <returns></returns>
        public bool ValidateProjectInfoNo(string ProJectNo)
        {
            try
            {
                tmpsql = $"select * from RM_ProjectInfo where ProJectNo='{ProJectNo}'";
                dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                if (dt.Rows.Count > 0)
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

        #region 新增操作

        /// <summary>
        /// 新增项目
        /// </summary>
        /// <param name="projectModel">项目实体</param>
        /// <returns></returns>
        public bool AddProjectInfo(ProjectModel projectModel)
        {
            try
            {
                tmpsql = $@"INSERT INTO [dbo].[RM_ProjectInfo]
                               ([ProJectNo]
                               ,[ProJectName]
                               ,[PM]
                               ,[PM_Tel]
                               ,[PM_Job]
                               ,[CreateDate]
                               ,[CreateUser]
                               ,[BeginDate]
                               ,[EndDate]
                               ,[Location]
                               ,[PartyAUnit]
                               ,[PartyAPersonName]
                               ,[PartyAPersonTel]
                               ,[PartyAPersonJob]
                               ,[PartyBUnit]
                               ,[PartyBPersonName]
                               ,[PartyBPersonTel]
                               ,[PartyBPersonJob]
                               ,[Remark])
                         VALUES
                               ('{projectModel.ProJectNo}'
                               ,'{projectModel.ProJectName}'
                               ,'{projectModel.PM}'
                               ,'{projectModel.PM_Tel}'
                               ,'{projectModel.PM_Job}'
                               ,'{projectModel.CreateDate}'
                               ,'{projectModel.CreateUser}'
                               ,'{projectModel.BeginDate}'
                               ,'{projectModel.EndDate}'
                               ,'{projectModel.Location}'
                               ,'{projectModel.PartyAUnit}'
                               ,'{projectModel.PartyAPersonName}'
                               ,'{projectModel.PartyAPersonTel}'
                               ,'{projectModel.PartyAPersonJob}'
                               ,'{projectModel.PartyBUnit}'
                               ,'{projectModel.PartyBPersonName}'
                               ,'{projectModel.PartyBPersonTel}'
                               ,'{projectModel.PartyBPersonJob}'
                               ,'{projectModel.Remark}')";
                int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
                if (count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region 修改操作

        /// <summary>
        /// 修改项目
        /// </summary>
        /// <param name="projectModel">项目实体</param>
        /// <returns></returns>
        public bool EditProjectInfo(ProjectModel projectModel)
        {
            try
            {
                tmpsql = $@"UPDATE [dbo].[RM_ProjectInfo]
                               SET 
                                   [ProJectName] = '{projectModel.ProJectName}'
                                  ,[PM] = '{projectModel.PM}'
                                  ,[PM_Tel] = '{projectModel.PM_Tel}'
                                  ,[PM_Job] = '{projectModel.PM_Job}'
                                  ,[ModifyDate] = '{projectModel.ModifyDate}'
                                  ,[ModifyUser] = '{projectModel.ModifyUser}'
                                  ,[BeginDate] = '{projectModel.BeginDate}'
                                  ,[EndDate] = '{projectModel.EndDate}'
                                  ,[Location] = '{projectModel.Location}'
                                  ,[PartyAUnit] = '{projectModel.PartyAUnit}'
                                  ,[PartyAPersonName] ='{projectModel.PartyAPersonName}'
                                  ,[PartyAPersonTel] = '{projectModel.PartyAPersonTel}'
                                  ,[PartyAPersonJob] = '{projectModel.PartyAPersonJob}'
                                  ,[PartyBUnit] = '{projectModel.PartyBUnit}'
                                  ,[PartyBPersonName] = '{projectModel.PartyBPersonName}'
                                  ,[PartyBPersonTel] = '{projectModel.PartyBPersonTel}'
                                  ,[PartyBPersonJob] = '{projectModel.PartyBPersonJob}'
                                  ,[Remark] = '{projectModel.Remark}'
                             WHERE [ID]= '{projectModel.ID}'";
                int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
                if (count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region 删除操作
        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="ArrayID">要删除的选中行ID字符串，以逗号分隔</param>
        /// <returns></returns>
        public bool DelProjectInfo(string ArrayID)
        {
            try
            {
                tmpsql = $"DELETE [dbo].[RM_ProjectInfo] WHERE ID in('{ArrayID.Replace(",", "','")}')";
                int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
                if (count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region 指派项目成员操作

        /// <summary>
        /// 获取不在项目的成员
        /// </summary>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public DataTable GetNotInProjectUser(string ProjectID)
        {
            try
            {
                //tmpsql = $"select * from RM_ProjectInfo where id='{ProjectID}'";
                //dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                //if (dt.Rows.Count > 0)
                //{
                //    string Users = dt.Rows[0]["ProjectTeamMembers"].ToString() == null ? "" : dt.Rows[0]["ProjectTeamMembers"].ToString();
                //    if (Users != "")
                //    {
                //        tmpsql = $"select * from Sys_Accounts_Users where userName not in('{Users.Replace(",", "','")}')";
                //        dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                //    }
                //}
                tmpsql = $@"select * from Sys_Accounts_Users where UserID not in(select DISTINCT u.UserID from Sys_Accounts_Users u
                            inner join Project_ref_User pru on u.UserID=pru.UserID
                            where pru.ProjectID='{ProjectID}')";
                dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 获取在项目的成员
        /// </summary>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public DataTable GetInProjectUser(string ProjectID)
        {
            try
            {
                //tmpsql = $"select * from RM_ProjectInfo where id='{ProjectID}'";
                //dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                //if (dt.Rows.Count > 0)
                //{
                //    string Users = dt.Rows[0]["ProjectTeamMembers"].ToString() == null ? "" : dt.Rows[0]["ProjectTeamMembers"].ToString();
                //    if (Users != "")
                //    {
                //        tmpsql = $"select * from Sys_Accounts_Users where userName in('{Users.Replace(",", "','")}')";
                //        dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                //    }
                //}

                tmpsql = $@"select u.* from Sys_Accounts_Users u
                            inner join Project_ref_User pru on u.UserID = pru.UserID
                            where pru.ProjectID = '{ProjectID}'";
                dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 指派项目组成员
        /// </summary>
        /// <param name="ArrayUser"></param>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public bool SetProjectTeamMembers(string ArrayUser, string ProjectID)
        {
            try
            {
                //tmpsql = $@"UPDATE RM_ProjectInfo SET ProjectTeamMembers='{ArrayUser}' WHERE ID='{ProjectID}'";
                //int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
                //if (count > 0)
                //{
                //    return true;
                //}

                for (int i = 0; i < ArrayUser.Split(',').Length; i++)
                {
                    tmpsql += $@"INSERT INTO Project_ref_User(UserID,ProjectID) VALUES('{ArrayUser.Split(',')[i].ToString()}','{ProjectID}');";
                }
                int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
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

        /// <summary>
        /// 获取某一个项目信息
        /// </summary>
        /// <param name="ID">当前选中行ID</param>
        /// <returns></returns>
        public string GetUserNameByID(string ArrayID)
        {
            try
            {
                string str = "";
                tmpsql = $"select * from Sys_Accounts_Users where UserID in('{ArrayID.Replace(",", "','")}')";
                dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        str += dt.Rows[i]["userName"] + ",";
                    }
                    str = str.Substring(0, str.Length - 1);
                }
                return str;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 指派项目组成员
        /// </summary>
        /// <param name="ArrayUser"></param>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public bool SetProjectTeamMembersTwo(string ArrayUser, string ProjectID)
        {
            try
            {
                tmpsql = $@"UPDATE RM_ProjectInfo SET ProjectTeamMembers='{ArrayUser}' WHERE ID='{ProjectID}'";
                int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
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

        /// <summary>
        /// 移除项目组成员
        /// </summary>
        /// <param name="ArrayUser"></param>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public bool RemoveProjectUser(string ArrayUser, string ProjectID)
        {
            try
            {
                //tmpsql = $@"UPDATE RM_ProjectInfo SET ProjectTeamMembers='{ArrayUser}' WHERE ID='{ProjectID}'";
                //int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
                //if (count > 0)
                //{
                //    return true;
                //}

                for (int i = 0; i < ArrayUser.Split(',').Length; i++)
                {
                    tmpsql += $@"DELETE Project_ref_User WHERE UserID='{ArrayUser.Split(',')[i].ToString()}' AND ProjectID='{ProjectID}';";
                }
                int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
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

        /// <summary>
        /// 移除项目组成员
        /// </summary>
        /// <param name="ArrayUser"></param>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public bool RemoveProjectUserTwo(string ArrayUser, string ProjectID)
        {
            try
            {
                tmpsql = $@"UPDATE RM_ProjectInfo SET ProjectTeamMembers='{ArrayUser}' WHERE ID='{ProjectID}'";
                int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
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

        #region 进度统计操作

        #endregion

    }
}