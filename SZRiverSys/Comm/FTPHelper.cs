using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
namespace SZRiverSys.Comm
{
    public class FTPHelper
    {
        ///如果有在创建时会报异常（不让异常抛出），如果没有则创建，只能创建一层
        /// <summary>
        /// 在FTP上创建文件夹
        /// </summary>
        /// <param name="url">ftp地址，如：ftp://192.168.1.43/files/ </param>
        /// <param name="uid">FTP用户名</param>
        /// <param name="pwd">FTP密码</param>
        public static void CreatFolder(string url, string uid, string pwd)
        {
            FtpWebRequest frequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
            frequest.Credentials = new NetworkCredential(uid, pwd);
            frequest.Method = WebRequestMethods.Ftp.MakeDirectory;
            try
            {
                FtpWebResponse response = frequest.GetResponse() as FtpWebResponse;
            }
            catch
            {
            }
        }

        /// <summary>
        /// HttpPostedFileBase FTP文件上传（适合webform项目）
        /// </summary>
        /// <param name="hpf">Request.Files[0]</param>
        /// <param name="fileName">文件路径+文件名如：ftp://192.168.1.43/files/A.jpg </param>
        /// <param name="uid">FTP用户名</param>
        /// <param name="pwd">FTP密码</param>
        /// <param name="msg">提示</param>
        /// <returns>上传成功返回true,失败FALSE</returns>
        public static bool FtpUpload(HttpPostedFile hpfb, string fileName, string uid, string pwd, out string msg)
        {
            HttpPostedFileBase hpf = new HttpPostedFileWrapper(hpfb) as HttpPostedFileBase;
            FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(fileName));
            // 指定数据传输类型
            reqFTP.UseBinary = true;
            // ftp用户名和密码
            reqFTP.Credentials = new NetworkCredential(uid, pwd);

            reqFTP.KeepAlive = false;
            // 指定执行什么命令
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            // 上传文件时通知服务器文件的大小
            reqFTP.ContentLength = hpf.InputStream.Length;

