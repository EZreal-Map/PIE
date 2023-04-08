using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SZRiverSys.Model
{
    public class ProjectPlanModel
    {
        public ProjectPlanModel()
        {
            this.CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.BeginDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.EndDate = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd HH:mm:ss");
            this.PlanStatus = 1;
        }

        /// <summary>
        /// 唯一ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 所属项目ID
        /// </summary>
        public int ProjectID { get; set; }

        /// <summary>
        /// 计划名称
        /// </summary>
        public string ProjectPlanName { get; set; }

        /// <summary>
        /// 所属计划大类ID
        /// </summary>
        public int PlanClassID { get; set; }

        /// <summary>
        /// 频次类别【h=小时、d=天、m=月、y=年】
        /// </summary>
        public string Frequencyt_Type { get; set; }

        /// <summary>
        /// 频次值
        /// </summary>
        public string Frequencyt_Value { get; set; }

        /// <summary>
        /// 计划任务总次数【根据频次、开始时间与结束时间计算而得】
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public string ModifyDate { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifyUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// 提交人
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 起始日期
        /// </summary>
        public string BeginDate { get; set; }

        /// <summary>
        /// 到期日
        /// </summary>
        public string EndDate { get; set; }


        /// <summary>
        /// 计划状态【0已完成、1未完成、2进行中】
        /// </summary>
        public int PlanStatus { get; set; }
        
        

        #region 列表页关联表特殊字段
        public string PlanStatusName { get; set; }
        public string ProJectName { get; set; }
        public string PlanClassName { get; set; }

        /// <summary>
        /// 打卡次数
        /// </summary>
        public int PunchCardCount { get; set; }

        /// <summary>
        /// 计划进度
        /// </summary>
        public double ProjectSchedule { get; set; }

        /// <summary>
        /// 项目计划已完成次数
        /// </summary>
        public int PlanIsOKCount { get; set; }
        
        /// <summary>
        /// 项目实际需要完成次数
        /// </summary>
        public int PlanCount { get; set; }
        #endregion

        public Project7002HTML Operate { get; set; }//角色
    }
}