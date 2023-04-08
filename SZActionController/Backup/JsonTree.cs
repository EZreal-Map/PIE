using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Data.Sql;
using System.Linq;
using System.IO;

namespace cn.ctgu.webgis.WebService
{
    public class JsonTree
    {
        DataSet ds_Json = null;
        DataSet Errds_Json = null;
        public JsonTree(DataSet ds)
        {
            this.ds_Json = ds;
        }

        /// <summary>
        /// 查询树桩菜单Json文件生成
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="catalog"></param>
        public string GetJsonTree(int catalog)
        {
            return GetJson(ds_Json, catalog);
        }

        /// <summary>
        /// 变位图Json文件生成
        /// </summary>
        public JsonTree()
        {
            if (ds_Json == null || ds_Json.Tables.Count == 0)
                return;

            //生成树桩菜单Json
            JsonString totalString = new JsonString();
            var list = ds_Json.Tables[0].AsEnumerable().ToList();

            foreach (var v1 in list.ToList().FindAll(p0 => p0["catalog1"].ToString().Trim() != "").GroupBy(p => p.Field<string>("catalog1")))
            {
                totalString.Text = v1.Key;
                foreach (var v2 in v1.GroupBy(p => p.Field<string>("catalog2")))
                {
                    if (v2.Key != null)
                    {
                        JsonString tempJT02 = new JsonString();
                        tempJT02.Text = v2.Key;
                        foreach (var v3 in v2.GroupBy(p => p.Field<string>("catalog3")))
                        {
                            if (v3.Key != null)
                            {
                                JsonString tempJT03 = new JsonString();
                                tempJT03.Text = v3.Key;
                                foreach (var v4 in v3.GroupBy(p => p.Field<string>("catalog4")))
                                {
                                    if (v4.Key != null)
                                    {
                                        JsonString tempJT04 = new JsonString();
                                        tempJT04.Text = v4.Key;
                                        tempJT03.AppendChild(tempJT04);
                                    }
                                }
                                tempJT02.AppendChild(tempJT03);
                            }
                        }
                        totalString.AppendChild(tempJT02);
                    }
                }
            }
            totalString.ConvertToJson(1);
        }

        /// <summary>
        /// 数据分析树桩菜单Json文件
        /// </summary>
        /// <param name="catalog"></param>
        /// <returns></returns>
        public string GetDataAnsysJsontree(int catalog)
        {
            //判定树桩菜单格式
            string statss = "";
            if (catalog == 0) //人工单点
            {
                statss = "人工";
            }
            else if (catalog == 1) //自动化单点
            {
                statss = "自动化";
            }

            //生成树桩菜单Json
            JsonString totalString = new JsonString();
            var list = ds_Json.Tables[0].AsEnumerable().ToList().FindAll(p => p["gather"].ToString().Trim().Contains(statss));

            foreach (var v1 in list.ToList().FindAll(p0 => p0["catalog1"].ToString().Trim() != "").GroupBy(p => p.Field<string>("catalog1")))
            {
                JsonString tempJT01 = new JsonString();
                tempJT01.Text = v1.Key;
                foreach (var v2 in v1.GroupBy(p => p.Field<string>("catalog2")))
                {
                    if (v2.Key != null)
                    {
                        JsonString tempJT02 = new JsonString();
                        tempJT02.Text = v2.Key;
                        foreach (var v3 in v2.GroupBy(p => p.Field<string>("catalog3")))
                        {
                            if (v3.Key != null)
                            {
                                JsonString tempJT03 = new JsonString();
                                tempJT03.Text = v3.Key;
                                foreach (var v4 in v3.GroupBy(p => p.Field<string>("catalog4")))
                                {
                                    if (v4.Key != null)
                                    {
                                        JsonString tempJT04 = new JsonString();
                                        tempJT04.Text = v4.Key;
                                        foreach (var v5 in v4.GroupBy(p => p.Field<string>("catalog5")))
                                        {
                                            if (v5.Key != null)
                                            {
                                                JsonString tempJT05 = new JsonString();
                                                tempJT05.Text = v5.Key;
                                                foreach (DataRow v6 in v5.ToList())
                                                {
                                                    if (v6["instrumentname"] != null)
                                                    {
                                                        JsonString tempJT06 = new JsonString();
                                                        tempJT06.Text = v6["instrumentname"].ToString();
                                                        tempJT06.id = v6["number"].ToString();
                                                        tempJT06.IsLeaf = true;
                                                        tempJT06.IconCls = "myicon5";
                                                        tempJT05.AppendChild(tempJT06);
                                                    }
                                                }
                                                tempJT04.AppendChild(tempJT05);
                                            }
                                            else
                                            {
                                                foreach (DataRow v in v5.ToList().FindAll(p => p["instrumentname"].ToString().Trim() != ""))
                                                {
                                                    JsonString tempJT05 = new JsonString();
                                                    tempJT05.Text = v["instrumentname"].ToString();
                                                    tempJT05.id = v["number"].ToString();
                                                    tempJT05.IsLeaf = true;
                                                    tempJT05.IconCls = "myicon5";
                                                    tempJT04.AppendChild(tempJT05);
                                                }
                                            }
                                        }
                                        tempJT03.AppendChild(tempJT04);
                                    }
                                    else
                                    {
                                        foreach (DataRow v in v4.ToList().FindAll(p => p["instrumentname"].ToString().Trim() != ""))
                                        {
                                            JsonString tempJT04 = new JsonString();
                                            tempJT04.Text = v["instrumentname"].ToString();
                                            tempJT04.id = v["number"].ToString();
                                            tempJT04.IsLeaf = true;
                                            tempJT04.IconCls = "myicon5";
                                            tempJT03.AppendChild(tempJT04);
                                        }
                                    }
                                }
                                tempJT02.AppendChild(tempJT03);
                            }
                            else
                            {
                                foreach (DataRow v in v3.ToList().FindAll(p => p["instrumentname"].ToString().Trim() != ""))
                                {
                                    JsonString tempJT03 = new JsonString();
                                    tempJT03.Text = v["instrumentname"].ToString();
                                    tempJT03.id = v["number"].ToString();
                                    tempJT03.IsLeaf = true;
                                    tempJT03.IconCls = "myicon5";
                                    tempJT03.IsLeaf = true;
                                    tempJT02.AppendChild(tempJT03);
                                }
                            }
                        }
                        tempJT01.AppendChild(tempJT02);
                    }
                    else
                    {
                        foreach (DataRow v in v2.ToList().FindAll(p => p["instrumentname"].ToString().Trim() != ""))
                        {
                            JsonString tempJT02 = new JsonString();
                            tempJT02.Text = v["instrumentname"].ToString();
                            tempJT02.id = v["number"].ToString();
                            tempJT02.IsLeaf = true;
                            tempJT02.IconCls = "myicon5";
                            tempJT02.IsLeaf = true;
                            tempJT01.AppendChild(tempJT02);
                        }
                    }
                }
                totalString.AppendChild(tempJT01);
            }
            return totalString.ConvertToJson(2);
        }

