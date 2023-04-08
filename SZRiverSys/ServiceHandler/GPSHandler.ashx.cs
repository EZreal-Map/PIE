using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SZRiverSys.ServiceHandler
{
    /// <summary>
    /// GPSHandler 的摘要说明
    /// </summary>
    public class GPSHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string msg = "{\"code\":1,\"success\":false,\"msg\":\"请求参数不对\"}";            
            string method = context.Request["method"];
            SQL.GPSSql sql = new SQL.GPSSql();
            JSON.JsonHelper tojson = new JSON.JsonHelper();
            string json = "";
            if (method != null)
            {
                string token = context.Request.Params["token"] == null ? "" : context.Request.Params["token"];
                Model.UserModel tmpUser = CacheHelper.CacheHelper.Get(token) as Model.UserModel;
                if(tmpUser == null)
                {
                    msg = "{\"code\":-1,success:false,msg:\"用户登录失效,请重新登录\"}";
                    context.Response.ContentType = "text/plain; charset=utf-8";
                    context.Response.Write(msg);//构造json数据格式
                    context.Response.End();
                    context.Response.Close();
                    return;
                }
                if (CacheHelper.CacheHelper.IsContain(token))
                {    
                    if (method == "upload")//上传定位数据
                    {
                        string tmpmonitortime = context.Request.Params["time"];
                        string tmplongitudeStr = context.Request.Params["longitude"];
                        string tmplatitudeStr = context.Request.Params["latitude"];
                        string tmpaltitudeStr = context.Request.Params["altitude"];
                        string tmpspeedStr = context.Request.Params["speed"];
                        string tmpaccuracyStr = context.Request.Params["accuracy"];
                        string tmpisfirstStr = context.Request.Params["isfirst"];
                        string tmpAddress = context.Request.Params["address"];
                        string tmpRemark = context.Request.Params["remark"];
                        if (tmpisfirstStr != null)
                            tmpisfirstStr = "1";
                        else
                            tmpisfirstStr = "0";
                        if (tmpAddress == null)
                            tmpAddress = "";
                        if (tmpRemark == null)
                            tmpRemark = "";
                        DateTime tmpTime = DateTime.Now;
                        if (DateTime.TryParse(tmpmonitortime, out tmpTime))
                        {
                            if (float.Parse(tmpaccuracyStr) <100)
                            {
                                string tmpSQL = "Insert Into UserGPS_" + tmpTime.ToString("yyyyMM")
                                + " (username,gpsTime,longitude,latitude,altitude,speed,accuracy,isfirst,address,remark) Values ('"
                                + tmpUser.usercode + "','" + tmpTime.ToString("yyyy-MM-dd HH:mm:ss") + "'," + tmplongitudeStr
                                + "," + tmplatitudeStr + "," + tmpaltitudeStr + "," + tmpspeedStr + "," + tmpaccuracyStr + "," + tmpisfirstStr
                                + ",'" + tmpAddress + "','" + tmpRemark + "')";
                                if (DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSQL) > 0)
                                {
                                    if (tmpUser.Tag == null)
                                    {
                                        tmpUser.Tag = false;
                                        DataTable tmpTable = DBHelper.DBHelperMsSql.ExecuteDataTable("Select ID From UserGPSFinal Where username='" + tmpUser.usercode + "'");
                                        if (tmpTable != null && tmpTable.Rows.Count > 0)
                                            tmpUser.Tag = true;
                                    }
                                    if ((bool)tmpUser.Tag)//更新
                                    {
                                        tmpSQL = "Update UserGPSFinal Set gpsTime='" + tmpTime.ToString("yyyy-MM-dd HH:mm:ss") + "',longitude=" + tmplongitudeStr
                                            + ",latitude=" + tmplatitudeStr + ",altitude=" + tmpaltitudeStr + ",speed=" + tmpspeedStr + ",accuracy=" + tmpaccuracyStr
                                            + ",address='"+ tmpAddress + "',remark='"+ tmpRemark + "'  Where username='"
                                            + tmpUser.usercode + "'";
                                        DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSQL);
                                    }
                                    else//第一次插入
                                    {
                                        tmpSQL = "Insert Into UserGPSFinal (username,gpsTime,longitude,latitude,altitude,speed,accuracy,address,remark) Values ('"
                                            + tmpUser.usercode + "','" + tmpTime.ToString("yyyy-MM-dd HH:mm:ss") + "'," + tmplongitudeStr
                                            + "," + tmplatitudeStr + "," + tmpaltitudeStr + "," + tmpspeedStr + "," + tmpaccuracyStr + ",'" + tmpAddress + "','" + tmpRemark + "')";
                                        DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpSQL);
                                        tmpUser.Tag = true;
                                    }

                                    msg = "{\"code\":0,success:true,msg:\"上传成功\"}";
                                }
                            }

                        }
                    }
    
                }
                else
                {
                    msg = "{\"code\":-1,success:false,msg:\"用户登录失效,请重新登录\"}";
                }
                if (method == "getusergps")//获取用户当前坐标
                {
                    string username = context.Request["username"];
                    DataTable dt = sql.GetUserGPSNow(username,tmpUser);
                    if (dt!=null && dt.Rows.Count > 0)
                    {
                        json = tojson.DataTableToJsonForLayUi(dt);
                        msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                    }
                }
                else if (method == "getusergpsbytime")//获取用户在某一时间段内的坐标信息
                {
                    string username = context.Request["username"];
                    string starttime = context.Request["starttime"];
                    string endtime = context.Request["endtime"];
                    string tmpAccuracy = context.Request["accuracy"];
                    if (tmpAccuracy == null)
                        tmpAccuracy = "";
                    if (starttime!=null && endtime!=null)
                    {
                        starttime = starttime.Replace('_', ' ');
                        endtime = endtime.Replace('_', ' ');
                        DataTable dt = sql.GetUserGPSTrack(username, starttime, endtime);
                        if (dt!=null && dt.Rows.Count > 0)
                        {
                            json = tojson.DataTableToJsonForLayUi(dt);
                            msg = "{\"code\":0,\"success\":true,\"msg\":" + json + "}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无数据\"}";
                        }
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