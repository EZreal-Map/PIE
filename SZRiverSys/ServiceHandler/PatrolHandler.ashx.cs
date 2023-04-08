using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using SZRiverSys.Comm;
using SZRiverSys.Model;

namespace SZRiverSys.ServiceHandler
{
    /// <summary>
    /// PatrolHandler 的摘要说明
    /// </summary>
    public class PatrolHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string msg = "{\"code\":1,\"success\":false,\"msg\":\"请求参数不对\"}";
            string method = context.Request["method"];
            SQL.PatrolSql sql = new SQL.PatrolSql();
            JSON.JsonHelper Json = new JSON.JsonHelper();
            string json = "";
            #region 实例化实体类
            List<PatrolPlan> PatrolPlans = new List<PatrolPlan>();
            PatrolPlan PatrolPlan = new PatrolPlan();
            #endregion
            #region  实例化权限模块
            XX9001HTML patrol2002HTML = new XX9001HTML();
            RolePowerHelper _ROLEPOWER = new RolePowerHelper();
            #endregion
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            if (method != null)
            {
                string token = context.Request.Params["token"] == null ? "" : context.Request.Params["token"];
                if (CacheHelper.CacheHelper.IsContain(token))
                {
                    Model.UserModel tmpUser = CacheHelper.CacheHelper.Get(token) as Model.UserModel;
                    if (method == "getpatrollist")//获取巡护列表
                    {
                        int pageindex = int.Parse(context.Request["pageindex"]);
                        int pagesize = int.Parse(context.Request["pagesize"]);
                        string type = context.Request["type"];
                        string date = context.Request["date"];
                        int total = 0;
                        DataTable dt = sql.GetPatrolList(pageindex, pagesize, type, date, out total);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            json = Json.DataTableToJsonForLayUi(dt);
                            msg = "{\"code\":0,\"success\":true,\"data\":" + json + ",\"total\":" + total + "}";
                        }
                        else
                        {
                            msg = "{\"code\":0,\"success\":false,\"data\":[]}";
                        }
                    }
                    else if (method == "uploadpatrolimg")//上传巡护照片
                    {
                        HttpFileCollection hfc = context.Request.Files;
                        HttpPostedFile hpf = null;
                        //if (hfc.Count > 0)
                        //{
                        //    hpf = context.Request.Files[0];
                        //}                       
                        string FileNames = "";
                        bool errormsg = true;
                        string errormsgstr = "";
                        if (hfc.Count > 0)
                        {
                            for (int i = 0; i < hfc.Count; i++)
                            {
                                hpf = context.Request.Files[i];
                                string date = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                                var filetype = hpf.FileName.Split('.')[1];
                                var filename = tmpUser.usercode + "-" + date + "." + filetype;
                                var filepath = Path.Combine(HttpContext.Current.Server.MapPath("~/Upfile/Patrolphoto"), filename);
                                string directoryName = Path.GetDirectoryName(filepath);
                                if (!Directory.Exists(directoryName))
                                {
                                    Directory.CreateDirectory(directoryName);
                                }
                                try
                                {
                                    hpf.SaveAs(filepath);
                                }
                                catch (Exception ex)
                                {
                                    errormsg = false;
                                    errormsgstr = ex.Message;
                                }
                                FileNames += filename + ";";
                            }
                        }
                        if (errormsg)
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"上传成功\",\"FileName\":\"" + FileNames + "\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"上传失败\"}";
                        }

                    }
                    else if (method == "addpatrolinfo")//上传巡护事件
                    {
                        Model.PatrolInfo m = new Model.PatrolInfo();
                        m.uid = tmpUser.UserID.ToString();
                        m.Type = context.Request["Type"];
                        m.infomemo = context.Request["infomemo"];
                        m.photopath = context.Request["photopath"];
                        m.longitude = context.Request["longitude"];
                        m.latitude = context.Request["latitude"];
                        m.addressinfo = context.Request["addressinfo"];
                        m.RTU = "";
                        if (sql.AddPatrolInfo(m))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"添加成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"添加失败\"}";
                        }
                    }
                    else if (method == "getphotourl")//获取巡护图片地址
                    {
                        string filename = context.Request["photopath"];
                        var filelist = filename.TrimEnd(';').Split(';');
                        List<string> filepath = new List<string>();
                        for (int i = 0; i < filelist.Length; i++)
                        {
                            filepath.Add("Upfile/Patrolphoto/" + filelist[i]);
                        }
                        //string filepath=HttpContext.Current.Server.MapPath("~/Upfile/RiverManageFile/") +filename;
                        //if (File.Exists(filepath))
                        //{
                        //    DownFileList(filepath);
                        //}
                        msg = "{\"code\":0,\"success\":true,\"data\":" + js.Serialize(filepath) + "}";
                    }
                    else if (method == "getmypatrolplan")//获取我的巡线计划
                    {
                        int pageindex = int.Parse(context.Request["pageindex"]);
                        int pagesize = int.Parse(context.Request["pagesize"]);
                        string userid = tmpUser.UserID.ToString();
                        int total = 0;
                        DataTable dt = sql.GetMyPatrolPlan(pageindex, pagesize, userid,tmpUser.RoleID.ToString(), out total);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            patrol2002HTML = _ROLEPOWER.RULE_XX9001HTML(tmpUser.RoleID);
                            PatrolPlans = DataTableToEntity<PatrolPlan>.ConvertToModel(dt);
                            for (int i = 0; i < PatrolPlans.Count; i++)
                            {
                                PatrolPlans[i].Operate = patrol2002HTML;
                            }
                            msg = "{\"code\":0,\"success\":true,\"data\":" + js.Serialize(PatrolPlans) + ",\"total\":" + total + ",\"msg\":" + js.Serialize(patrol2002HTML) + "}";
                        }
                        else
                        {
                            msg = "{\"code\":0,\"success\":false,\"data\":[]}";
                        }
                    }
                    else if (method == "addmypatrolplan")//添加个人巡线计划
                    {
                        Model.PatrolPlan m = new Model.PatrolPlan();
                        m.UserId = context.Request["UserId"];
                        m.PatrolRank = context.Request["PatrolRank"];
                        m.PatrolContent = context.Request["PatrolContent"];
                        m.PatrolLength = context.Request["PatrolLength"];
                        m.PlanStartDate = context.Request["PlanStartDate"];
                        m.PlanEndDate = context.Request["PlanEndDate"];
                        if(sql.CreatePatrolPlan(m))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"添加成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"添加失败\"}";
                        }
                    }
                    else if (method == "delpatrolplan")//删除个人巡线计划
                    {
                        string ID = context.Request["ID"];
                        if (sql.DelPatrolPlan(ID))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"删除成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"删除失败\"}";
                        }
                    }
                    else if(method== "getpatroltotal")//获取人员巡河距离统计
                    {
                        string date = context.Request["date"];
                        string uname = context.Request["uname"];
                        DataTable dt = sql.GetPatrolTotal(date, uname);
                        if (dt.Rows.Count > 0)
                        {
                             json = Json.DataTableToJsonForLayUi(dt);
                            msg = "{\"code\":0,\"success\":true,\"data\":" + json + ",\"Unit\":\"米\",\"count\":"+dt.Rows.Count+"}";
                        }
                        else
                        {
                            msg = "{\"code\":0,\"success\":true,\"data\":[],\"count\":" + dt.Rows.Count + "}";
                        }
                    }
                }
                else if (method == "getpatroldata")//获取地图上现实的巡护事件
                {
                    DataTable dt = sql.GetPatrolData();
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        json = Json.DataTableToJsonForLayUi(dt);
                        msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                    }
                    else
                    {
                        msg = "{\"code\":0,\"success\":false,\"data\":[]}";
                    }
                }
                else
                {
                    msg = "{\"code\":-1,success:false,msg:\"用户登录失效,请重新登录\"}";
                }

            }
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.Write(msg);//构造json数据格式
            context.Response.End();
            context.Response.Close();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}