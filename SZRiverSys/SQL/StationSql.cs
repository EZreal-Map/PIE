using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
namespace SZRiverSys.SQL
{
    public class StationSql
    {
        JSON.JsonHelper tojson = new JSON.JsonHelper();
        #region 公共参数
        DataTable dt;
        string tmpsql;
        #endregion
        #region 查询监测站信息
        public DataTable GetRTUInfo(string type)
        {
            if (type != "" && type != null)
            {
                string where = " and RTUTYPE="+type+"";
            }
            tmpsql = string.Format(@"select * from PMSDBSZ0403.dbo.WaterCalc_RTU where RTUTYPE>=15 and RTUTYPE<=19 {0}",type);
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 获取所有测站信息(包含预警状态)
        /// </summary>
        /// <returns></returns>
        public DataTable GetStationInfo()
        {
            tmpsql = string.Format("select * from PMSDBSZ.[dbo].[Sys_Station_Info]");
            DataTable dt = new DataTable();
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            DataColumn StationState = new DataColumn("stationstate", typeof(string));
            dt.Columns.Add(StationState);       
            //循环所有测点
            for(int i = 0; i < dt.Rows.Count; i++)
            {
                string state = "正常";            
                string stationid = dt.Rows[i]["STCDT"].ToString();
                //获取当前测站所有监测指标
                DataTable stdt = new DataTable();
                stdt = GetSenSorIndexById(stationid);
                //循环测点下的所有监测指标
                for(int j = 0; j < stdt.Rows.Count; j++)
                {
                    string sendid = stdt.Rows[j]["SENID"].ToString();
                    tmpsql =string.Format(@"select top 1 a.*,b.*,case when  a.V>c.UPLIM then '预警' when a.V<c.DOWNLIM then '预警' else '正常' end as state from PMSDBSZ.[dbo].[WaterCalc_TSDB] a 
                           left join WaterCalc_Sersor b on a.SENID = b.SENID
                           left join PMSDBSZ.[dbo].[WaterCalc_ALDEF]
                           c on a.SENID=c.SENID
                           where a.SENID = '{0}'  order by TIME desc", sendid);
                    DataTable datadt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                    if (datadt.Rows.Count > 0)
                    {
                        if (datadt.Rows[0]["state"].ToString() == "预警")
                        {
                            state = "预警";
                        }
                    }
                }
                DataTable calcdt = GetCaleIndexById(stationid);
                //循环测点下的所有计算指标
                for(int k = 0; k < calcdt.Rows.Count; k++)
                {
                    string sendid = calcdt.Rows[k]["CALCID"].ToString();
                    tmpsql = string.Format(@"select top 1 a.*,b.*,case when  a.V>c.UPLIM then '预警' when a.V<c.DOWNLIM then '预警' else '正常' end as state from PMSDBSZ.[dbo].[WaterCalc_TSDB] a 
                           left join PMSDBSZ.[dbo].[WaterCalc_Calc] b on a.SENID = b.CALCID 
						   left join PMSDBSZ.[dbo].[WaterCalc_ALDEF] c on a.SENID=c.SENID
						   where a.SENID = '{0}'  order by TIME desc", sendid);
                    DataTable datadt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                    if (datadt.Rows.Count > 0)
                    {
                        if (datadt.Rows[0]["state"].ToString() == "预警")
                        {
                            state = "预警";
                        }
                    }
                }
                dt.Rows[i]["stationstate"] = state;
            }
            return dt;
        }
        /// <summary>
        /// 根据站点ID获取该站点的所有监测指标ID及指标名称。
        /// </summary>
        /// <returns></returns>
        public DataTable GetSenSorIndexById(string id)
        {
            tmpsql = string.Format("select * from PMSDBSZ.[dbo].[WaterCalc_Sersor] where  CHARINDEX('{0}',SENID)>0 ", id);// SUBSTRING(CONVERT(CHAR(18),SENID,120),5,3)='{0}'
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 获取站点/断面  下的雨量等指标数据
        /// </summary>
        /// <returns></returns>
        public DataTable GetStationData()
        {
            tmpsql = "select * from PMSDBSZ.[dbo].[Sys_Station_Info] where ELSO!=0";
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            DataColumn StationState = new DataColumn("DataJson", typeof(string));
            dt.Columns.Add(StationState);
            for(int i = 0; i < dt.Rows.Count; i++)
            {
                string sendid = dt.Rows[i]["STCDT"].ToString();
                DataTable paradt = GetSenSorIndexById(sendid);
                string jsondata = "[";
                for(int j = 0; j < paradt.Rows.Count; j++)
                {
                    sendid = paradt.Rows[j]["SENID"].ToString();
                    tmpsql = string.Format(@"  select top 1 * from PMSDBSZ.[dbo].[WaterCalc_Sersor] a
                             left join PMSDBSZ.[dbo].[WaterCalc_TSDB] b on a.SENID=b.SENID where  CHARINDEX('{0}',a.SENID)>0 order by TIME desc",sendid);
                    DataTable datadt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                    if (jsondata != "[")
                    {
                        jsondata += ",";
                    }
                    jsondata += tojson.DataTableToJsondt(datadt);
                    
                }
                jsondata += "]";
                dt.Rows[i]["DataJson"] = jsondata;
            }
            return dt;
        }
        public DataTable GetCalcByName(string name)
        {
            tmpsql = "select * from PMSDBSZ.[dbo].[WaterCalc_Calc] where CHARINDEX('" + name + "',CALCNAME)>0";
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 根据id及类型查询对应得检测指标
        /// </summary>
        /// <param name="senid"></param>
        /// <returns></returns>
        public DataTable GetSensorByName(string senid,string typeid)
        {
            tmpsql = string.Format(" select * from PMSDBSZ.[dbo].[WaterCalc_Sersor] where  CHARINDEX('{0}',SENID)>0 and SENTYPE='{1}' and (SENNAME like '%水位%' or SENNAME like '%位移%' or SENNAME like '%CHL%' or SENNAME like '%TURB%' or SENNAME like '%ORP%' or SENNAME like '%DO%' or SENNAME like '%雨量%')", senid,typeid);// SUBSTRING(CONVERT(CHAR(18),SENID,120),5,3)='{0}' (SENNAME like '%水位%' or SENNAME like '%位移%' or SENNAME like '%CHL%' or SENNAME like '%TURB%' or SENNAME like '%ORP%' or SENNAME like '%DO%' or SENNAME like '%PC%')
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 获取站点/断面  下的指定类型等指标的实时数据数据
        /// </summary>
        /// <returns></returns>
        public DataTable GetStationDataNow(string typeid)
        {
            tmpsql = "select * from PMSDBSZ.[dbo].[Sys_Station_Info] where STCDT in ('10000500','10000300','10000400','10000600')";
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            DataColumn SENDID = new DataColumn("SENID", typeof(string));
            dt.Columns.Add(SENDID);
            DataColumn StationState = new DataColumn("DataJson", typeof(string));
            dt.Columns.Add(StationState);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string stdid = dt.Rows[i]["STCDT"].ToString();
                if (stdid== "10000500")
                {
                    dt.Rows[i]["SENID"] = "105000";
                }
                else if(stdid== "10000300")
                {
                    dt.Rows[i]["SENID"] = "102000";
                }
                else if(stdid== "10000400")
                {
                    dt.Rows[i]["SENID"] = "103000";
                }
                else if (stdid == "10000600")
                {
                    dt.Rows[i]["SENID"] = "104000";
                }
                //拼接指标数据json
                string jsondata = "[";
                //获取站点相关的流量数据
                string calcname = dt.Rows[i]["STNM"].ToString().Replace("断面", "流量");
                DataTable caledt = GetCalcByName(calcname);
                for(int k = 0; k < caledt.Rows.Count; k++)
                {
                    string senid = caledt.Rows[k]["CALCID"].ToString();
                    tmpsql = string.Format(@"select top 1 CALCID,CALCNAME,TIME,V,AVGV,MAXV,MAXT from PMSDBSZ.[dbo].[WaterCalc_Calc] a
                             left join PMSDBSZ.[dbo].[WaterCalc_TSDB] b on a.CALCID=b.SENID where  a.CALCID='{0}' order by TIME desc", senid);
                    DataTable calevdt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                    if (caledt.Rows.Count > 0)
                    {
                        if (jsondata != "[")
                        {
                            jsondata += ",";
                        }
                        jsondata += tojson.DataTableToJsondt(calevdt);
                    }
                }
                jsondata = jsondata.Replace("CALCID", "SENID");
                jsondata = jsondata.Replace("CALCNAME", "SENNAME");
                //根据站点的senid获取相关的监测指标
                string sendid = dt.Rows[i]["SENID"].ToString();
                DataTable paradt = GetSensorByName(sendid, typeid);                
                for (int j = 0; j < paradt.Rows.Count; j++)
                {
                    sendid = paradt.Rows[j]["SENID"].ToString();
                    tmpsql = string.Format(@"  select top 1 a.SENID,a.SENNAME,b.V,b.AVGV,b.MAXV,b.MAXT from PMSDBSZ.[dbo].[WaterCalc_Sersor] a
                             left join PMSDBSZ.[dbo].[WaterCalc_TSDB] b on a.SENID=b.SENID where  CHARINDEX('{0}',a.SENID)>0 order by TIME desc", sendid);
                    DataTable datadt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                    if (datadt.Rows.Count > 0)
                    {
                        if (jsondata != "[")
                        {
                            jsondata += ",";
                        }
                        jsondata += tojson.DataTableToJsondt(datadt);
                    }
                }
                jsondata += "]";
                dt.Rows[i]["DataJson"] = jsondata;
            }
            return dt;
        }
        /// <summary>
        /// 获取指定断面指定类型的监测指标
        /// </summary>
        /// <param name="senid"></param>
        /// <param name="typeid"></param>
        /// <returns></returns>
        public DataTable GetSensorByType(string senid, string typeid)
        {
            tmpsql = string.Format(" select * from [PMSDBSZ].[dbo].[WaterCalc_Sersor] where  CHARINDEX('{0}',SENID)>0 and SENTYPE='{1}'", senid, typeid);
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 获取指定断面的指定类型指标的实时数据
        /// </summary>
        /// <param name="typeid"></param>
        /// <returns></returns>
        public DataTable GetStationSensorDataNow(string typeid)
        {
            tmpsql = "select * from [PMSDBSZ].[dbo].[Sys_Station_Info] where STCDT in ('10000500','10000300','10000400','10000600',10000200)";
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            DataColumn SENDID = new DataColumn("SENID", typeof(string));
            dt.Columns.Add(SENDID);
            DataColumn StationState = new DataColumn("DataJson", typeof(string));
            dt.Columns.Add(StationState);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string stdid = dt.Rows[i]["STCDT"].ToString();
                if (stdid == "10000500")
                {
                    dt.Rows[i]["SENID"] = "104000";
                }
                else if (stdid == "10000300")
                {
                    dt.Rows[i]["SENID"] = "102000";
                }
                else if (stdid == "10000400")
                {
                    dt.Rows[i]["SENID"] = "103000";
                }
                else if (stdid == "10000600")
                {
                    dt.Rows[i]["SENID"] = "105000";
                }
                else if (stdid == "10000200")
                {
                    dt.Rows[i]["SENID"] = "101000";
                }
                //拼接指标数据json
                string jsondata = "[";             
                //根据站点的senid获取相关的监测指标
                string sendid = dt.Rows[i]["SENID"].ToString();
                DataTable paradt = GetSensorByType(sendid, typeid);
                for (int j = 0; j < paradt.Rows.Count; j++)
                {
                    sendid = paradt.Rows[j]["SENID"].ToString();
                    tmpsql = string.Format(@"  select top 1 a.SENID,a.SENNAME,b.V,b.AVGV,b.MAXV,b.MAXT from [PMSDBSZ].[dbo].[WaterCalc_Sersor] a
                             left join [PMSDBSZ].[dbo].[WaterCalc_TSDB] b on a.SENID=b.SENID where  CHARINDEX('{0}',a.SENID)>0 order by TIME desc", sendid);
                    DataTable datadt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                    if (datadt.Rows.Count > 0)
                    {
                        if (jsondata != "[")
                        {
                            jsondata += ",";
                        }
                        jsondata += tojson.DataTableToJsondt(datadt);
                    }
                }
                jsondata += "]";
                dt.Rows[i]["DataJson"] = jsondata;
            }
            return dt;
        }
        /// <summary>
        /// 获取断面的雨量数据
        /// </summary>
        /// <returns></returns>
        public DataTable GetStationRain()
        {
            tmpsql = "select * from [PMSDBSZ].[dbo].[WaterCalc_Sersor] where SENNAME like '%雨量'";
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            DataColumn Value = new DataColumn("Value", typeof(string));
            dt.Columns.Add(Value);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string senid = dt.Rows[i]["SENID"].ToString();
                tmpsql = "select top 1 * from [PMSDBSZ].[dbo].[WaterCalc_TSDB] where SENID='" + senid+"' order by TIME desc";
                DataTable data = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                if (data.Rows.Count > 0) {
                    dt.Rows[i]["Value"] = data.Rows[0]["V"].ToString();                       
                }
            }
            return dt;
        }
        /// <summary>
        /// 根据类型和时间获取站点下的预警数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public DataTable GetStationAlData(string type ,string date)
        {
            tmpsql = "select * from PMSDBSZ.[dbo].[Sys_Station_Info] where STCDT in ('10000500','10000300','10000400','10000600')";
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            DataColumn StationState = new DataColumn("DataJson", typeof(string));
            dt.Columns.Add(StationState);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string stdid = dt.Rows[i]["STCDT"].ToString();
                string stationname = "";
                if (stdid == "10000300")
                {
                    stationname = "横沥口水";
                }
                else if (stdid == "10000400")
                {
                    stationname = "茂仔水水";
                }
                else if (stdid == "10000500")
                {
                    stationname = "赤水洞水水";
                }
                else if (stdid == "10000600")
                {
                    stationname = "坑背河水";
                }
                string jsondata = "[";
                DataTable caledt = GetCalcByName(stationname);
                for(int j = 0; j < caledt.Rows.Count; j++)
                {
                    string where = "1=1";
                    if (type != null && type != "")
                    {
                        where += "and ALTYPE='"+type+"'";
                    }
                    if (date != null && date != "")
                    {
                        where += " and CONVERT(varchar(100), TIME, 23)='"+date+"'";
                    }
                    string senid = caledt.Rows[j]["CALCID"].ToString();
                    tmpsql = string.Format(@"  select top 1 a.CALCID,a.CALCNAME,b.TIME,b.ALTYPE,b.AL_V,b.UPDOWN,b.ALLEVEL from PMSDBSZ.[dbo].[WaterCalc_Calc] a
                             left join PMSDBSZ.[dbo].[WaterCalc_ALARMDB] b on a.CALCID=b.SENID where  CHARINDEX('{0}',a.CALCID)>0  and {1} order by TIME desc", senid,where);
                    DataTable datadt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                    if (datadt.Rows.Count > 0)
                    {
                        if (jsondata != "[")
                        {
                            jsondata += ",";
                        }
                        jsondata += tojson.DataTableToJsondt(datadt);
                    }
                }
                jsondata += "]";
                dt.Rows[i]["DataJson"] = jsondata;
            }
            return dt;
        }
        /// <summary>
        /// 根据时间和指标id查询所有站点的水情水位数据
        /// </summary>
        /// <param name="date"></param>
        /// <param name="senid"></param>
        /// <returns></returns>
        public DataTable GetStationByWR(string date,string typeid)
        {
            tmpsql = string.Format("select * from  [PMSDBSZ].[dbo].[WaterCalc_Calc] where CHARINDEX('220',CALCID)>0 and SUBSTRING(CONVERT(varchar(20), CALCID),7,2)='{0}' order by CALCID asc", typeid);
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            if (dt == null)
                return null;
            DataColumn StationState = new DataColumn("DataJson", typeof(string));
            dt.Columns.Add(StationState);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string where = "";
                string senid = dt.Rows[i]["CALCID"].ToString();
                if (date != "")
                {
                    where += " and TIME='" +date+"' ";
                }
                tmpsql = string.Format(" select top 1 * from  [PMSDBSZ].[dbo].[WaterCalc_TSDB] where SENID='{0}' {1}   order by TIME desc", senid,where);
                DataTable datadt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                if (datadt!=null && datadt.Rows.Count > 0)
                {
                    dt.Rows[i]["DataJson"] = datadt.Rows[0]["V"].ToString();
                }else
                { 
                    dt.Rows[i]["DataJson"] = "0";
                }
            }
            return dt;
        }
        /// <summary>
        /// 根据参数S获取所有站点对应数据
        /// </summary>
        /// <param name="S"></param>
        /// <returns></returns>
        public DataTable GetStationDataByS(string S)
        {
            tmpsql = @"							 select a.*,b.CALCNAME from [dbo].[WaterCalc_TSDB] a 
							 left join [dbo].[WaterCalc_Calc] b on a.SENID=b.CALCID where S='"+S+"'";
            dt= DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 获取断面某小时的所有数据
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string GetStationByHouse(string date,string type)
        {
            tmpsql = string.Format(" select * from PMSDBSZ.[dbo].[WaterCalc_Calc] where CHARINDEX('220',CALCID)>0 and SUBSTRING(CONVERT(varchar(20), CALCID),7,2)='{0}'", type);
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            string datelist = "";
            for(int i = 0; i < dt.Rows.Count; i++)
            {
                string senid = dt.Rows[i]["CALCID"].ToString();
                tmpsql = string.Format(" select * from  PMSDBSZ.[dbo].[WaterCalc_TSDB] where SENID='{0}' and  CONVERT(varchar(100), TIME, 120) like '%{1}%'", senid, date);
                DataTable datadt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                if (datadt.Rows.Count > 0)
                {
                    for (int j = 0; j < datadt.Rows.Count; j++)
                    {
                        string now = datadt.Rows[j]["TIME"].ToString();
                        string datelist0 = now.Replace('/', '-');
                        string datelist1 = datelist0.Split(' ')[0];
                        string[] datelist2 = datelist1.Split('-');
                        string month = datelist2[1];
                        if (int.Parse(datelist2[1]) < 10)
                        {
                            month = "0" + datelist2[1];
                        }
                        string datelists = datelist2[0] + "-" + month + "-" + datelist2[2] + " " + datelist0.Split(' ')[1];
                        if (!datelist.Contains(datelists))
                        {                          
                            datelist += datelists + ";";
                        }
                    }
                }
            }
            datelist= datelist.TrimEnd(';');
            return datelist;
        }
        /// <summary>
        /// 根据站点ID获取该站点的所有计算指标ID及指标名称。
        /// </summary>
        /// <returns></returns>
        public DataTable GetCaleIndexById(string id)
        {
            tmpsql = string.Format("select * from PMSDBSZ.[dbo].[WaterCalc_Calc] where  CHARINDEX('{0}',CALCID)>0 ", id);// SUBSTRING(CONVERT(CHAR(18),CALCID,120),5,3)='{0}'
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 根据站点ID获取该站点所有监测指标的实时数据。
        /// </summary>
        /// <returns></returns>
        public List<Model.StationCalcData> GetAllSensorByid(string id)
        {
            List<Model.StationCalcData> calcM = new List<Model.StationCalcData>();
            tmpsql = string.Format(@"select * from PMSDBSZ.[dbo].[WaterCalc_Sersor] where  CHARINDEX('{0}',SENID)>0 ", id);// SUBSTRING(CONVERT(CHAR(18),SENID,120),5,3)='{0}'
            DataTable dt1 = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            for(int i = 0; i < dt1.Rows.Count; i++)
            {
                Model.StationCalcData m = new Model.StationCalcData();
                string sendid = dt1.Rows[i]["SENID"].ToString();
                tmpsql = @"select top 1 a.*,b.*,case when  a.V>c.UPLIM then '预警' when a.V<c.DOWNLIM then '预警' else '正常' end as state from PMSDBSZ.[dbo].[WaterCalc_TSDB] a 
                           left join WaterCalc_Sersor b on a.SENID = b.SENID
                           left join PMSDBSZ.[dbo].[WaterCalc_ALDEF]
                           c on a.SENID=c.SENID
                           where a.SENID = '"+sendid+"'  order by TIME desc";
                dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                if (dt.Rows.Count > 0)
                {
                    m.sendid = dt.Rows[0]["SENID"].ToString();
                    m.name = dt.Rows[0]["SENNAME"].ToString();
                    m.data = dt.Rows[0]["V"].ToString();
                    m.datetime = dt.Rows[0]["TIME"].ToString();
                    m.state = dt.Rows[0]["state"].ToString();                 
                    calcM.Add(m);
                }
            }
            return calcM;
        }
        /// <summary>
        /// 根据站点ID以及监测指标ID（可一次返回多个）的实时数据。
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sendid"></param>
        /// <returns></returns>
        public List<Model.StationCalcData> GetSensorDataById(string id,string sendid)
        {
            List<Model.StationCalcData> calcM = new List<Model.StationCalcData>();
            string[] idlist = sendid.Split(',');
            for (int i = 0; i < idlist.Count(); i++) {
                Model.StationCalcData m = new Model.StationCalcData();
                tmpsql = string.Format(@"select top 1 * from PMSDBSZ.[dbo].[WaterCalc_TSDB] a 
                           left join WaterCalc_Sersor b on a.SENID = b.SENID where a.SENID = '{0}'  order by TIME desc",idlist[i]);
                dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                if (dt.Rows.Count > 0)
                {
                    m.sendid = dt.Rows[0]["SENID"].ToString();
                    m.name = dt.Rows[0]["SENNAME"].ToString();
                    m.data = dt.Rows[0]["V"].ToString();
                    m.datetime = dt.Rows[0]["TIME"].ToString();
                    calcM.Add(m);
                }

            }
            return calcM;
        }

        #endregion
        #region 获取站点计算指标数值
        /// <summary>
        /// 根据站点ID获取该站点最后的所有计算指标的实时数据。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Model.StationCalcData> GetAllCalcByStationId(string id)
        {
            List<Model.StationCalcData> calcM = new List<Model.StationCalcData>();
            tmpsql = string.Format(@"select * from PMSDBSZ.[dbo].[WaterCalc_Calc] where  CHARINDEX('{0}',CALCID)>0 ", id);
            DataTable dt1 = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                Model.StationCalcData m = new Model.StationCalcData();
                string sendid = dt1.Rows[i]["CALCID"].ToString();
                tmpsql = @"select top 1 a.*,b.*,case when  a.V>c.UPLIM then '预警' when a.V<c.DOWNLIM then '预警' else '正常' end as state from PMSDBSZ.[dbo].[WaterCalc_TSDB] a 
                           left join PMSDBSZ.[dbo].[WaterCalc_Calc] b on a.SENID = b.CALCID 
						   left join PMSDBSZ.[dbo].[WaterCalc_ALDEF] c on a.SENID=c.SENID
						   where a.SENID = '"+sendid+"'  order by TIME desc";
                dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                if (dt.Rows.Count > 0)
                {
                    m.sendid = dt.Rows[0]["SENID"].ToString();
                    m.name = dt.Rows[0]["CALCNAME"].ToString();
                    m.data = dt.Rows[0]["V"].ToString();
                    m.datetime = dt.Rows[0]["TIME"].ToString();
                    m.state= dt.Rows[0]["state"].ToString();
                    calcM.Add(m);
                }
            }
            return calcM;
        }
        /// <summary>
        /// 根据站点ID以及计算指标ID（可一次返回多个）的实时数据。
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sendid"></param>
        /// <returns></returns>
        public List<Model.StationCalcData> GetCalcDataById(string id, string sendid)
        {
            List<Model.StationCalcData> calcM = new List<Model.StationCalcData>();
            string[] idlist = sendid.Split(',');
            for (int i = 0; i < idlist.Count(); i++)
            {
                Model.StationCalcData m = new Model.StationCalcData();
                tmpsql = string.Format(@"select top 1 * from PMSDBSZ.[dbo].[WaterCalc_TSDB] a 
                           left join WaterCalc_Calc b on a.SENID = b.CALCID where a.SENID = '{0}'  order by TIME desc", idlist[i]);
                dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                if (dt.Rows.Count > 0)
                {
                    m.sendid=dt.Rows[0]["SENID"].ToString();
                    m.name = dt.Rows[0]["CALCNAME"].ToString();
                    m.data = dt.Rows[0]["V"].ToString();
                    m.datetime = dt.Rows[0]["TIME"].ToString();
                    calcM.Add(m);
                }

            }
            return calcM;
        }

        #endregion
        /// <summary>
        /// 根据指定的监测指标名称查询所有站点上该指标的实时监测数据。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Model.AllStationOneSensor> GetAllStationByOneSensor(string id)
        {
            List<Model.AllStationOneSensor> ListM = new List<Model.AllStationOneSensor>();
            DataTable stadt = GetStationInfo();
            string stationName;
            string stationid;
            for (int i = 0; i < stadt.Rows.Count; i++)
            {
                //获取监测站编号
                stationid = stadt.Rows[i]["STCDT"].ToString();
                stationName = stadt.Rows[i]["STNM"].ToString();
                
                tmpsql = string.Format(@"select top 1 * from PMSDBSZ.[dbo].[WaterCalc_TSDB] a 
                           left join WaterCalc_Sersor b on a.SENID = b.SENID 
                           where SUBSTRING(CONVERT(CHAR(18),a.SENID,120),5,3)='{0}' and  SUBSTRING(CONVERT(CHAR(18),a.SENID,120),8,3)='{1}' order by TIME desc", stationid,id);
                dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                if (dt.Rows.Count > 0)
                {
                    Model.AllStationOneSensor m = new Model.AllStationOneSensor();
                    m.sendid= dt.Rows[0]["SENID"].ToString();
                    m.StationName =stationName;
                    m.data = dt.Rows[0]["V"].ToString();
                    m.SensorName = dt.Rows[0]["SENNAME"].ToString();
                    m.datetime = dt.Rows[0]["TIME"].ToString();
                    ListM.Add(m);
                }
            }
            return ListM;
        }
        /// <summary>
        /// 根据指定的计算指标名称查询所有站点上该指标的实时计算数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Model.AllStationOneSensor> GetAllStationByOneCalc(string id)
        {
            List<Model.AllStationOneSensor> ListM = new List<Model.AllStationOneSensor>();
            DataTable stadt = GetStationInfo();
            string stationName;
            string stationid;
            for (int i = 0; i < stadt.Rows.Count; i++)
            {
                //获取监测站编号
                stationid = stadt.Rows[i]["STCDT"].ToString();
                stationName = stadt.Rows[i]["STNM"].ToString();
                //拼接指标的编码
                string sendid = "8" + stationid + id;
                tmpsql = string.Format(@"select top 1 * from PMSDBSZ.[dbo].[WaterCalc_TSDB] a 
                           left join WaterCalc_Calc b on a.SENID = b.CALCID 
                           where SUBSTRING(CONVERT(CHAR(18),a.SENID,120),5,3)='{0}' and  SUBSTRING(CONVERT(CHAR(18),a.SENID,120),8,3)='{1}'  order by TIME desc", stationid, id);
                dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
                if (dt.Rows.Count > 0)
                {
                    Model.AllStationOneSensor m = new Model.AllStationOneSensor();
                    m.sendid = dt.Rows[0]["SENID"].ToString();
                    m.StationName = stationName;
                    m.data = dt.Rows[0]["V"].ToString();
                    m.SensorName = dt.Rows[0]["CALCNAME"].ToString();
                    m.datetime = dt.Rows[0]["TIME"].ToString();
                    ListM.Add(m);
                }
            }
            return ListM;
        }
        /// <summary>
        ///根据站点ID、监测指标ID以及开始时间与结束时间获取该站点在指定时间
        /// </summary>
        /// <param name="sendid"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public DataTable GetSenSorDataByTime(string sendid,string starttime,string endtime)
        {
            tmpsql = string.Format(@"select * from PMSDBSZ.[dbo].[WaterCalc_TSDB] a 
                           left join PMSDBSZ.[dbo].[WaterCalc_Sersor] b on a.SENID = b.SENID where a.SENID = '{0}'  and TIME between '{1}' and '{2}'",sendid,starttime,endtime);
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 根据站点ID、计算指标ID以及开始时间与结束时间获取该站点在指定时间
        /// </summary>
        /// <param name="sendid"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public DataTable GetCalcDataByTime(string sendid, string starttime, string endtime)
        {
            tmpsql = string.Format(@"select * from PMSDBSZ.[dbo].[WaterCalc_TSDB] a 
                                     left join PMSDBSZ.[dbo].[WaterCalc_Calc] b on a.SENID = b.CALCID where a.SENID = '{0}'  and TIME between '{1}' and '{2}'",sendid,starttime,endtime);
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }

        /// <summary>
        /// 获取所有设施信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllRtuInfo(string type)
        {
            tmpsql = "select * from PMSDBSZ.[dbo].[WaterCalc_RTU] where ESLO !='' and RTUTYPE='"+ type + "'";
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 获取河道设施类型
        /// </summary>
        /// <returns></returns>
        public DataTable GetRtuType()
        {
            tmpsql = "select * from WaterCalc_RTUTYPE";
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 根据id获取设施名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetRtuTypeById(string id)
        {
            tmpsql = "select * from WaterCalc_RTUTYPE where RTUTYPE='"+id+"'";
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }

        #region 获取预警类型及数据
        /// <summary>
        /// 获取预警类型
        /// </summary>
        /// <returns></returns>
        public DataTable GetAlType()
        {
            tmpsql = "select * from WaterCalc_ALTYPE";
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 根据预警类型及时间查询预警值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public DataTable GetAlDataByType(string type,string starttime,string endtime)
        {
            tmpsql = string.Format("select * from PMSDBSZ.[dbo].[WaterCalc_ALARMDB] WHERE ALTYPE='{0}' and  TIME between '{1}' and '{2}'", type,starttime,endtime);
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 根据指标Id以及时间查询对应值
        /// </summary>
        /// <param name="sendid"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public DataTable GetTSDBByID(string sendid, string starttime, string endtime)
        {
            tmpsql = string.Format("select * from PMSDBSZ.[dbo].[WaterCalc_TSDB] where SENID='{0}' and TIME between '{1}' and '{2}'", sendid, starttime, endtime);
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        #endregion
    }
}