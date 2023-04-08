using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace SZRiverSys.ServiceHandler
{
    /// <summary>
    /// CameraHandler 的摘要说明
    /// </summary>
    public class CameraHandler : IHttpHandler
    {
        /// <summary>
        /// 摄像头硬盘机访问地址
        /// </summary>
        static string NVRAddress = "http://192.168.1.64";
        static string NVRUser = "admin";
        static string NVRPWD = "sz20190220";

        public void ProcessRequest(HttpContext context)
        {
            string method = context.Request["method"];
            string msg = "";

            if (method != null)
            {
                string token = context.Request.Params["requestkey"] == null ? "" : context.Request.Params["requestkey"];
                if(token == "ABCDEFG09823AD")
                {
                    if (method == "queryevents")//查询时间范围内的事件
                    {
                        string tmpStartTimeStr = context.Request["starttime"];
                        string tmpEndTimeStr = context.Request["endtime"];
                        string tmpSql = "Select ID,cameraID,startTime,eventType,alarmLevel From Camera_AlarmDB Where startTime >='"+ tmpStartTimeStr + "' AND startTime <'"+tmpEndTimeStr+ "' Order By startTime DESC";
                        DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                        if(table!=null && table.Rows.Count>0)
                        {
                            JSON.JsonHelper Json = new JSON.JsonHelper();
                            string jsonStr = Json.DataTableToJsonForLayUi(table);
                            msg = "{\"code\":0,\"success\":true,\"data\":" + jsonStr + "}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无事件\"}";
                        }
                    }
                    else if (method == "geteventfile")//获取事件的图片文件
                    {
                        string ID = context.Request["eventid"];
                        string tmpSql = "Select path From Camera_AlarmDB Where ID="+ID;
                        DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                        if (table != null && table.Rows.Count > 0)
                        {
                            string tmpFilePath = table.Rows[0].ItemArray[0].ToString();
                            //tmpFilePath = @"E:\Workspace\图标\111111.jpg";
                            string tmpFilePath2 = tmpFilePath.Substring(tmpFilePath.LastIndexOf('\\'));
                            //context.Response.AddHeader("Content-Disposition", "attachment;filename=" + tmpFilePath2);
                            //context.Response.WriteFile(tmpFilePath);
                            context.Response.ClearContent();
                            context.Response.ContentType = "image/jpeg";
                            context.Response.WriteFile(tmpFilePath);
                            context.Response.StatusCode = 200;
                            return;
                        }
                        else
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"暂无事件\"}";
                    }
                    else if (method == "cameracontrol")//摄像头云台控制
                    {
                        string ID = context.Request["cameraid"];
                        string tmpMotion = context.Request["control"];
                        if(tmpMotion == "left")
                        {
                            CameraLeft(NVRAddress, NVRUser, NVRPWD, ID);
                            msg = "{\"code\":0,\"success\":true,\"data\":\"操作成功\"}";
                        }
                        else if (tmpMotion == "right")
                        {
                            CameraRight(NVRAddress, NVRUser, NVRPWD, ID);
                            msg = "{\"code\":0,\"success\":true,\"data\":\"操作成功\"}";
                        }
                        else if (tmpMotion == "up")
                        {
                            CameraUp(NVRAddress, NVRUser, NVRPWD, ID);
                            msg = "{\"code\":0,\"success\":true,\"data\":\"操作成功\"}";
                        }
                        else if (tmpMotion == "down")
                        {
                            CameraDown(NVRAddress, NVRUser, NVRPWD, ID);
                            msg = "{\"code\":0,\"success\":true,\"data\":\"操作成功\"}";
                        }
                        else if (tmpMotion == "zoomin")
                        {
                            CameraZoomIn(NVRAddress, NVRUser, NVRPWD, ID);
                            msg = "{\"code\":0,\"success\":true,\"data\":\"操作成功\"}";
                        }
                        else if (tmpMotion == "zoomout")
                        {
                            CameraZoomOut(NVRAddress, NVRUser, NVRPWD, ID);
                            msg = "{\"code\":0,\"success\":true,\"data\":\"操作成功\"}";
                        }
                        else if (tmpMotion == "goto")
                        {
                            string tmpPresetIndex = context.Request["presetindex"];
                            CameraGoToPreset(NVRAddress, NVRUser, NVRPWD, ID, int.Parse(tmpPresetIndex));
                            msg = "{\"code\":0,\"success\":true,\"data\":\"操作成功\"}";
                        }
                    }
                }
            }
             //msg = "{\"code\":0,\"success\":true,\"filepath\":\"" + string.Join(";", filepath) + "\"}";

            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.Write(msg);//构造json数据格式
            context.Response.End();
            context.Response.Close();
        }
        
        static void CameraRight(string url, string username, string password, string cameraID)
        {
            CameraControl(url, username, password, cameraID, 60, 0);
            System.Threading.Thread.Sleep(500);
            CameraControl(url, username, password, cameraID, 0, 0);
        }

        static void CameraLeft(string url, string username, string password, string cameraID)
        {
            CameraControl(url, username, password, cameraID, -60, 0);
            System.Threading.Thread.Sleep(500);
            CameraControl(url, username, password, cameraID, 0, 0);
        }
        static void CameraUp(string url, string username, string password, string cameraID)
        {
            CameraControl(url, username, password, cameraID, 0, 60);
            System.Threading.Thread.Sleep(500);
            CameraControl(url, username, password, cameraID, 0, 0);
        }

        static void CameraDown(string url, string username, string password, string cameraID)
        {
            CameraControl(url, username, password, cameraID, 0, -60);
            System.Threading.Thread.Sleep(500);
            CameraControl(url, username, password, cameraID, 0, 0);
        }

        static void CameraZoomIn(string url, string username, string password, string cameraID)
        {
            try
            {
                string URI = url + "/ISAPI/ContentMgmt/PTZCtrlProxy/channels/" + cameraID + "/continuous";
                Stream outstream = null;
                StreamReader sr = null;
                string tmpXMLTxt = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
                    + "<PTZData><zoom>60</zoom></PTZData>";
                byte[] data = System.Text.Encoding.UTF8.GetBytes(tmpXMLTxt);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URI);
                // 要注意的这是这个编码方式，还有内容的Xml内容的编码方式
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                //byte[] data = System.Text.Encoding.UTF8.GetBytes(xml.InnerXml);
                req.Method = "PUT";
                req.ContentType = "text/plain;charset=UTF-8";
                //添加用户名密码完成Authorization认证
                string userPasswd = username + ":" + password;
                CredentialCache mycache = new CredentialCache();
                mycache.Add(new Uri(URI), "Basic", new NetworkCredential(username, password));
                req.Credentials = mycache;
                req.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(new ASCIIEncoding().GetBytes(userPasswd)));
                //发送内容
                outstream = req.GetRequestStream();
                outstream.Write(data, 0, data.Length);
                outstream.Flush();
                outstream.Close();

                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                //接收NVR返回数据信息
                res = req.GetResponse() as HttpWebResponse;
            }
            catch
            {

            }
            CameraControl(url, username, password, cameraID, 0, 0);
        }
        static void CameraZoomOut(string url, string username, string password, string cameraID)
        {
            try
            {
                string URI = url + "/ISAPI/ContentMgmt/PTZCtrlProxy/channels/" + cameraID + "/continuous";
                Stream outstream = null;
                StreamReader sr = null;
                string tmpXMLTxt = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
                    + "<PTZData><zoom>-60</zoom></PTZData>";
                byte[] data = System.Text.Encoding.UTF8.GetBytes(tmpXMLTxt);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URI);
                // 要注意的这是这个编码方式，还有内容的Xml内容的编码方式
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                //byte[] data = System.Text.Encoding.UTF8.GetBytes(xml.InnerXml);
                req.Method = "PUT";
                req.ContentType = "text/plain;charset=UTF-8";
                //添加用户名密码完成Authorization认证
                string userPasswd = username + ":" + password;
                CredentialCache mycache = new CredentialCache();
                mycache.Add(new Uri(URI), "Basic", new NetworkCredential(username, password));
                req.Credentials = mycache;
                req.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(new ASCIIEncoding().GetBytes(userPasswd)));
                //发送内容
                outstream = req.GetRequestStream();
                outstream.Write(data, 0, data.Length);
                outstream.Flush();
                outstream.Close();

                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                //接收NVR返回数据信息
                res = req.GetResponse() as HttpWebResponse;
            }
            catch
            {

            }
            CameraControl(url, username, password, cameraID, 0, 0);
        }

        static bool CameraControl(string url, string username, string password, string cameraID, int pan, int tilt)
        {
            try
            {
                string URI = url + "/ISAPI/ContentMgmt/PTZCtrlProxy/channels/" + cameraID + "/continuous";
                Stream outstream = null;
                StreamReader sr = null;
                string tmpXMLTxt = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
                    + "<PTZData><pan>" + pan.ToString() + "</pan><tilt>" + tilt.ToString() + "</tilt></PTZData>";

                byte[] data = System.Text.Encoding.UTF8.GetBytes(tmpXMLTxt);//xml.InnerXml

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URI);
                // 要注意的这是这个编码方式，还有内容的Xml内容的编码方式
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                //byte[] data = System.Text.Encoding.UTF8.GetBytes(xml.InnerXml);
                req.Method = "PUT";
                req.ContentType = "text/plain;charset=UTF-8";
                //添加用户名密码完成Authorization认证
                string userPasswd = username + ":" + password;
                CredentialCache mycache = new CredentialCache();
                mycache.Add(new Uri(URI), "Basic", new NetworkCredential(username, password));
                req.Credentials = mycache;
                req.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(new ASCIIEncoding().GetBytes(userPasswd)));
                //发送内容
                outstream = req.GetRequestStream();
                outstream.Write(data, 0, data.Length);
                outstream.Flush();
                outstream.Close();

                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                //接收NVR返回数据信息
                res = req.GetResponse() as HttpWebResponse;
                ////直到req.GetResponse()程序才开始向目标网页发送Post请求
                //instream = res.GetResponseStream();
                //sr = new StreamReader(instream, encoding);
                ////返回结果网页代码
                //string content = sr.ReadToEnd();
                //Console.Write(content);
                return true;
            }
            catch(Exception ex)
            {

            }
            return false;
        }

        static bool CameraGoToPreset(string url, string username, string password, string cameraID, int presetID)
        {
            try
            {
                string URI = url + "/ISAPI/ContentMgmt/PTZCtrlProxy/channels/" + cameraID.ToString() + "/presets/" + presetID.ToString() + "/goto";
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URI);
                // 要注意的这是这个编码方式，还有内容的Xml内容的编码方式
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                //byte[] data = System.Text.Encoding.UTF8.GetBytes(xml.InnerXml);
                req.Method = "PUT";
                req.ContentType = "text/plain;charset=UTF-8";
                //添加用户名密码完成Authorization认证
                string userPasswd = username + ":" + password;
                CredentialCache mycache = new CredentialCache();
                mycache.Add(new Uri(URI), "Basic", new NetworkCredential(username, password));
                req.Credentials = mycache;
                req.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(new ASCIIEncoding().GetBytes(userPasswd)));

                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                //接收NVR返回数据信息
                res = req.GetResponse() as HttpWebResponse;
                ////直到req.GetResponse()程序才开始向目标网页发送Post请求
                //instream = res.GetResponseStream();
                //sr = new StreamReader(instream, encoding);
                ////返回结果网页代码
                //string content = sr.ReadToEnd();
                //Console.Write(content);
                return true;
            }
            catch
            {

            }
            return false;
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