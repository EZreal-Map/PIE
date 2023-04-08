using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using SZRiverSys.Model;

namespace SZRiverSys.Comm
{
    public class RolePowerHelper
    {
        /// <summary>
        /// 角色管理
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public RoleListHTML ROLE_RoleListHTML(int RoleID)
        {
            try
            {
                List<RoleListHTML> ListEntity = new List<RoleListHTML>();
                RoleListHTML _ROLH = new RoleListHTML();
                DataTable dt = GetRolePower(RoleID, 8001);
                foreach (DataRow dr in dt.Rows)
                {
                    switch (dr["OperateCode"].ToString())
                    {
                        case "btnAdd":
                            _ROLH.btnAdd = true;
                            break;
                        case "btnEdit":
                            _ROLH.btnEdit = true;
                            break;
                        case "btnDel":
                            _ROLH.btnDel = true;
                            break;
                        case "btnSetMenuRole":
                            _ROLH.btnSetMenuRole = true;
                            break; ;
                        case "btnSetOperateRole":
                            _ROLH.btnSetOperateRole = true;
                            break;
                    }
                }
                return _ROLH;
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// 基础信息管理
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public User1001HTML RULE_User1001HTML(int RoleID)
        {
            try
            {
                List<User1001HTML> ListEntity = new List<User1001HTML>();
                User1001HTML OperaHTML = new User1001HTML();
                DataTable dt = GetRolePower(RoleID, 1001);
                foreach (DataRow dr in dt.Rows)
                {
                    switch (dr["OperateCode"].ToString())
                    {
                        case "btnAdd":
                            OperaHTML.btnAdd = true;
                            break;
                        case "btnEdit":
                            OperaHTML.btnEdit = true;
                            break;
                        case "btnDel":
                            OperaHTML.btnDel = true;
                            break;
                    }
                }
                //ListEntity.Add(_RULH);
                return OperaHTML;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 工作轨迹管理
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public User1002HTML RULE_User1002HTML(int RoleID)
        {
            try
            {
                List<User1002HTML> ListEntity = new List<User1002HTML>();
                User1002HTML OperaHTML = new User1002HTML();
                DataTable dt = GetRolePower(RoleID, 1002);
                foreach (DataRow dr in dt.Rows)
                {
                    switch (dr["OperateCode"].ToString())
                    {
                        case "btnAdd":
                            OperaHTML.btnAdd = true;
                            break;
                        case "btnEdit":
                            OperaHTML.btnEdit = true;
                            break;
                        case "btnDel":
                            OperaHTML.btnDel = true;
                            break;
                    }
                }
                //ListEntity.Add(_RULH);
                return OperaHTML;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 工作时长
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public User1003HTML RULE_User1003HTML(int RoleID)
        {
            try
            {
                List<User1003HTML> ListEntity = new List<User1003HTML>();
                User1003HTML OperaHTML = new User1003HTML();
                DataTable dt = GetRolePower(RoleID, 1003);
                foreach (DataRow dr in dt.Rows)
                {
                    switch (dr["OperateCode"].ToString())
                    {
                        case "btnAdd":
                            OperaHTML.btnAdd = true;
                            break;
                        case "btnEdit":
                            OperaHTML.btnEdit = true;
                            break;
                        case "btnDel":
                            OperaHTML.btnDel = true;
                            break;
                    }
                }
                //ListEntity.Add(_RULH);
                return OperaHTML;
            }
            catch (Exception)
            {

                throw;
            }
        }

 

        /// <summary>
        /// 设备管理
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public Device2002HTML RULE_Device2002HTML(int RoleID)
        {
            try
            {
                List<Device2002HTML> ListEntity = new List<Device2002HTML>();
                Device2002HTML OperaHTML = new Device2002HTML();
                DataTable dt = GetRolePower(RoleID, 2002);
                foreach (DataRow dr in dt.Rows)
                {
                    switch (dr["OperateCode"].ToString())
                    {
                        case "btnAdd":
                            OperaHTML.btnAdd = true;
                            break;
                        case "btnEdit":
                            OperaHTML.btnEdit = true;
                            break;
                        case "btnDel":
                            OperaHTML.btnDel = true;
                            break;
                    }
                }
                //ListEntity.Add(_RULH);
                return OperaHTML;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 物资管理
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public Device2003HTML RULE_Device2003HTML(int RoleID)
        {
            try
            {
                List<Device2003HTML> ListEntity = new List<Device2003HTML>();
                Device2003HTML OperaHTML = new Device2003HTML();
                DataTable dt = GetRolePower(RoleID, 2003);
                foreach (DataRow dr in dt.Rows)
                {
                    switch (dr["OperateCode"].ToString())
                    {
                        case "btnAdd":
                            OperaHTML.btnAdd = true;
                            break;
                        case "btnEdit":
                            OperaHTML.btnEdit = true;
                            break;
                        case "btnDel":
                            OperaHTML.btnDel = true;
                            break;
                    }
                }
                //ListEntity.Add(_RULH);
                return OperaHTML;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        ///巡线计划管理
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public Patrol2002HTML RULE_Patrol2002HTML(int RoleID)
        {
            try
            {
                List<Patrol2002HTML> ListEntity = new List<Patrol2002HTML>();
                Patrol2002HTML OperaHTML = new Patrol2002HTML();
                DataTable dt = GetRolePower(RoleID, 2002);
                foreach (DataRow dr in dt.Rows)
                {
                    switch (dr["OperateCode"].ToString())
                    {
                        case "btnAdd":
                            OperaHTML.btnAdd = true;
                            break;
                        case "btnDel":
                            OperaHTML.btnDel = true;
                            break;
                    }
                }
                //ListEntity.Add(_RULH);
                return OperaHTML;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 文档管理
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public File2005HTML RULE_File2005HTML(int RoleID)
        {
            try
            {
                List<File2005HTML> ListEntity = new List<File2005HTML>();
                File2005HTML OperaHTML = new File2005HTML();
                DataTable dt = GetRolePower(RoleID, 2005);
                foreach (DataRow dr in dt.Rows)
                {
                    switch (dr["OperateCode"].ToString())
                    {
                        case "btnAdd":
                            OperaHTML.btnAdd = true;
                            break;
                        case "btnEdit":
                            OperaHTML.btnEdit = true;
                            break;
                        case "btnDel":
                            OperaHTML.btnDel = true;
                            break;
                    }
                }
                //ListEntity.Add(_RULH);
                return OperaHTML;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 水质
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public Dispatch3001HTML RULE_Dispatch3001HTML(int RoleID)
        {
            try
            {
                List<Dispatch3001HTML> ListEntity = new List<Dispatch3001HTML>();
                Dispatch3001HTML OperaHTML = new Dispatch3001HTML();
                DataTable dt = GetRolePower(RoleID, 3001);
                foreach (DataRow dr in dt.Rows)
                {
                    switch (dr["OperateCode"].ToString())
                    {
                        case "btnAdd":
                            OperaHTML.btnAdd = true;
                            break;
                        case "btnEdit":
                            OperaHTML.btnEdit = true;
                            break;
                        case "btnDel":
                            OperaHTML.btnDel = true;
                            break;
                    }
                }
                //ListEntity.Add(_RULH);
                return OperaHTML;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 调度
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public Dispatch3002HTML RULE_Dispatch3002HTML(int RoleID)
        {
            try
            {
                List<Dispatch3002HTML> ListEntity = new List<Dispatch3002HTML>();
                Dispatch3002HTML OperaHTML = new Dispatch3002HTML();
                DataTable dt = GetRolePower(RoleID, 3002);
                foreach (DataRow dr in dt.Rows)
                {
                    switch (dr["OperateCode"].ToString())
                    {
                        case "btnAdd":
                            OperaHTML.btnAdd = true;
                            break;
                        case "btnEdit":
                            OperaHTML.btnEdit = true;
                            break;
                        case "btnDel":
                            OperaHTML.btnDel = true;
                            break;
                    }
                }
                //ListEntity.Add(_RULH);
                return OperaHTML;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 预警指示
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public Dispatch3003HTML RULE_Dispatch3003HTML(int RoleID)
        {
            try
            {
                List<Dispatch3003HTML> ListEntity = new List<Dispatch3003HTML>();
                Dispatch3003HTML OperaHTML = new Dispatch3003HTML();
                DataTable dt = GetRolePower(RoleID, 3003);
                foreach (DataRow dr in dt.Rows)
                {
                    switch (dr["OperateCode"].ToString())
                    {
                        case "btnAdd":
                            OperaHTML.btnAdd = true;
                            break;
                        case "btnEdit":
                            OperaHTML.btnEdit = true;
                            break;
                        case "btnDel":
                            OperaHTML.btnDel = true;
                            break;
                    }
                }
                //ListEntity.Add(_RULH);
                return OperaHTML;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 任务下发
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public Task4001HTML RULE_Task4001HTML(int RoleID)
        {
            try
            {
                List<Task4001HTML> ListEntity = new List<Task4001HTML>();
                Task4001HTML OperaHTML = new Task4001HTML();
                DataTable dt = GetRolePower(RoleID, 4001);
                foreach (DataRow dr in dt.Rows)
                {
                    switch (dr["OperateCode"].ToString())
                    {
                        case "btnAdd":
                            OperaHTML.btnAdd = true;
                            break;
                        case "btnEdit":
                            OperaHTML.btnEdit = true;
                            break;
                        case "btnDel":
                            OperaHTML.btnDel = true;
                            break;
                    }
                }
                //ListEntity.Add(_RULH);
                return OperaHTML;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 工单下发
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public Task4002HTML RULE_Task4002HTML(int RoleID)
        {
            try
            {
                List<Task4002HTML> ListEntity = new List<Task4002HTML>();
                Task4002HTML OperaHTML = new Task4002HTML();
                DataTable dt = GetRolePower(RoleID, 4002);
                foreach (DataRow dr in dt.Rows)
                {
                    switch (dr["OperateCode"].ToString())
                    {
                        case "btnAdd":
                            OperaHTML.btnAdd = true;
                            break;
                        case "btnEND":
                            OperaHTML.btnEND = true;
                            break;
                        case "btnDel":
                            OperaHTML.btnDel = true;
                            break;
                        case "btnEdit":
                            OperaHTML.btnEdit = true;
                            break;
                    }
                }
                //ListEntity.Add(_RULH);
                return OperaHTML;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 档案资料管理
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public File6000HTML RULE_File6000HTML(int RoleID)
        {
            try
            {
                List<File6000HTML> ListEntity = new List<File6000HTML>();
                File6000HTML OperaHTML = new File6000HTML();
                DataTable dt = GetRolePower(RoleID, 6000);
                foreach (DataRow dr in dt.Rows)
                {
                    switch (dr["OperateCode"].ToString())
                    {
                        case "btnAdd":
                            OperaHTML.btnAdd = true;
                            break;
                        case "btnEdit":
                            OperaHTML.btnEdit = true;
                            break;
                        case "btnDel":
                            OperaHTML.btnDel = true;
                            break;
                    }
                }
                //ListEntity.Add(_RULH);
                return OperaHTML;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 项目列表
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public Project7001HTML RULE_Project7001HTML(int RoleID)
        {
            try
            {
                List<Project7001HTML> ListEntity = new List<Project7001HTML>();
                Project7001HTML OperaHTML = new Project7001HTML();
                DataTable dt = GetRolePower(RoleID, 7001);
                foreach (DataRow dr in dt.Rows)
                {
                    switch (dr["OperateCode"].ToString())
                    {
                        case "btnAdd":
                            OperaHTML.btnAdd = true;
                            break;
                        case "btnEdit":
                            OperaHTML.btnEdit = true;
                            break;
                        case "btnDel":
                            OperaHTML.btnDel = true;
                            break;
                        case "btnProjectUser":
                            OperaHTML.btnProjectUser = true;
                            break;
                    }
                }
                //ListEntity.Add(_RULH);
                return OperaHTML;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 项目计划
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public Project7002HTML RULE_Project7002HTML(int RoleID)
        {
            try
            {
                List<Project7002HTML> ListEntity = new List<Project7002HTML>();
                Project7002HTML OperaHTML = new Project7002HTML();
                DataTable dt = GetRolePower(RoleID, 7002);
                foreach (DataRow dr in dt.Rows)
                {
                    switch (dr["OperateCode"].ToString())
                    {
                        case "btnAdd":
                            OperaHTML.btnAdd = true;
                            break;
                        case "btnEdit":
                            OperaHTML.btnEdit = true;
                            break;
                        case "btnDel":
                            OperaHTML.btnDel = true;
                            break;
                    }
                }
                //ListEntity.Add(_RULH);
                return OperaHTML;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 计划类别管理
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public Project7003HTML RULE_Project7003HTML(int RoleID)
        {
            try
            {
                List<Project7003HTML> ListEntity = new List<Project7003HTML>();
                Project7003HTML OperaHTML = new Project7003HTML();
                DataTable dt = GetRolePower(RoleID, 7003);
                foreach (DataRow dr in dt.Rows)
                {
                    switch (dr["OperateCode"].ToString())
                    {
                        case "btnAdd":
                            OperaHTML.btnAdd = true;
                            break;
                        case "btnEdit":
                            OperaHTML.btnEdit = true;
                            break;
                        case "btnDel":
                            OperaHTML.btnDel = true;
                            break;
                    }
                }
                //ListEntity.Add(_RULH);
                return OperaHTML;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 巡线计划
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public XX9001HTML RULE_XX9001HTML(int RoleID)
        {
            try
            {
                List<XX9001HTML> ListEntity = new List<XX9001HTML>();
                XX9001HTML OperaHTML = new XX9001HTML();
                DataTable dt = GetRolePower(RoleID, 9001);
                foreach (DataRow dr in dt.Rows)
                {
                    switch (dr["OperateCode"].ToString())
                    {
                        case "btnAdd":
                            OperaHTML.btnAdd = true;
                            break;
                        case "btnEdit":
                            OperaHTML.btnEdit = true;
                            break;
                        case "btnDel":
                            OperaHTML.btnDel = true;
                            break;
                    }
                }
                //ListEntity.Add(_RULH);
                return OperaHTML;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 巡查报告
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public XX9002HTML RULE_XX9002HTML(int RoleID)
        {
            try
            {
                List<XX9002HTML> ListEntity = new List<XX9002HTML>();
                XX9002HTML OperaHTML = new XX9002HTML();
                DataTable dt = GetRolePower(RoleID, 9002);
                foreach (DataRow dr in dt.Rows)
                {
                    switch (dr["OperateCode"].ToString())
                    {
                        case "btnAdd":
                            OperaHTML.btnAdd = true;
                            break;
                        case "btnEdit":
                            OperaHTML.btnEdit = true;
                            break;
                        case "btnDel":
                            OperaHTML.btnDel = true;
                            break;
                    }
                }
                //ListEntity.Add(_RULH);
                return OperaHTML;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 巡线轨迹
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public XX9003HTML RULE_XX9003HTML(int RoleID)
        {
            try
            {
                List<XX9003HTML> ListEntity = new List<XX9003HTML>();
                XX9003HTML OperaHTML = new XX9003HTML();
                DataTable dt = GetRolePower(RoleID, 9003);
                foreach (DataRow dr in dt.Rows)
                {
                    switch (dr["OperateCode"].ToString())
                    {
                        case "btnAdd":
                            OperaHTML.btnAdd = true;
                            break;
                        case "btnEdit":
                            OperaHTML.btnEdit = true;
                            break;
                        case "btnDel":
                            OperaHTML.btnDel = true;
                            break;
                    }
                }
                //ListEntity.Add(_RULH);
                return OperaHTML;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 获取角色的权限
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public DataTable GetRolePower(int RoleID, int MenuID)
        {
            try
            {
                string tmpSql = $@"select sop.* from sys_operate sop
                                    INNER JOIN role_operate rop on sop.id = rop.operateid
                                    where rop.roleid = {RoleID} and MenuID={MenuID};";
                DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}