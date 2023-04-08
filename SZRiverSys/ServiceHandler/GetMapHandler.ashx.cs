using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace SZRiverSys.ServiceHandler
{
    /// <summary>
    /// GetMapHander 的摘要说明
    /// </summary>
    public class GetMapHander : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string msg = "{\"code\":1,\"success\":false,\"msg\":\"请求参数不对\"}";
            SQL.UserSql Sql = new SQL.UserSql();
            Model.UserModel User = new Model.UserModel();
            string method = context.Request["method"];
            if (method != null)
            {
                string token = context.Request.Params["token"] == null ? "" : context.Request.Params["token"];
                if (method == "getrivermaplist")//获取深圳河流的地图切片
                {
                    string x_a = context.Request.Params["x"];
                    string y_a = context.Request.Params["y"];
                    string level_a = context.Request.Params["level"];
                    //string path_a = "";
                    //path_a = @""+AppDomain.CurrentDomain.BaseDirectory+ "RiverMap\\" + "SZMap" + "\\L" + "" + level_a + "\\" + "" + y_a + "-" + x_a + ".png";
                    //if (!File.Exists(path_a))
                    //{
                    //    path_a= AppDomain.CurrentDomain.BaseDirectory + "mapimage\\imagenull.png";
                    //}
                    //byte[] imgbyte = ConvertToBinary(path_a);
                    //if (imgbyte.Length > 0)
                    //{
                    //    FileStream fstream = File.Create(path_a, imgbyte.Length);

                    //    try
                    //    {

                    //        fstream.Write(imgbyte, 0, imgbyte.Length);//二进制转换成文件 
                    //        Bitmap myImage = new Bitmap(fstream);
                    //        MemoryStream ms = new MemoryStream();
                    //        myImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    //        HttpContext.Current.Response.ClearContent();
                    //        HttpContext.Current.Response.ContentType = "image/Png";
                    //        HttpContext.Current.Response.BinaryWrite(ms.ToArray());

                    //        HttpContext.Current.Response.End();
                    //        //HttpContext.Current.Response.Redirect("MapPublishingHandler.ashx", false);  
                    //        HttpContext.Current.Response.Close();
                    //       // HttpContext.Current.ApplicationInstance.CompleteRequest();
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        //抛出异常信息
                    //    }
                    //    finally
                    //    {
                    //        fstream.Close();
                    //    }
                    //}
                    byte[] imgbyte = null;
                    string path = @"" + AppDomain.CurrentDomain.BaseDirectory + "RiverMap\\" + "SZMap" + "\\L" + "" + level_a + "\\" + "" + y_a + "-" + x_a + ".png";// context.Server.MapPath(tmppath + "\\L" + "" + level + "\\" + "" + y + "-" + x + ".png");
                    if (!File.Exists(path))//文件不存在就用默认图片
                        imgbyte = getNullBmpBytes(context);
                    else
                        imgbyte = ConvertToBinary(path);
                    if (imgbyte != null && imgbyte.Length > 0)
                    {
                        context.Response.ClearContent();
                        context.Response.ContentType = "image/png";
                        context.Response.BinaryWrite(imgbyte);
                        context.Response.StatusCode = 200;
                        imgbyte = null;
                        return;
                    }
                    return;

                }
                else if (method == "getriverzone_blue")
                {
                    string x_a = context.Request.Params["x"];
                    string y_a = context.Request.Params["y"];
                    string level_a = context.Request.Params["level"];
                    //string path_a = "";
                    //path_a = @"" + AppDomain.CurrentDomain.BaseDirectory + "RiverMap\\" + "RiverZone_Blue_Txt" + "\\L" + "" + level_a + "\\" + "" + y_a + "-" + x_a + ".png";
                    //if (!File.Exists(path_a))
                    //{
                    //    path_a = AppDomain.CurrentDomain.BaseDirectory + "mapimage\\imagenull.png";
                    //}
                    //byte[] imgbyte = ConvertToBinary(path_a);
                    //if (imgbyte.Length > 0)
                    //{
                    //    FileStream fstream = File.Create(path_a, imgbyte.Length);
                    //    try
                    //    {

                    //        fstream.Write(imgbyte, 0, imgbyte.Length);//二进制转换成文件 
                    //        Bitmap myImage = new Bitmap(fstream);
                    //        MemoryStream ms = new MemoryStream();
                    //        myImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    //        HttpContext.Current.Response.ClearContent();
                    //        HttpContext.Current.Response.ContentType = "image/Png";
                    //        HttpContext.Current.Response.BinaryWrite(ms.ToArray());
                    //        HttpContext.Current.Response.End();
                    //        //HttpContext.Current.Response.Redirect("MapPublishingHandler.ashx", false);  
                    //        HttpContext.Current.Response.Close();
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        //抛出异常信息
                    //    }
                    //    finally
                    //    {
                    //        fstream.Close();
                    //    }
                    //}
                    byte[] imgbyte = null;
                    string path = @"" + AppDomain.CurrentDomain.BaseDirectory + "RiverMap\\" + "RiverZone_Blue_Txt" + "\\L" + "" + level_a + "\\" + "" + y_a + "-" + x_a + ".png";// context.Server.MapPath(tmppath + "\\L" + "" + level + "\\" + "" + y + "-" + x + ".png");
                    if (!File.Exists(path))//文件不存在就用默认图片
                        imgbyte = getNullBmpBytes(context);
                    else
                        imgbyte = ConvertToBinary(path);
                    if (imgbyte != null && imgbyte.Length > 0)
                    {
                        context.Response.ClearContent();
                        context.Response.ContentType = "image/png";
                        context.Response.BinaryWrite(imgbyte);
                        context.Response.StatusCode = 200;
                        imgbyte = null;
                        return;
                    }
                    return;
                }
                else if (method == "getrasterlayer")
                {
                    string x_a = context.Request.Params["x"];
                    string y_a = context.Request.Params["y"];
                    string level_a = context.Request.Params["level"];
                    //string path_a = "";
                    //path_a = @"" + AppDomain.CurrentDomain.BaseDirectory + "RiverMap\\" + "Raster_0109" + "\\L" + "" + level_a + "\\" + "" + y_a + "-" + x_a + ".png";
                    //if (!File.Exists(path_a))
                    //{
                    //    path_a = AppDomain.CurrentDomain.BaseDirectory + "mapimage\\imagenull.png";
                    //}
                    //byte[] imgbyte = ConvertToBinary(path_a);
                    //if (imgbyte.Length > 0)
                    //{
                    //    FileStream fstream = File.Create(path_a, imgbyte.Length);
                    //    try
                    //    {

                    //        fstream.Write(imgbyte, 0, imgbyte.Length);//二进制转换成文件 
                    //        Bitmap myImage = new Bitmap(fstream);
                    //        MemoryStream ms = new MemoryStream();
                    //        myImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    //        HttpContext.Current.Response.ClearContent();
                    //        HttpContext.Current.Response.ContentType = "image/Png";
                    //        HttpContext.Current.Response.BinaryWrite(ms.ToArray());
                    //        HttpContext.Current.Response.End();
                    //        //HttpContext.Current.Response.Redirect("MapPublishingHandler.ashx", false);  
                    //        HttpContext.Current.Response.Close();
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        //抛出异常信息
                    //    }
                    //    finally
                    //    {
                    //        fstream.Close();
                    //    }
                    //}
                    byte[] imgbyte = null;
                    string path = @"" + AppDomain.CurrentDomain.BaseDirectory + "RiverMap\\" + "Raster_0109" + "\\L" + "" + level_a + "\\" + "" + y_a + "-" + x_a + ".png";// context.Server.MapPath(tmppath + "\\L" + "" + level + "\\" + "" + y + "-" + x + ".png");
                    if (!File.Exists(path))//文件不存在就用默认图片
                        imgbyte = getNullBmpBytes(context);
                    else
                        imgbyte = ConvertToBinary(path);
                    if (imgbyte != null && imgbyte.Length > 0)
                    {
                        context.Response.ClearContent();
                        context.Response.ContentType = "image/png";
                        context.Response.BinaryWrite(imgbyte);
                        context.Response.StatusCode = 200;
                        imgbyte = null;
                        return;
                    }
                    return;
                }
            }
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.Write(msg);//构造json数据格式
            context.Response.End();
            context.Response.Close();
        }
        ///// <summary>
        ///// 将文件转换成二进制
        ///// </summary>
        ///// <param name="Path">文件路径</param>
        ///// <returns></returns>
        //public static byte[] ConvertToBinary(string Path)
        //{
        //    FileStream stream = new FileInfo(Path).OpenRead();
        //    byte[] buffer = new byte[stream.Length];
        //    stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
        //    stream.Close();
        //    return buffer;
        //}
        static byte[] m_NullBytes = null;

        public static byte[] getNullBmpBytes(HttpContext context)
        {
            if (m_NullBytes == null)
            {
                m_NullBytes = ConvertToBinary(context.Server.MapPath("~\\mapimage\\imagenull.png"));
            }
            return m_NullBytes;
        }

        /// <summary>
        /// 将文件转换成二进制
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <returns></returns>
        public static byte[] ConvertToBinary(string Path)
        {
            byte[] buffer = null;
            using (FileStream stream = new FileInfo(Path).OpenRead())
            {
                buffer = new byte[stream.Length];
                stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
                stream.Flush();
                stream.Close();
            }
            return buffer;
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