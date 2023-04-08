using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SZRiverSys.Model
{
    /// <summary>
    /// 进度统计表
    /// </summary>
    public class ProjectDataReportModel
    {

        public ProjectDataReportModel()
        {
            this.ProjectPlanCount = 0;
            this.CompletedCount = 0;
            this.Unfinished = 0;
            this.PunchCardCount = 0;
            this.PunchCardingCount = 0;
            //this.IsOK = false;
        }
        /// <summary>
        /// 总计划数
        /// </summary>
        public int ProjectPlanCount { get; set; }

        /// <summary>
        /// 已完成计划数
        /// </summary>
        public int CompletedCount { get; set; }

        /// <summary>
        /// 未完成计划数
        /// </summary>
        public int Unfinished { get; set; }
        
        /// <summary>
        /// 打卡次数
        /// </summary>
        public int PunchCardCount { get; set; }


        /// <summary>
        /// 进行中次数
        /// </summary>
        public int PunchCardingCount { get; set; }


        ///// <summary>
        ///// 当前时间段是否达标
        ///// </summary>
        //public bool IsOK { get; set; }

        /// <summary>
        /// 计划表
        /// </summary>
        public List<ProjectPlanModel> DataList { get; set; }
    }
}