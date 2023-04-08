using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SZRiverSys.Model
{
    public class Role
    {
        public int RoleID { get; set; }
        public string Description { get; set; }
        public int RolePid { get; set; }
        public int RoleGroupID { get; set; }
        public int RoleGrade { get; set; }
        public string Remark { get; set; }

        #region 列表其它字段
        public RoleListHTML Operate { get; set; }//角色
        #endregion
    }

    public class Tree
    {
        public int code { get; set; }
        public List<TreeEntity> Data { get; set; }
    }

    public class TreeEntity
    {
        public TreeEntity()
        {
            this.@checked = false;
            this.disabled = true;
        }
        public string id { get; set; }
        public int pid { get; set; }
        public string label { get; set; }
        public List<TreeEntity> children { get; set; }
        public bool disabled { get; set; }
        public bool @checked { get; set; }
    }
}