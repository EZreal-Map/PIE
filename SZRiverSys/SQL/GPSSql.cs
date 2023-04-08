using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
namespace SZRiverSys.SQL
{
    public class GPSSql
    {
        #region 公共变量
        public string tmpsql;
        DateTime time = DateTime.Now;
        #endregion
        #region
        /// <summary>
        /// 获取用户最新的gps坐标
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public DataTable GetUserGPSNow(string username,Model.UserModel m)
        {
            string where = "1=1 and a.username!='"+m.usercode+"'";
            if(username!="")
            {
                where += " and  a.username='"+username+"'";
            }
            if (m != null)
            {
                if (m.RoleID == 29)
                {
                    where += " and  a.username='" + m.usercode + "'";
                }
            }
            tmpsql = string.Format(@"select a.*,b.userName as RealName,case when datediff(minute,gpsTime,getdate())>5 then '2'else '1' end as state  from [dbo].[UserGPSFinal] a
                                   left join Sys_Accounts_Users b on a.username COLLATE Chinese_PRC_CI_AS =b.usercode  where {0}", where);
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 获取某一用户某一时间段内的坐标
        /// </summary>
        /// <param name="username"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public DataTable GetUserGPSTrack(string username,string starttime,string endtime, string accuracy = "")
        {
            starttime = starttime.Replace(@"/", "-");
            string datetime = starttime.Split(' ')[0];
            var date = datetime.Split('-');
            var year = date[0];
            var month = date[1];
            string database_name = "UserGPS_" + year + month;
            tmpsql = string.Format("select * from [dbo].{0} where username='{1}' and gpsTime between '{2}' and '{3}' ", database_name,username,starttime,endtime);
            if(accuracy != "")
                tmpsql += " and accuracy < "+ accuracy;
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        #endregion
    }
}