        /// <summary>
        /// 异常信息树桩菜单Json文件
        /// </summary>
        /// <param name="Errds">异常信息记录表</param>
        /// <param name="catalog">人工、自动化</param>
        /// <param name="sqlCatalog">数据库类型</param>
        /// <returns></returns>
        public string GetErrDataJsontree(DataSet Errds, int catalog, string sqlCatalog)
        {
            this.Errds_Json = Errds;
            DataSet ds_instrument = new DataSet();
            if (Errds_Json != null && Errds_Json.Tables.Count != 0)
            {
                if (Errds_Json.Tables[0].Rows.Count != 0)
                {
                    var tempErrds = Errds_Json.Tables[0].AsEnumerable().ToList().FindAll(p => p["type"].ToString().Contains(sqlCatalog));
                    if (tempErrds.Count != 0)
                    {
                        var tempinstrumentBase = ds_Json.Tables[0].AsEnumerable().ToList().FindAll(p => tempErrds.Find(p0 => p0["InstuctNum"].ToString().Trim() == p["number"].ToString().Trim()) != null);
                        if (tempinstrumentBase.Count != 0)
                        {
                            ds_instrument.Tables.Clear();
                            ds_instrument.Tables.Add(tempinstrumentBase.CopyToDataTable());
                        }
                    }
                }
            }

            return (GetJson(ds_instrument, catalog));
        }

