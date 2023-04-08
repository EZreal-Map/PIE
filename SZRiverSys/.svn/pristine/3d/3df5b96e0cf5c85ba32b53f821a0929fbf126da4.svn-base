using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Newtonsoft.Json;
namespace SZRiverSys.ServiceHandler
{
    /// <summary>
    /// StationHander 的摘要说明
    /// </summary>
    public class StationHander : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string msg = "{\"code\":1,\"success\":false,\"msg\":\"请求参数不对\"}";
            SQL.StationSql sql = new SQL.StationSql();
            Model.UserModel User = new Model.UserModel();
            string method = context.Request["method"];
            DataTable dt;
            JSON.JsonHelper tojson = new JSON.JsonHelper();
            if (method != null)
            {
                string token = context.Request.Params["token"] == null ? "" : context.Request.Params["token"];
                if (method == "getstationinfo")//获取所有监测站信息
                {
                    dt = sql.GetStationInfo();
                    if (dt.Rows.Count > 0)
                    {
                        string json = tojson.DataTableToJsonForLayUi(dt);
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
                else if (method == "getwtsrtuinfo")//获取设施信息
                {
                    string type = context.Request["type"];
                    dt = sql.GetRTUInfo(type);
                    if (dt.Rows.Count > 0)
                    {
                        string json = tojson.DataTableToJsonForLayUi(dt);
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
                #region 预警数据接口
                else if (method == "getaltype") //获取预警类型
                {
                    dt = sql.GetAlType();
                    if (dt.Rows.Count > 0)
                    {
                        string json = tojson.DataTableToJsonForLayUi(dt);
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
                else if (method == "getsensordata")//获取断面下的相关指标数据
                {
                    string typeid = context.Request["typeid"];
                    dt = sql.GetStationDataNow(typeid);
                    if (dt.Rows.Count > 0)
                    {
                        string json = tojson.DataTableToJsonForLayUi(dt);
                        json= json.Replace("\"[", "[");
                        json= json.Replace("]\"","]");
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
                else if (method == "getsensordatabytype")//获取横沥口,赤水洞水,茂仔水,坑背河四个断面上各类监测指标的实时数据
                {
                    string typeid = context.Request["typeid"];
                    dt = sql.GetStationSensorDataNow(typeid);
                    if (dt.Rows.Count > 0)
                    {
                        string json = tojson.DataTableToJsonForLayUi(dt);
                        json = json.Replace("\"[", "[");
                        json = json.Replace("]\"", "]");
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
                else if (method == "getsectionrain")//获取断面的雨量
                {
                    dt = sql.GetStationRain();
                    if (dt.Rows.Count > 0)
                    {
                        string json = tojson.DataTableToJsonForLayUi(dt);
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
                else if (method == "getstationbywr")//获取所有断面下水位与水情的数值
                {
                    string date = context.Request["date"];
                    string senid = context.Request["senid"];
                    dt = sql.GetStationByWR(date,senid);
                    if (dt !=null && dt.Rows.Count > 0)
                    {
                        string json = tojson.DataTableToJsonForLayUi(dt);
                        json = json.Replace("\"[", "[");
                        json = json.Replace("]\"", "]");
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
                else if (method == "getsectiondatabys")//根据S参数返回所有断面对应指标数据
                {
                    string S = context.Request["S"];
                    dt = sql.GetStationDataByS(S);
                    if (dt.Rows.Count > 0)
                    {
                        string json = tojson.DataTableToJsonForLayUi(dt);
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
                else if(method=="getstationbyhouse")//获取断面某小时的数据时间
                {
                    string date_house = context.Request["date_house"];
                    string senid = context.Request["senid"];
                    string json = sql.GetStationByHouse(date_house,senid);
                    msg = "{\"code\":0,\"success\":true,\"msg\":\"" + json + "\"}";
                }
                else if (method == "getalsensordata")//根据时间和类型获取站点下的预警数据
                {
                    string type = context.Request["type"];
                    string date = context.Request["date"];
                    dt = sql.GetStationAlData(type, date);
                    if (dt.Rows.Count > 0)
                    {
                        string json = tojson.DataTableToJsonForLayUi(dt);
                        json = json.Replace("\"[", "[");
                        json = json.Replace("]\"", "]");
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
                else if (method == "getalarmdb") //获取某一时间某一类型的预警值
                {
                    string starttime = context.Request["starttime"];
                    string endtime = context.Request["endtime"];
                    string type = context.Request["type"];
                    dt = sql.GetAlDataByType(type,starttime,endtime);
                    if (dt.Rows.Count > 0)
                    {
                        string json = tojson.DataTableToJsonForLayUi(dt);
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
                else if (method == "getwatertsdb")//获取指标数据
                {
                    string sendid = context.Request["sendid"];
                    string starttime = context.Request["starttime"];
                    string endtime = context.Request["endtime"];
                    dt = sql.GetTSDBByID(sendid, starttime,endtime);
                    if (dt.Rows.Count > 0)
                    {
                        string json = tojson.DataTableToJsonForLayUi(dt);
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
                #endregion
                else if (method =="getsensorbystationid")//根据站点ID获取该站点的所有监测指标ID及指标名称。
                {
                    string id = context.Request["id"];
                    dt = sql.GetSenSorIndexById(id);
                    if (dt.Rows.Count > 0)
                    {
                        string json = tojson.DataTableToJsonForLayUi(dt);
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
                else if (method == "getcaleinfobystationid")//根据站点ID获取该站点的所有计算指标ID及指标名称。
                {
                    string id = context.Request["id"];
                    dt = sql.GetCaleIndexById(id);
                    if (dt.Rows.Count > 0)
                    {
                        string json = tojson.DataTableToJsonForLayUi(dt);
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
                else if (method == "getallsensordatabystationid")//根据站点ID获取该站点所有监测指标的实时数据。
                {
                    string id = context.Request["id"];
                    List<Model.StationCalcData> m = sql.GetAllSensorByid(id);
                    if (m.Count > 0)
                    {
                        string json = JsonConvert.SerializeObject(m);
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }else if(method=="getsomesensorbyid")//根据站点ID以及监测指标ID（可一次返回多个）的实时数据
                {
                    string sendid = context.Request["sendid"];
                    List<Model.StationCalcData> m = sql.GetSensorDataById("",sendid);
                    if (m.Count > 0)
                    {
                        string json = JsonConvert.SerializeObject(m);
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
                else if (method == "getallcalcbystationid")//根据站点ID获取该站点最后的所有计算指标的实时数据。
                {
                    string id = context.Request["id"];
                    List<Model.StationCalcData> m = sql.GetAllCalcByStationId(id);
                    if (m.Count > 0)
                    {
                        string json = JsonConvert.SerializeObject(m);
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
                else if (method == "getsomecalcbyid")//根据站点ID以及计算指标ID（可一次返回多个）获取实时数据。
                {
                    string sendid = context.Request["sendid"];
                    List<Model.StationCalcData> m = sql.GetCalcDataById("", sendid);
                    if (m.Count > 0)
                    {
                        string json = JsonConvert.SerializeObject(m);
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
                else if(method=="getallstationbyonsensor")//根据指定的监测指标名称查询所有站点上该指标的实时监测数据。
                {
                    string id = context.Request["id"];
                    List<Model.AllStationOneSensor> listm = sql.GetAllStationByOneSensor(id);
                    if (listm.Count > 0)
                    {
                        string json = JsonConvert.SerializeObject(listm);
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
                else if (method == "getallstationbyonecalc")//根据指定的计算指标名称查询所有站点上该指标的实时计算数据
                {
                    string id = context.Request["id"];
                    List<Model.AllStationOneSensor> listm = sql.GetAllStationByOneCalc(id);
                    if (listm.Count > 0)
                    {
                        string json = JsonConvert.SerializeObject(listm);
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
                else if (method == "getsensordatabytime")//根据站点ID、监测指标ID以及开始时间与结束时间获取该站点在指定时间段的监测数据。
                {
                    string sendid = context.Request["sendid"];
                    string starttime = context.Request["starttime"];
                    string endtime = context.Request["endtime"];
                    dt = sql.GetSenSorDataByTime(sendid, starttime, endtime);
                    if (dt.Rows.Count > 0)
                    {
                        string json = tojson.DataTableToJsonForLayUi(dt);
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
                else if(method== "getcalcdatabytime")//根据站点ID、计算指标ID以及开始时间与结束时间获取该站点在指定时间段的计算数据。
                {
                    string sendid = context.Request["sendid"];
                    string starttime = context.Request["starttime"];
                    string endtime = context.Request["endtime"];
                    dt = sql.GetCalcDataByTime(sendid, starttime, endtime);
                    if (dt.Rows.Count > 0)
                    {
                        string json = tojson.DataTableToJsonForLayUi(dt);
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
                else if (method == "getallstationwl")//返回所有测站水位数据
                {
                    string waterid = "001";
                    List<Model.AllStationOneSensor> listm = sql.GetAllStationByOneSensor(waterid);
                    if (listm.Count > 0)
                    {
                        string json = JsonConvert.SerializeObject(listm);
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
                else if (method == "getallrtuinfo")
                {
                    string type = context.Request["type"];
                    dt = sql.GetAllRtuInfo(type);
                    DataTable rtudt = sql.GetRtuTypeById(type);
                    string typename = rtudt.Rows[0]["RTUNAME"].ToString();
                    if (dt.Rows.Count > 0)
                    {
                        string json = tojson.DataTableToJsonForLayUi(dt);
                        msg = "{\"code\":0,\"type\":\""+ typename + "\",\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
                else if (method == "getrtutype")
                {
                    dt = sql.GetRtuType();
                    if (dt.Rows.Count > 0)
                    {
                        string json = tojson.DataTableToJsonForLayUi(dt);
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
            }
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.Write(msg);//构造json数据格式
            context.Response.End();
            context.Response.Close();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}