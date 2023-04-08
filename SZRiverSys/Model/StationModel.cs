using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SZRiverSys.Model
{
    public class StationModel
    {
    }

    public class StationCalcData
    {
       public string sendid { get; set; }
       public string name { get; set; }
       public string data { get; set; }
       public string datetime { get; set; }
       public string state { get; set; }
    }
    /// <summary>
    /// 监测站检测参数实体
    /// </summary>
    public class AllStationOneSensor
    {
        public string sendid { get; set; }
        public string StationName { get; set; }
        public string SensorName { get; set; }
        public string data { get; set; }
        public string datetime { get; set; }
    }
}