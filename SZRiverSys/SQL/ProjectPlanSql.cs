using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using SZRiverSys.Model;

namespace SZRiverSys.SQL
{
    /// <summary>
    /// 项目计划
    /// </summary>
    public class ProjectPlanSql
    {
        #region 公共变量
        DataTable dt = new DataTable();
        string tmpsql = "";
        #endregion

        #region 查询操作
        /// <summary>
        /// 获取项目计划信息
        /// </summary>
        /// <param name="Search">筛选条件</param>
        /// <param name="ProjectID">项目ID</param>
        /// <returns></returns>
        public DataTable GetProjectPlanInfo(string Search, string ProjectID)
        {
            try
            {
                tmpsql = $@"select rm_plan.ID,rm_plan.ProjectID,rm_plan.ProjectPlanName,rm_plan.PlanClassID,rm_plan.Frequencyt_Type,rm_plan.Frequencyt_Value,rm_plan.[Count],rm_plan.CreateDate,rm_plan.CreateUser,rm_plan.BeginDate,rm_plan.EndDate,rm_plan.PlanStatus,case rm_plan.PlanStatus when 0 then '已完成' when 1 then '未完成' when 2 then '进行中' when 3 then '逾期' else '未知' end as PlanStatusName,rm_pj.ProJectName,rm_planclass.PlanClassName from RM_ProJectPlan rm_plan
                            inner join RM_ProjectInfo rm_pj on rm_pj.ID=rm_plan.ProjectID
                            inner join RM_ProJectPlanClass rm_planclass on rm_planclass.ID=rm_plan.PlanClassID where rm_plan.ProjectID = '{ProjectID}' ";
                if (Search != "")
                {
                    tmpsql += $" and rm_plan.ProjectPlanName like '%{Search}%'";
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
        /// 获取某一个项目计划信息
        /// </summary>
        /// <param name="ID">当前选中行ID</param>
        /// <returns></returns>
        public DataTable GetProjectInfoPlanByID(int ID)
        {
            try
            {
                tmpsql = $"select * from RM_ProJectPlan where ID='{ID}'";
                dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region 新增操作
        /// <summary>
        /// 新增项目计划
        /// </summary>
        /// <param name="projectModel">项目实体</param>
        /// <returns></returns>
        public bool AddProjectPlanInfo(ProjectPlanModel projectPlanModel)
        {
            try
            {
                tmpsql = $@"INSERT INTO [dbo].[RM_ProJectPlan]
                           ([ProjectID]
                           ,[ProjectPlanName]
                           ,[PlanClassID]
                           ,[Frequencyt_Type]
                           ,[Frequencyt_Value]
                           ,[Count]
                           ,[CreateDate]
                           ,[CreateUser]
                           ,[BeginDate]
                           ,[EndDate]
                           ,[PlanStatus])
                     VALUES
                           ('{projectPlanModel.ProjectID}'
                           ,'{projectPlanModel.ProjectPlanName}'
                           ,'{projectPlanModel.PlanClassID}'
                           ,'{projectPlanModel.Frequencyt_Type}'
                           ,'{projectPlanModel.Frequencyt_Value}'
                           ,'{projectPlanModel.Count}'
                           ,'{projectPlanModel.CreateDate}'
                           ,'{projectPlanModel.CreateUser}'
                           ,'{projectPlanModel.BeginDate}'
                           ,'{projectPlanModel.EndDate}'
                           ,'{projectPlanModel.PlanStatus}')";
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
        /// 修改项目计划
        /// </summary>
        /// <param name="projectModel">项目实体</param>
        /// <returns></returns>
        public bool EditProjectPlanInfo(ProjectPlanModel projectPlanModel)
        {
            try
            {
                tmpsql = $@"UPDATE [dbo].[RM_ProJectPlan]
                           SET [ProjectID] = '{projectPlanModel.ProjectID}'
                              ,[ProjectPlanName] = '{projectPlanModel.ProjectPlanName}'
                              ,[PlanClassID] = '{projectPlanModel.PlanClassID}'
                              ,[Frequencyt_Type] = '{projectPlanModel.Frequencyt_Type}'
                              ,[Frequencyt_Value] = '{projectPlanModel.Frequencyt_Value}'
                              ,[Count] ='{projectPlanModel.Count}'
                              ,[ModifyDate] ='{projectPlanModel.ModifyDate}'
                              ,[ModifyUser] = '{projectPlanModel.ModifyUser}'
                              ,[BeginDate] = '{projectPlanModel.BeginDate}'
                              ,[EndDate] = '{projectPlanModel.EndDate}'
                              ,[PlanStatus] = '{projectPlanModel.PlanStatus}'
                         WHERE [ID]='{projectPlanModel.ID}'";
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
        /// 删除项目计划
        /// </summary>
        /// <param name="ArrayID">要删除的选中行ID字符串，以逗号分隔</param>
        /// <returns></returns>
        public bool DelProjectPlanInfo(string ArrayID)
        {
            try
            {
                tmpsql = $"DELETE [dbo].[RM_ProJectPlan] WHERE ID in('{ArrayID.Replace(",", "','")}')";
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

        #region 打卡操作
        /// <summary>
        /// 添加打卡记录
        /// </summary>
        /// <param name="proJectPlanPunchCardModel"></param>
        /// <returns></returns>
        public bool AddPunchCardInfo(ProJectPlanPunchCardModel proJectPlanPunchCardModel)
        {
            try
            {
                tmpsql = $@"INSERT INTO [dbo].[RM_ProJectPlanPunchCard]
                                   ([ProJectPlanID]
                                   ,[PunchCardPerson]
                                   ,[TodayPlanStatus]
                                   ,[ProblemContent]
                                   ,[ApprovalStatus]
                                   ,[ApprovalUser]
                                   ,[ApprovalCreate]
                                   ,[PunchCardDate]
                                   ,[GUID])
                             VALUES
                                   ('{proJectPlanPunchCardModel.ProJectPlanID}'
                                   ,'{proJectPlanPunchCardModel.PunchCardPerson}'
                                   ,'{proJectPlanPunchCardModel.TodayPlanStatus}'
                                   ,'{proJectPlanPunchCardModel.ProblemContent}'
                                   ,'{proJectPlanPunchCardModel.ApprovalStatus}'
                                   ,'{proJectPlanPunchCardModel.ApprovalUser}'
                                   ,'{proJectPlanPunchCardModel.ApprovalCreate}'
                                   ,'{proJectPlanPunchCardModel.PunchCardDate}'
                                   ,'{proJectPlanPunchCardModel.GUID}')";
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

        /// <summary>
        /// 判断今天是否打卡
        /// </summary>
        /// <param name="GUID"></param>
        /// <returns></returns>
        public bool TodayIsPunchCard(string ProJectPlanID)
        {
            try
            {
                tmpsql = "select max(PunchCardDate) as PunchCardDate from [dbo].[RM_ProJectPlanPunchCard] where ProJectPlanID='" + ProJectPlanID + "'";
                string dtNow = DateTime.Now.ToString("yyyy-MM-dd");
                dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                if (dt.Rows.Count > 0)
                {
                    string bjDt = DateTime.Parse(dt.Rows[0]["PunchCardDate"].ToString()).ToString("yyyy-MM-dd");
                    DateTime t1 = Convert.ToDateTime(dtNow);
                    DateTime t2 = Convert.ToDateTime(bjDt);
                    if (DateTime.Compare(t1, t2) == 0)
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// 根据GUID验证打卡记录是否存在
        /// </summary>
        /// <param name="GUID"></param>
        /// <returns></returns>
        public bool ValidateExistGUID(string GUID)
        {
            try
            {
                tmpsql = "select * from [dbo].[RM_ProJectPlanPunchCard] where GUID='" + GUID + "'";
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

        /// <summary>
        /// 添加一条打卡记录的附件
        /// </summary>
        /// <param name="PunchCardFileModel">打卡记录附件实体类</param>
        /// <returns></returns>
        public bool AddRM_PunchCardFile(PunchCardFileModel punchCardFileModel)
        {
            try
            {
                tmpsql = $@"INSERT INTO [dbo].[RM_PunchCardFile]
                           ([MsgGUID]
                           ,[FilePath]
                           ,[FileName]
                           ,[FileType]
                           ,[CreateDate]
                           ,[Remark])
                     VALUES
                           ('{punchCardFileModel.MsgGUID}'
                           ,'{punchCardFileModel.FilePath}'
                           ,'{punchCardFileModel.FileName}'
                           ,'{punchCardFileModel.FileType}'
                           ,'{punchCardFileModel.CreateDate}'
                           ,'{punchCardFileModel.Remark}')";
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

        /// <summary>
        /// 获取某个计划的打卡详情记录
        /// </summary>
        /// <param name="ProJectPlanID">计划ID</param>
        /// <returns></returns>
        public DataTable GetPunchCardInfo(string ProJectPlanID)
        {
            try
            {
                tmpsql = $@"select rm_ppc.*,rm_plan.ProjectPlanName from [dbo].[RM_ProJectPlanPunchCard] rm_ppc
                            inner join RM_ProJectPlan rm_plan on rm_ppc.ProJectPlanID=rm_plan.ID
                            where rm_ppc.ProJectPlanID='{ProJectPlanID}'";
                dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 更新项目计划状态位进行中
        /// </summary>
        /// <param name="ProjectPlanID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool UpdatePlanStatus(int PlanStatus, string ID)
        {
            try
            {
                tmpsql = $@"UPDATE [dbo].[RM_ProJectPlan] SET PlanStatus ='{PlanStatus}' where ID='{ID}'";
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

        /// <summary>
        /// 审批更新当天打卡记录状态
        /// </summary>
        /// <param name="ProjectPlanID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool UpdatePunchCardStatus(ProJectPlanPunchCardModel proJectPlanPunchCardModel)
        {
            try
            {
                tmpsql = $@"UPDATE [dbo].[RM_ProJectPlanPunchCard] SET ApprovalStatus ='{proJectPlanPunchCardModel.ApprovalStatus}',ApprovalUser='{proJectPlanPunchCardModel.ApprovalUser}',IsApproval=0,ApprovalCreate='{proJectPlanPunchCardModel.ApprovalCreate}' where ID='{proJectPlanPunchCardModel.ID}'";
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

        /// <summary>
        /// 我要审批的项目计划状态
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        public DataTable GetMyApprovalInfo(UserModel userModel)
        {
            try
            {
                tmpsql = $@"select rm_ppc.*,rm_plan.ProjectPlanName,rm_p.ProJectName from RM_ProJectPlanPunchCard rm_ppc
                        inner join RM_ProJectPlan rm_plan on rm_ppc.ProJectPlanID=rm_plan.ID
                        inner join RM_ProjectInfo rm_p on rm_plan.ProjectID=rm_p.ID
                        where CHARINDEX('{userModel.userName}',rm_p.PM)>0 and rm_ppc.ApprovalStatus=2";
                dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public DataTable GetfileimgbyGUID(string guid)
        {
            try
            {
                tmpsql = $@"SELECT * FROM RM_PunchCardFile WHERE MsgGUID='{guid}'";
                dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region 计划大类操作

        /// <summary>
        /// 查询计划大类列表
        /// </summary>
        /// <param name="Search"></param>
        /// <returns></returns>
        public DataTable GetProJectPlanClass(string Search)
        {
            try
            {
                tmpsql = "select * from [dbo].[RM_ProJectPlanClass] where 1=1 ";

                if (Search != "")
                {
                    tmpsql += $" and PlanClassName like '%{Search}%'";
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
        /// 绑定计划大类下拉框
        /// </summary>
        /// <returns></returns>
        public DataTable BindSelect()
        {
            try
            {
                tmpsql = "select * from [dbo].[RM_ProJectPlanClass] where 1=1 ";
                dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 根据ID获取某一条计划大类信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public DataTable GetProJectPlanClassByID(int ID)
        {
            try
            {
                try
                {
                    tmpsql = $"select * from [dbo].[RM_ProJectPlanClass] where ID='{ID}'";
                    dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                    return dt;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 新增计划任务大类信息
        /// </summary>
        /// <param name="proJectPlanClassModel"></param>
        /// <returns></returns>
        public bool AddProJectPlanClass(ProJectPlanClassModel proJectPlanClassModel)
        {
            try
            {
                tmpsql = $@"INSERT INTO [dbo].[RM_ProJectPlanClass]
                                               ([PlanClassName]
                                               ,[Remark])
                                         VALUES
                                               ('{proJectPlanClassModel.PlanClassName}'
                                               ,'{proJectPlanClassModel.Remark}')";
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

        /// <summary>
        /// 修改计划任务大类信息
        /// </summary>
        /// <param name="proJectPlanClassModel"></param>
        /// <returns></returns>
        public bool EditProJectPlanClass(ProJectPlanClassModel proJectPlanClassModel)
        {
            try
            {
                tmpsql = $@"UPDATE [dbo].[RM_ProJectPlanClass]
                               SET [PlanClassName] = '{proJectPlanClassModel.PlanClassName}'
                                  ,[Remark] = '{proJectPlanClassModel.Remark}'
                             WHERE ID='{proJectPlanClassModel.ID}'";
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

        /// <summary>
        /// 删除计划任务大类信息
        /// </summary>
        /// <param name="ArrayID"></param>
        /// <returns></returns>
        public bool DelProJectPlanClass(string ArrayID)
        {
            try
            {
                tmpsql = $"DELETE [dbo].[RM_ProJectPlanClass] WHERE ID in('{ArrayID.Replace(",", "','")}')";
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

        #region 项目统计
        public DataTable GetPunchCardInfo(string ProJectPlanID, string type, string time)
        {
            try
            {
                if (type == "d")
                {
                    tmpsql = $@"select rm_ppc.*,rm_plan.ProjectPlanName,rm_plan.Frequencyt_Value from [dbo].[RM_ProJectPlanPunchCard] rm_ppc
                            inner join RM_ProJectPlan rm_plan on rm_ppc.ProJectPlanID=rm_plan.ID
                            where rm_ppc.ApprovalStatus=0 and rm_ppc.ProJectPlanID='{ProJectPlanID}' and CONVERT(varchar(100), rm_ppc.PunchCardDate, 23)<=CONVERT(varchar(100), '{time}', 23)";

                }
                else if (type == "m")
                {
                    tmpsql = $@"select rm_ppc.*,rm_plan.ProjectPlanName from [dbo].[RM_ProJectPlanPunchCard] rm_ppc
                            inner join RM_ProJectPlan rm_plan on rm_ppc.ProJectPlanID=rm_plan.ID
                            where rm_ppc.ApprovalStatus=0 and rm_ppc.ProJectPlanID='{ProJectPlanID}' and CONVERT(varchar(100), rm_ppc.PunchCardDate, 23)<=CONVERT(varchar(100), '{time}', 23)";

                }
                else if (type == "y")
                {
                    tmpsql = $@"select rm_ppc.*,rm_plan.ProjectPlanName from [dbo].[RM_ProJectPlanPunchCard] rm_ppc
                            inner join RM_ProJectPlan rm_plan on rm_ppc.ProJectPlanID=rm_plan.ID
                            where rm_ppc.ApprovalStatus=0 and rm_ppc.ProJectPlanID='{ProJectPlanID}' and DATENAME(year,rm_ppc.PunchCardDate)<='{time}'";

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
        /// 获取项目计划信息
        /// </summary>
        /// <param name="Search">筛选条件</param>
        /// <param name="ProjectID">项目ID</param>
        /// <returns></returns>
        public DataTable GetProjectPlanInfo(string Search, string ProjectID, string type)
        {
            try

            {
                tmpsql = $@"select rm_plan.ID,rm_plan.ProjectID,rm_plan.ProjectPlanName,rm_plan.PlanClassID,rm_plan.Frequencyt_Type,rm_plan.Frequencyt_Value,rm_plan.[Count],rm_plan.CreateDate,rm_plan.CreateUser,rm_plan.BeginDate,rm_plan.EndDate,rm_plan.PlanStatus,case rm_plan.PlanStatus when 0 then '已完成' when 1 then '未完成' when 2 then '进行中' when 3 then '逾期' else '未知' end as PlanStatusName,rm_pj.ProJectName,rm_planclass.PlanClassName from RM_ProJectPlan rm_plan
                            inner join RM_ProjectInfo rm_pj on rm_pj.ID=rm_plan.ProjectID
                            inner join RM_ProJectPlanClass rm_planclass on rm_planclass.ID=rm_plan.PlanClassID where rm_plan.ProjectID = '{ProjectID}' and rm_plan.Frequencyt_Type='{type}'";
                if (Search != "")
                {
                    tmpsql += $" and rm_plan.ProjectPlanName like '%{Search}%'";
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
        /// 项目计划进度统计
        /// </summary>
        /// <param name="type"></param>
        /// <param name="time"></param>
        /// <returns>[0]=项目进度、[1]=项目计划完成次数、[2]=项目实际完成次数</returns>
        public DataSet Watercalc_Index_DayControlLevel(string type, string time)
        {
            try
            {
                switch (type)
                {
                    case "DD":
                        tmpsql = $"select top 1* from benchmark_index_data where indexcode ='001' and CONVERT(varchar(7), indextime, 120)='{time}' order by indextime desc;";
                        tmpsql = $"select top 1* from benchmark_index_data where indexcode ='002' and CONVERT(varchar(7), indextime, 120)='{time}' order by indextime desc";
                        tmpsql = $"select top 1* from benchmark_index_data where indexcode ='003' and CONVERT(varchar(7), indextime, 120)='{time}' order by indextime desc";
                        break;
                    case "MM":
                        tmpsql = $"select top 1* from benchmark_index_data where indexcode ='001' and CONVERT(varchar(100), indextime, 23)='{time}' order by indextime desc;";
                        tmpsql = $"select top 1* from benchmark_index_data where indexcode ='002' and CONVERT(varchar(100), indextime, 23)='{time}' order by indextime desc";
                        tmpsql = $"select top 1* from benchmark_index_data where indexcode ='003' and CONVERT(varchar(100), indextime, 23)='{time}' order by indextime desc";
                        break;
                    case "YY":
                        tmpsql = $"select top 1* from benchmark_index_data where indexcode ='001' and DATENAME(year,indextime)='{time}' order by indextime desc;";
                        tmpsql = $"select top 1* from benchmark_index_data where indexcode ='002' and DATENAME(year,indextime)='{time}' order by indextime desc";
                        tmpsql = $"select top 1* from benchmark_index_data where indexcode ='003' and DATENAME(year,indextime)='{time}' order by indextime desc";
                        break;
                }
                DataSet ds = DBHelper.DBHelperMsSql.ExecuteDataSet(tmpsql);
                return ds;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void JS()
        {
            try
            {
                tmpsql = "select * from ";
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region 获取不同用户的项目任务



        #endregion
    }
}