            // 缓冲大小设置为kb 
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            // 打开一个文件流(System.IO.FileStream) 去读上传的文件
            try
            {
                // 把上传的文件写入流
                Stream strm = reqFTP.GetRequestStream();
                // 每次读文件流的kb
                contentLen = hpf.InputStream.Read(buff, 0, buffLength);
                // 流内容没有结束
                while (contentLen != 0)
                {
                    // 把内容从file stream 写入upload stream 
                    strm.Write(buff, 0, contentLen);
                    contentLen = hpf.InputStream.Read(buff, 0, buffLength);
                }
                // 关闭两个流
                strm.Close();
                hpf.InputStream.Close();
                msg = "完成";
                return true;
            }
            catch (Exception ex)
            {
                msg = string.Format("因{0},无法完成上传", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// FileInfo Ftp文件上传（适合winform项目）
        /// </summary>
        /// <param name="filename">本地文件路径</param>
        /// <param name="ftpUrl">需上传到FTP的地址+文件名</param>
        /// <param name="uid">FTP用户名</param>
        /// <param name="pwd">FTP密码</param>
        /// <param name="errorinfo"> 提示信息</param>
        /// <returns></returns>
        public static bool FtpUpload(string filename, string ftpUrl, string uid, string pwd, out string errorinfo) //上面的代码实现了从ftp服务器上载文件的功能
        {
            FileInfo fileInf = new FileInfo(filename);
            FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpUrl));
            // 指定数据传输类型
            reqFTP.UseBinary = true;
            // ftp用户名和密码
            reqFTP.Credentials = new NetworkCredential(uid, pwd);

            // 默认为true，连接不会被关闭
            // 在一个命令之后被执行
            reqFTP.KeepAlive = false;
            // 指定执行什么命令
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            // 上传文件时通知服务器文件的大小
            reqFTP.ContentLength = fileInf.Length;
            // 缓冲大小设置为kb 
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            // 打开一个文件流(System.IO.FileStream) 去读上传的文件
            FileStream fs = fileInf.OpenRead();
            try
            {
                // 把上传的文件写入流
                Stream strm = reqFTP.GetRequestStream();
                // 每次读文件流的kb
                contentLen = fs.Read(buff, 0, buffLength);
                // 流内容没有结束
                while (contentLen != 0)
                {
                    // 把内容从file stream 写入upload stream 
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                // 关闭两个流
                strm.Close();
                fs.Close();
                errorinfo = "完成";
                return true;
            }
            catch (Exception ex)
            {
                errorinfo = string.Format("因{0},无法完成上传", ex.Message);
                return false;
            }
        }

        #region 下载文件
        private static List<string> ImageExtend = new List<string> { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="ftpUserID"></param>
        /// <param name="ftpPassword"></param>
        /// <param name="filePath"></param>
        public static void Download(string ftpUrl, string ftpUserID, string ftpPassword, string filePath)
        {
            //需要现在的文件在ftp上的完整路径  
            //string fileUploadPath = ftpServer + ftpDefaultUrl;
            Uri uri = new Uri(filePath);
            //string downloadUrl = @"E:\MyDownloads\" + Path.GetFileName(uri.LocalPath); 
            var downloadUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/UpFiles/ExcelFiles"), Path.GetFileName(uri.LocalPath));
            //下载后存放的路径  
            string FileName = Path.GetFullPath(downloadUrl) + Path.DirectorySeparatorChar.ToString() + Path.GetFileName(uri.LocalPath);

            //创建文件流  
            FileStream fs = null;
            Stream responseStream = null;
            try
            {
                //创建一个与FTP服务器联系的FtpWebRequest对象  
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
                //设置请求的方法是FTP文件下载  
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                //连接登录FTP服务器  
                request.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                //获取一个请求响应对象  
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                //获取请求的响应流  
                responseStream = response.GetResponseStream();
                if (ImageExtend.Contains(Path.GetExtension(filePath).ToLower()))
                {
                    var stream = (Stream)responseStream;
                    Image file = Image.FromStream(stream);

                    ImageFormat format = file.RawFormat;
                    MemoryStream ms = new MemoryStream();
                    file.Save(ms, ImageFormat.Png);
                    byte[] testDownImage = new byte[ms.Length];
                    //Image.Save()会改变MemoryStream的Position，需要重新Seek到Begin
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Read(testDownImage, 0, testDownImage.Length);
                    ms.Close();
                    DownLoadFileResponse(uri, testDownImage);
                }
                //判断本地文件是否存在，如果存在，则打开和重写本地文件  
                if (File.Exists(downloadUrl))
                {
                    fs = File.Open(downloadUrl, FileMode.Open, FileAccess.ReadWrite);
                }
                else
                {
                    fs = File.Create(downloadUrl);
                }

                if (fs != null)
                {
                    int buffer_count = 65536;
                    byte[] buffer = new byte[buffer_count];
                    int size = 0;
                    while ((size = responseStream.Read(buffer, 0, buffer_count)) > 0)
                    {
                        fs.Write(buffer, 0, size);
                    }
                    fs.Flush();
                    fs.Close();
                    responseStream.Close();
                    DownLoadFileResponse(uri, buffer);
                }
            }
            finally
            {
                if (fs != null)
                    fs.Close();
                if (responseStream != null)
                    responseStream.Close();
            }
        }

        private static void DownLoadFileResponse(Uri uri, byte[] testDownImage)
        {
            HttpResponse _response = HttpContext.Current.Response;
            _response.ContentType = "application/octet-stream";
            //通知浏览器下载文件而不是打开
            _response.AddHeader("Content-Disposition", "attachment;  filename=" + HttpContext.Current.Server.UrlPathEncode(Path.GetFileName(uri.LocalPath)));
            _response.BinaryWrite(testDownImage);
            _response.Flush();
            _response.End();
        }


        #endregion
    }
}