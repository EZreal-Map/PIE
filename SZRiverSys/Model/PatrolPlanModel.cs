using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SZRiverSys.Model
{
    /// <summary>
    /// 管养计划
    /// </summary>
    public class PatrolPlanModel
    {
        public string ID;
        public string UserId;
        public string StartTime;
        public string EndTime;
        public string Content;
        public string IndexName;
        public string V;
        public string ManagementRange;
        public string state;
    }
    /// <summary>
    /// 巡线计划
    /// </summary>
    public class PatrolPlan
    {
        public string ID { get; set; }
        public string UserId { get; set; }
        public string PatrolRank { get; set; }
        public string PatrolContent { get; set; }
        public string PlanStartDate { get; set; }
        public string PatrolLength { get; set; }
        public string PlanEndDate { get; set; }
        public string userName { get; set; }
        public XX9001HTML Operate { get; set; }
    }

    public class PatrolInfo
    {
        public string uid;
        public string Type;
        public string timecreate;
        public string infomemo;
        public string photopath;
        public string longitude;
        public string addressinfo;
        public string latitude;
        public string RTU;
    }
}