        /// <summary>
        /// 树桩菜单Json文件生成
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="catalog"></param>
        private string GetJson(DataSet instrumentds, int catalog)
        {
            //判定树桩菜单格式
            string statss = "";
            if (catalog == 0) //人工单点
            {
                statss = "人工";
            }
            else if (catalog == 1) //自动化单点
            {
                statss = "自动化";
            }

            //生成树桩菜单Json
            JsonString totalString = new JsonString();
            var list = instrumentds.Tables[0].AsEnumerable().ToList().FindAll(p => p["gather"].ToString().Trim().Contains(statss));

            foreach (var v1 in list.ToList().FindAll(p0 => p0["catalog1"].ToString().Trim() != "").GroupBy(p => p.Field<string>("catalog1")))
            {
                JsonString tempJT01 = new JsonString();
                tempJT01.Text = v1.Key;
                foreach (var v2 in v1.GroupBy(p => p.Field<string>("catalog2")))
                {
                    if (v2.Key != null)
                    {
                        JsonString tempJT02 = new JsonString();
                        tempJT02.Text = v2.Key;
                        foreach (var v3 in v2.GroupBy(p => p.Field<string>("catalog3")))
                        {
                            if (v3.Key != null)
                            {
                                JsonString tempJT03 = new JsonString();
                                tempJT03.Text = v3.Key;
                                foreach (var v4 in v3.GroupBy(p => p.Field<string>("catalog4")))
                                {
                                    if (v4.Key != null)
                                    {
                                        JsonString tempJT04 = new JsonString();
                                        tempJT04.Text = v4.Key;
                                        foreach (var v5 in v4.GroupBy(p => p.Field<string>("catalog5")))
                                        {
                                            if (v5.Key != null)
                                            {
                                                JsonString tempJT05 = new JsonString();
                                                tempJT05.Text = v5.Key;
                                                foreach (DataRow v6 in v5.ToList())
                                                {
                                                    if (v6["instrumentname"] != null)
                                                    {
                                                        JsonString tempJT06 = new JsonString();
                                                        tempJT06.Text = v6["instrumentname"].ToString();
                                                        tempJT06.IsLeaf = true;
                                                        tempJT06.id = v6["number"].ToString();
                                                        tempJT06.IconCls = "myicon5";
                                                        tempJT05.AppendChild(tempJT06);
                                                    }
                                                }
                                                tempJT04.AppendChild(tempJT05);
                                            }
                                            else
                                            {
                                                foreach (DataRow v in v5.ToList().FindAll(p => p["instrumentname"].ToString().Trim() != ""))
                                                {
                                                    JsonString tempJT05 = new JsonString();
                                                    tempJT05.Text = v["instrumentname"].ToString();
                                                    tempJT05.IsLeaf = true;
                                                    tempJT05.id = v["number"].ToString();
                                                    tempJT05.IconCls = "myicon5";
                                                    tempJT04.AppendChild(tempJT05);
                                                }
                                            }
                                        }
                                        tempJT03.AppendChild(tempJT04);
                                    }
                                    else
                                    {
                                        foreach (DataRow v in v4.ToList().FindAll(p => p["instrumentname"].ToString().Trim() != ""))
                                        {
                                            JsonString tempJT04 = new JsonString();
                                            tempJT04.Text = v["instrumentname"].ToString();
                                            tempJT04.IsLeaf = true;
                                            tempJT04.id = v["number"].ToString();
                                            tempJT04.IconCls = "myicon5";
                                            tempJT03.AppendChild(tempJT04);
                                        }
                                    }
                                }
                                tempJT02.AppendChild(tempJT03);
                            }
                            else
                            {
                                foreach (DataRow v in v3.ToList().FindAll(p => p["instrumentname"].ToString().Trim() != ""))
                                {
                                    JsonString tempJT03 = new JsonString();
                                    tempJT03.Text = v["instrumentname"].ToString();
                                    tempJT03.IsLeaf = true;
                                    tempJT03.id = v["number"].ToString();
                                    tempJT03.IconCls = "myicon5";
                                    tempJT02.AppendChild(tempJT03);
                                }
                            }
                        }
                        tempJT01.AppendChild(tempJT02);
                    }
                    else
                    {
                        foreach (DataRow v in v2.ToList().FindAll(p => p["instrumentname"].ToString().Trim() != ""))
                        {
                            JsonString tempJT02 = new JsonString();
                            tempJT02.Text = v["instrumentname"].ToString();
                            tempJT02.IsLeaf = true;
                            tempJT02.id = v["number"].ToString();
                            tempJT01.AppendChild(tempJT02);
                        }
                    }
                }
                totalString.AppendChild(tempJT01);
            }
            return totalString.ConvertToJson(1);
        }
    }

    /// <summary>
    /// Json文件组装
    /// </summary>
    public class JsonString
    {
        public string Text = "";
        /// <summary>
        /// 图标
        /// </summary>
        public string IconCls = "myicon1";
        public bool IsLeaf = false;
        public string id = "";
        public System.Collections.ArrayList Children = new System.Collections.ArrayList();

        public void AppendChild(JsonString temObj)
        {
            Children.Add(temObj);
        }

        public string ConvertToJson(int tag)
        {
            string result = "{\"text\":\"" + Text + "\",\"iconCls\":\"" + IconCls + "\"";
            if (IsLeaf)
                if (tag == 1)
                    result += ",\"leaf\":true,\"id\":\"" + id + "\"";
                else
                    result += ",\"leaf\":false,\"id\":\"" + id + "\",\"checked\":false";
            else
            {
                result += ",\"children\":[";
                if (Children.Count > 0)
                {
                    int i = 0, count = Children.Count;
                    JsonString tempObj;
                    for (i = 0; i < count; i++)
                    {
                        tempObj = Children[i] as JsonString;
                        result += tempObj.ConvertToJson(tag);
                        if (i != count - 1)
                            result += ",";
                    }
                }
                result += "]";
            }
            result += "}";
            return result;
        }
    }

