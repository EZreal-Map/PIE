using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SZRiverSys.Model
{
    /// <summary>
    /// 项目计划打卡记录表
    /// </summary>
    public class ProJectPlanPunchCardModel
    {
        public ProJectPlanPunchCardModel()
        {
            this.TodayPlanStatus = 0;
            this.ApprovalStatus = 2;
            this.PunchCardDate = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 唯一ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 项目计划ID
        /// </summary>
        public string ProJectPlanID { get; set; }

        /// <summary>
        /// 打卡人
        /// </summary>
        public string PunchCardPerson { get; set; }

        /// <summary>
        /// 当天任务计划状态【0已完成、1未完成、2其它原因】
        /// </summary>
        public int TodayPlanStatus { get; set; }

        /// <summary>
        /// 问题内容
        /// </summary>
        public string ProblemContent { get; set; }

        /// <summary>
        /// 审批状态【0已完成、1未完成、2待审批】
        /// </summary>
        public int ApprovalStatus { get; set; }


        /// <summary>
        /// 审批人
        /// </summary>
        public string ApprovalUser { get; set; }


        /// <summary>
        /// 审批时间
        /// </summary>
        public string ApprovalCreate { get; set; }

        /// <summary>
        /// 打卡时间
        /// </summary>
        public string PunchCardDate { get; set; }

        /// <summary>
        /// 唯一标识符
        /// </summary>
        public string GUID { get; set; }

        
        #region 列表页关联表特殊字段
        public string ProjectPlanName { get; set; }
        public string ProJectName { get; set; }
        #endregion
    }
}