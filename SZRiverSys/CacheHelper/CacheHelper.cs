﻿//                            _ooOoo_  
//                           o8888888o  
//                           88" . "88  
//                           (| -_- |)  
//                            O\ = /O  
//                        ____/`---'\____  
//                      .   ' \\| |// `.  
//                       / \\||| : |||// \  
//                     / _||||| -:- |||||- \  
//                       | | \\\ - /// | |  
//                     | \_| ''\---/'' | |  
//                      \ .-\__ `-` ___/-. /  
//                   ___`. .' /--.--\ `. . __  
//                ."" '< `.___\_<|>_/___.' >'"".  
//               | | : `- \`.;`\ _ /`;.`/ - ` : | |  
//                 \ \ `-. \_ __\ /__ _/ .-` / /  
//         ======`-.____`-.___\_____/___.-`____.-'======  
//                            `=---='  
//  
//         .............................................  
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using SZRiverSys.Model;

namespace SZRiverSys.CacheHelper
{
    /// <summary>
    /// class_memo:服务器缓存操作类 
    /// create_user:王铄文
    /// create_time:2019-3-27
    /// </summary>
    public class CacheHelper
    {
        public object this[string index] { get { return dicStrObj[index]; } set { lock (obj) { dicStrObj[index] = value; } } }
        private static Dictionary<string, object> dicStrObj = new Dictionary<string, object>();
        private static object obj = new object();
        public static string ServerURL="";

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key">key值</param>
        /// <returns></returns>
        public static object Get(string key)
        {
            if (key == null || !dicStrObj.ContainsKey(key))
                return null;
            return dicStrObj[key];
        }

        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="key">key值</param>
        /// <param name="value">value值</param>
        public static void Set(string key, object value)
        {
            lock (obj)
            {
                dicStrObj.Add(key, value);
            }
        }

        /// <summary>
        /// 检索是否有此key
        /// </summary>
        /// <param name="key">key值</param>
        /// <returns>返回布尔类型</returns>
        public static bool IsContain(string key)
        {
            return dicStrObj.ContainsKey(key);
        }
        
        /// <summary>
        /// 删除相对应的key值数据
        /// </summary>
        /// <param name="key">key值</param>
        public static void Remove(string key)
        {
            lock (obj)
            {
                if (dicStrObj.Keys.Contains(key))
                {
                    dicStrObj.Remove(key);
                }
            }
        }


        /// <summary>
        /// 缓存用户信息
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <returns></returns>
        public static string CacheUserInfo(string userNo, out int roleID, int logintype)
        {
            SQL.UserSql Sql = new SQL.UserSql();
            Model.UserModel User = new Model.UserModel();            
            string result = "";
            roleID = 0;
            //缓存数据
            try
            {
                DataTable table = Sql.GetUserInfoByUserCode(userNo);
                if (table != null && table.Rows.Count > 0)
                {
                    User.UserID = Convert.ToInt32(table.Rows[0]["UserID"].ToString());
                    User.Password = table.Rows[0]["Password"].ToString();
                    User.EmailAddress = table.Rows[0]["EmailAddress"].ToString();
                    User.userName = table.Rows[0]["userName"].ToString();
                    User.Company = table.Rows[0]["Company"].ToString();
                    User.UserSex = table.Rows[0]["UserSex"].ToString();
                    User.Age = table.Rows[0]["Age"].ToString();
                    User.remark = table.Rows[0]["remark"].ToString();
                    User.usercode = table.Rows[0]["usercode"].ToString();
                    string token = userNo + Guid.NewGuid().ToString();
                    try
                    {
                        DataTable dt = Sql.GetUserRoleIdByUserCode(User.UserID.ToString());
                        if (table != null && dt.Rows.Count > 0)
                        {
                            Model.Role role = new Model.Role();
                            roleID = int.Parse(dt.Rows[0]["RoleID"].ToString());
                            User.tel = table.Rows[0]["tel"].ToString();
                            User.projectid = table.Rows[0]["projectid"].ToString();
                            role.RoleID = int.Parse(table.Rows[0]["RoleID"].ToString());
                            role.RolePid = int.Parse(table.Rows[0]["RolePid"].ToString());
                            role.RoleGrade = int.Parse(table.Rows[0]["RoleGrade"].ToString());
                            role.Description = table.Rows[0]["Description"].ToString();
                            role.Remark = table.Rows[0]["Remark"].ToString();
                            User.RoleList = role;
                            User.RoleID = roleID;
                        }
                    }
                    catch
                    {

                    }                                      
                    if (userNo == "lsd")
                    {
                        token = "adminxxxxxxxxxx";//临时测试
                    }
                    else if (userNo == "xzy0")
                    {
                        token = "xzy0xxx";//临时测试
                    }
                    if (logintype == 0)//app登录
                    {
                        token = token + "app";
                        Set(token, User);//缓存到字典里
                        Set(userNo, token);//此操作用于找到token 来删除token
                    }
                    else if (logintype == 1)//web 端登录
                    {
                        token = token + "web";
                        Set(token, User);//缓存到字典里
                        Set(userNo, token);//此操作用于找到token 来删除token
                    }
                    result = token;
                }
            }
            catch (Exception ex)
            {
                //throw;
            }
            return result;
        }



