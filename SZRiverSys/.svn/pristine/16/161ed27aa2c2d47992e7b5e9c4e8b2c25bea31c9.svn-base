using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
namespace SZRiverSys.SQL
{
    public class FileManageSql
    {
        #region 公共变量
        public string tmpsql;
        DataTable dt;
        #endregion

        #region 文档管理
        public DataTable GetFileList(int pageindex, int pagesize,string search, string type, out int total)
        {
            string where = "1=1";
            if (type != "" && type != null)
            {
                where += " and FileType='" + type + "'";
            }
            if (search != "" && search != null)
            {
                where += " and FileName like '%" + search + "%'";
            }
            DataSet ds = new DataSet();
            ds = DBHelper.DBHelperMsSql.GetByPage(@"RM_FileManager",//查询的表
                                                    "*",//显示的字段
                                                    "ID",
                                                    pagesize,
                                                    pageindex,
                                                    where,
                                                    "ID",
                                                    out total);
            return ds.Tables[0];
        }
        /// <summary>
        /// 添加文档信息
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool AddFileInfo(Model.FileManageModel m)
        {
            string tmpsql = string.Format(@"insert into RM_FileManager(FileName,FileDescribe,FileType,FilePath,Creater,CreateTime) values('{0}','{1}','{2}','{3}','{4}',getdate())", m.FileName,m.FileDescribe,m.FileType,m.FilePath,m.Creater);
            int result = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 根据ID获取文档信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public DataTable GetFileInfoByID(string ID)
        {         
            tmpsql = "select * from [dbo].[RM_FileManager] where ID=" + ID + "";
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 修改文档信息
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool EditFileInfo(Model.FileManageModel m)
        {
            string tmpsql = string.Format(@"update [dbo].[RM_FileManager] set FileName='{0}',FileDescribe='{1}',FileType='{2}' where ID={3}",m.FileName,m.FileDescribe,m.FileType,m.ID);
            int res = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
            if (res > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 删除文档信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Del_FileInfo(string id)
        {
            string tmpsql = "delete RM_FileManager where ID in(" + id + ")";
            int res = DBHelper.DBHelperMsSql.ExecuteNonQuery(tmpsql);
            if (res > 0)
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