﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SZRiverSys.SQL
{
    public class UserSql
    {

        #region 用户列表增删改查
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="search"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public DataTable GetUserList(int pageindex, int pagesize, string search,out int total)
        {
            string where = "1=1";
            if (search != "" && search != null)
            {
                where += " and userName like '" + search + "%'";
            }
            DataSet ds = new DataSet();
            ds = DBHelper.DBHelperMsSql.GetByPage(@"Sys_Accounts_Users a
                                                    inner join Sys_Accounts_UserRoles b on a.UserID=b.UserID
                                                    inner join Sys_Accounts_Roles c on b.RoleID=c.RoleID",//查询的表
                                                    "a.*,c.Description",//显示的字段
                                                    "a.UserID",
                                                    pagesize,
                                                    pageindex,
                                                    where,
                                                    "a.UserID",
                                                    out total);
            return ds.Tables[0];
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="search"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public DataTable GetUserList2(int pageindex, int pagesize, string search, out int total)
        {
            string where = "1=1";
            if (search != "" && search != null)
            {
                where += " and userName like '" + search + "%'";
            }
            DataSet ds = new DataSet();
            ds = DBHelper.DBHelperMsSql.GetByPage(@"Sys_Accounts_Users",//查询的表
                                                    "*",//显示的字段
                                                    "UserID",
                                                    pagesize,
                                                    pageindex,
                                                    where,
                                                    "UserID",
                                                    out total);
            return ds.Tables[0];
        }
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool CreateUser(Model.UserModel m)
        {
            string tmpsql = string.Format(@"insert into Sys_Accounts_Users(Password,EmailAddress,userName,Company,UserSex,Age,remark,usercode,tel,projectid, refIMEI) 
                                            values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')", 
                                            m.Password,m.EmailAddress,m.userName,m.Company,m.UserSex,m.Age,m.remark,m.usercode,m.tel,m.projectid,m.refIMEI);
            int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
            if (count > 0)
            {
                DataTable dt = GetUserInfoByUserCode(m.usercode);
                string UserId = dt.Rows[0]["UserID"].ToString();
                tmpsql = "insert into Sys_Accounts_UserRoles values('"+ UserId + "','"+m.RoleID+"')";
                DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
                return true;
            }else
            {
                return false;
            }

        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        public bool DelUser(string ID)
        {
            int result = 0;
            try
            {
                string tmpSql = string.Format("delete Sys_Accounts_UserRoles where UserID in ('{0}')", ID);
                //string tmpSql = "delete from tuserinfo where mobilecode='" + mobilecode + "'";
                result = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql);

                if (result > 0)
                {
                     tmpSql = string.Format("delete Sys_Accounts_Users where UserID in ('{0}')", ID);
                    //string tmpSql = "delete from tuserinfo where mobilecode='" + mobilecode + "'";
                    result = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql);
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
        /// 获取单个用户信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public DataTable GetUserByID(string ID)
        {
            string tmpsql = @"
select * from Sys_Accounts_Users a
                                                    inner join Sys_Accounts_UserRoles b on a.UserID = b.UserID
                                                    inner join Sys_Accounts_Roles c on b.RoleID = c.RoleID
            where a.UserID='"+ID+"'";
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 修改用户全部信息
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool EditUser(Model.UserModel m)
        {
            try
            {
                string tmpSql = string.Format("Update Sys_Accounts_Users set EmailAddress='{0}',userName='{1}',Company='{2}',UserSex='{3}',Age='{4}',remark='{5}',tel='{6}',projectid='{7}',refIMEI='{8}' where UserID='{9}'",
                                               m.EmailAddress, m.userName,m.Company,m.UserSex,m.Age,m.remark,m.tel,m.projectid,m.refIMEI, m.UserID);
               int result = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql);
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
        /// 修改用户部分信息
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool UpdateUser(Model.UserModel m)
        {
            try
            {
                string tmpSql = string.Format("Update Sys_Accounts_Users set EmailAddress='{0}',userName='{1}',UserSex='{2}',tel='{3}' where UserID='{4}'",
                                               m.EmailAddress, m.userName, m.UserSex, m.tel, m.UserID);
                int result = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql);
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
        /// 修改用户密码
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool UpdateUserPass(string newpass,int UserID)
        {
            try
            {
                string tmpSql = string.Format("Update Sys_Accounts_Users set password='{0}' where UserID='{1}'",newpass, UserID);
                int result = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSql);
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
        /// 修改用户角色
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public  bool EditUserPower(string UserId,string RoleID)
        {
            string tmpsql = string.Format("update Sys_Accounts_UserRoles set RoleID='{0}' where UserID='{1}'",RoleID,UserId);
            int result = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
            if (result > 0)
            {
                return true;
            }
            return false;
        }
        #endregion


        /// <summary>
        /// 判断账号密码是否正确
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public bool ValidateUser(string uid, string pwd)
        {
            bool result = false;
            try
            {
                string tmpSql = string.Format("select * from dbo.Sys_Accounts_Users where usercode='{0}' and Password='{1}'", uid, pwd);
                //DataTable dt = DBHelper.DBhelper.ReadTable(tmpSql);
                DataTable dt =DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    result = true;
                    return result;
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取要修改的用户信息
        /// </summary>
        /// <param name="ID">用户ID</param>
        /// <returns></returns>
        public DataTable GetEditUserInfo(Model.UserModel m)
        {
            string tmpSql = string.Format(@"select a.*,c.Description as RoleName from Sys_Accounts_Users a
left join Sys_Accounts_UserRoles b on a.UserID = b.UserID
left join Sys_Accounts_Roles c on b.RoleID = c.RoleID where a.UserID='{0}'", m.UserID);
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
            DataColumn ProJectName = new DataColumn("ProJectName", typeof(string));
            dt.Columns.Add(ProJectName);
            tmpSql = $@"select * from RM_ProjectInfo where CHARINDEX('{m.userName}',ProjectTeamMembers)>0 ";
            DataTable pdt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
            string proName = "";
            for(int i = 0; i < pdt.Rows.Count; i++)
            {
                proName += pdt.Rows[i]["ProJectName"].ToString()+",";
            }
            dt.Rows[0]["ProJectName"] = proName.TrimEnd(',');
            return dt;

        }
        /// <summary>
        /// 根据账号查询用户信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public DataTable GetUserInfoByUserCode(string code)
        {
            string tmpSql = string.Format(@"select a.*,c.* from dbo.Sys_Accounts_Users a
left join Sys_Accounts_UserRoles b on a.UserID = b.UserID
left join Sys_Accounts_Roles c on b.RoleID = c.RoleID where usercode = '{0}'",code);
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
            return dt;
        }
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetUserList()
        {
            string tmpSql = string.Format("select * from dbo.Sys_Accounts_Users");
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
            return dt;
        }
        /// <summary>
        /// 根据用户编号获取用户角色
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public DataTable GetUserRoleIdByUserCode(string code)
        {
            string tmpSql = string.Format("select * from dbo.Sys_Accounts_UserRoles where UserID='{0}'", code);
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
            return dt;
        }
        /// <summary>
        /// 上传用户坐标
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        //public bool PutUserGps(Model.UserGPSModel m)
        //{
        //    string tmpsql = string.Format("insert into cgs_baseinfo.dbo.UserGPS values('{0}','{1}','{2}','{3}','','',now())", m.ProjectID,m.UserID,m.lon,m.lat);
        //    int result = DBHelper.DBHelperMsSql.ExecuteSql(tmpsql);
        //    if(result>0)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetUserInfo(int pageindex, int pagesize, string search, out int total)
        {
            //total = 0;
            //int m = (pageindex - 1) * pagesize;
            //string tmpSql = @"select u.*,ur.id as rid,ur.rolename from tuserinfo u 
            //            inner join USER_ROLE_REF urr on urr.userid=u.ID
            //            inner join USER_ROLE ur on ur.id=urr.roleid where 1=1 ";
            #region sql分页存储过程
            string where = "1=1 ";
            try
            {
                if (search != "")
                {
                    where += " and u.mobilecode like'" + search + "%' or u.code like '" + search + "%' or u.name LIKE '" + search + "%'";
                }
                DataSet ds = new DataSet();
                ds = DBHelper.DBHelperMsSql.GetByPage(@"cgs_baseinfo.dbo.tuserinfo u 
                                                    inner join cgs_baseinfo.dbo.USER_ROLE_REF urr on urr.userid=u.ID
                                                    inner join cgs_baseinfo.dbo.USER_ROLE ur on ur.id=urr.roleid",//查询的表
                                                        "u.*,ur.id as rid,ur.rolename",//显示的字段
                                                        "u.id",
                                                        pagesize,
                                                        pageindex,
                                                        where,
                                                        "u.id",
                                                        out total);
                return ds.Tables[0];
            }catch(Exception)
            {
                throw;
            }
            #endregion
            //DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
            //total = dt.Rows.Count;
            //tmpSql += " limit " + m + "," + pagesize + "";
            //try
            //{
            //    dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql); ;
            //    return dt;
            //}
            //catch (Exception)
            //{

            //    throw;
            //}
        }



 

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public bool UpdatePassWord(string md5pass, string pass, string mobilecode)
        {
            int result = 0;
            try
            {
                string tmpSql = string.Format("Update cgs_baseinfo.dbo.tuserinfo set passmd5='{0}',pass='{1}' where mobilecode='{2}'", md5pass, pass, mobilecode);
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
        /// 注册
        /// </summary>
        /// <param name="User">用户实体</param>
        /// <returns></returns>
        //public bool Register(Model.UserModel User)
        //{
        //    int result = 0;
        //    try
        //    {
        //        string tmpSql = string.Format(@"INSERT INTO cgs_baseinfo.dbo.tuserinfo (name, mobilecode, passmd5, pass, sex, createdt,[identity],code) 
        //                            VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');", User.Name, User.Mobilecode, User.Passwordmd5, User.Password, User.sex, User.CreateDate, User.identity, User.Code);
        //        result = DBHelper.DBHelperMsSql.ExecuteSql(tmpSql);

        //        //获取新增用户的ID
        //        tmpSql = "select id from cgs_baseinfo.dbo.tuserinfo where mobilecode='" + User.Mobilecode + "'";
        //        DataTable dt = DBHelper.DBHelperMsSql.Query(tmpSql).Tables[0];

        //        //初始化用户时默认角色为普通员工
        //        tmpSql = "insert into cgs_baseinfo.dbo.USER_ROLE_REF(roleid,userid) values(2,'" + dt.Rows[0][0].ToString() + "')";
        //        DBHelper.DBHelperMsSql.ExecuteSql(tmpSql);
        //        if (result > 0)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        /// <summary>
        /// 获取用户编号
        /// </summary>
        /// <param name="mobilecode"></param>
        /// <returns></returns>
        //public DataTable GetUsercode(string mobilecode)
        //{
        //    try
        //    {
        //        //string tmpSql = "SELECT right(CRC32(concat(ifnull('" + mobilecode + "',''),NOW())),32) as Usercode";
        //        string tmpSql = "select cast( floor(rand()*999999) as varchar(500)) as Usercode";
        //        DataTable dt = DBHelper.DBHelperMsSql.Query(tmpSql).Tables[0];
        //        return dt;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="User">用户实体</param>
        /// <returns></returns>
        //public bool EditUser(Model.UserModel User)
        //{
        //    int result = 0;
        //    try
        //    {
        //        string tmpSql = string.Format(@"UPDATE cgs_baseinfo.dbo.tuserinfo SET  
        //                                                                name='{0}', 
        //                                                                mobilecode='{1}',
        //                                                                sex='{2}', 
        //                                                                [identity]='{3}', 
        //                                                                remark='{4}', 
        //                                                                createdt='{5}'  
        //                                                                WHERE (id='{6}') 
        //                                                                AND (code='{7}');"
        //                                                                , User.Name
        //                                                                , User.Mobilecode
        //                                                                , User.sex
        //                                                                , User.identity
        //                                                                , User.remark
        //                                                                , User.CreateDate
        //                                                                , User.id
        //                                                                , User.Code);
        //        result = DBHelper.DBHelperMsSql.ExecuteSql(tmpSql);
        //        if (result > 0)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}


        /// <summary>
        /// 根据mobilecode获取用户信息
        /// </summary>
        /// <returns></returns>
        //public DataTable GetUserInfo_ByID(string mobilecode, string code)
        //{
        //    string tmpSql = "select * from cgs_baseinfo.dbo.tuserinfo where mobilecode='" + mobilecode + "' or code='" + code + "'";
        //    try
        //    {
        //        DataTable dt = DBHelper.DBHelperMsSql.Query(tmpSql).Tables[0];
        //        return dt;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        //public DataTable GetProjectInThePMByMe(string uid)
        //{
        //    string tmpSql = "select id,ProjectNo,ProjectName from cgs_baseinfo.dbo.ProjectInfo where PM ='" + uid + "'";
        //    try
        //    {
        //        DataTable dt = DBHelper.DBHelperMsSql.Query(tmpSql).Tables[0];
        //        return dt;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        /// <summary>
        /// 根据mobilecode获取用户信息
        /// </summary>
        /// <returns></returns>
        //public DataTable GetUserInfo_Bymobile(string mobilecode)
        //{
        //    string tmpSql = @"select u.*,ur.id as rid,ur.rolename from cgs_baseinfo.dbo.tuserinfo u 
        //                    inner join cgs_baseinfo.dbo.USER_ROLE_REF urr on urr.userid=u.ID
        //                    inner join cgs_baseinfo.dbo.USER_ROLE ur on ur.id=urr.roleid where mobilecode='" + mobilecode + "'";
        //    try
        //    {
        //        DataTable dt = DBHelper.DBHelperMsSql.Query(tmpSql).Tables[0];
        //        return dt;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
        /// <summary>
        /// 获取所有用户数据
        /// </summary>
        /// <returns></returns>
        //public DataTable GetAllUser()
        //{
        //    string tmpsql = @"select * from [dbo].[tuserinfo] u 
        //                    inner join cgs_baseinfo.dbo.USER_ROLE_REF urr on urr.userid = u.ID
        //                    inner join cgs_baseinfo.dbo.USER_ROLE ur on ur.id = urr.roleid";
        //    DataTable dt = DBHelper.DBHelperMsSql.Query(tmpsql).Tables[0];
        //    return dt;
        //}
    }
}