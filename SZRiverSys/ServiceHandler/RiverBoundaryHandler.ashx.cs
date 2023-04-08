using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using SZRiverSys.Comm;

namespace SZRiverSys.ServiceHandler
{
    /// <summary>
    /// 用于获取河道断面GeoJson数据，河道
    /// </summary>
    public class RiverBoundaryHandler : IHttpHandler
    {
        SQL.StationSql sql = new SQL.StationSql();
        private void sendResponse(HttpContext context, string callbackFun, string result)
        {
            if (callbackFun != null)
                result = callbackFun + "(" + result + ")";
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.Write(result);
            context.Response.End();
            context.Response.Close();
        }
        /// <summary>
        /// 默认河道
        /// </summary>
        static string m_DefaultRiverSectionJson = null;
        /// <summary>
        /// 获取默认河道的GeoJson边界
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetDefaultRiverSectionJson(HttpContext context)
        {
            if (m_DefaultRiverSectionJson == null)
            {
                string tmpPath = context.Server.MapPath("~/riversection.geojson");
                if(System.IO.File.Exists(tmpPath))
                {
                    try
                    {
                        m_DefaultRiverSectionJson = System.IO.File.ReadAllText(tmpPath, System.Text.Encoding.GetEncoding("gb2312"));
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
            return m_DefaultRiverSectionJson;
        }

        static ArrayList m_RiverSectionCoords = null;

        public static ArrayList GetRiverSectionCoords(HttpContext context)
        {
            if (m_RiverSectionCoords == null)
            {
                string tmpPath = context.Server.MapPath("~/rivercenterlinecoords.txt");
                if (System.IO.File.Exists(tmpPath))
                {
                    try
                    {
                        StreamReader srReadFile = new StreamReader(tmpPath);
                        // 读取流直至文件末尾结束
                        while (!srReadFile.EndOfStream)
                        {
                            string strReadLine = srReadFile.ReadLine(); //读取每行数据
                            if(strReadLine!=null && strReadLine.Length > 0)
                            {
                                string[] tmpStrs = strReadLine.Split(',');
                                if (tmpStrs!=null && tmpStrs.Length >1)
                                {
                                    double tmpX, tmpY;
                                    if(double.TryParse(tmpStrs[0], out tmpX)
                                        && double.TryParse(tmpStrs[1], out tmpY))
                                    {
                                        if (m_RiverSectionCoords == null)
                                            m_RiverSectionCoords = new ArrayList();

                                        double[] tmpXYs = CoordinateConversion.lonLatToMercator(tmpX, tmpY);
                                        double[] tmpPoint03 = CoordinateConversion.mercatorToLonLat(tmpXYs[0], tmpXYs[1]);
                                        m_RiverSectionCoords.Add(tmpXYs);
                                    }
                                }
                            }
                        }
                        // 关闭读取流文件
                        srReadFile.Close();
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
            return m_RiverSectionCoords;
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            bool result = false;
            string msg = "";
            string strFun = context.Request["jsonpcallback"];//传递参数
            string method = context.Request["method"];
            if(method=="getorigriverboundary")//获取原始
            {
                string tmpContent = GetDefaultRiverSectionJson(context);
                sendResponse(context, strFun, tmpContent);
                return;
            }
            else if (method == "initialdata")//初始数据
            {
                m_RiverSectionCoords = null;
                m_DefaultRiverSectionJson = null;
                GetDefaultRiverSectionJson(context);
                GetRiverSectionCoords(context);
                result = true;
                msg = "数据初始化成功";
                string tmpContent02 = "{success:" + result.ToString().ToLower() + ",msg:" + msg + "}";
                sendResponse(context, strFun, tmpContent02);
                return;
            }
            else if (method == "getriversectionbytime")//根据时间获取当前河道断面GeoJson数据
            {
                string tmpTimeStr = context.Request.Params["time"];
                if(tmpTimeStr!=null)
                {
                    ArrayList tmpArray = GetRiverSectionCoords(context);
                    if (tmpArray != null && tmpArray.Count > 0)
                    {
                        DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable("execute sp_WaterCalc_getTSDBLeveldata '" + tmpTimeStr + "'");
                        if (table != null && table.Rows.Count > 0)
                        {
                            int tmpLen = tmpArray.Count;
                            if (tmpLen > table.Rows.Count)
                                tmpLen = table.Rows.Count;
                            string tmpTotalJson = "{\"type\": \"FeatureCollection\",\"features\":[ ";

                            double tmpWaterWidth = 0;
                            double[] tmpPoint01, tmpPoint02;
                            double[] tmpPoint03, tmpPoint04;
                            tmpPoint01 = (double[])tmpArray[0];

                            //计算第一个点
                            {
                                tmpPoint02 = (double[])tmpArray[1];
                                DataRow tmpRow = table.Rows[0];
                                tmpWaterWidth = double.Parse(tmpRow.ItemArray[3].ToString());//第4列表示水位宽度
                                double tmpDis = Math.Sqrt((tmpPoint02[0] - tmpPoint01[0]) * (tmpPoint02[0] - tmpPoint01[0]) + (tmpPoint02[1] - tmpPoint01[1]) * (tmpPoint02[1] - tmpPoint01[1]));
                                double tmpSin = (tmpPoint01[1] - tmpPoint02[1]) / tmpDis;
                                double tmpCos = (tmpPoint01[0] - tmpPoint02[0]) / tmpDis;
                                tmpPoint03 = new double[2];
                                tmpPoint04 = new double[2];
                                tmpPoint03[0] = tmpPoint01[0] - tmpWaterWidth * tmpSin;
                                tmpPoint03[1] = tmpPoint01[1] + tmpWaterWidth * tmpCos;
                                tmpPoint04[0] = tmpPoint01[0] + tmpWaterWidth * tmpSin;
                                tmpPoint04[1] = tmpPoint01[1] - tmpWaterWidth * tmpCos;
                                tmpPoint03 = CoordinateConversion.mercatorToLonLat(tmpPoint03);
                                tmpPoint04 = CoordinateConversion.mercatorToLonLat(tmpPoint04);

                                string tmpSingleGeo = "{\"type\": \"Feature\",\"properties\": {\"name\": \""+ tmpRow.ItemArray[0].ToString() + "\",\"monitorvalue\": "+ tmpRow.ItemArray[2].ToString() + "},\"geometry\": {\"type\": \"Polygon\",\"coordinates\": [ [";
                                string tmpFirst = "[" + tmpPoint03[0].ToString() + "," + tmpPoint03[1].ToString() + "]";
                                tmpSingleGeo += tmpFirst;
                                tmpSingleGeo += ",[" + tmpPoint04[0].ToString() + "," + tmpPoint04[1].ToString() + "]";

                                tmpPoint03[0] = tmpPoint02[0] - tmpWaterWidth * tmpSin;
                                tmpPoint03[1] = tmpPoint02[1] + tmpWaterWidth * tmpCos;
                                tmpPoint04[0] = tmpPoint02[0] + tmpWaterWidth * tmpSin;
                                tmpPoint04[1] = tmpPoint02[1] - tmpWaterWidth * tmpCos;
                                tmpPoint03 = CoordinateConversion.mercatorToLonLat(tmpPoint03);
                                tmpPoint04 = CoordinateConversion.mercatorToLonLat(tmpPoint04);

                                tmpSingleGeo += ",[" + tmpPoint04[0].ToString() + "," + tmpPoint04[1].ToString() + "]";
                                tmpSingleGeo += ",[" + tmpPoint03[0].ToString() + "," + tmpPoint03[1].ToString() + "]";
                                tmpSingleGeo += ","+ tmpFirst;

                                tmpSingleGeo += "]]}}";

                                tmpTotalJson += tmpSingleGeo;
                                tmpPoint01 = tmpPoint02;
                            }

                            for(int i=2;i< tmpLen;i++)
                            {
                                tmpPoint02 = (double[])tmpArray[i];
                                tmpWaterWidth = double.Parse(table.Rows[0].ItemArray[3].ToString());//第4列表示水位宽度
                                double tmpDis = Math.Sqrt((tmpPoint02[0] - tmpPoint01[0]) * (tmpPoint02[0] - tmpPoint01[0]) + (tmpPoint02[1] - tmpPoint01[1]) * (tmpPoint02[1] - tmpPoint01[1]));
                                double tmpSin = (tmpPoint01[1] - tmpPoint02[1]) / tmpDis;
                                double tmpCos = (tmpPoint01[0] - tmpPoint02[0]) / tmpDis;

                                string tmpSingleGeo = "{\"type\": \"Feature\",\"properties\": {\"name\": \"流域2\",\"monitorvalue\": 50.2},\"geometry\": {\"type\": \"Polygon\",\"coordinates\": [ [";
                                string tmpFirst = "[" + tmpPoint03[0].ToString() + "," + tmpPoint03[1].ToString() + "]";
                                tmpSingleGeo += tmpFirst;
                                tmpSingleGeo += ",[" + tmpPoint04[0].ToString() + "," + tmpPoint04[1].ToString() + "]";

                                tmpPoint03[0] = tmpPoint02[0] - tmpWaterWidth * tmpSin;
                                tmpPoint03[1] = tmpPoint02[1] + tmpWaterWidth * tmpCos;
                                tmpPoint04[0] = tmpPoint02[0] + tmpWaterWidth * tmpSin;
                                tmpPoint04[1] = tmpPoint02[1] - tmpWaterWidth * tmpCos;
                                tmpPoint03 = CoordinateConversion.mercatorToLonLat(tmpPoint03);
                                tmpPoint04 = CoordinateConversion.mercatorToLonLat(tmpPoint04);

                                tmpSingleGeo += ",[" + tmpPoint04[0].ToString() + "," + tmpPoint04[1].ToString() + "]";
                                tmpSingleGeo += ",[" + tmpPoint03[0].ToString() + "," + tmpPoint03[1].ToString() + "]";
                                tmpSingleGeo += "," + tmpFirst;

                                tmpSingleGeo += "]]}}";

                                tmpTotalJson += ","+tmpSingleGeo;
                                tmpPoint01 = tmpPoint02;
                            }

                            tmpTotalJson += "]}";
                            sendResponse(context, strFun, tmpTotalJson);
                            return;
                        }
                        else
                            msg = "当前时间没有数据.";
                    }
                    else
                        msg = "河道断面数据不存在.";
                }
                else
                    msg = "时间参数time缺失";
            }
            else if (method == "getriversectionbyid")//根据时间获取当前河道断面GeoJson数据
            {
                string id = context.Request.Params["id"];
                if (id != null)
                {
                    ArrayList tmpArray = GetRiverSectionCoords(context);
                    if (tmpArray != null && tmpArray.Count > 0)
                    {
                        DataTable table = sql.GetStationDataByS(id);
                        if (table != null && table.Rows.Count > 0)
                        {
                            int tmpLen = tmpArray.Count;
                            if (tmpLen > table.Rows.Count)
                                tmpLen = table.Rows.Count;
                            string tmpTotalJson = "{\"type\": \"FeatureCollection\",\"features\":[ ";

                            double tmpWaterWidth = 0;
                            double[] tmpPoint01, tmpPoint02;
                            double[] tmpPoint03, tmpPoint04;
                            tmpPoint01 = (double[])tmpArray[0];

                            //计算第一个点
                            {
                                tmpPoint02 = (double[])tmpArray[1];
                                DataRow tmpRow = table.Rows[0];
                                tmpWaterWidth = double.Parse(tmpRow.ItemArray[3].ToString());//第4列表示水位宽度
                                double tmpDis = Math.Sqrt((tmpPoint02[0] - tmpPoint01[0]) * (tmpPoint02[0] - tmpPoint01[0]) + (tmpPoint02[1] - tmpPoint01[1]) * (tmpPoint02[1] - tmpPoint01[1]));
                                double tmpSin = (tmpPoint01[1] - tmpPoint02[1]) / tmpDis;
                                double tmpCos = (tmpPoint01[0] - tmpPoint02[0]) / tmpDis;
                                tmpPoint03 = new double[2];
                                tmpPoint04 = new double[2];
                                tmpPoint03[0] = tmpPoint01[0] - tmpWaterWidth * tmpSin;
                                tmpPoint03[1] = tmpPoint01[1] + tmpWaterWidth * tmpCos;
                                tmpPoint04[0] = tmpPoint01[0] + tmpWaterWidth * tmpSin;
                                tmpPoint04[1] = tmpPoint01[1] - tmpWaterWidth * tmpCos;
                                tmpPoint03 = CoordinateConversion.mercatorToLonLat(tmpPoint03);
                                tmpPoint04 = CoordinateConversion.mercatorToLonLat(tmpPoint04);

                                string tmpSingleGeo = "{\"type\": \"Feature\",\"properties\": {\"name\": \"断面1\",\"monitorvalue\": " + tmpRow.ItemArray[2].ToString() + ",\"WaterWidth\": " + tmpRow.ItemArray[3].ToString() + "},\"geometry\": {\"type\": \"Polygon\",\"coordinates\": [ [";
                                string tmpFirst = "[" + tmpPoint03[0].ToString() + "," + tmpPoint03[1].ToString() + "]";
                                tmpSingleGeo += tmpFirst;
                                tmpSingleGeo += ",[" + tmpPoint04[0].ToString() + "," + tmpPoint04[1].ToString() + "]";

                                tmpPoint03[0] = tmpPoint02[0] - tmpWaterWidth * tmpSin;
                                tmpPoint03[1] = tmpPoint02[1] + tmpWaterWidth * tmpCos;
                                tmpPoint04[0] = tmpPoint02[0] + tmpWaterWidth * tmpSin;
                                tmpPoint04[1] = tmpPoint02[1] - tmpWaterWidth * tmpCos;
                                tmpPoint03 = CoordinateConversion.mercatorToLonLat(tmpPoint03);
                                tmpPoint04 = CoordinateConversion.mercatorToLonLat(tmpPoint04);

                                tmpSingleGeo += ",[" + tmpPoint04[0].ToString() + "," + tmpPoint04[1].ToString() + "]";
                                tmpSingleGeo += ",[" + tmpPoint03[0].ToString() + "," + tmpPoint03[1].ToString() + "]";
                                tmpSingleGeo += "," + tmpFirst;

                                tmpSingleGeo += "]]}}";

                                tmpTotalJson += tmpSingleGeo;
                                tmpPoint01 = tmpPoint02;
                            }

                            for (int i = 2; i < tmpLen; i++)
                            {
                                tmpPoint02 = (double[])tmpArray[i];
                                tmpWaterWidth = double.Parse(table.Rows[i-1].ItemArray[3].ToString());//第4列表示水位宽度
                                double tmpDis = Math.Sqrt((tmpPoint02[0] - tmpPoint01[0]) * (tmpPoint02[0] - tmpPoint01[0]) + (tmpPoint02[1] - tmpPoint01[1]) * (tmpPoint02[1] - tmpPoint01[1]));
                                double tmpSin = (tmpPoint01[1] - tmpPoint02[1]) / tmpDis;
                                double tmpCos = (tmpPoint01[0] - tmpPoint02[0]) / tmpDis;

                                string tmpSingleGeo = "{\"type\": \"Feature\",\"properties\": {\"name\": \"断面"+i+"\",\"monitorvalue\": "+table.Rows[i-1].ItemArray[2].ToString() + ",\"WaterWidth\": " + table.Rows[i - 1].ItemArray[3].ToString() + "},\"geometry\": {\"type\": \"Polygon\",\"coordinates\": [ [";
                                string tmpFirst = "[" + tmpPoint03[0].ToString() + "," + tmpPoint03[1].ToString() + "]";
                                tmpSingleGeo += tmpFirst;
                                tmpSingleGeo += ",[" + tmpPoint04[0].ToString() + "," + tmpPoint04[1].ToString() + "]";

                                tmpPoint03[0] = tmpPoint02[0] - tmpWaterWidth * tmpSin;
                                tmpPoint03[1] = tmpPoint02[1] + tmpWaterWidth * tmpCos;
                                tmpPoint04[0] = tmpPoint02[0] + tmpWaterWidth * tmpSin;
                                tmpPoint04[1] = tmpPoint02[1] - tmpWaterWidth * tmpCos;
                                tmpPoint03 = CoordinateConversion.mercatorToLonLat(tmpPoint03);
                                tmpPoint04 = CoordinateConversion.mercatorToLonLat(tmpPoint04);

                                tmpSingleGeo += ",[" + tmpPoint04[0].ToString() + "," + tmpPoint04[1].ToString() + "]";
                                tmpSingleGeo += ",[" + tmpPoint03[0].ToString() + "," + tmpPoint03[1].ToString() + "]";
                                tmpSingleGeo += "," + tmpFirst;

                                tmpSingleGeo += "]]}}";

                                tmpTotalJson += "," + tmpSingleGeo;
                                tmpPoint01 = tmpPoint02;
                            }

                            tmpTotalJson += "]}";
                            sendResponse(context, strFun, tmpTotalJson);
                            return;
                        }
                        else
                            msg = "当前时间没有数据.";
                    }
                    else
                        msg = "河道断面数据不存在.";
                }
                else
                    msg = "时间参数S缺失";
            }

            string tmpContent01 = "{success:" + result.ToString().ToLower() + ",msg:" + msg + "}";
            sendResponse(context, strFun, tmpContent01);
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