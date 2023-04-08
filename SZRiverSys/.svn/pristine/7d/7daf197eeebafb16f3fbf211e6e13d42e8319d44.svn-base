using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace SZRiverSys.SQL
{
    public class PatrolSql
    {
        #region 公共变量
        public string tmpsql;
        DataTable dt;
        #endregion
        #region 巡线管理
        /// <summary>
        /// 巡护列表（分页）
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="Search"></param>
        /// <param name="uid"></param>
        /// <param name="date"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public DataTable GetPatrolList(int pageIndex, int pageSize, string Type,string date, out int total)
        {
            try
            {
                string where = " 1=1";
                if (Type != "" && Type != null)
                {
                    where += " and a.Type='" + Type + "'";
                }
                if (date != "" && date != null)
                {
                    where += " and CONVERT(varchar(10),timecreate, 23) ='" + date + "'";
                }
                DataSet ds = new DataSet();
                ds = DBHelper.DBHelperMsSql.GetByPage(@"[dbo].[RM_PatrolInfo] a
                                                        left join [dbo].[Sys_Accounts_Users] b on a.uid=b.UserID",//表（可以写关联语句）
                                                        "a.*,b.userName",//查询字段
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
        /// 获取巡护事件图层
        /// </summary>
        /// <returns></returns>
        public DataTable GetPatrolData()
        {
            tmpsql = string.Format("select * from [dbo].[RM_PatrolInfo]  where  DATEDIFF(DAY,timecreate,GETDATE()) <7");
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 添加巡护事件
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool AddPatrolInfo(Model.PatrolInfo m)
        {
            tmpsql = string.Format(@"insert into [dbo].[RM_PatrolInfo](uid,Type,timecreate,infomemo,photopath,longitude,addressinfo,latitude,RTU) 
                                    values('{0}','{1}',GETDATE(),'{2}','{3}','{4}','{5}','{6}','{7}');",m.uid,m.Type,m.infomemo,m.photopath,m.longitude,m.addressinfo,m.latitude,m.RTU);
            int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
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
        /// 获取当日的巡河统计
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="uname">巡护人员</param>
        /// <returns></returns>
        public DataTable GetPatrolTotal(string date, string uname)
        {
            var time = date.Split('-');
            string datemonth = time[0] + time[1];
            tmpsql = string.Format(@"select a.*,b.userName as userRealName FROM (select username,MIN(gpsTime) as starttime,MAX(gpsTime) as endtime from [dbo].UserGPS_{2} WHERE CONVERT(nvarchar(10),gpsTime,121)='{0}' GROUP BY username) a
                       LEFT JOIN Sys_Accounts_Users b on a.username COLLATE Chinese_PRC_CI_AS = b.usercode
                       WHERE b.userName like '%{1}%'", date, uname,datemonth);
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            DataColumn column = new DataColumn("PatrolLength", typeof(string));
            dt.Columns.Add(column);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                double Length = 0;
                string uid = dt.Rows[i]["username"].ToString();
                DataTable trackdt = GetPatrolTrackByOneDay(uid, date);
                for (int j = 0; j < trackdt.Rows.Count - 1; j++)
                {
                    double x1 = double.Parse(trackdt.Rows[j]["longitude"].ToString());
                    double y1 = double.Parse(trackdt.Rows[j]["latitude"].ToString());
                    double x2 = double.Parse(trackdt.Rows[j + 1]["longitude"].ToString());
                    double y2 = double.Parse(trackdt.Rows[j + 1]["latitude"].ToString());
                    Length += Comm.PointLength.GetDistance(y1, x1, y2, x2);
                }
                dt.Rows[i]["PatrolLength"] =(int)Length;
            }
            return dt;
        }
        /// <summary>
        /// 获取某一用户某一天的巡护轨迹
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public DataTable GetPatrolTrackByOneDay(string username, string date)
        {
            var time = date.Split('-');
            string datemonth = time[0] + time[1];
            tmpsql = string.Format(@"select * from [dbo].UserGPS_{0} a
                                     left join [dbo].Sys_Accounts_Users b on a.username COLLATE Chinese_PRC_CI_AS=b.usercode where a.username='{1}' and CONVERT(nvarchar(10),gpsTime,121)='{2}' and CONVERT(FLOAT,a.accuracy)<100 ORDER BY gpsTime ", datemonth, username, date);
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        #endregion
        #region 巡线计划管理
        /// <summary>
        /// 获取自己的巡线计划
        /// </summary>
        /// <returns></returns>
        public DataTable GetMyPatrolPlan(int pageIndex, int pageSize,string UserId,string RoleID, out int total)
        {
            try
            {
                string where = " 1=1";
                if (int.Parse(RoleID) > 2)
                {
                    where += "  and a.UserId='" + UserId + "'";
                }
                DataSet ds = new DataSet();
                ds = DBHelper.DBHelperMsSql.GetByPage(@"[dbo].[RM_PatrolPlan] a
                                                        left join Sys_Accounts_Users b on a.UserId=b.UserID",//表（可以写关联语句）
                                                        "a.*,b.userName",//查询字段
                                                        "ID",
                                                        pageSize,
                                                        pageIndex,
                                                        where, "ID", out total);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 创建巡线计划
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool CreatePatrolPlan(Model.PatrolPlan m)
        {
            tmpsql = string.Format("insert into [dbo].[RM_PatrolPlan](UserId,PatrolRank,PatrolContent,PlanStartDate,PatrolLength,CreateTime,PlanEndDate) values('{0}','{1}','{2}','{3}','{4}',GETDATE(),'{5}');", m.UserId,m.PatrolRank,m.PatrolContent,m.PlanStartDate,m.PatrolLength,m.PlanEndDate);
            int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
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
        /// 删除巡线计划
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool DelPatrolPlan(string ID)
        {
            tmpsql = "delete [dbo].[RM_PatrolPlan] where ID in(" + ID + ")";
            int count = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}