        //private static Dictionary<string, UserModel> usersDict = new Dictionary<string, UserModel>();

        //private static Dictionary<string, string> userTokenNameDict = new Dictionary<string, string>();
        //private static Dictionary<string, string> userTokenNameDict2 = new Dictionary<string, string>();
        //private static Dictionary<string, string> userNameTokenDict = new Dictionary<string, string>();
               
        //public static string CacheUserInfo(string userNo, out int roleID, int logintype)
        //{
        //    if(userNameTokenDict.ContainsKey(userNo))
        //    {
        //        string tmpToken = userNameTokenDict[userNo];
        //        if
        //    }

        //    Model.UserModel User = null;
        //    if (usersDict.ContainsKey(userNo))
        //        User = usersDict[userNo];
        //    else
        //    {
        //        SQL.UserSql Sql = new SQL.UserSql();
        //        User = new Model.UserModel();
        //        Model.Role role = new Model.Role();
        //        string result = "";
        //        roleID = 0;
        //        //缓存数据
        //        try
        //        {
        //            DataTable table = Sql.GetUserInfoByUserCode(userNo);
        //            if (table != null && table.Rows.Count > 0)
        //            {
        //                User.UserID = Convert.ToInt32(table.Rows[0]["UserID"].ToString());
        //                User.Password = table.Rows[0]["Password"].ToString();
        //                User.EmailAddress = table.Rows[0]["EmailAddress"].ToString();
        //                User.userName = table.Rows[0]["userName"].ToString();
        //                User.Company = table.Rows[0]["Company"].ToString();
        //                User.UserSex = table.Rows[0]["UserSex"].ToString();
        //                User.Age = table.Rows[0]["Age"].ToString();
        //                User.remark = table.Rows[0]["remark"].ToString();
        //                User.usercode = table.Rows[0]["usercode"].ToString();
        //                string token = userNo + Guid.NewGuid().ToString();
        //                DataTable dt = Sql.GetUserRoleIdByUserCode(User.UserID.ToString());
        //                roleID = int.Parse(dt.Rows[0]["RoleID"].ToString());
        //                User.tel = table.Rows[0]["tel"].ToString();
        //                User.projectid = table.Rows[0]["projectid"].ToString();
        //                role.RoleID = int.Parse(table.Rows[0]["RoleID"].ToString());
        //                role.RolePid = int.Parse(table.Rows[0]["RolePid"].ToString());
        //                role.RoleGrade = int.Parse(table.Rows[0]["RoleGrade"].ToString());
        //                role.Description = table.Rows[0]["Description"].ToString();
        //                role.Remark = table.Rows[0]["Remark"].ToString();
        //                User.RoleList = role;
        //                User.RoleID = roleID;
        //                if (userNo == "lsd")
        //                {
        //                    token = "adminxxxxxxxxxx";//临时测试
        //                }
        //                else if (userNo == "xzy0")
        //                {
        //                    token = "xzy0xxx";//临时测试
        //                }
        //                if (logintype == 0)//app登录
        //                {
        //                    token = token + "app";
        //                    Set(token, User);//缓存到字典里
        //                    Set(userNo, token);//此操作用于找到token 来删除token
        //                }
        //                else if (logintype == 1)//web 端登录
        //                {
        //                    token = token + "web";
        //                    Set(token, User);//缓存到字典里
        //                    Set(userNo, token);//此操作用于找到token 来删除token
        //                }
        //                result = token;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            //throw;
        //        }
        //    }
            
        //    return result;
        //}
    }
}