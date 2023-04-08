using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace SZRiverSys.ServiceHandler
{
    /// <summary>
    /// BusinessHandler 的摘要说明
    /// </summary>
    public class BusinessHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string msg = "{\"code\":1,\"success\":false,\"msg\":\"请求参数不对\"}";
            string method = context.Request["method"];
            JSON.JsonHelper Json = new JSON.JsonHelper();
            string json = "";
            SQL.BusinessSql sql = new SQL.BusinessSql();
            if (method != null)
            {
                string token = context.Request.Params["token"] == null ? "" : context.Request.Params["token"];
                if (CacheHelper.CacheHelper.IsContain(token))
                {
                    Model.UserModel tmpUser = CacheHelper.CacheHelper.Get(token) as Model.UserModel;
                    if (method == "createbusiness")//创建业务
                    {
                        Model.BusinessModel m = new Model.BusinessModel();
                        m.Business_Type = context.Request["Business_Type"];
                        m.Creater = tmpUser.usercode;
                        m.Starttime = context.Request["Starttime"];
                        m.Endtime = context.Request["Endtime"];
                        m.Assignor = context.Request["Assignor"];
                        m.Range = context.Request["Range"];
                        m.state = context.Request["state"];
                        if (sql.AddBusiness(m))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"创建成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"创建失败\"}";
                        }
                    }
                    else if (method == "getbusinesstype")//获取业务类型
                    {
                        DataTable dt = sql.GetBusinessType();
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            json = Json.DataTableToJsonForLayUi(dt);
                            msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                        }
                        else
                        {
                            msg = "{\"code\":0,\"success\":false,\"data\":[]}";
                        }
                    }
                    else if(method== "getbusinesslist")//获取业务列表
                    {
                        int pageindex = int.Parse(context.Request["pageindex"]);
                        int pagesize = int.Parse(context.Request["pagesize"]);
                        int total = 0;
                        DataTable dt = sql.GetBusinessList(tmpUser,pagesize,pageindex,out total);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            json = Json.DataTableToJsonForLayUi(dt);
                            msg = "{\"code\":0,\"success\":true,\"data\":" + json + ",\"total\":" + total + "}";
                        }
                        else
                        {
                            msg = "{\"code\":0,\"success\":false,\"data\":[]}";
                        }
                    }
                    else if (method == "getuserbusiness")//获取用户自身业务
                    {
                        string username = tmpUser.usercode;
                        string Business_Type = context.Request["Business_Type"];
                        DataTable dt = sql.GetUserBusiness(username, Business_Type);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            json = Json.DataTableToJsonForLayUi(dt);
                            msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                        }
                        else
                        {
                            msg = "{\"code\":0,\"success\":false,\"data\":[]}";
                        }
                    }
                    else if(method== "uploadbusinessfile")//用户上传业务文件
                    {
                        HttpFileCollection hfc = context.Request.Files;
                        HttpPostedFile hpf = null;                        
                        //if (hfc.Count > 0)
                        //{
                        //    hpf = context.Request.Files[0];
                        //}                       
                        string FileNames = "";
                        bool errormsg = true;
                        string errormsgstr = "";
                        if (hfc.Count > 0)
                        {
                            for (int i = 0; i < hfc.Count; i++)
                            {
                                hpf = context.Request.Files[i];
                                string date = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                                var filetype = hpf.FileName.Split('.')[1];
                                var filename = tmpUser.usercode + "-" + date + "." + filetype;
                                var filepath = Path.Combine(HttpContext.Current.Server.MapPath("~/Upfile/RiverManageFile"), filename);
                                string directoryName = Path.GetDirectoryName(filepath);
                                if (!Directory.Exists(directoryName))
                                {
                                    Directory.CreateDirectory(directoryName);
                                }
                                try
                                {
                                    hpf.SaveAs(filepath);
                                }
                                catch (Exception ex)
                                {
                                    errormsg = false;
                                    errormsgstr = ex.Message;
                                }
                                FileNames += filename + ";";
                            }
                        }
                        if (errormsg)
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"上传成功\",\"FileName\":\"" + FileNames + "\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"上传失败\"}";
                        }

                    }
                    else if (method == "insertbusinessfinishdata")//插入业务完成信息
                    {
                        Model.BusinessModel m = new Model.BusinessModel();
                        m.ID = context.Request["ID"];
                        m.FinishExplain = context.Request["FinishExplain"];
                        m.FinishTime = DateTime.Now.ToLocalTime().ToString();
                        m.Enclosure = context.Request["Enclosure"];
                        if (sql.FinishBusiness(m))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"添加成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"添加失败\"}";
                        }
                    }
                    else if (method == "downonefile")//下载单个文件
                    {
                        string filename = context.Request["filename"];
                        var filelist = filename.TrimEnd(';').Split(';');
                        string[] filepath = new string[filelist.Length];
                        for(int i = 0; i < filelist.Length; i++)
                        {
                            filepath[i] = CacheHelper.CacheHelper.ServerURL + "Upfile/RiverManageFile/" + filelist[i];
                        }
                        //string filepath=HttpContext.Current.Server.MapPath("~/Upfile/RiverManageFile/") +filename;
                        //if (File.Exists(filepath))
                        //{
                        //    DownFileList(filepath);
                        //}

                        msg = "{\"code\":0,\"success\":true,\"filepath\":\"" + string.Join(";",filepath) +"\"}";
                        
                    }
                    else if (method == "downbusinessfile") //下载附件
                    {
                        string ID = context.Request["ID"];
                        DataTable dt = sql.GetBusinessById(ID);
                        string filelist = dt.Rows[0]["Enclosure"].ToString();
                        if (filelist != "")
                        {
                            ///创建zip父级文件夹
                            var file = filelist.Split(';');
                            string filename=tmpUser.usercode+"_"+ DateTime.Now.ToString("yyyyMMddHHmmssfff");
                            string newfilepath = HttpContext.Current.Server.MapPath("~/Upfile/downzip/")+filename;
                            System.IO.Directory.CreateDirectory(newfilepath);
                            for(var i = 0; i < file.Length; i++)
                            {
                                string filepath= Path.Combine(HttpContext.Current.Server.MapPath("~/Upfile/RiverManageFile"), file[i]);
                                if (File.Exists(filepath))
                                {
                                    if (!File.Exists(newfilepath + "/" + file[i]))
                                    {
                                        File.Copy(filepath, newfilepath + "/" + file[i]);
                                    }
                                }
                            }
                            string Source = newfilepath;
                            string TartgetFile = newfilepath + ".zip";
                            Directory.CreateDirectory(Path.GetDirectoryName(TartgetFile));
                            using (ZipOutputStream s = new ZipOutputStream(File.Create(TartgetFile)))
                            {
                                s.SetLevel(6);
                                Comm.ZIP.Compress(Source, s);
                                s.Finish();
                                s.Close();
                            }
                            Comm.FileHelper.DeleteFolder(newfilepath);
                            DownFileList(TartgetFile);
                        }
                        else
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"暂无文件\"}";
                        }
                    }
                    else if(method== "getbusinessnofinish")//获取时间内未完成业务
                    {
                        string username = tmpUser.usercode;
                        DataTable dt = sql.GetUserBusiness(username);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            json = Json.DataTableToJsonForLayUi(dt);
                            msg = "{\"code\":0,\"success\":true,\"data\":" + json + ",\"count\":" + dt.Rows.Count + "}";
                        }
                        else
                        {
                            msg = "{\"code\":0,\"success\":false,\"data\":[]}";
                        }
                    }
                    else if (method == "getbusinesspasstime")//获取逾期未完成业务
                    {
                        string username = tmpUser.usercode;
                        DataTable dt = sql.GetUserBusinessPassTime(username);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            json = Json.DataTableToJsonForLayUi(dt);
                            msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                        }
                        else
                        {
                            msg = "{\"code\":0,\"success\":false,\"data\":[]}";
                        }
                    }
                    else if(method== "getuserfinishbusiness")//获取用户完成的业务
                    {
                        DataTable dt = sql.GetUserBusinessFinish(tmpUser.usercode);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            json = Json.DataTableToJsonForLayUi(dt);
                            msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                        }
                        else
                        {
                            msg = "{\"code\":0,\"success\":false,\"data\":[]}";
                        }
                    }
                    else if(method== "getbusinesslistbytime")//根据时间获取所有业务
                    {
                        string starttime = context.Request["starttime"];
                        string endtime = context.Request["endtime"];
                        DataTable dt = sql.GetBusinessByTime(starttime, endtime);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            json = Json.DataTableToJsonForLayUi(dt);
                            msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                        }
                        else
                        {
                            msg = "{\"code\":0,\"success\":false,\"data\":[]}";
                        }
                    }
                    else if (method == "evaluatebusiness")//用户进行业务评价
                    {
                        Model.BusinessModel m = new Model.BusinessModel();
                        m.ID = context.Request["ID"];
                        m.Score = context.Request["Score"];
                        m.Comment = context.Request["Comment"];
                        if (sql.evaluate(m))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"评价成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"评价失败\"}";
                        }
                    }                  
                }
                else
                {
                    msg = "{\"code\":-1,success:false,msg:\"用户登录失效,请重新登录\"}";
                }
            }
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.Write(msg);//构造json数据格式
            context.Response.End();
            context.Response.Close();
        }
        /// <summary>
        /// 将文件夹转换成zip压缩文件并下载
        /// </summary>
        /// <param name="ParentPath">文件夹地址</param>
        public void DownZipFile(string ParentPath)
        {
            string Source = ParentPath;
            string TartgetFile = ParentPath + ".zip";
            Directory.CreateDirectory(Path.GetDirectoryName(TartgetFile));
            using (ZipOutputStream s = new ZipOutputStream(File.Create(TartgetFile)))
            {
                s.SetLevel(6);
                Comm.ZIP.Compress(Source, s);
                s.Finish();
                s.Close();
            }
            Comm.FileHelper.DeleteFolder(ParentPath);
            DownFileList(TartgetFile);
        }
        public void DownFileList(string downpath)
        {

            HttpResponse _respose = HttpContext.Current.Response;
            //string fileName = "test.doc";//客户端保存的文件名 
            //以字符流的形式下载文件 
            FileStream fs = new FileStream(downpath, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            _respose.ContentType = "image/jpeg";
            //通知浏览器下载文件而不是打开 
            _respose.AddHeader("Content-Disposition", "attachment;  filename=" + HttpUtility.UrlEncode(downpath, System.Text.Encoding.UTF8));
            _respose.BinaryWrite(bytes);
            _respose.Flush();
            _respose.End();
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