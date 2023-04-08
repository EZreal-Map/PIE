using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Data.SqlClient;
namespace cn.ctgu.webgis.WebService
{
    public class Config
    {
        public static Hashtable userList = null;
        public static String connString = null;
        public static SqlConnection cn = null;
        public static string manJsonTree = null;   //查询树桩菜单
        public static string autoJsonTree = null;  //查询树桩菜单
        public static string manDataAnsysTree = null;   //数据分析树桩菜单
        public static string autoDataAnsysTree = null;  //数据分析树桩菜单
        public static string manPicAnsysTree = null;    //分布图树桩菜单
        public static string autoPicAnsysTree = null;   //分布图树桩菜单
        public static string manysJsonTree = null;     //异常查询
        public static string manzbJsonTree = null;//异常查询
        public static string manjsJsonTree = null;//异常查询
        public static string autoysJsonTree = null;//异常查询
        public static string autozbJsonTree = null;//异常查询
        public static string autojsJsonTree = null;//异常查询
        public static DataSet InstructBase = null;//异常查询
        public static DataSet InstructInforBase = null;
        public static DataSet InstructLogoBase = null;

        private Config()
        {
        }
        private static void buildUserInfo()
        {
            //2.查询所有的用户数据储存到hash表
            //查询数据库
            SqlDataAdapter MyDataAdapter = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand();
            DataSet ds = new DataSet();
            cn.Open();
            cmd = new SqlCommand("select name,code from accounts", cn);//查询用户表
            MyDataAdapter.SelectCommand = cmd;
            SqlCommandBuilder custCB = new SqlCommandBuilder(MyDataAdapter);
            MyDataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            MyDataAdapter.Fill(ds);
            cn.Close();
            DataTable usertable = ds.Tables[0];
            userList = new Hashtable();
            for (int i = 0; i < usertable.Rows.Count; i++)
            {
                object tempObj = usertable.Rows[i][1];
                string tempPwd = "";
                if (tempObj != null)
                {
                    tempPwd = tempObj.ToString().Trim();
                    int len = tempPwd.Length;
                    if (len < 8)
                        for (int j = 0; j < 8 - len; j++)
                            tempPwd = tempPwd + "0";
                }
                userList.Add(usertable.Rows[i][0].ToString().Trim(), tempPwd);
            }
        }
        //预加载树桩菜单数据
        private static void IniJsonTree()
        {

            //JsonTree数据表生成
            DataSet instrumentds = getData("select * from instrumentBase");
            DataSet Errds = getData("select * from ErrTables");

            JsonTree Jsontree = new JsonTree(instrumentds);
            manJsonTree = Jsontree.GetJsonTree(0);
            autoJsonTree = Jsontree.GetJsonTree(1);
            manDataAnsysTree = Jsontree.GetDataAnsysJsontree(0);
            autoDataAnsysTree = Jsontree.GetDataAnsysJsontree(1);
            manysJsonTree = Jsontree.GetErrDataJsontree(Errds, 0, "原始");
            manjsJsonTree = Jsontree.GetErrDataJsontree(Errds, 0, "技术");
            manzbJsonTree = Jsontree.GetErrDataJsontree(Errds, 0, "整编");
            autoysJsonTree = Jsontree.GetErrDataJsontree(Errds, 1, "原始");
            autojsJsonTree = Jsontree.GetErrDataJsontree(Errds, 1, "技术");
            autozbJsonTree = Jsontree.GetErrDataJsontree(Errds, 1, "整编");
        }

        //预加载数据表
        private static DataSet getData(string sqlStr)
        {
            cn.Open();
            try
            {
                SqlDataAdapter MyDataAdapter = new SqlDataAdapter();
                SqlCommand cmd = new SqlCommand();
                DataSet ds = new DataSet();
                cmd = new SqlCommand(sqlStr, cn);//查询用户表
                MyDataAdapter.SelectCommand = cmd;
                SqlCommandBuilder custCB = new SqlCommandBuilder(MyDataAdapter);
                MyDataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                MyDataAdapter.Fill(ds);
                cn.Close();

                return ds;
            }
            catch
            {
                cn.Close();
                return null;
            }
        }
        //预加载仪器识别符
        private static void getLogoBase()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("jsname", typeof(string));
            dt.Columns.Add("gather", typeof(string));
            dt.Columns.Add("nametable", typeof(string));
            dt.Columns.Add("instrumentname", typeof(string));
            InstructLogoBase = new DataSet();
            InstructLogoBase.Tables.Clear();
            InstructLogoBase.Tables.Add(dt);

            foreach (DataRow dr in InstructBase.Tables[0].Rows)
            {
                string jsname = "";
                for (int i = 1; i <= 5; i++)
                {
                    if (dr["catalog" + i.ToString()].ToString().Trim() != "")
                        jsname += dr["catalog" + i.ToString()].ToString().Trim() + ";";
                }
                if (jsname.Trim() != "")
                {
                    jsname += dr["instrumentname"].ToString();
                    DataRow row = InstructLogoBase.Tables[0].NewRow();
                    row["jsname"] = jsname;
                    row["gather"] = dr["gather"];
                    row["nametable"] = dr["nametable"];
                    row["instrumentname"] = dr["instrumentname"];
                    InstructLogoBase.Tables[0].Rows.Add(row);
                }
            }
        }

        private static void buildConn()
        {
            //1.从web配置文件中读入连接字符串
            connString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
            cn = new SqlConnection(connString);
        }
        public static void InitConfig()
        {
            buildConn();
            buildUserInfo();
            IniJsonTree();

            //加载仪器入库表
            InstructBase = getData("select * from instrumentBase order by number");
            InstructInforBase = getData("select * from instrumentInforBase order by number");

            //预加载树桩菜单识别符
            getLogoBase();
        }
    }
}
