using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SZRiverSys.Model
{
    public class BusinessModel
    {
        public string ID;
        /// <summary>
        /// 业务类型
        /// </summary>
        public string Business_Type;
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creater;
        /// <summary>
        /// 开始时间
        /// </summary>
        public string Starttime;
        /// <summary>
        /// 结束时间
        /// </summary>
        public string Endtime;
        /// <summary>
        /// 指派人
        /// </summary>
        public string Assignor;
        /// <summary>
        /// 范围
        /// </summary>
        public string Range;
        /// <summary>
        /// 完成时间
        /// </summary>
        public string FinishTime;
        /// <summary>
        /// 完成说明
        /// </summary>
        public string FinishExplain;
        /// <summary>
        /// 完成附件
        /// </summary>
        public string Enclosure;
        /// <summary>
        /// 评分
        /// </summary>
        public string Score;
        /// <summary>
        /// 评语
        /// </summary>
        public string Comment;

        public string Remark;

        public string CreateTime;

        public string state;
    }
    /// <summary>
    /// 工单模型
    /// </summary>
    public class Business_WorkForm
    {
        public string ID { get; set; }
        public string WorkName { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// 紧急程度
        /// </summary>
        public string Emergencylevel { get; set; }
        /// <summary>
        /// 计划完成工时
        /// </summary>
        public string PlanFinishWorkHouse { get; set; }
        /// <summary>
        /// 计划完成日期
        /// </summary>
        public string PlanFinishTime { get; set; }
        /// <summary>
        /// 工作要求
        /// </summary>
        public string WorkNeed { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 指派人
        /// </summary>
        public string Assignor { get; set; }
        public string Creater { get; set; }

        public string Editer { get; set; }

        public string EditTime { get; set; }

        public string state { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public string FinishTime { get; set; }
        /// <summary>
        /// 完成说明
        /// </summary>
        public string FinishExplain { get; set; }
        /// <summary>
        /// 完成附件
        /// </summary>
        public string Enclosure { get; set; }

        public string projectid { get; set; }

        public string islook { get; set; }
        public string Range { get; set; }
        public string Workload { get; set; }

        public string RangeUnit { get; set; }

        public string Longitude { get; set; }

        public string Latitude { get; set; }

        public Task4002HTML Operate { get; set; }
    }

    /// <summary>
    /// 工单执行过程
    /// </summary>
    public class WorkFormTrack
    {
        public string FormId;
        public string userid;
        public string Content;
        public string state;
        public string photo;
    }
    public class PlaceInfo
    {
        public string PlaceName;
        public string Pid;
        public string Remark;
    }
    public class Commonlanguage
    {
        public string Content;
        public string type;
    }
}