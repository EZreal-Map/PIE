using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SZRiverSys.Model
{
    public class ProJectPlanClassModel
    {
        public int ID { get; set; }
        public string PlanClassName { get; set; }
        public string Remark { get; set; }

        public Project7003HTML Operate { get; set; }//角色
    }
}