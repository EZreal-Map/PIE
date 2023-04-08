using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using SZRiverSys.Model;

namespace SZRiverSys.Comm
{
    public class Tool
    {
        #region 获取角色拥有的按钮权限
        public string GetTreeBySysOperate(int roleid)
        {
            try
            {
                string tmpSql = $@"SELECT
                                   m.*
                                 FROM
                                   New_Menu m
                                 where 1=1";
                DataTable dt =DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                //获取全部集合  
                List<TreeEntity> allModel = new List<TreeEntity>();
                foreach (DataRow drAll in dt.Rows)
                {
                    TreeEntity Model = new TreeEntity();
                    Model.label = drAll["MenuName"].ToString();
                    Model.id = drAll["MenuID"].ToString();
                    Model.pid = int.Parse(drAll["ParentMenuID"].ToString());
                    allModel.Add(Model);
                }
                //筛选顶级分类  
                List<TreeEntity> topModels = new List<TreeEntity>();
                var topItems = allModel.Where(e => e.pid == 0).ToList(); //顶级分类筛选
                foreach (var item in topItems)
                {
                    TreeEntity topModel = new TreeEntity();
                    topModel.label = item.label;
                    topModel.id = item.id;
                    topModel.pid = item.pid;
                    LoopToAppendChildren(allModel, topModel, roleid);
                    topModels.Add(topModel);

                }
                string json = JsonConvert.SerializeObject(topModels);
                return json;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>  
        /// 追溯子类  
        /// </summary>  
        /// <param name="allList">全部数据</param>  
        /// <param name="curItem">当前节点</param>  
        protected void LoopToAppendChildren(List<TreeEntity> allList, TreeEntity curItem, int roleid)
        {
            List<TreeEntity> subItemsModel = new List<TreeEntity>();
            var subItems = allList.Where(ee => ee.pid == int.Parse(curItem.id)).ToList();
            curItem.children = new List<TreeEntity>();
            curItem.children.AddRange(subItems);
            if (subItems.Count == 0)
            {
                //找按钮
                string sql = $"select * from sys_operate where MenuId='{curItem.id}'";
                DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    curItem.children = new List<TreeEntity>();
                    foreach (DataRow item in dt.Rows)
                    {
                        TreeEntity btn = new TreeEntity();
                        btn.label = item["OperateName"].ToString();
                        btn.id = item["id"].ToString();
                        btn.children = subItemsModel;
                        btn.disabled = false;
                        sql = $"select * from role_operate where roleid='{roleid}'";
                        DataTable dt2 = DBHelper.DBHelperMsSql.ExecuteDataTable(sql);
                        if (dt2.Rows.Count > 0)
                        {
                            foreach (DataRow item2 in dt2.Rows)
                            {
                                if (item2["operateid"].ToString() == item["id"].ToString())
                                {
                                    btn.@checked = true;
                                }
                            }
                        }
                        curItem.children.Add(btn);
                    }
                }
            }
            foreach (var subItem in subItems)
            {
                LoopToAppendChildren(allList, subItem, roleid);
            }
        }
        #endregion

        #region 角色拥有的菜单
        public string GetTreeByMenu(int roleid)
        {
            try
            {
                string tmpSql = $@"SELECT * from New_Menu";
                string sql = $"select* from New_ROLE_Menu where roleid = '{roleid}'";
                DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(sql);
                DataSet ds = DBHelper.DBHelperMsSql.ExecuteDataSet(tmpSql);
                //获取全部集合  
                List<TreeEntity> allModel = new List<TreeEntity>();
                foreach (DataRow drAll in ds.Tables[0].Rows)
                {
                    TreeEntity Model = new TreeEntity();
                    Model.label = drAll["MenuName"].ToString();
                    Model.id = drAll["MenuID"].ToString();
                    Model.pid = int.Parse(drAll["ParentMenuID"].ToString());
                    allModel.Add(Model);
                }
                //筛选顶级分类  
                List<TreeEntity> topModels = new List<TreeEntity>();
                var topItems = allModel.Where(e => e.pid == 0).ToList(); //顶级分类筛选
                foreach (var item in topItems)
                {
                    TreeEntity topModel = new TreeEntity();
                    topModel.label = item.label;
                    topModel.id = item.id;
                    topModel.pid = item.pid;
                    item.disabled = false;
                    topModel.disabled = false;
                    LoopToAppendChildrenTwo(allModel, topModel, dt);
                    topModels.Add(topModel);

                }
                string json = JsonConvert.SerializeObject(topModels);
                return json;

            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>  
        /// 追溯子类  
        /// </summary>  
        /// <param name="allList">全部数据</param>  
        /// <param name="curItem">当前节点</param>  
        protected void LoopToAppendChildrenTwo(List<TreeEntity> allList, TreeEntity curItem, DataTable dt)
        {
            List<TreeEntity> subItemsModel = new List<TreeEntity>();
            var subItems = allList.Where(ee => ee.pid == int.Parse(curItem.id)).ToList();
            curItem.children = new List<TreeEntity>();
            curItem.children.AddRange(subItems);
            curItem.disabled = false;
            if (subItems.Count == 0)
            {
                //找角色拥有的菜单权限
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        curItem.disabled = false;
                        if (curItem.id == item["MenuID"].ToString())
                        {
                            curItem.@checked = true;
                        }
                    }
                }
            }
            foreach (var subItem in subItems)
            {
                LoopToAppendChildrenTwo(allList, subItem, dt);
            }
        }



        #endregion

        /// <summary>
        /// 填充完整菜单ID,找到顶级菜单ID
        /// </summary>
        /// <param name="MenuList"></param>
        public List<string> GetMenuParent(string[] MenuList)
        {
            List<string> StrTmp = new List<string>();
            foreach (var item in MenuList)
            {
                StrTmp.Add(item);
            }
            for (int i = 0; i < MenuList.Length; i++)
            {
                string tmpSql = "select MenuID,ParentMenuID from New_Menu where MenuID='" + MenuList[i] + "'";
                DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["ParentMenuID"].ToString().Equals("0"))
                    {
                        //表示传了父级菜单
                        continue;
                    }
                    else
                    {
                        string tmpPID = dt.Rows[0]["ParentMenuID"].ToString();
                        string tmpMenuID = "";
                        while (true)
                        {
                            tmpSql = "select MenuID,ParentMenuID from New_Menu where MenuID='" + tmpPID + "'";
                            DataTable dt2 = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpSql);
                            if (dt2.Rows.Count > 0)
                            {

                                tmpPID = dt2.Rows[0]["ParentMenuID"].ToString();
                                tmpMenuID = dt2.Rows[0]["MenuID"].ToString();
                                if (tmpPID.Equals("0"))
                                {
                                    if (!StrTmp.Contains(tmpMenuID))
                                    {
                                        //添加
                                        StrTmp.Add(tmpMenuID);
                                    }
                                    break;
                                }
                                else
                                {
                                    //添加
                                    if (!StrTmp.Contains(tmpMenuID))
                                    {
                                        //添加
                                        StrTmp.Add(tmpMenuID);
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
            return StrTmp;
        }
    }
}