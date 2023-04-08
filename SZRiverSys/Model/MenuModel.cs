using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SZRiverSys.Model
{
    public class MenuModel
    { 
        /// <summary>
        /// 菜单ID
        /// </summary>
        public int MenuID { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { get; set; }

        /// <summary>
        /// 菜单地址
        /// </summary>
        public string MenuPath { get; set; }

        /// <summary>
        /// 父级菜单ID
        /// </summary>
        public int ParentMenuID { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public int orders { get; set; }

        /// <summary>
        /// 菜单状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

       
    }
}