using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
namespace SZRiverSys.SQL
{
    public class WXSql
    {
        #region 公共参数
        DataTable dt;
        string tmpsql;
        #endregion
        #region 查询巡护事件
        public DataTable GetPatrolList(int pageIndex, int pageSize , string Search, string uid,string date, out int total)
        {
            try
            {
                string where = " 1=1";
                if (uid != "" && uid != null)
                {
                    where += " and a.uid='" + uid + "'";
                }
                if (date != "" && date != null)
                {
                    where += " and CONVERT(varchar(10),timecreate, 23) ='"+date+"'";
                }
                //tmpsql = string.Format(@"select * from [dbo].[wxinfopatrols] a
                //                     left join [dbo].[wxusers] b on a.uid=b.uid where {0}", where);
                DataSet ds = new DataSet();
                ds= DBHelper.aliyunDbHelper.GetByPage(@"[dbo].[wxinfopatrols] a
                                                        left join [dbo].[wxusers] b on a.uid=b.uid",//表（可以写关联语句）
                                                        "a.*,b.uname",//查询字段
                                                        "a.Id",
                                                        pageSize,
                                                        pageIndex,
                                                        where, "a.Id", out total);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 添加实时坐标
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool InsertGpsTrack(Model.GPSModel m)
        {
            tmpsql = string.Format(@"INSERT INTO [wxdatabase].[dbo].[wxinfoGPXs] ( [latitude], [longitude], [altitude], [speed], [uid], [datetimelog],[accuracy]) VALUES ( N'{0}', N'{1}', N'{2}', N'{3}', N'{4}', '{6}','{5}');",m.latitude,m.longitude,m.altitude,m.speed,m.uid,m.accuracy,m.datetimelog);
            int count= DBHelper.aliyunDbHelper.ExecuteNonQuery(tmpsql);
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 查询图片地址
        /// </summary>
        public DataTable GetImgById(string id)
        {
            tmpsql = string.Format(@"select photopath from wxinfopatrols where id='{0}'", id);
            dt = DBHelper.aliyunDbHelper.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 获取巡护的历史轨迹
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public DataTable GetPatrolTrack(string uid,string starttime,string endtime)
        {
            tmpsql = string.Format(@"select * from wxinfoGPXs a
                                     left join [dbo].[wxusers] b on a.uid=b.uid where a.uid='{0}' and a.uid!='' and a.uid!='string' and a.latitude!='' and a.latitude!='string' and a.datetimelog BETWEEN '{1}' AND '{2}' and CONVERT(FLOAT,a.accuracy)<50 ORDER BY datetimelog ", uid,starttime,endtime);
            dt = DBHelper.aliyunDbHelper.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 获取某一用户某一天的巡护轨迹
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public DataTable GetPatrolTrackByOneDay(string uid,string date)
        {
            tmpsql = string.Format(@"select * from wxdatabase.dbo.wxinfoGPXs a
                                     left join wxdatabase.[dbo].[wxusers] b on a.uid=b.uid where a.uid='{0}' and a.uid!='' and a.uid!='string' and a.latitude!='' and a.latitude!='string' and CONVERT(nvarchar(10),datetimelog,121)='{1}' and CONVERT(FLOAT,a.accuracy)<50 ORDER BY datetimelog ",uid,date);
            dt = DBHelper.aliyunDbHelper.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 获取所有用户的实时位置
        /// </summary>
        /// <returns></returns>
        public DataTable GetUserGPSData()
        {
            tmpsql = string.Format(@"with
tmpTable
as(
select a.longitude,a.latitude,b.uname,a.datetimelog,b.uid from dbo.wxinfoGPXs a
left join wxusers b on a.uid=b.uid 
where datetimelog = (select max(datetimelog) from dbo.wxinfoGPXs  where uid= a.uid )
)
select max(datetimelog) as datetimelog,case when datediff(minute,max(datetimelog),getdate())>5 then '2'else '1' end as state,longitude,latitude,uid,uname  from tmpTable
group by  datetimelog,longitude,latitude,uid,uname order by uid");
            dt = DBHelper.aliyunDbHelper.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 获取当日的巡河统计
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="uname">巡护人员</param>
        /// <returns></returns>
        public DataTable GetPatrolTotal(string date,string uname)
        {
            tmpsql = string.Format(@"select a.*,b.uname FROM (select uid,MIN(datetimelog) as starttime,MAX(datetimelog) as endtime from wxinfoGPXs WHERE CONVERT(nvarchar(10),datetimelog,121)='{0}' GROUP BY uid )a
                       LEFT JOIN wxusers b on a.uid = b.uid
                       WHERE b.uname like '%{1}%'", date, uname);
            DataTable dt = DBHelper.aliyunDbHelper.ExecuteDataTable(tmpsql);
            DataColumn column = new DataColumn("PatrolLength", typeof(string));
            dt.Columns.Add(column);
            for(int i = 0; i < dt.Rows.Count; i++)
            {
                double Length = 0;
                string uid = dt.Rows[i]["uid"].ToString();
                DataTable trackdt = GetPatrolTrackByOneDay(uid,date);
                for(int j = 0; j < trackdt.Rows.Count-1; j++)
                {
                    double x1 =double.Parse(trackdt.Rows[j]["longitude"].ToString());
                    double y1 =double.Parse(trackdt.Rows[j]["latitude"].ToString());
                    double x2 = double.Parse(trackdt.Rows[j + 1]["longitude"].ToString());
                    double y2 = double.Parse(trackdt.Rows[j + 1]["latitude"].ToString());
                    Length += Comm.PointLength.GetDistance(y1, x1, y2, x2);
                }
                dt.Rows[i]["PatrolLength"] = Length;
            }
            return dt;
        }

        #endregion
        /// <summary>
        /// 获取用户下拉框
        /// </summary>
        /// <returns></returns>
        public DataTable GetUserDDL()
        {
            tmpsql = "select * from [dbo].[wxusers]";
            dt = DBHelper.aliyunDbHelper.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 获取设施维修信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetRtuPatrol()
        {
            tmpsql = "select a.* from [dbo].[wxinfopatrols] a where timecreate = (select max(timecreate) from [dbo].[wxinfopatrols] where RTU = a.RTU) and  DATEDIFF(DAY,timecreate,GETDATE())<7 order by a.RTU";
            dt = DBHelper.aliyunDbHelper.ExecuteDataTable(tmpsql);
            return dt;
        }

    }
}