using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SZRiverSys.Model
{
    /// <summary>
    /// 基础信息管理
    /// </summary>
    public class User1001HTML
    {
        public User1001HTML()
        {
            this.btnAdd = false;
            this.btnEdit = false;
            this.btnDel = false;
        }
        public bool btnAdd { get; set; }
        public bool btnEdit { get; set; }
        public bool btnDel { get; set; }

    }

    /// <summary>
    /// 角色管理
    /// </summary>
    public class RoleListHTML
    {
        public RoleListHTML()
        {

            this.btnAdd = false;
            this.btnEdit = false;
            this.btnDel = false;
            this.btnSetMenuRole = false;
            this.btnSetOperateRole = false;
        }
        public bool btnAdd { get; set; }
        public bool btnEdit { get; set; }
        public bool btnDel { get; set; }
        public bool btnSetMenuRole { get; set; }
        public bool btnSetOperateRole { get; set; }

    }

    /// <summary>
    /// 工作轨迹管理
    /// </summary>
    public class User1002HTML
    {
        public User1002HTML()
        {

            this.btnAdd = false;
            this.btnEdit = false;
            this.btnDel = false;
        }
        public bool btnAdd { get; set; }
        public bool btnEdit { get; set; }
        public bool btnDel { get; set; }
    }

    /// <summary>
    /// 工作时长
    /// </summary>
    public class User1003HTML
    {
        public User1003HTML()
        {

            this.btnAdd = false;
            this.btnEdit = false;
            this.btnDel = false;
        }
        public bool btnAdd { get; set; }
        public bool btnEdit { get; set; }
        public bool btnDel { get; set; }
    }

    /// <summary>
    /// 车辆管理
    /// </summary>
    public class Device2001HTML
    {
        public Device2001HTML()
        {

            this.btnAdd = false;
            this.btnEdit = false;
            this.btnDel = false;
        }
        public bool btnAdd { get; set; }
        public bool btnEdit { get; set; }
        public bool btnDel { get; set; }

    }

    /// <summary>
    /// 设备管理
    /// </summary>
    public class Device2002HTML
    {
        public Device2002HTML()
        {

            this.btnAdd = false;
            this.btnEdit = false;
            this.btnDel = false;
        }
        public bool btnAdd { get; set; }
        public bool btnEdit { get; set; }
        public bool btnDel { get; set; }
    }
    /// <summary>
    /// 巡线计划管理
    /// </summary>
    public class Patrol2002HTML
    {
        public Patrol2002HTML()
        {

            this.btnAdd = false;
            this.btnDel = false;
        }
        public bool btnAdd { get; set; }
        public bool btnDel { get; set; }
    }
    /// <summary>
    /// 物资管理
    /// </summary>
    public class Device2003HTML
    {
        public Device2003HTML()
        {

            this.btnAdd = false;
            this.btnEdit = false;
            this.btnDel = false;
        }
        public bool btnAdd { get; set; }
        public bool btnEdit { get; set; }
        public bool btnDel { get; set; }
    }

    /// <summary>
    /// 办公室管理
    /// </summary>
    public class Device2004HTML
    {
        public Device2004HTML()
        {

            this.btnAdd = false;
            this.btnEdit = false;
            this.btnDel = false;
        }
        public bool btnAdd { get; set; }
        public bool btnEdit { get; set; }
        public bool btnDel { get; set; }
    }

    /// <summary>
    /// 文档管理
    /// </summary>
    public class File2005HTML
    {
        public File2005HTML()
        {

            this.btnAdd = false;
            this.btnEdit = false;
            this.btnDel = false;
        }
        public bool btnAdd { get; set; }
        public bool btnEdit { get; set; }
        public bool btnDel { get; set; }
    }

    /// <summary>
    /// 水质
    /// </summary>
    public class Dispatch3001HTML
    {
        public Dispatch3001HTML()
        {

            this.btnAdd = false;
            this.btnEdit = false;
            this.btnDel = false;
        }
        public bool btnAdd { get; set; }
        public bool btnEdit { get; set; }
        public bool btnDel { get; set; }
    }

    /// <summary>
    /// 调度
    /// </summary>
    public class Dispatch3002HTML
    {
        public Dispatch3002HTML()
        {

            this.btnAdd = false;
            this.btnEdit = false;
            this.btnDel = false;
        }
        public bool btnAdd { get; set; }
        public bool btnEdit { get; set; }
        public bool btnDel { get; set; }

    }
    /// <summary>
    /// 预警指示
    /// </summary>
    public class Dispatch3003HTML
    {
        public Dispatch3003HTML()
        {

            this.btnAdd = false;
            this.btnEdit = false;
            this.btnDel = false;
        }
        public bool btnAdd { get; set; }
        public bool btnEdit { get; set; }
        public bool btnDel { get; set; }
    }
    /// <summary>
    /// 任务下发
    /// </summary>
    public class Task4001HTML
    {
        public Task4001HTML()
        {

            this.btnAdd = false;
            this.btnEdit = false;
            this.btnDel = false;
        }
        public bool btnAdd { get; set; }
        public bool btnEdit { get; set; }
        public bool btnDel { get; set; }
    }

    /// <summary>
    /// 工单下发
    /// </summary>
    public class Task4002HTML
    {
        public Task4002HTML()
        {

            this.btnAdd = false;
            this.btnDel = false;
            this.btnEND = false;
            this.btnEdit = false;
        }
        public bool btnAdd { get; set; }
        public bool btnDel { get; set; }
        public bool btnEND { get; set; }

        public bool btnEdit { get; set; }
    }
    /// <summary>
    /// 档案资料管理
    /// </summary>
    public class File6000HTML
    {
        public File6000HTML()
        {

            this.btnAdd = false;
            this.btnEdit = false;
            this.btnDel = false;
        }
        public bool btnAdd { get; set; }
        public bool btnEdit { get; set; }
        public bool btnDel { get; set; }
    }
    /// <summary>
    /// 项目列表
    /// </summary>
    public class Project7001HTML
    {
        public Project7001HTML()
        {

            this.btnAdd = false;
            this.btnEdit = false;
            this.btnProjectUser = true;
        }
        public bool btnAdd { get; set; }
        public bool btnEdit { get; set; }
        public bool btnDel { get; set; }
        public bool btnProjectUser { get; set; }
    }
    /// <summary>
    /// 项目计划
    /// </summary>
    public class Project7002HTML
    {
        public Project7002HTML()
        {

            this.btnAdd = false;
            this.btnEdit = false;
            this.btnDel = false;
        }
        public bool btnAdd { get; set; }
        public bool btnEdit { get; set; }
        public bool btnDel { get; set; }
    }
    /// <summary>
    /// 计划类别管理
    /// </summary>
    public class Project7003HTML
    {
        public Project7003HTML()
        {

            this.btnAdd = false;
            this.btnEdit = false;
            this.btnDel = false;
        }
        public bool btnAdd { get; set; }
        public bool btnEdit { get; set; }
        public bool btnDel { get; set; }

    }

    /// <summary>
    /// 巡线计划
    /// </summary>
    public class XX9001HTML
    {
        public XX9001HTML()
        {
            this.btnAdd = false;
            this.btnEdit = false;
            this.btnDel = false;
        }
        public bool btnAdd { get; set; }
        public bool btnEdit { get; set; }
        public bool btnDel { get; set; }

    }

    /// <summary>
    /// 巡查报告
    /// </summary>
    public class XX9002HTML
    {
        public XX9002HTML()
        {
            this.btnAdd = false;
            this.btnEdit = false;
            this.btnDel = false;
        }
        public bool btnAdd { get; set; }
        public bool btnEdit { get; set; }
        public bool btnDel { get; set; }

    }

    /// <summary>
    /// 巡线轨迹
    /// </summary>
    public class XX9003HTML
    {
        public XX9003HTML()
        {
            this.btnAdd = false;
            this.btnEdit = false;
            this.btnDel = false;
        }
        public bool btnAdd { get; set; }
        public bool btnEdit { get; set; }
        public bool btnDel { get; set; }
    }
}