    /// <summary>
    /// 文件夹树状菜单
    /// </summary>
    public class fileJsonTree
    {
        public string JsonTree(string filename)
        {

            string path = HttpContext.Current.Server.MapPath("系统资料\\" + filename);
            //context.Server.MapPath(uploadPath);
            JsonString totalString = new JsonString();
            JsonString tempJT = new JsonString();
            tempJT.Text = filename;
            fileTree(tempJT, path);

            totalString.AppendChild(tempJT);
            return totalString.ConvertToJson(1);
        }

        void fileTree(JsonString tempJT, string path)
        {
            try
            {
                if (Directory.GetDirectories(path).Count() != 0)
                {
                    foreach (string str in Directory.GetDirectories(path))
                    {
                        JsonString tempJT01 = new JsonString();
                        string str1 = str.Split('\\').Last();
                        tempJT01.Text = str1;
                        if (Directory.GetDirectories(str).Count() != 0)
                        {
                            tempJT01.IsLeaf = false;
                        }
                        else
                        {
                            tempJT01.IsLeaf = true;
                        }
                        fileTree(tempJT01, path + "\\" + str1);
                        tempJT.AppendChild(tempJT01);
                    }
                }
            }
            catch (Exception ex)
            {
                string _ex = ex.ToString();
            }
        }
    }

    /// <summary>
    /// 不同功能界面树桩菜单组装
    /// </summary>
    public class DeviceJsonTree
    {
        /// <summary>
        /// 树桩菜单组装
        /// </summary>
        /// <returns>Jsontree</returns>
        public string Jsontree(string JsontreeID)
        {
            string[] Jsonstring = JsontreeID.Split(';');
            string tree = "";
            if (Jsonstring[0].Contains("监测信息"))
            {
                if (Jsonstring[1].Contains("人工"))
                    tree = Config.manJsonTree;
                else if (Jsonstring[1].Contains("自动"))
                    tree = Config.autoJsonTree;
            }
            else if (Jsonstring[0].Contains("异常查询"))
            {
                if (Jsonstring[1].Contains("人工") && Jsonstring[2].Contains("原始"))
                    tree = Config.manysJsonTree;
                else if (Jsonstring[1].Contains("人工") && Jsonstring[2].Contains("技术"))
                    tree = Config.manjsJsonTree;
                else if (Jsonstring[1].Contains("人工") && Jsonstring[2].Contains("整编"))
                    tree = Config.manzbJsonTree;
                else if (Jsonstring[1].Contains("自动化") && Jsonstring[2].Contains("原始"))
                    tree = Config.autoysJsonTree;
                else if (Jsonstring[1].Contains("自动化") && Jsonstring[2].Contains("技术"))
                    tree = Config.autojsJsonTree;
                else if (Jsonstring[1].Contains("自动化") && Jsonstring[2].Contains("整编"))
                    tree = Config.autozbJsonTree;
            }
            else if (Jsonstring[0].Contains("特征值") || Jsonstring[0].Contains("变位率"))
            {
                if (Jsonstring[1].Contains("人工"))
                    tree = Config.manDataAnsysTree;
                else if (Jsonstring[1].Contains("自动"))
                    tree = Config.autoDataAnsysTree;
            }
            else if (Jsonstring[0].Contains("监测项目"))
            {
                var temp = Config.InstructBase.Tables[0].AsEnumerable().ToList().Find(p => p.Field<int>("number") == int.Parse(Jsonstring[1]));
                tree = "[";
                //生成动态监测项目子节点
                for (int i = 1; i <= 15; i++)
                {
                    string valuename = temp["value" + i.ToString()].ToString();
                    if (valuename.Trim() == "")
                        continue;

                    tree += "{text:\"" + valuename + "\",leaf:true,checked:false,id:\"" + Jsonstring[1] + "-" + i.ToString() + "\"},";
                }
                if (tree[tree.Length - 1] == ',')
                    tree = tree.Remove(tree.Length - 1);
                tree = tree + "]";
            }
            else if (Jsonstring[0].Contains("分布图"))
            {
               // tree = Config.PicAnsysJsonTree;
            }
            else if (Jsonstring[0].Contains("文档查询"))
            {
                fileJsonTree filetree = new fileJsonTree();
                tree = filetree.JsonTree(Jsonstring[1]);
            }

            return tree;
        }
    }
}