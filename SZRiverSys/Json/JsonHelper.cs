using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Xml;
namespace SZRiverSys.JSON
{
    public class JsonHelper
    {

        /// <summary>
        /// 将datatable转换成json
        /// </summary>
        /// <returns></returns>
        public string DataTableToJson(DataTable dt)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{\"");
            jsonBuilder.Append(dt.TableName);
            jsonBuilder.Append("\":[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j].ToString());
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]");
            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }

        /// <summary>
        /// 将datatable转换成json
        /// </summary>
        /// <returns></returns>
        public string DataTableToJsonForLayUi(DataTable dt)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j].ToString().Trim() + "\"");
                    jsonBuilder.Append(",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]");
            return jsonBuilder.ToString();
        }

        /// <summary>
        /// 将datatable转换成json
        /// </summary>
        /// <returns></returns>
        public string DataTableToJsondt(DataTable dt)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j].ToString().Trim() + "\"");
                    jsonBuilder.Append(",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            return jsonBuilder.ToString();
        }

        /// <summary>
        /// 将xmlData转换成DataSet
        /// </summary>
        /// <param name="xmlData"></param>
        /// <returns></returns>
        public DataSet ConvertXMLToDataSet(string xmlData)
        {
            StringReader stream = null;
            XmlTextReader reader = null;
            try
            {
                DataSet xmlDS = new DataSet();
                stream = new StringReader(xmlData);
                reader = new XmlTextReader(stream);
                xmlDS.ReadXml(reader);
                return xmlDS;
            }
            catch (Exception ex)
            {
                string strTest = ex.Message;
                return null;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }


        #region 热力图
        public string getRlt(DataTable dt)
        {
            StringBuilder sbu = new StringBuilder();

            string str = "";
            sbu.Append("{");
            sbu.Append("\"type\"");
            sbu.Append(":");
            sbu.Append("\"FeatureCollection\"");
            sbu.Append(",");
            sbu.Append("\"features\"");
            sbu.Append(":");
            sbu.Append("[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                str = dt.Rows[i]["FaceInfo"].ToString();
                if (str != "")
                {
                    str = str.Split(',')[0].ToString() + "," + str.Split(',')[1].ToString();
                }
                else
                {
                    str = -999 + "," + -999;
                }
                sbu.Append("{");
                sbu.Append("\"type\"");
                sbu.Append(":");
                sbu.Append("\"Feature\"");
                sbu.Append(",");
                sbu.Append("\"properties\"");
                sbu.Append(":");
                sbu.Append("{}");
                sbu.Append(",");
                sbu.Append("\"geometry\"");
                sbu.Append(":");
                sbu.Append("{");
                sbu.Append("\"type\"");
                sbu.Append(":");
                sbu.Append("\"Point\"");
                sbu.Append(",");
                sbu.Append("\"coordinates\"");
                sbu.Append(":");
                sbu.Append("[");
                sbu.Append("" + str + "");
                sbu.Append("]");
                sbu.Append("}");
                sbu.Append("}");
                sbu.Append(",");
            }
            sbu.Remove(sbu.Length - 1, 1);
            sbu.Append("]");
            sbu.Append("}");
            return sbu.ToString();
        }
        #endregion
    }
}