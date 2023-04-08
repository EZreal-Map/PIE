using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using SZRiverSys.Comm;
using SZRiverSys.Model;
namespace SZRiverSys.ServiceHandler
{
    /// <summary>
    /// FileManager 的摘要说明
    /// </summary>
    public class FileManager : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string msg = "{\"code\":1,\"success\":false,\"msg\":\"请求参数不对\"}";
            string method = context.Request["method"];
            DataTable dt;
            JSON.JsonHelper Json = new JSON.JsonHelper();
            SQL.FileManageSql sql = new SQL.FileManageSql();
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            string json = "";
            #region 实例化实体类
            List<FileManageModel> fileModels = new List<FileManageModel>();//物资列表集合
            FileManageModel fileModel = new FileManageModel();
            #endregion

            #region  实例化权限模块
            File2005HTML file2005HTML = new File2005HTML();
            RolePowerHelper _ROLEPOWER = new RolePowerHelper();
            #endregion
            if (method != null)
            {
                string token = context.Request.Params["token"] == null ? "" : context.Request.Params["token"];
                Model.UserModel tmpUser = CacheHelper.CacheHelper.Get(token) as Model.UserModel;
                if (CacheHelper.CacheHelper.IsContain(token))
                {
                    if (method == "getfilemanagelist")//获取文档列表
                    {
                        int pageindex = int.Parse(context.Request["pageindex"]);
                        int pagesize = int.Parse(context.Request["pagesize"]);
                        string search = context.Request["search"];
                        string type = context.Request["type"];
                        int total = 0;
                        dt = sql.GetFileList(pageindex, pagesize, search, type, out total);
                        file2005HTML = _ROLEPOWER.RULE_File2005HTML(tmpUser.RoleID);
                        if (dt != null && dt.Rows.Count > 0)
                        {

                            fileModels = DataTableToEntity<FileManageModel>.ConvertToModel(dt);
                            for (int i = 0; i < fileModels.Count; i++)
                            {
                                fileModels[i].Operate = file2005HTML;
                            }
                            msg = "{\"code\":0,\"success\":true,\"data\":" + js.Serialize(fileModels.Skip((pageindex - 1) * pagesize).Take(pagesize)) + ",\"total\":" + total + ",\"msg\":" + js.Serialize(file2005HTML) + "}";
                        }
                        else
                        {
                            msg = "{\"code\":0,\"success\":false,\"data\":[],\"msg\":" + js.Serialize(file2005HTML) + "}";
                        }
                    }
                    else if(method == "addfileinfo")//新增文档信息
                    {
                        Model.FileManageModel m = new FileManageModel();
                        m.FileName = context.Request["FileName"];
                        m.FileDescribe = context.Request["FileDescribe"];
                        m.FileType = context.Request["FileType"];
                        m.FilePath = context.Request["FilePath"];
                        m.Creater = tmpUser.userName;
                        if (sql.AddFileInfo(m))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"添加成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"添加失败\"}";
                        }
                    }
                    else if (method == "uploadfile")//上传文档
                    {
                        HttpFileCollection hfc = context.Request.Files;
                        HttpPostedFile hpf = null;                     
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
                                var filepath = Path.Combine(HttpContext.Current.Server.MapPath("~/Upfile/FileManage"), filename);
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
                    else if (method == "downfile")//下载文档
                    {
                        string filename = context.Request["filepath"];
                        var filelist = filename.TrimEnd(';').Split(';');
                       
                        for (int i = 0; i < filelist.Length; i++)
                        {
                            string filePath = HttpContext.Current.Server.MapPath("../Upfile/FileManage/" + filelist[i]);//路径 
                            if (File.Exists(filePath))
                            {
                                DownFileList(filePath);
                            }
                        }
                        //string filepath=HttpContext.Current.Server.MapPath("~/Upfile/RiverManageFile/") +filename;
                        //if (File.Exists(filepath))
                        //{
                        //    DownFileList(filepath);
                        //}
                        
                    }
                    else if (method == "getfileinfobyid")//获取文档信息
                    {
                        string ID = context.Request["ID"];
                        dt = sql.GetFileInfoByID(ID);
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
                    else if (method == "editfileinfo")//修改文档信息
                    {
                        Model.FileManageModel m = new FileManageModel();
                        m.ID = context.Request["ID"];
                        m.FileName = context.Request["FileName"];
                        m.FileDescribe = context.Request["FileDescribe"];
                        m.FileType = context.Request["FileType"];
                        if (sql.EditFileInfo(m))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"修改成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"修改失败\"}";
                        }
                    }
                    else if (method == "delfileinfo")//删除文档信息
                    {
                        string ID = context.Request["ID"];
                        if (sql.Del_FileInfo(ID))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"删除成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"删除失败\"}";
                        }
                    }
                    else if (method == "downmonthreport")//下载管养月报模板
                    {

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
        public void DownFileList(string downpath)
        {
            HttpResponse _respose = HttpContext.Current.Response;
            //string fileName = "test.doc";//客户端保存的文件名 
            //以字符流的形式下载文件 
            FileStream fs = new FileStream(downpath, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            _respose.ContentType = "application/octet-stream";
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