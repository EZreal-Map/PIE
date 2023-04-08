using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Newtonsoft.Json;
namespace SZRiverSys.ServiceHandler
{
    /// <summary>
    /// WXPatrolHander 的摘要说明
    /// </summary>
    public class WXPatrolHander : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string msg = "{\"code\":1,\"success\":false,\"msg\":\"请求参数不对\"}";
            SQL.WXSql Sql = new SQL.WXSql();
            Model.UserModel User = new Model.UserModel();
            string method = context.Request["method"];
            JSON.JsonHelper tojson = new JSON.JsonHelper();
            DataTable dt;
            if (method != null)
            {
                string token = context.Request.Params["token"] == null ? "" : context.Request.Params["token"];
                if (method == "getpatrollist")
                {
                    string search = context.Request["search"];
                    int PageIndex = int.Parse(context.Request["pageindex"].ToString());
                    int PageSize = int.Parse(context.Request["pagesize"].ToString());
                    string date = context.Request["date"];
                    string uid = context.Request["uid"];
                    int total = 0;
                    dt = Sql.GetPatrolList(PageIndex, PageSize, search,uid, date, out total);
                    if (dt.Rows.Count > 0)
                    {
                        string json = tojson.DataTableToJsonForLayUi(dt);
                        msg = "{\"code\":0,\"msg\":\"OK\",\"data\":" + json + ",\"count\":" + total + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"msg\":\"暂无数据\",\"data\":\"暂无数据\"}";
                    }
                }
                else if (method == "insertwxgps")
                {
                   // string gpslist = context.Request["gpslist"];
                   // string[] gps = gpslist.Split(',');       
                   //if(Sql.InsertGpsTrack(gps[0], gps[1]))
                   // {
                   //     msg = "{\"code\":0,\"msg\":\"OK\"}";
                   // }
                   // else
                   // {
                   //     msg = "{\"code\":1,\"msg\":\"NO\"}";
                   // }
        
                }
                else if (method == "getuserddl")
                {
                    dt = Sql.GetUserDDL();
                    if (dt.Rows.Count > 0)
                    {
                        string json = tojson.DataTableToJsonForLayUi(dt);
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }else if (method == "lookimgbyid")
                {
                    string id = context.Request["id"];
                    dt = Sql.GetImgById(id);
                    string filename = dt.Rows[0]["photopath"].ToString();
                    filename = filename.TrimStart('{');
                    filename = filename.TrimEnd('}');
                    string[] filelist = filename.Split(',');
                    string imgurl = "http://www.sinogeoscience.com/wxpatrolimg/";
                    string [] imglist=new string[filelist.Count()];
                    for (int i = 0; i < filelist.Count(); i++)
                    {
                        string imgname = filelist[i].Split(':')[1];
                        imglist[i] = imgurl+ imgname;
                    }
                    msg = "{\"code\":0,\"success\":true,\"msg\":" + JsonConvert.SerializeObject(imglist) + "}";                   
                }else if(method== "getpatroltrack")//获取巡护轨迹
                {
                    string uid = context.Request["uid"];
                    string starttime = context.Request["starttime"];
                    string endtime = context.Request["endtime"];
                    dt = Sql.GetPatrolTrack(uid, starttime, endtime);
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
                else if (method == "getrtustate") //获取设备维修信息
                {
                    dt = Sql.GetRtuPatrol();
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
                else if (method == "getpatroltotal") //获取巡河统计
                {
                    string date = context.Request["date"];
                    string uname = context.Request["uname"];
                    dt = Sql.GetPatrolTotal(date,uname);
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
                else if (method == "getusergps")//获取巡河人员的实时坐标(五分钟内)
                {
                    dt = Sql.GetUserGPSData();
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
                else if (method == "getpointtolength")
                {
                    double x1 = 114.182306020508;
                    double y1 = 22.5943658349211;
                    double x2 = 114.182567939043;
                    double y2 = 22.5942667634767;
                    double lenth = Comm.PointLength.GetDistance(y1, x1, y2, x2);
                    msg = "{\"code\":0,\"success\":true,\"msg\":" + lenth + "}";

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