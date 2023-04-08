using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
namespace SZRiverSys.SQL
{
    public class BusinessSql
    {
        #region 公共变量
        public string tmpsql;
        #endregion
        #region 录入工单数据
        public bool InsertCommonlanguage(Model.Commonlanguage m)
        {
            tmpsql =string.Format( "insert into RM_Commonlanguage(Content,type,createtime) values('{0}','{1}',GETDATE());",m.Content,m.type);
            int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool InsertPlaceInfo(Model.PlaceInfo m)
        {
            tmpsql = string.Format("insert into RM_PlaceInfo(PlaceName,Pid,Remark) values('{0}','{1}','{2}');", m.PlaceName, m.Pid,m.Remark);
            int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public bool delCommonlanguage(string id)
        {
            tmpsql = "delete RM_Commonlanguage where ID in (" + id + ")";
            int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool delPlaceInfo(string id)
        {
            tmpsql = "delete RM_PlaceInfo where ID in (" + id + ")";
            int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public DataTable GetCommonlanguage(string search)
        {
            tmpsql = string.Format("select * from RM_Commonlanguage where Content like '%{0}%'", search);
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        public DataTable GetPlaceInfo(string search,string Pid)
        {
            tmpsql = string.Format("select * from RM_PlaceInfo where PlaceName like '%{0}%' and Pid='{1}'", search,Pid);
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }

        public DataTable GetLanguageByPage(int pagesize, int pageindex,string search, out int total)
        {
            string where = "1=1";
            DataSet ds = new DataSet();
            ds = DBHelper.DBHelperMsSql.GetByPage(@"RM_Commonlanguage",//查询的表
                                                    "*",//显示的字段
                                                    "ID",
                                                    pagesize,
                                                    pageindex,
                                                    where,
                                                    "ID",
                                                    out total);
            return ds.Tables[0];
        }
        public DataTable GetPlaceByPage(int pagesize, int pageindex, string search, out int total)
        {
            string where = "1=1";
            DataSet ds = new DataSet();
            ds = DBHelper.DBHelperMsSql.GetByPage(@"RM_PlaceInfo",//查询的表
                                                    "*",//显示的字段
                                                    "ID",
                                                    pagesize,
                                                    pageindex,
                                                    where,
                                                    "ID",
                                                    out total);
            return ds.Tables[0];
        }
        #endregion
        /// <summary>
        /// 创建业务
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool AddBusiness(Model.BusinessModel m)
        {
            tmpsql = string.Format("insert into Business_flow(Business_Type,Creater,Starttime,Endtime,Assignor,Range,CreateTime,state) values('{0}','{1}','{2}','{3}','{4}','{5}',getdate(),'{6}');", m.Business_Type,m.Creater,m.Starttime,m.Endtime,m.Assignor,m.Range,m.state);
            int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// 获取业务类型
        /// </summary>
        /// <returns></returns>
        public DataTable GetBusinessType()
        {
            tmpsql = "select * from Business_Type WHERE Type='1'";
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 获取工单类型
        /// </summary>
        /// <returns></returns>
        public DataTable GetWorkFormType()
        {
            tmpsql = "select * from Business_Type WHERE Type='2'";
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 获取紧急程度下拉框
        /// </summary>
        /// <returns></returns>
        public DataTable GetEmergencyLevel()
        {
            tmpsql = "select * from Business_Type WHERE Type='3'";
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 获取我参与的业务
        /// </summary>
        /// <returns></returns>
        public DataTable GetUserBusiness(string username,string type)
        {
            string time= DateTime.Now.ToLocalTime().ToString();
            string where = " 1=1";
            if (type != "")
            {
                where += " and Business_Type='"+type+"'";
            }
            tmpsql = string.Format("select * from Business_flow where  CHARINDEX('{0}',Assignor)>0 and '{1}' between Starttime and Endtime and {2}",username,time,where);
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 根据权限获取业务列表
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public DataTable GetBusinessList(Model.UserModel m,int pagesize,int pageindex,out int total)
        {
            string where = "1=1";
            if (m.RoleID == 1)
            {
          //      tmpsql = string.Format("select * from Business_flow");
            }
            else
            {
                where += " and  CHARINDEX('"+m.usercode+"',Assignor)>0";
                //string time = DateTime.Now.ToLocalTime().ToString();
                //tmpsql = string.Format("select * from Business_flow where  CHARINDEX('{0}',Assignor)>0 }", m.UserID);
            }
            DataSet ds = new DataSet();
            ds = DBHelper.DBHelperMsSql.GetByPage(@"Business_flow",//查询的表
                                                    "*",//显示的字段
                                                    "ID",
                                                    pagesize,
                                                    pageindex,
                                                    where,
                                                    "ID",
                                                    out total);
            return ds.Tables[0];
        }
        /// <summary>
        /// 完成提交业务资料
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool FinishBusiness(Model.BusinessModel m)
        {
            DataTable dt = GetBusinessById(m.ID);
            string finishtime = dt.Rows[0]["FinishTime"].ToString();
            if (finishtime == null || finishtime == "")
            {
                tmpsql = string.Format("update Business_flow set FinishTime='{0}',FinishExplain='{1}',Enclosure='{2}' where ID='{3}' ",m.FinishTime,m.FinishExplain,m.Enclosure,m.ID);
            }
            else
            {
                tmpsql= string.Format("update Business_flow set Enclosure=Enclosure+'{0}' where ID='{1}'",m.Enclosure,m.ID);
            }
            int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 根据ID获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetBusinessById(string id)
        {
            tmpsql = "select * from Business_flow where ID=" + id + "";
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }        
        /// <summary>
        /// 对业务进行评价
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool evaluate(Model.BusinessModel m)
        {
            tmpsql = string.Format("update [dbo].[Business_flow] set Score='{0}',Comment='{1}' where ID='{2}'",m.Score,m.Comment,m.ID);
            int result = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 获取指定时间内的业务
        /// </summary>
        /// <param name="startime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public DataTable GetBusinessByTime(string startime,string endtime)
        {
            tmpsql = string.Format("select * from Business_flow where CreateTime between '{0}' and '{1}'", startime,endtime);
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        public DataTable GetWorkFormByTime(string startime, string endtime,Model.UserModel User)
        {
            string where = "";
            if (User.RoleID >5)
            {
                where += " and  CHARINDEX('" + User.usercode + "',Assignor)>0 ";
            }
            tmpsql = string.Format("select * from Business_WorkForm where CreateTime between '{0}' and '{1}' {2}", startime, endtime,where);
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 时间内未完成业务
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public DataTable GetUserBusinessNoFinish(string username)
        {
            string time = DateTime.Now.ToLocalTime().ToString();
            tmpsql = string.Format("select * from Business_flow where  CHARINDEX('{0}',Assignor)>0 and '{1}' between Starttime and Endtime and (FinishTime=''or FinishTime is null)", username, time);
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        public DataTable GetUserBusiness(string username)
        {
            tmpsql = string.Format("select * from Business_flow where  CHARINDEX('{0}',Assignor)>0 ", username);
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 时间内未完成的工单
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public DataTable GetUserWorkFormNoFinish(string username)
        {
            string time = DateTime.Now.ToLocalTime().ToString();
            tmpsql = string.Format("select * from Business_WorkForm where  CHARINDEX('{0}',Assignor)>0 and '{1}'<PlanFinishTime and (FinishTime=''or FinishTime is null)", username, time);
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 获取用户自身的工单
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public DataTable GetUserWorkFormList(string username)
        {
            tmpsql = string.Format("select * from Business_WorkForm where  CHARINDEX('{0}',Assignor)>0 ", username);
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 时间内未完成的计划
        /// </summary>
        /// <returns></returns>
        public DataTable UserNotFinishPlan(Model.UserModel m)
        {
            string time = DateTime.Now.ToLocalTime().ToString();
            tmpsql = string.Format("select * from [dbo].[RM_UserPatrolPlan] where UserId='{0}'",m.UserID,time);
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 创建个人管养计划
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public bool CreateUserPlan(Model.PatrolPlanModel m)
        {
            tmpsql = string.Format("insert into RM_UserPatrolPlan(UserId,StartTime,EndTime,[Content],ManagementRange,state) values('{0}','{1}','{2}','{3}','{4}','{5}')",m.UserId,m.StartTime,m.EndTime,m.Content,m.ManagementRange,m.state);
            int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 获取我发布的工单
        /// </summary>
        /// <returns></returns>
        public DataTable GetMyCreateWorkFomr(string usercode)
        {
            tmpsql = string.Format("select * from [dbo].[Business_WorkForm] where  Creater='{0}' ", usercode);
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 获取用户计划列表
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public DataTable GetUserPlanList(Model.UserModel m, int pagesize, int pageindex,string search, out int total)
        {
            string where = "1=1";
            where += " and  UserId='" + m.UserID + "'";
            if (search!="")
            {
                where += " and Content like '%"+search+"%'";
            }
            DataSet ds = new DataSet();
            ds = DBHelper.DBHelperMsSql.GetByPage(@"RM_UserPatrolPlan",//查询的表
                                                    "*",//显示的字段
                                                    "ID",
                                                    pagesize,
                                                    pageindex,
                                                    where,
                                                    "ID",
                                                    out total);
            return ds.Tables[0];
        }
        /// <summary>
        /// 工单提示消息
        /// </summary>
        /// <returns></returns>
        public DataTable GetWorkFormMsg(Model.UserModel m)
        {
            tmpsql = string.Format("select * from [dbo].[Business_WorkForm] where  CHARINDEX('{0}',Assignor)>0 and islook !='1'", m.usercode);
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 获取工单执行记录
        /// </summary>
        /// <param name="formid"></param>
        /// <returns></returns>
        public DataTable GetWorkFormTrack(string formid)
        {
            tmpsql = string.Format(@"select a.*,b.userName from [dbo].[RM_WorkFormDetail] a
left join Sys_Accounts_Users b on a.userid = b.UserID where FormID = '{0}'",formid);
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 添加工单执行过程
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool ADDWorkFormTrack(Model.WorkFormTrack m)
        {
            tmpsql = "update [dbo].[Business_WorkForm] set state='进行中' where ID='" + m.FormId+ "'";
            DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
            tmpsql = string.Format("insert into [dbo].[RM_WorkFormDetail](FormID,userid,[Content],CreateTime,Photo) values('{0}','{1}','{2}',getdate(),'{3}')", m.FormId,m.userid,m.Content,m.photo);
            int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 接受工单
        /// </summary>
        /// <returns></returns>
        public bool acceptwork(string workid,string usercode)
        {
            tmpsql = string.Format("update [dbo].[Business_WorkForm] set Assignor='{0}',state='进行中' where ID='{1}'",usercode,workid);
            int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 设置工单为已读
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool WorkFormAleadread(string id)
        {
            tmpsql = string.Format("update [dbo].[Business_WorkForm] set islook='1' where ID='{0}'",id);
            int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
       
       
        /// <summary>
        /// 时间内完成
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public DataTable GetUserBusinessFinish(string username)
        {
            string time = DateTime.Now.ToLocalTime().ToString();
            tmpsql = string.Format("select * from Business_flow where  CHARINDEX('{0}',Assignor)>0 and FinishTime!=''", username);
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 预期未完成
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public DataTable GetUserBusinessPassTime(string username)
        {
            string time = DateTime.Now.ToLocalTime().ToString();
            tmpsql = string.Format("select * from Business_flow where  CHARINDEX('{0}',Assignor)>0 and '{1}' > Endtime and (FinishTime=''or FinishTime is null) ", username, time);
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        #region  工单下发的操作方法
        /// <summary>
        /// 创建工单
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool ADDWorkForm(Model.Business_WorkForm m)
        {
            tmpsql = string.Format(@"insert into Business_WorkForm(WorkName,Type,[Content],Assignor,Emergencylevel,PlanFinishWorkHouse,PlanFinishTime,WorkNeed,CreateTime,Creater,state,projectid,islook,Enclosure,Range,Workload,RangeUnit,lng,lat) 
                                     values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',getdate(),'{8}','{9}','{10}','{11}','','{12}','{13}','{14}',{15},{16})", m.WorkName,m.Type,m.Content,m.Assignor,m.Emergencylevel,m.PlanFinishWorkHouse,m.PlanFinishTime,m.WorkNeed,m.Creater,m.state,m.projectid,m.islook,m.Range,m.Workload,m.RangeUnit,m.Longitude,m.Latitude );
            int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 根据用户权限获取工单
        /// </summary>
        /// <returns></returns>
        public DataTable GetWorkForm(Model.UserModel m,int pagesize,int pageindex,out int total)
        {
            string where = "1=1";
            if (m.RoleID != 1)
            {
                where += " and  CHARINDEX('" + m.usercode + "',Assignor)>0 ";
            }
            DataSet ds = new DataSet();
            ds = DBHelper.DBHelperMsSql.GetByPage(@"Business_WorkForm",//查询的表
                                                    "*",//显示的字段
                                                    "ID",
                                                    pagesize,
                                                    pageindex,
                                                    where,
                                                    "CreateTime desc",
                                                    out total);
            return ds.Tables[0];
        }
        /// <summary>
        /// 删除工单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool delworkform(string id)
        {
            tmpsql = "delete from [dbo].[Business_WorkForm] where ID in ("+id+")";
            int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
            if (count > 0)
            {
                tmpsql = "delete from [dbo].[RM_WorkFormDetail] where FormID in ("+id+")";
                DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取单个工单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetWorkFormById(string id)
        {
            tmpsql = string.Format("select * from Business_WorkForm where ID='{0}'",id);
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        public bool EditWorkForm(Model.Business_WorkForm m)
        {
            tmpsql = string.Format(@"update Business_WorkForm set WorkName='{0}',Type='{1}',[Content]='{2}',Assignor='{3}',Emergencylevel='{4}',PlanFinishWorkHouse='{5}',PlanFinishTime='{6}',
                                    WorkNeed='{7}',Workload='{8}',RangeUnit='{10}' where ID='{9}'", m.WorkName,m.Type,m.Content,m.Assignor,m.Emergencylevel,m.PlanFinishWorkHouse,m.PlanFinishTime,m.WorkNeed,m.Workload,m.ID,m.RangeUnit);
            int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// 添加完成工单信息
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool FinishWorkForm(Model.Business_WorkForm m)
        {         
            tmpsql = string.Format("update Business_WorkForm set FinishTime='{0}',FinishExplain='{1}',state='已完成' where ID='{2}' ", m.FinishTime, m.FinishExplain, m.ID);
            int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 上传文件时将文件信息保存到数据库
        /// </summary>
        /// <returns></returns>
        public bool AddWorkFormFileData(string ID,string filename)
        {
            tmpsql =string.Format("update [dbo].[Business_WorkForm] set Enclosure=Enclosure+'{0}' where ID='{1}' ",filename,ID);
            int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 逾期未完成的工单
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public DataTable GetUserWorkFormNoFinishPassTime(string username)
        {
            string time = DateTime.Now.ToLocalTime().ToString();
            tmpsql = string.Format("select * from Business_WorkForm where  CHARINDEX('{0}',Assignor)>0 and '{1}'>PlanFinishTime and (FinishTime=''or FinishTime is null)", username, time);
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 时间内完成
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public DataTable GetWorkFormFinish(string username)
        {
            string time = DateTime.Now.ToLocalTime().ToString();
            tmpsql = string.Format("select * from Business_WorkForm where  CHARINDEX('{0}',Assignor)>0 and FinishTime!=''", username);
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        #endregion
    }
}