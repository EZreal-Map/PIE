using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SZRiverSys.Model
{
    public class USER_REF_ROLE
    {
        public int URID { get; set; }
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public int CreateUserID { get; set; }
        public DateTime CreateDate { get; set; }
        public int ModifyUserID { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}