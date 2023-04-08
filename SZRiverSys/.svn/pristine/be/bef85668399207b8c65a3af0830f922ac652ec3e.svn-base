using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace SZRiverSys.DBHelper
{
    public class aliyunDbHelper
    {
        public static readonly string connectionString = ConfigurationManager.ConnectionStrings["aliyunserver"].ConnectionString;

        #region ExecuteNonQuery命令
        /// <summary>  
        /// 对数据库执行增、删、改命令  
        /// </summary>  
        /// <param name="safeSql">T-Sql语句</param>  
        /// <returns>受影响的记录数</returns>  
        public static int ExecuteNonQuery(string safeSql)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();
                SqlTransaction trans = Connection.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand(safeSql, Connection);
                    cmd.Transaction = trans;

                    if (Connection.State != ConnectionState.Open)
                    {
                        Connection.Open();
                    }
                    int result = cmd.ExecuteNonQuery();
                    trans.Commit();
                    return result;
                }
                catch
                {
                    trans.Rollback();
                    return 0;
                }
            }
        }
        /// <summary>
        /// 执行多条增、删、改命令,一条失败则全部回滚
        /// </summary>
        /// <param name="listSql">sql命令集合</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(List<string> listSql)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();
                SqlTransaction trans = Connection.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Transaction = trans;
                    cmd.Connection = Connection;

                    if (Connection.State != ConnectionState.Open)
                    {
                        Connection.Open();
                    }
                    int result = 0;
                    //循环读取Sql语句
                    for (int i = 0; i < listSql.Count; i++)
                    {
                        cmd.CommandText = listSql[i];
                        result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            trans.Commit();
                        }
                        else
                        {
                            trans.Rollback();
                        }
                    }
                    return result;
                }
                catch
                {
                    trans.Rollback();
                    return 0;
                }
            }
        }

        /// <summary>  
        /// 对数据库执行增、删、改命令  
        /// </summary>  
        /// <param name="sql">T-Sql语句</param>  
        /// <param name="values">参数数组</param>  
        /// <returns>受影响的记录数</returns>  
        public static int ExecuteNonQuery(string sql, SqlParameter[] values)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();
                SqlTransaction trans = Connection.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand(sql, Connection);
                    cmd.Transaction = trans;
                    cmd.Parameters.AddRange(values);
                    if (Connection.State != ConnectionState.Open)
                    {
                        Connection.Open();
                    }
                    int result = cmd.ExecuteNonQuery();
                    trans.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return 0;
                }
            }
        }
        #endregion

        #region ExecuteScalar命令
        /// <summary>  
        /// 查询结果集中第一行第一列的值  
        /// </summary>  
        /// <param name="safeSql">T-Sql语句</param>  
        /// <returns>第一行第一列的值</returns>  
        public static int ExecuteScalar(string safeSql)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                SqlCommand cmd = new SqlCommand(safeSql, Connection);
                int result = Convert.ToInt32(cmd.ExecuteScalar());
                return result;
            }
        }

        /// <summary>  
        /// 查询结果集中第一行第一列的值  
        /// </summary>  
        /// <param name="sql">T-Sql语句</param>  
        /// <param name="values">参数数组</param>  
        /// <returns>第一行第一列的值</returns>  
        public static int ExecuteScalar(string sql, SqlParameter[] values)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                SqlCommand cmd = new SqlCommand(sql, Connection);
                cmd.Parameters.AddRange(values);
                int result = Convert.ToInt32(cmd.ExecuteScalar());
                return result;
            }
        }
        #endregion

        #region ExecuteReader命令
        /// <summary>  
        /// 创建数据读取器  
        /// </summary>  
        /// <param name="safeSql">T-Sql语句</param>  
        /// <param name="Connection">数据库连接</param>  
        /// <returns>数据读取器对象</returns>  
        public static SqlDataReader ExecuteReader(string safeSql, SqlConnection Connection)
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();
            SqlCommand cmd = new SqlCommand(safeSql, Connection);
            SqlDataReader reader = cmd.ExecuteReader();
            return reader;
        }

        /// <summary>  
        /// 创建数据读取器  
        /// </summary>  
        /// <param name="sql">T-Sql语句</param>  
        /// <param name="values">参数数组</param>  
        /// <param name="Connection">数据库连接</param>  
        /// <returns>数据读取器</returns>  
        public static SqlDataReader ExecuteReader(string sql, SqlParameter[] values, SqlConnection Connection)
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();
            SqlCommand cmd = new SqlCommand(sql, Connection);
            cmd.Parameters.AddRange(values);
            SqlDataReader reader = cmd.ExecuteReader();
            return reader;
        }
        #endregion

        #region ExecuteDataTable命令
        /// <summary>  
        /// 执行指定数据库连接对象的命令,指定存储过程参数,返回DataTable  
        /// </summary>  
        /// <param name="type">命令类型(T-Sql语句或者存储过程)</param>  
        /// <param name="safeSql">T-Sql语句或者存储过程的名称</param>  
        /// <param name="values">参数数组</param>  
        /// <returns>结果集DataTable</returns>  
        public static DataTable ExecuteDataTable(CommandType type, string safeSql, params SqlParameter[] values)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand(safeSql, Connection);
                cmd.CommandType = type;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                return ds.Tables[0];
            }
        }

        /// <summary>  
        /// 执行指定数据库连接对象的命令,指定存储过程参数,返回DataTable  
        /// </summary>  
        /// <param name="safeSql">T-Sql语句</param>  
        /// <returns>结果集DataTable</returns>  
        public static DataTable ExecuteDataTable(string safeSql)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand(safeSql, Connection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                try
                {
                    da.Fill(ds);
                }
                catch (Exception ex)
                {

                }
                return ds.Tables[0];
            }
        }

        /// <summary>  
        /// 执行指定数据库连接对象的命令,指定存储过程参数,返回DataSet  
        /// </summary>  
        /// <param name="safeSql">T-Sql语句</param>  
        /// <returns>结果集DataTable</returns>  
        public static DataSet ExecuteDataSet(string safeSql)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand(safeSql, Connection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                try
                {
                    da.Fill(ds);
                }
                catch (Exception ex)
                {

                }
                return ds;
            }
        }

        /// <summary>  
        /// 执行指定数据库连接对象的命令,指定存储过程参数,返回DataTable  
        /// </summary>  
        /// <param name="sql">T-Sql语句</param>  
        /// <param name="values">参数数组</param>  
        /// <returns>结果集DataTable</returns>  
        public static DataTable ExecuteDataTable(string sql, params SqlParameter[] values)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand(sql, Connection);
                cmd.CommandTimeout = 0;
                cmd.Parameters.AddRange(values);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                return ds.Tables[0];
            }
        }
        #endregion

        #region GetDataSet命令
        /// <summary>  
        /// 取出数据  
        /// </summary>  
        /// <param name="safeSql">sql语句</param>  
        /// <param name="tabName">DataTable别名</param>  
        /// <param name="values"></param>  
        /// <returns></returns>  
        public static DataSet GetDataSet(string safeSql, string tabName, params SqlParameter[] values)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand(safeSql, Connection);

                if (values != null)
                    cmd.Parameters.AddRange(values);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                try
                {
                    da.Fill(ds, tabName);
                }
                catch (Exception ex)
                {

                }
                return ds;
            }
        }
        #endregion

        #region ExecureData 命令
        /// <summary>  
        /// 批量修改数据  
        /// </summary>  
        /// <param name="ds">修改过的DataSet</param>  
        /// <param name="strTblName">表名</param>  
        /// <returns></returns>  
        public static int ExecureData(DataSet ds, string strTblName)
        {
            try
            {
                //创建一个数据库连接  
                using (SqlConnection Connection = new SqlConnection(connectionString))
                {
                    if (Connection.State != ConnectionState.Open)
                        Connection.Open();

                    //创建一个用于填充DataSet的对象  
                    SqlCommand myCommand = new SqlCommand("SELECT * FROM " + strTblName, Connection);
                    SqlDataAdapter myAdapter = new SqlDataAdapter();
                    //获取SQL语句，用于在数据库中选择记录  
                    myAdapter.SelectCommand = myCommand;

                    //自动生成单表命令，用于将对DataSet所做的更改与数据库更改相对应  
                    SqlCommandBuilder myCommandBuilder = new SqlCommandBuilder(myAdapter);

                    return myAdapter.Update(ds, strTblName);  //更新ds数据  
                }

            }
            catch (Exception err)
            {
                throw err;
            }
        }

        #endregion

        #region 插入大量数据
        /// <summary>
        /// 插入多条数据【适用于百万数据】
        /// </summary>
        /// <param name="connectionString">目标连接字符</param>
        /// <param name="TableName">目标表</param>
        /// <param name="dt">源数据</param>
        public static void SqlBulkCopyByDatatable(string TableName, DataTable dt)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlBulkCopy sqlbulkcopy = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.UseInternalTransaction))
                {
                    try
                    {
                        sqlbulkcopy.DestinationTableName = TableName;
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            sqlbulkcopy.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                        }
                        sqlbulkcopy.WriteToServer(dt);
                    }
                    catch (System.Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }
        #endregion

        #region 执行分页存储过程

        /// <summary>
        /// 执行分页查询
        /// </summary>
        /// <param name="tblName">查询的表名</param>
        /// <param name="PageSize">分页大小</param>
        /// <param name="PageIndex">当前页码</param>
        /// <param name="strWhere">查询条件</param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static DataSet GetRecordByPage(string tblName, int PageSize, int PageIndex, string strWhere, out int count)
        {
            return GetRecordByPage(tblName, "*", "ID", PageSize, PageIndex, strWhere, out count);
        }

        public static DataSet GetRecordByPage(string tblName, string strGetFields, string fldName, int PageSize, int PageIndex, string strWhere, out int count)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    SetParams(cmd);

                    cmd.Parameters[0].Value = tblName;
                    cmd.Parameters[1].Value = strGetFields;
                    cmd.Parameters[2].Value = fldName;
                    cmd.Parameters[3].Value = PageSize;
                    cmd.Parameters[4].Value = PageIndex;
                    cmd.Parameters[5].Value = 0;
                    cmd.Parameters[6].Value = strWhere;

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "p_Page";
                    cmd.CommandTimeout = 180;

                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = cmd;

                    DataSet source = new DataSet();
                    adapter.Fill(ds);
                    //ds.Tables.RemoveAt(0);  

                    object o = cmd.Parameters["@total"].Value;
                    count = (o == null || o == DBNull.Value) ? 0 : System.Convert.ToInt32(o);
                }
            }
            catch (SqlException e)
            {
                throw e;
            }
            return ds;
        }

        private static void SetParams(SqlCommand cmd)
        {
            cmd.Parameters.Add(new SqlParameter("@tblName", SqlDbType.VarChar, 255));
            cmd.Parameters.Add(new SqlParameter("@strGetFields", SqlDbType.VarChar, 1000));
            cmd.Parameters.Add(new SqlParameter("@fldName", SqlDbType.VarChar, 255));
            cmd.Parameters.Add(new SqlParameter("@PageSize", SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@PageIndex", SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@OrderType", SqlDbType.Bit));
            cmd.Parameters.Add(new SqlParameter("@strWhere", SqlDbType.VarChar, 1500));

            SqlParameter param = new SqlParameter("@total", SqlDbType.Int);
            param.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(param);
        }
        #endregion

        #region 支持多表查询分页存储过程
        /// <summary>
        /// 多表查询分页存储过程
        /// </summary>
        /// <param name="tblName">表名（例如：AAA a inner join BBB b  on a.id=b.id）</param>
        /// <param name="strGetFields">查出来的字段显示</param>
        /// <param name="fldName">主键名字</param>
        /// <param name="PageSize">一共显示多少行</param>
        /// <param name="PageIndex">当前页码</param>
        /// <param name="strWhere">条件</param>
        /// <param name="sort">排序字段</param>
        /// <param name="count">总行数</param>
        /// <returns></returns>
        public static DataSet GetByPage(string tblName, string strGetFields, string fldName, int PageSize, int PageIndex, string strWhere, string sort, out int count)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    SetParam(cmd);

                    cmd.Parameters[0].Value = tblName;
                    cmd.Parameters[1].Value = PageSize;
                    cmd.Parameters[2].Value = PageIndex;
                    cmd.Parameters[3].Value = strGetFields;
                    cmd.Parameters[4].Value = sort;
                    cmd.Parameters[5].Value = fldName;
                    cmd.Parameters[6].Value = strWhere;

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "pro_sys_GetRecordByPage";
                    cmd.CommandTimeout = 180;

                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = cmd;

                    DataSet source = new DataSet();
                    adapter.Fill(ds);
                    //ds.Tables.RemoveAt(0);  

                    object o = cmd.Parameters["@RecordCount"].Value;
                    count = (o == null || o == DBNull.Value) ? 0 : System.Convert.ToInt32(o);
                }
            }
            catch (SqlException e)
            {
                throw e;
            }
            return ds;
        }

        private static void SetParam(SqlCommand cmd)
        {
            cmd.Parameters.Add(new SqlParameter("@Source", SqlDbType.VarChar, 1500));
            cmd.Parameters.Add(new SqlParameter("@PageSize", SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@CurrentPage", SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@FieldList", SqlDbType.VarChar, 1000));
            cmd.Parameters.Add(new SqlParameter("@Sort", SqlDbType.VarChar, 255));
            cmd.Parameters.Add(new SqlParameter("@FdName", SqlDbType.VarChar, 1500));
            cmd.Parameters.Add(new SqlParameter("@Where", SqlDbType.VarChar, 1500));

            SqlParameter param = new SqlParameter("@RecordCount", SqlDbType.Int);
            param.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(param);
        }
        #endregion
    }
}