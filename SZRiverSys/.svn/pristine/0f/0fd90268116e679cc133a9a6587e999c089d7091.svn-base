using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SZRiverSys.ServiceHandler
{
    /// <summary>
    /// MonthReport 的摘要说明
    /// </summary>
    public class MonthReport : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string msg = "{\"code\":1,\"success\":false,\"msg\":\"请求参数不对\"}";
            string method = context.Request["method"];
            if (method != null)
            {
                string token = context.Request.Params["token"] == null ? "" : context.Request.Params["token"];
                Model.UserModel tmpUser = CacheHelper.CacheHelper.Get(token) as Model.UserModel;
                if (method == "generatemonthreport")//生成每月月报模板
                {
                    WriteToPublicationOfResult();
                }
                else if (method == "downmonthreport")//下载每月月报模板
                {

                }
            }
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.Write(msg);//构造json数据格式
            context.Response.End();
            context.Response.Close();
        }
        public struct PublicationInfor

        {

            public string reportmonth;

            public string reportmonthday;

            public string fullSite;

            public string area;

            public string deadLine;

            public string publicationTime;

        }
            ///<summary>
            /// 测试村实测结果公示公告
            /// </summary>
            public static void WriteToPublicationOfResult()
            {
                #region    获取时间
                DateTime dt = DateTime.Now;
                string ym = dt.Year.ToString() + "年" + dt.Month + "月";
                int monthDay = DateTime.DaysInMonth(dt.Year, dt.Month);
                #endregion
                string filepath = HttpContext.Current.Server.MapPath("~/ReportTemplate/《罗湖区河道及水库管养服务项目》深圳水库入库支流管养2018年3月份月报_注记版.doc");
                FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);

                XWPFDocument myDocx = new NPOI.XWPF.UserModel.XWPFDocument(fs);//打开07（.docx）以上的版本的文档

                PublicationInfor plcInfor = new PublicationInfor

                {

                    reportmonth = ym,
                    reportmonthday = ym + "(" + ym + "1日-" + ym + monthDay + "日" + ")",

                    fullSite = "神圣兽国游尾郡窝窝乡",

                    area = "70.60",

                    deadLine = "2018年12月12日",

                    publicationTime = "2018年11月12日"

                };

                //遍历word中的段落

                foreach (var para in myDocx.Paragraphs)

                {

                    string oldtext = para.ParagraphText;

                    if (oldtext == "")

                        continue;

                    string temptext = para.ParagraphText;



                    //以下为替换文档模版中的关键字

                    if (temptext.Contains("{$reportmonth}"))

                        temptext = temptext.Replace("{$reportmonth}", plcInfor.reportmonth);



                    if (temptext.Contains("{$reportmonthday}"))

                        temptext = temptext.Replace("{$reportmonthday}", plcInfor.reportmonthday);



                    if (temptext.Contains("{$area}"))

                        temptext = temptext.Replace("{$area}", plcInfor.area);



                    if (temptext.Contains("{$deadLine}"))

                        temptext = temptext.Replace("{$deadLine}", plcInfor.deadLine);



                    if (temptext.Contains("{$publicationTime}"))

                        temptext = temptext.Replace("{$publicationTime}", plcInfor.publicationTime);



                    para.ReplaceText(oldtext, temptext);

                }

            

            FileStream output = new FileStream(@"测试村实测结果公示公告.doc", FileMode.Create);

                myDocx.Write(output);

                fs.Close();

                fs.Dispose();

                output.Close();

                output.Dispose();

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