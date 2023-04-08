using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SZRiverSys.Comm
{
    public class CommonClass
    {
        static Dictionary<string, string> m_UsersNameAndCodeDict = new Dictionary<string, string>();

        static Dictionary<string, string> m_UsersCodeAndNameDict = new Dictionary<string, string>();

        public static void InitialUserNameAndCodeDict()
        {
            string tmpSql = "Select usercode,username from Sys_Accounts_Users";
            DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
            if (table != null && table.Rows.Count > 0)
            {
                m_UsersNameAndCodeDict = new Dictionary<string, string>();
                m_UsersCodeAndNameDict = new Dictionary<string, string>();
                foreach (DataRow tmpRow in table.Rows)
                {
                    string tmpCode = tmpRow.ItemArray[0].ToString();
                    string tmpName = tmpRow.ItemArray[1].ToString();
                    if(!m_UsersNameAndCodeDict.ContainsKey(tmpName))
                        m_UsersNameAndCodeDict.Add(tmpName, tmpCode);
                    m_UsersCodeAndNameDict.Add(tmpCode, tmpName);
                }
            }
        }
        /// <summary>
        /// 根据用户昵称获取用户名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetUserCodeByName(string name)
        {
            if (m_UsersNameAndCodeDict.ContainsKey(name))
                return m_UsersNameAndCodeDict[name];
            string tmpSql = "Select usercode from Sys_Accounts_Users Where username='"+ name + "'";
            DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
            if (table != null && table.Rows.Count > 0)
            {
                string tmpResult = table.Rows[0].ItemArray[0].ToString();
                m_UsersNameAndCodeDict.Add(name, tmpResult);
                return tmpResult;
            }
            return null;
        }
        /// <summary>
        /// 根据用户名获取用户昵称
        /// </summary>
        /// <param name="usercode"></param>
        /// <returns></returns>
        public static string GetUserNameByCode(string usercode)
        {
            if (m_UsersCodeAndNameDict.ContainsKey(usercode))
                return m_UsersCodeAndNameDict[usercode];
            string tmpSql = "Select username from Sys_Accounts_Users Where usercode='" + usercode + "'";
            DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
            if (table != null && table.Rows.Count > 0)
            {
                string tmpResult = table.Rows[0].ItemArray[0].ToString();
                m_UsersCodeAndNameDict.Add(usercode, tmpResult);
                return tmpResult;
            }
            return null;
        }

        /// <summary>
        /// 根据河道id获取河道名称
        /// </summary>
        /// <param name="usercode"></param>
        /// <returns></returns>
        public static string GetRiverNameById(string BelongSectionID)
        {
            if (m_UsersCodeAndNameDict.ContainsKey(BelongSectionID))
                return m_UsersCodeAndNameDict[BelongSectionID];
            string tmpSql = "select sectionname from X_ProjectSectionInfo where sectionid = '" + BelongSectionID + "'";
            DataTable table = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
            if (table != null && table.Rows.Count > 0)
            {
                string tmpResult = table.Rows[0].ItemArray[0].ToString();
                m_UsersCodeAndNameDict.Add(BelongSectionID, tmpResult);
                return tmpResult;
            }
            return null;
        }

        /// <summary>
        /// 替换Table中某列数据为用户昵称
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        public static void ReplaceTableColumnName(DataTable table, string columnName)
        {
            int tmpColIndex = table.Columns.IndexOf(columnName);
            if(tmpColIndex>-1)
            {
                foreach(DataRow tmpRow in table.Rows)
                {
                    string tmpStr = tmpRow.ItemArray[tmpColIndex].ToString();
                    string tmpStr2 = GetUserNameByCode(tmpStr);
                    if (tmpStr2 != null)
                        tmpRow[tmpColIndex] = tmpStr2;
                    //tmpRow.ItemArray[tmpColIndex] = tmpStr2;
                }
            }
        }

        /// <summary>
        /// 替换Table中某列数据为河道名称
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        public static void ReplaceTableRiverName(DataTable table, string BelongSectionID)
        {
            int tmpColIndex = table.Columns.IndexOf(BelongSectionID);
            if (tmpColIndex > -1)
            {
                foreach (DataRow tmpRow in table.Rows)
                {
                    string tmpStr = tmpRow.ItemArray[tmpColIndex].ToString();
                    string tmpStr2 = GetRiverNameById(tmpStr);
                    if (tmpStr2 != null)
                        tmpRow[tmpColIndex] = tmpStr2;
                    //tmpRow.ItemArray[tmpColIndex] = tmpStr2;
                }
            }
        }


        public static void ReplaceTableColumnsName(DataTable table, string[] columnNames)
        {
            int[] tmpColIndexs = new int[columnNames.Length];
            bool tmpBool = false;
            int tmpTid = 0;
            foreach(string tmpcolumnName in columnNames)
            {
                int tmpI = table.Columns.IndexOf(tmpcolumnName);
                if (tmpI > -1)
                {
                    tmpColIndexs[tmpTid++] = tmpI;
                    tmpBool = true;
                }
            }            
            if (tmpBool)
            {
                foreach (DataRow tmpRow in table.Rows)
                {
                    foreach(int tmpI in tmpColIndexs)
                    {
                        string tmpStr = tmpRow.ItemArray[tmpI].ToString();
                        if(tmpStr!="")
                        {
                            string[] tmpStrs001 = tmpStr.Split(',');
                            if(tmpStrs001!=null && tmpStrs001.Length > 0)
                            {
                                string tmpStrResult = "";
                                foreach(string tmpStr001 in tmpStrs001)
                                {
                                    if(tmpStr001.Length > 0)
                                    {
                                        if (tmpStrResult.Length > 0)
                                            tmpStrResult += ",";
                                        string tmpStr2 = GetUserNameByCode(tmpStr001);
                                        if (tmpStr2 != null)
                                            tmpStrResult += tmpStr2;
                                        else
                                            tmpStrResult += tmpStr001;
                                    }
                                }
                                tmpRow[tmpI] = tmpStrResult;
                            }
                            

                        }
                        
                    }
                }
            }
        }
        /// <summary>
        /// 替换字段的值为数据字典对应值
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        /// <param name="dictValue"></param>
        public static void ReplaceTableColumnsValue(DataTable table, string orgcolumnName, string newColumnName, Dictionary<string,string> dictValue)
        {
            int tmpI = table.Columns.IndexOf(orgcolumnName);
            if (tmpI > -1)
            {
                DataColumn tmpColumn = table.Columns.Add(newColumnName, typeof(string));
                int tmpJ = tmpColumn.Ordinal;
                foreach (DataRow tmpRow in table.Rows)
                {
                    string tmpStr = tmpRow.ItemArray[tmpI].ToString();
                    if(dictValue.ContainsKey(tmpStr))
                    {
                        string tmpValue = dictValue[tmpStr];
                        tmpRow[tmpJ] = tmpValue;
                    }
                }
            }
        }
    }
}