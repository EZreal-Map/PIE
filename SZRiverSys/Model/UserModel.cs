using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SZRiverSys.Model
{
    /// <summary>
    /// 用户类
    /// </summary>
    public class UserModel
    {
        public int UserID { get; set; }
        public string userName { get; set; }
        public string Mobilecode { get; set; }
        public string Passwordmd5 { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public string Company { get; set; }
        public string UserSex { get; set; }
        public string Age { get; set; }
        public string remark { get; set; }
        public string usercode { get; set; }

        public object Tag { get; set; }

        //public string Img { get; set; }
        public int RoleID { get; set; }
        public string tel { get; set; }
        //public string RoleName { get; set; }
        public string projectid;


        public Role RoleList { get; set; }//角色信息
        public User1001HTML Operate { get; set; }//角色

        public string refIMEI { get; set; }
    }
}