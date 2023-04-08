using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using SZRiverSys.Model;
using SZRiverSys.Comm;

namespace SZRiverSys.ServiceHandler
{
    /// <summary>
    /// WorkFormHandler 的摘要说明
    /// </summary>
    public class WorkFormHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string msg = "{\"code\":1,\"success\":false,\"msg\":\"请求参数不对\"}";
            string method = context.Request["method"];
            JSON.JsonHelper Json = new JSON.JsonHelper();
            string json = "";
            SQL.BusinessSql sql = new SQL.BusinessSql();
            #region 实例化实体类
            List<Business_WorkForm> workforms = new List<Business_WorkForm>();
            Business_WorkForm workform = new Business_WorkForm();
            #endregion

            #region  实例化权限模块
            Task4002HTML task4002HTML = new Task4002HTML();
            RolePowerHelper _ROLEPOWER = new RolePowerHelper();
            #endregion
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            if (method != null)
            {
                string token = context.Request.Params["token"] == null ? "" : context.Request.Params["token"];
                if (CacheHelper.CacheHelper.IsContain(token))
                {
                    Model.UserModel tmpUser = CacheHelper.CacheHelper.Get(token) as Model.UserModel;         
                     if (method == "downonefile")//下载单个文件
                    {
                        string filename = context.Request["filename"];
                        var filelist = filename.TrimEnd(';').Split(';');
                        string[] filepath = new string[filelist.Length];
                        for (int i = 0; i < filelist.Length; i++)
                        {
                            filepath[i] = CacheHelper.CacheHelper.ServerURL + "Upfile/RiverManageFile/" + filelist[i];
                        }
                        //string filepath=HttpContext.Current.Server.MapPath("~/Upfile/RiverManageFile/") +filename;
                        //if (File.Exists(filepath))
                        //{
                        //    DownFileList(filepath);
                        //}

                        msg = "{\"code\":0,\"success\":true,\"filepath\":\"" + string.Join(";", filepath) + "\"}";

                    }
                    else if (method == "downbusinessfile") //下载附件
                    {
                        string ID = context.Request["ID"];
                        DataTable dt = sql.GetBusinessById(ID);
                        string filelist = dt.Rows[0]["Enclosure"].ToString();
                        if (filelist != "")
                        {
                            ///创建zip父级文件夹
                            var file = filelist.Split(';');
                            string filename = tmpUser.usercode + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                            string newfilepath = HttpContext.Current.Server.MapPath("~/Upfile/downzip/") + filename;
                            System.IO.Directory.CreateDirectory(newfilepath);
                            for (var i = 0; i < file.Length; i++)
                            {
                                string filepath = Path.Combine(HttpContext.Current.Server.MapPath("~/Upfile/RiverManageFile"), file[i]);
                                if (File.Exists(filepath))
                                {
                                    if (!File.Exists(newfilepath + "/" + file[i]))
                                    {
                                        File.Copy(filepath, newfilepath + "/" + file[i]);
                                    }
                                }
                            }
                            string Source = newfilepath;
                            string TartgetFile = newfilepath + ".zip";
                            Directory.CreateDirectory(Path.GetDirectoryName(TartgetFile));
                            using (ZipOutputStream s = new ZipOutputStream(File.Create(TartgetFile)))
                            {
                                s.SetLevel(6);
                                Comm.ZIP.Compress(Source, s);
                                s.Finish();
                                s.Close();
                            }
                            Comm.FileHelper.DeleteFolder(newfilepath);
                            DownFileList(TartgetFile);
                        }
                        else
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"暂无文件\"}";
                        }
                    }
                    else if (method == "getbusinessnofinish")//获取时间内未完成业务
                    {
                        string username = tmpUser.usercode;
                        DataTable dt = sql.GetUserBusinessNoFinish(username);
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
                    else if (method == "getbusinesspasstime")//获取逾期未完成业务
                    {
                        string username = tmpUser.usercode;
                        DataTable dt = sql.GetUserBusinessPassTime(username);
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
                    else if (method == "getuserfinishbusiness")//获取用户完成的业务
                    {
                        DataTable dt = sql.GetUserBusinessFinish(tmpUser.usercode);
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
                    else if (method == "getbusinesslistbytime")//根据时间获取所有业务
                    {
                        string starttime = context.Request["starttime"];
                        string endtime = context.Request["endtime"];
                        DataTable dt = sql.GetBusinessByTime(starttime, endtime);
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
                    else if (method == "evaluatebusiness")//用户进行业务评价
                    {
                        Model.BusinessModel m = new Model.BusinessModel();
                        m.ID = context.Request["ID"];
                        m.Score = context.Request["Score"];
                        m.Comment = context.Request["Comment"];
                        if (sql.evaluate(m))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"评价成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"评价失败\"}";
                        }
                    }
                    #region  工单操作的接口
                    else if (method == "createworkform")//创建工单
                    {
                        Model.Business_WorkForm m = new Model.Business_WorkForm();
                        m.WorkName = context.Request["WorkName"];
                        m.Type = context.Request["Type"];
                        m.Content = context.Request["Content"];
                        m.Assignor = context.Request["Assignor"];
                        m.Emergencylevel = context.Request["Emergencylevel"];
                        m.PlanFinishWorkHouse = context.Request["PlanFinishWorkHouse"];
                        m.PlanFinishTime = context.Request["PlanFinishTime"];
                        m.WorkNeed = context.Request["WorkNeed"] == null ? "" : context.Request.Params["WorkNeed"];
                        m.Creater = tmpUser.usercode;
                        m.state = "未进行";
                        m.islook = "0";
                        m.projectid = context.Request["projectid"];
                        m.Range = context.Request["Range"];
                        m.Workload = context.Request["Workload"];
                        m.RangeUnit = context.Request["RangeUnit"];
                        m.Longitude = context.Request["Longitude"];
                        m.Latitude = context.Request["Latitude"];
                        if (m.Longitude == null)
                            m.Longitude = "0";
                        if (m.Latitude == null)
                            m.Latitude = "0";
                        if (sql.ADDWorkForm(m))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"创建成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"创建失败\"}";
                        }

                    }
                    else if (method == "delworkform")
                    {
                        string id = context.Request["id"];
                        if (sql.delworkform(id))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"删除成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"删除失败\"}";
                        }
                    }
                    else if (method == "getworkform")//获取工单
                    {
                        int pageindex = int.Parse(context.Request["pageindex"]);
                        int pagesize = int.Parse(context.Request["pagesize"]);
                        string search = context.Request["search"];
                        int total = 0;
                        DataTable dt = sql.GetWorkForm(tmpUser, pagesize, pageindex, out total);
                        task4002HTML = _ROLEPOWER.RULE_Task4002HTML(tmpUser.RoleID);
                        if (dt != null && dt.Rows.Count > 0)
                        {                      
                            workforms = DataTableToEntity<Business_WorkForm>.ConvertToModel(dt);
                            for (int i = 0; i < workforms.Count; i++)
                            {
                                task4002HTML = new Task4002HTML();
                                task4002HTML = _ROLEPOWER.RULE_Task4002HTML(tmpUser.RoleID);
                                if (tmpUser.usercode == workforms[i].Creater)
                                {
                                    task4002HTML.btnEND = true;
                                }
                                if (workforms[i].FinishTime!=null)
                                {
                                    task4002HTML.btnEND = false;
                                }
                                workforms[i].Operate = task4002HTML;
                            }
                            msg = "{\"code\":0,\"success\":true,\"data\":" + js.Serialize(workforms) + ",\"total\":" + total + ",\"msg\":" + js.Serialize(task4002HTML) + "}";
                        }
                        else
                        {
                            msg = "{\"code\":0,\"success\":false,\"data\":[],\"total\":" + total + ",\"msg\":" + js.Serialize(task4002HTML) + "}";
                        }
                    }
                    else if (method == "uploadworkformfile")//上传工单文件
                    {
                        //string ID = context.Request["ID"];
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
                                var filepath = Path.Combine(HttpContext.Current.Server.MapPath("~/Upfile/WorkFile"), filename);
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
                    else if (method == "insertworkformfinishdata")//插入工单完成完成信息
                    {
                        Model.Business_WorkForm m = new Model.Business_WorkForm();
                        m.ID = context.Request["ID"];
                        m.FinishExplain = context.Request["FinishExplain"];
                        m.FinishTime = DateTime.Now.ToLocalTime().ToString();
                        if (sql.FinishWorkForm(m))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"添加成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"添加失败\"}";
                        }
                    }
                    else if (method == "getnotfinishworkform")//获取时间内未完成工单
                    {
                        DataTable dt = sql.GetUserWorkFormList(tmpUser.usercode);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            json = Json.DataTableToJsonForLayUi(dt);
                            msg = "{\"code\":0,\"success\":true,\"data\":" + json + ",\"count\":" + dt.Rows.Count + "}";
                        }
                        else
                        {
                            msg = "{\"code\":0,\"success\":false,\"data\":[]}";
                        }
                    }
                    else if (method == "getnotfinishworkformbypasstime")//获取逾期未完成工单
                    {
                        DataTable dt = sql.GetUserWorkFormNoFinishPassTime(tmpUser.usercode);
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
                    else if (method == "getfinishworkform")//获取已完成工单
                    {
                        DataTable dt = sql.GetWorkFormFinish(tmpUser.usercode);
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
                    else if (method == "getworkformphoto")//根据名称下载工单文件
                    {
                        string filename = context.Request["filename"];
                        var filelist = filename.TrimEnd(';').Split(';');
                        string[] filepath = new string[filelist.Length];
                        for (int i = 0; i < filelist.Length; i++)
                        {
                            filepath[i] = CacheHelper.CacheHelper.ServerURL + "Upfile/WorkFile/" + filelist[i];
                        }
                        //string filepath=HttpContext.Current.Server.MapPath("~/Upfile/RiverManageFile/") +filename;
                        //if (File.Exists(filepath))
                        //{
                        //    DownFileList(filepath);
                        //}
                        msg = "{\"code\":0,\"success\":true,\"filepath\":\"" + string.Join(";", filepath) + "\"}";
                    }
                    else if (method == "getworkformlistbydate")//根据日期查询工单
                    {
                        string starttime = context.Request["starttime"];
                        string endtime = context.Request["endtime"];
                        DataTable dt = sql.GetWorkFormByTime(starttime, endtime,tmpUser);
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
                    #endregion
                    else if (method == "adduserpatrolplan")//添加管养计划
                    {
                        Model.PatrolPlanModel m = new Model.PatrolPlanModel();
                        m.Content = context.Request["Content"];
                        m.UserId = tmpUser.UserID.ToString();
                        m.StartTime = context.Request["StartTime"];
                        m.EndTime = context.Request["EndTime"];
                        m.ManagementRange = context.Request["ManagementRange"];
                        if (sql.CreateUserPlan(m))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"创建成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":-1,\"success\":false,\"msg\":\"创建失败\"}";
                        }
                    }
                    else if (method == "getworktype")  //工单类型
                    {
                        DataTable dt = sql.GetWorkFormType();
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
                    else if (method == "getemergencylevel")//获取紧急程度下拉框
                    {
                        DataTable dt = sql.GetEmergencyLevel();
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
                    else if (method == "getuserplanlist")//获取计划列表
                    {
                        int pageindex = int.Parse(context.Request["pageindex"]);
                        int pagesize = int.Parse(context.Request["pagesize"]);
                        string search = context.Request["search"];
                        int total = 0;
                        DataTable dt = sql.GetUserPlanList(tmpUser, pagesize, pageindex, search, out total);
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
                    else if (method == "workformmessge")//获取工单提示消息
                    {
                        DataTable dt = sql.GetWorkFormMsg(tmpUser);
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
                    else if (method == "workformisread")//设置工单为已读
                    {
                        string ID = context.Request["ID"];
                        if (sql.WorkFormAleadread(ID))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"修改成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":-1,\"success\":false,\"msg\":\"修改失败\"}";
                        }
                    }
                    else if(method== "acceptworkform")//接受工单
                    {
                        string ID = context.Request["ID"];
                        if (sql.acceptwork(ID, tmpUser.usercode))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"接单成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":-1,\"success\":false,\"msg\":\"接单失败\"}";
                        }
                    }
                    else if (method == "getusernotfinishplan")//获取用户时间内未完成计划
                    {
                        DataTable dt = sql.UserNotFinishPlan(tmpUser);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            json = Json.DataTableToJsonForLayUi(dt);
                            msg = "{\"code\":0,\"success\":true,\"data\":" + json + ",\"count\":" + dt.Rows.Count + "}";
                        }
                        else
                        {
                            msg = "{\"code\":0,\"success\":false,\"data\":[]}";
                        }
                    }
                    else if (method == "getmycreatework")//获取我创建的工单
                    {
                        DataTable dt = sql.GetMyCreateWorkFomr(tmpUser.usercode);
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
                    else if (method == "getworkformtrack")//获取工单跟踪信息
                    {
                        string FormID = context.Request["FormID"];
                        DataTable dt = sql.GetWorkFormTrack(FormID);
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
                     else if (method == "addworkformtrack")//添加工单执行过程
                    {
                        Model.WorkFormTrack m = new Model.WorkFormTrack();
                        m.FormId = context.Request["FormId"];
                        m.Content = context.Request["Content"];
                        m.userid = tmpUser.UserID.ToString();
                        m.photo = context.Request["photo"];
                        if (sql.ADDWorkFormTrack(m))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"添加成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":-1,\"success\":false,\"msg\":\"添加失败\"}";
                        }
                    }
                    #region  录入数据
                    else if (method == "insertplace") //添加工单位置
                    {
                        Model.PlaceInfo m = new PlaceInfo();
                        m.PlaceName = context.Request["PlaceName"];
                        m.Pid = context.Request["Pid"];
                        m.Remark = context.Request["Remark"];
                        if (sql.InsertPlaceInfo(m))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"添加成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":-1,\"success\":false,\"msg\":\"添加失败\"}";
                        }
                    }
                    else if (method == "insertcommonlanguage")//添加工单常用语
                    {
                        Model.Commonlanguage m = new Commonlanguage();
                        m.Content = context.Request["Content"];
                        m.type = context.Request["type"];
                        if (sql.InsertCommonlanguage(m))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"添加成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":-1,\"success\":false,\"msg\":\"添加失败\"}";
                        }
                    }
                    else if (method == "getplacebypage")//获取工单位置列表
                    {
                        int pageindex = int.Parse(context.Request["pageindex"]);
                        int pagesize = int.Parse(context.Request["pagesize"]);
                        string search = context.Request["search"];
                        int total = 0;
                        DataTable dt = sql.GetPlaceByPage(pagesize, pageindex, search, out total);
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
                    else if (method == "getcommonlanguagebypage")//获取常用语列表
                    {
                        int pageindex = int.Parse(context.Request["pageindex"]);
                        int pagesize = int.Parse(context.Request["pagesize"]);
                        string search = context.Request["search"];
                        int total = 0;
                        DataTable dt = sql.GetLanguageByPage(pagesize, pageindex, search, out total);
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
                    else if (method == "delplace")//删除位置信息
                    {
                        string id = context.Request["id"];
                        if (sql.delPlaceInfo(id))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"删除成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"删除失败\"}";
                        }
                    }
                    else if (method == "dellanguage")//删除常用语
                    {
                        string id = context.Request["id"];
                        if (sql.delCommonlanguage(id))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"删除成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"删除失败\"}";
                        }
                    }
                    else if (method == "getlanguageddl")//获取常用于下拉框
                    {
                        DataTable dt = sql.GetCommonlanguage("");
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            json = Json.DataTableToJsonForLayUi(dt);
                            msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                        }
                        else
                        {
                            msg = "{\"code\":0,\"success\":false,\"data\":[],\"msg\":\"暂无数据\"}";
                        }
                    }
                    else if (method == "getplaceddl")//获取工单实施位置下拉框
                    {
                        string Pid = context.Request["Pid"];
                        DataTable dt = sql.GetPlaceInfo("", Pid);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            json = Json.DataTableToJsonForLayUi(dt);
                            msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                        }
                        else
                        {
                            msg = "{\"code\":0,\"success\":false,\"data\":[],\"msg\":\"暂无数据\"}";
                        }
                    }
                    else if (method == "getworkformbyid") //获取单个工单内容
                    {
                        string id = context.Request["id"];
                        DataTable dt = sql.GetWorkFormById(id);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            json = Json.DataTableToJsonForLayUi(dt);
                            msg = "{\"code\":0,\"success\":true,\"data\":" + json + "}";
                        }
                        else
                        {
                            msg = "{\"code\":0,\"success\":false,\"data\":[],\"msg\":\"暂无数据\"}";
                        }
                    }
                    else if (method=="editworkform") //修改工单
                    {
                        Model.Business_WorkForm m = new Business_WorkForm();
                        m.ID = context.Request["ID"];
                        m.WorkName = context.Request["WorkName"];
                        m.Type = context.Request["Type"];
                        m.Content = context.Request["Content"];
                        m.Assignor = context.Request["Assignor"];
                        m.Emergencylevel = context.Request["Emergencylevel"];
                        m.PlanFinishWorkHouse = context.Request["PlanFinishWorkHouse"];
                        m.PlanFinishTime = context.Request["PlanFinishTime"];
                        m.WorkNeed = context.Request["WorkNeed"];
                        m.Workload = context.Request["Workload"];
                        m.RangeUnit = context.Request["RangeUnit"];
                        if (sql.EditWorkForm(m))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"修改成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"修改失败\"}";
                        }

                    }
                    #endregion
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
        /// <summary>
        /// 将文件夹转换成zip压缩文件并下载
        /// </summary>
        /// <param name="ParentPath">文件夹地址</param>
        public void DownZipFile(string ParentPath)
        {
            string Source = ParentPath;
            string TartgetFile = ParentPath + ".zip";
            Directory.CreateDirectory(Path.GetDirectoryName(TartgetFile));
            using (ZipOutputStream s = new ZipOutputStream(File.Create(TartgetFile)))
            {
                s.SetLevel(6);
                Comm.ZIP.Compress(Source, s);
                s.Finish();
                s.Close();
            }
            Comm.FileHelper.DeleteFolder(ParentPath);
            DownFileList(TartgetFile);
        }
        public void DownFileList(string downpath)
        {

            HttpResponse _respose = HttpContext.Current.Response;
            //string fileName = "test.doc";//客户端保存的文件名 
            //以字符流的形式下载文件 
            FileStream fs = new FileStream(downpath, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            _respose.ContentType = "image/jpeg";
            //通知浏览器下载文件而不是打开 
            _respose.AddHeader("Content-Disposition", "attachment;  filename=" + HttpUtility.UrlEncode(downpath, System.Text.Encoding.UTF8));
            _respose.BinaryWrite(bytes);
            _respose.Flush();
            _respose.End();
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