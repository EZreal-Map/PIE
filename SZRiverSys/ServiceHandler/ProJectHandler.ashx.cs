using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using SZRiverSys.Model;
using SZRiverSys.CacheHelper;
using SZRiverSys.Comm;
using System.IO;

namespace SZRiverSys.ServiceHandler
{
    /// <summary>
    /// class_memo:项目管理 
    /// create_user:王铄文
    /// create_time:2018-3-20
    /// </summary>
    public class ProJectHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            JsonMsgModel jsonMsg = new JsonMsgModel();
            try
            {
                string method = context.Request["method"];

                HttpFileCollection hfc = context.Request.Files;
                HttpPostedFile hpf = null;
                #region 实例化数据访问层
                SQL.ProjectSql projectSql = new SQL.ProjectSql();
                SQL.ProjectPlanSql projectPlanSql = new SQL.ProjectPlanSql();
                #endregion
                #region 实例化容器
                DataTable dt = new DataTable();
                #endregion
                #region 实例帮助类
                JSON.JsonHelper Json = new JSON.JsonHelper();
                #endregion
                #region 实例化实体类
                List<UserModel> userModels = new List<UserModel>();
                UserModel userModel = new UserModel();//用户实体
                List<ProjectModel> projectModels = new List<ProjectModel>();//项目列表页集合
                ProjectModel projectModel = new ProjectModel();//项目实体

                List<ProjectPlanModel> projectPlanModels = new List<ProjectPlanModel>();//项目计划列表页集合
                ProjectPlanModel projectPlanModel = new ProjectPlanModel();//项目计划实体

                List<ProJectPlanClassModel> proJectPlanClassModels = new List<ProJectPlanClassModel>();//项目计划列表页集合
                ProJectPlanClassModel proJectPlanClassModel = new ProJectPlanClassModel();//项目计划实体

                List<ProJectPlanPunchCardModel> proJectPlanPunchCardModels = new List<ProJectPlanPunchCardModel>();//项目计划打卡实体集合
                ProJectPlanPunchCardModel proJectPlanPunchCardModel = new ProJectPlanPunchCardModel();//项目计划打卡实体


                List<PunchCardFileModel> punchCardFileModels = new List<PunchCardFileModel>();//附件
                PunchCardFileModel punchCardFileModel = new PunchCardFileModel();//附件

                List<ProjectDataReportModel> projectDataReportModels = new List<ProjectDataReportModel>();//统计实体集合
                ProjectDataReportModel projectDataReportModel = new ProjectDataReportModel();//统计实体

                #endregion
                #region 实例化权限模块
                Project7001HTML project7001HTML = new Project7001HTML();
                Project7002HTML project7002HTML = new Project7002HTML();
                Project7003HTML project7003HTML = new Project7003HTML();

                RolePowerHelper _ROLEPOWER = new RolePowerHelper();
                #endregion
                #region 通用变量
                string date = "";
                int count = 0;
                int id = 0;
                int num = 0;
                string ArrayID = "";
                string ProjectID = "";
                string ProJectPlanID = "";
                #endregion
                if (method != null)
                {
                    #region 缓存信息及安全标识码
                    string token = context.Request.Params["token"] == null ? "" : context.Request.Params["token"];
                    userModel = CacheHelper.CacheHelper.Get(token) as UserModel;
                    #endregion

                    #region 列表分页专用
                    int pageSize = Convert.ToInt32(context.Request.Params["pagesize"]);//每页显示行数
                    int pageIndex = Convert.ToInt32(context.Request.Params["pageindex"]);//当前页索引
                    string search = context.Request.Params["search"] == null ? "" : context.Request.Params["search"].Trim();//筛选条件
                    #endregion

                    if (CacheHelper.CacheHelper.IsContain(token))
                    {

                        switch (method.ToLower())
                        {
                            #region 项目模块
                            case "getprojectinfo"://获取项目列表
                                dt = projectSql.GetProjectInfo(search, userModel);
                                project7001HTML = _ROLEPOWER.RULE_Project7001HTML(userModel.RoleID);
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    bool btnProjectUser = project7001HTML.btnProjectUser;
                                    projectModels = DataTableToEntity<ProjectModel>.ConvertToModel(dt);
                                    for (int i = 0; i < projectModels.Count; i++)
                                    {
                                        project7001HTML = new Project7001HTML();
                                        project7001HTML = _ROLEPOWER.RULE_Project7001HTML(userModel.RoleID);
                                        if (btnProjectUser)
                                        {
                                            if (projectModels[i].PM == userModel.userName)
                                            {
                                                project7001HTML.btnProjectUser = true;
                                            }
                                            else
                                            {
                                                project7001HTML.btnProjectUser = false;

                                            }
                                        }
                                        projectModels[i].Operate = project7001HTML;
                                    }
                                }
                                jsonMsg.code = 0;
                                jsonMsg.msg = project7001HTML;
                                jsonMsg.count = projectModels.Count;
                                jsonMsg.data = projectModels.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                                break;
                            case "bindgetproject"://绑定项目下拉框
                                dt = projectSql.GetProjectInfo(userModel);
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    projectModels = DataTableToEntity<ProjectModel>.ConvertToModel(dt);
                                }
                                jsonMsg.code = 0;
                                jsonMsg.msg = true;
                                jsonMsg.data = projectModels;
                                break;
                            case "getprojectinfobyid"://根据ID获取某一条项目信息
                                id = int.Parse(context.Request["id"]);
                                dt = projectSql.GetProjectInfoByID(id);
                                if (dt.Rows.Count > 0)
                                {
                                    projectModels = DataTableToEntity<ProjectModel>.ConvertToModel(dt);
                                }
                                jsonMsg.code = 0;
                                jsonMsg.msg = true;
                                jsonMsg.data = projectModels;
                                break;
                            case "addprojectinfo"://增加项目信息
                                                  //projectModel.ID = int.Parse(context.Request["ID"]);
                                projectModel.ProJectNo = context.Request["ProJectNo"];
                                projectModel.ProJectName = context.Request["ProJectName"];
                                projectModel.PM = context.Request["PM"];
                                projectModel.PM_Tel = context.Request["PM_Tel"];
                                projectModel.PM_Job = context.Request["PM_Job"];
                                projectModel.CreateUser = userModel.userName;
                                //projectModel.ModifyDate = context.Request["ModifyDate"];
                                //projectModel.ModifyUser = context.Request["ModifyUser"];
                                projectModel.BeginDate = context.Request["BeginDate"];
                                projectModel.EndDate = context.Request["EndDate"];
                                projectModel.Location = context.Request["Location"];
                                projectModel.PartyAUnit = context.Request["PartyAUnit"];
                                projectModel.PartyAPersonName = context.Request["PartyAPersonName"];
                                projectModel.PartyAPersonTel = context.Request["PartyAPersonTel"];
                                projectModel.PartyAPersonJob = context.Request["PartyAPersonJob"];
                                projectModel.PartyBUnit = context.Request["PartyBUnit"];
                                projectModel.PartyBPersonName = context.Request["PartyBPersonName"];
                                projectModel.PartyBPersonTel = context.Request["PartyBPersonTel"];
                                projectModel.PartyBPersonJob = context.Request["PartyBPersonJob"];
                                projectModel.Remark = context.Request["Remark"];
                                //验证是否存在
                                if (projectSql.ValidateProjectInfoNo(projectModel.ProJectNo))
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = false;
                                    jsonMsg.data = "该项目已存在";
                                    return;
                                }
                                if (projectSql.AddProjectInfo(projectModel))
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = true;
                                    jsonMsg.data = "新增项目成功";
                                }
                                else
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = false;
                                    jsonMsg.data = "新增项目失败";
                                }
                                break;
                            case "editprojectinfo"://修改项目数据
                                projectModel.ID = int.Parse(context.Request["ID"]);
                                projectModel.ProJectNo = context.Request["ProJectNo"];
                                projectModel.ProJectName = context.Request["ProJectName"];
                                projectModel.PM = context.Request["PM"];
                                projectModel.PM_Tel = context.Request["PM_Tel"];
                                projectModel.PM_Job = context.Request["PM_Job"];
                                //projectModel.CreateDate = context.Request["CreateDate"];
                                //projectModel.CreateUser = context.Request["CreateUser"];
                                projectModel.ModifyUser = userModel.userName;
                                projectModel.BeginDate = context.Request["BeginDate"];
                                projectModel.EndDate = context.Request["EndDate"];
                                projectModel.Location = context.Request["Location"];
                                projectModel.PartyAUnit = context.Request["PartyAUnit"];
                                projectModel.PartyAPersonName = context.Request["PartyAPersonName"];
                                projectModel.PartyAPersonTel = context.Request["PartyAPersonTel"];
                                projectModel.PartyAPersonJob = context.Request["PartyAPersonJob"];
                                projectModel.PartyBUnit = context.Request["PartyBUnit"];
                                projectModel.PartyBPersonName = context.Request["PartyBPersonName"];
                                projectModel.PartyBPersonTel = context.Request["PartyBPersonTel"];
                                projectModel.PartyBPersonJob = context.Request["PartyBPersonJob"];
                                projectModel.Remark = context.Request["Remark"];
                                if (projectSql.EditProjectInfo(projectModel))
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = true;
                                    jsonMsg.data = "修改项目成功";
                                }
                                else
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = false;
                                    jsonMsg.data = "修改项目失败";
                                }
                                break;
                            case "delprojectinfo"://删除项目数据
                                ArrayID = context.Request["ArrayID"];
                                if (projectSql.DelProjectInfo(ArrayID))
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = true;
                                    jsonMsg.data = "删除项目成功";
                                }
                                else
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = false;
                                    jsonMsg.data = "删除项目失败";
                                }
                                break;
                            #region 项目组成员操作
                            case "getuserinproject"://获取项目中的成员
                                ProjectID = context.Request["projectid"];
                                dt = projectSql.GetInProjectUser(ProjectID);
                                if (dt.Rows.Count > 0)
                                {
                                    userModels = DataTableToEntity<UserModel>.ConvertToModel(dt);
                                }
                                jsonMsg.code = 0;
                                jsonMsg.msg = true;
                                jsonMsg.count = userModels.Count;
                                jsonMsg.data = userModels;
                                break;
                            case "getusernotinproject"://获取不在项目中的成员
                                ProjectID = context.Request["projectid"];
                                dt = projectSql.GetNotInProjectUser(ProjectID);
                                if (dt.Rows.Count > 0)
                                {
                                    userModels = DataTableToEntity<UserModel>.ConvertToModel(dt);
                                }
                                jsonMsg.code = 0;
                                jsonMsg.msg = true;
                                jsonMsg.count = userModels.Count;
                                jsonMsg.data = userModels;
                                break;
                            case "setprojectteammembers"://指派项目成员
                                List<string> list = new List<string>();
                                string ArrayUser = context.Request["arrayuser"];//ID用户名
                                string ArrayUserNam= projectSql.GetUserNameByID(ArrayUser);//中文用户名
                                ProjectID = context.Request["projectid"];
                                dt = projectSql.GetProjectInfoByID(int.Parse(ProjectID));//获取某一条项目
                                if (dt.Rows.Count > 0)
                                {
                                    string tmpStr = dt.Rows[0]["ProjectTeamMembers"].ToString() == null ? "" : dt.Rows[0]["ProjectTeamMembers"].ToString();
                                    if (tmpStr == "")
                                    {
                                        tmpStr = ArrayUserNam;
                                    }
                                    else
                                    {
                                        for (int i = 0; i < tmpStr.Split(',').Length; i++)
                                        {
                                            list.Add(tmpStr.Split(',')[i].ToString());//赋值已存在的
                                        }
                                        if (!list.Contains(ArrayUserNam))
                                        {
                                            list.Add(ArrayUserNam);
                                        }
                                        tmpStr = string.Join(",", list.ToArray());
                                    }
                                    projectSql.SetProjectTeamMembers(ArrayUser, ProjectID);
                                    if (projectSql.SetProjectTeamMembersTwo(tmpStr, ProjectID))
                                    {
                                        jsonMsg.code = 0;
                                        jsonMsg.msg = true;
                                        jsonMsg.data = "指派成功";
                                    }
                                    else
                                    {
                                        jsonMsg.code = 0;
                                        jsonMsg.msg = false;
                                        jsonMsg.data = "指派失败";
                                    }
                                }
                                break;
                            case "removeprojectuser"://移除项目组的某些成员
                                string RemoveUser = context.Request["removeuser"];//ID用户名
                                string ArrayUserNameRemove = projectSql.GetUserNameByID(RemoveUser);//中文用户名
                                ProjectID = context.Request["projectid"];
                                dt = projectSql.GetProjectInfoByID(int.Parse(ProjectID));//获取某一条项目
                                if (dt.Rows.Count > 0)
                                {
                                    string tmpStr = dt.Rows[0]["ProjectTeamMembers"].ToString() == null ? "" : dt.Rows[0]["ProjectTeamMembers"].ToString();
                                    List<string> listRemove = new List<string>(tmpStr.Split(','));
                                    for (int i = 0; i < listRemove.Count; i++)
                                    {
                                        if (listRemove.Contains(ArrayUserNameRemove))
                                        {
                                            listRemove.Remove(listRemove[i]);
                                        }
                                    }
                                    string tmpInserStr = string.Join(",", listRemove.ToArray());
                                    projectSql.RemoveProjectUser(RemoveUser, ProjectID);
                                    if (projectSql.SetProjectTeamMembersTwo(tmpInserStr, ProjectID))
                                    {
                                        jsonMsg.code = 0;
                                        jsonMsg.msg = true;
                                        jsonMsg.data = "移除成功";
                                    }
                                    else
                                    {
                                        jsonMsg.code = 0;
                                        jsonMsg.msg = false;
                                        jsonMsg.data = "移除失败";
                                    }
                                }
                                break;
                            #endregion
                            #endregion
                            #region 项目计划模块
                            case "getprojectplaninfo"://获取某项目计划列表
                                ProjectID = context.Request["projectid"];//项目ID
                                string type = context.Request["type"];
                                dt = projectPlanSql.GetProjectPlanInfo(search, ProjectID);
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    project7002HTML = _ROLEPOWER.RULE_Project7002HTML(userModel.RoleID);
                                    projectPlanModels = DataTableToEntity<ProjectPlanModel>.ConvertToModel(dt);
                                    for (int i = 0; i < projectPlanModels.Count; i++)
                                    {
                                        projectPlanModels[i].Operate = project7002HTML;
                                    }
                                }
                                jsonMsg.code = 0;
                                jsonMsg.msg = project7002HTML;
                                jsonMsg.count = projectPlanModels.Count;
                                if (type == "web")
                                {
                                    jsonMsg.data = projectPlanModels.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                                }
                                else
                                {
                                    jsonMsg.data = projectPlanModels;
                                }
                                break;
                            case "getprojectinfoplanbyid"://获取某条项目计划信息
                                id = int.Parse(context.Request["id"]);
                                dt = projectPlanSql.GetProjectInfoPlanByID(id);
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    projectPlanModels = DataTableToEntity<ProjectPlanModel>.ConvertToModel(dt);
                                }
                                jsonMsg.code = 0;
                                jsonMsg.msg = true;
                                jsonMsg.data = projectPlanModels;
                                break;
                            case "addprojectplaninfo"://增加项目计划
                                projectPlanModel.ProjectID = int.Parse(context.Request["projectid"]);
                                projectPlanModel.ProjectPlanName = context.Request["projectplanname"];
                                projectPlanModel.PlanClassID = int.Parse(context.Request["planclassid"]);
                                projectPlanModel.Frequencyt_Type = context.Request["frequencyt_type"];//频次类型
                                projectPlanModel.Frequencyt_Value = context.Request["frequencyt_value"];//频次值
                                projectPlanModel.CreateUser = userModel.userName;
                                projectPlanModel.BeginDate = context.Request["begindate"];
                                projectPlanModel.EndDate = context.Request["enddate"];
                                #region 计算总次数
                                TimeSpan ts = new TimeSpan();//时间差
                                DateTime BenginDateTime = Convert.ToDateTime(projectPlanModel.BeginDate);
                                DateTime EndDateTime = Convert.ToDateTime(projectPlanModel.EndDate);
                                switch (projectPlanModel.Frequencyt_Type)
                                {
                                    case "h"://小时
                                        ts = EndDateTime.Subtract(BenginDateTime);
                                        num = (int)(ts.TotalHours * int.Parse(projectPlanModel.Frequencyt_Value));
                                        break;
                                    case "d"://天
                                        ts = EndDateTime.Subtract(BenginDateTime);
                                        num = (int)(ts.TotalDays * int.Parse(projectPlanModel.Frequencyt_Value)); ;
                                        break;
                                    case "m"://月
                                        int Month = (EndDateTime.Year - BenginDateTime.Year) * 12 + (EndDateTime.Month - BenginDateTime.Month);
                                        num = Month * int.Parse(projectPlanModel.Frequencyt_Value);
                                        break;
                                    case "x"://旬
                                        int x = (EndDateTime.Year - BenginDateTime.Year) * 12 + (EndDateTime.Month - BenginDateTime.Month);
                                        num = x * int.Parse(projectPlanModel.Frequencyt_Value);
                                        break;
                                    case "y"://年
                                        int Year = EndDateTime.Year - BenginDateTime.Year;
                                        num = Year * int.Parse(projectPlanModel.Frequencyt_Value);
                                        break;
                                }
                                if (num > 0)
                                {
                                    projectPlanModel.Count = num;
                                }
                                else
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = false;
                                    jsonMsg.data = "结束日期不能满足最基本的频次,请重新设定";
                                    return;
                                }
                                if (projectPlanSql.AddProjectPlanInfo(projectPlanModel))
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = true;
                                    jsonMsg.data = "新增成功";
                                }
                                else
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = false;
                                    jsonMsg.data = "新增失败";
                                }
                                #endregion
                                break;
                            case "editprojectplaninfo"://修改项目计划
                                projectPlanModel.ID = int.Parse(context.Request["id"]);
                                projectPlanModel.ProjectID = int.Parse(context.Request["projectid"]);
                                projectPlanModel.ProjectPlanName = context.Request["projectplanname"];
                                projectPlanModel.PlanClassID = int.Parse(context.Request["planclassid"]);
                                projectPlanModel.Frequencyt_Type = context.Request["frequencyt_type"];//频次类型
                                projectPlanModel.Frequencyt_Value = context.Request["frequencyt_value"];//频次值
                                projectPlanModel.BeginDate = context.Request["begindate"];
                                projectPlanModel.EndDate = context.Request["enddate"];
                                #region 计算总次数
                                TimeSpan ts_Edit = new TimeSpan();//时间差
                                DateTime BenginDateTime_Edit = Convert.ToDateTime(projectPlanModel.BeginDate);
                                DateTime EndDateTime_Edit = Convert.ToDateTime(projectPlanModel.EndDate);
                                switch (projectPlanModel.Frequencyt_Type)
                                {
                                    case "h"://小时
                                        ts_Edit = EndDateTime_Edit.Subtract(BenginDateTime_Edit);
                                        num = (int)(ts_Edit.TotalHours * int.Parse(projectPlanModel.Frequencyt_Value));
                                        break;
                                    case "d"://天
                                        ts_Edit = EndDateTime_Edit.Subtract(BenginDateTime_Edit);
                                        num = (int)(ts_Edit.TotalDays * int.Parse(projectPlanModel.Frequencyt_Value)); ;
                                        break;
                                    case "m"://月
                                        int Month = (EndDateTime_Edit.Year - BenginDateTime_Edit.Year) * 12 + (EndDateTime_Edit.Month - BenginDateTime_Edit.Month);
                                        num = Month * int.Parse(projectPlanModel.Frequencyt_Value);
                                        break;
                                    case "x"://旬
                                        int x = (EndDateTime_Edit.Year - BenginDateTime_Edit.Year) * 12 + (EndDateTime_Edit.Month - BenginDateTime_Edit.Month);
                                        num = x * int.Parse(projectPlanModel.Frequencyt_Value);
                                        break;
                                    case "y"://年
                                        int Year = EndDateTime_Edit.Year - BenginDateTime_Edit.Year;
                                        num = Year * int.Parse(projectPlanModel.Frequencyt_Value);
                                        break;
                                }
                                if (num > 0)
                                {
                                    projectPlanModel.Count = num;
                                }
                                else
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = false;
                                    jsonMsg.data = "结束日期不能满足最基本的频次,请重新设定";
                                    return;
                                }
                                if (projectPlanSql.EditProjectPlanInfo(projectPlanModel))
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = true;
                                    jsonMsg.data = "修改成功";
                                }
                                else
                                {

                                    jsonMsg.code = 0;
                                    jsonMsg.msg = false;
                                    jsonMsg.data = "修改失败";
                                }
                                #endregion
                                break;
                            case "delprojectplaninfo":
                                ArrayID = context.Request["arrayid"];
                                if (projectPlanSql.DelProjectPlanInfo(ArrayID))
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = true;
                                    jsonMsg.data = "删除成功";
                                }
                                else
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = false;
                                    jsonMsg.data = "删除失败";
                                }
                                break;
                            #endregion
                            #region 计划大类模块
                            case "getprojectplanclass"://获取计划大类信息
                                dt = projectPlanSql.GetProJectPlanClass(search);
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    project7003HTML = _ROLEPOWER.RULE_Project7003HTML(userModel.RoleID);
                                    proJectPlanClassModels = DataTableToEntity<ProJectPlanClassModel>.ConvertToModel(dt);
                                    for (int i = 0; i < proJectPlanClassModels.Count; i++)
                                    {
                                        proJectPlanClassModels[i].Operate = project7003HTML;
                                    }
                                }
                                jsonMsg.code = 0;
                                jsonMsg.msg = project7003HTML;
                                jsonMsg.count = proJectPlanClassModels.Count;
                                jsonMsg.data = proJectPlanClassModels.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                                break;
                            case "bindselect"://绑定下拉框
                                dt = projectPlanSql.BindSelect();
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    proJectPlanClassModels = DataTableToEntity<ProJectPlanClassModel>.ConvertToModel(dt);
                                }
                                jsonMsg.code = 0;
                                jsonMsg.msg = true;
                                jsonMsg.data = proJectPlanClassModels;
                                break;
                            case "getprojectplanclassbyid"://查询某条计划任务大类信息
                                id = int.Parse(context.Request["id"]);
                                dt = projectPlanSql.GetProJectPlanClassByID(id);
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    proJectPlanClassModels = DataTableToEntity<ProJectPlanClassModel>.ConvertToModel(dt);
                                }
                                jsonMsg.code = 0;
                                jsonMsg.msg = true;
                                jsonMsg.data = proJectPlanClassModels;
                                break;
                            case "addprojectplanclass"://添加计划任务大类信息
                                proJectPlanClassModel.PlanClassName = context.Request["planclassname"];
                                proJectPlanClassModel.Remark = context.Request["remark"];
                                if (projectPlanSql.AddProJectPlanClass(proJectPlanClassModel))
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = true;
                                    jsonMsg.data = "新增成功";
                                }
                                else
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = false;
                                    jsonMsg.data = "新增失败";
                                }
                                break;
                            case "editprojectplanclass"://修改计划任务大类信息
                                proJectPlanClassModel.ID = int.Parse(context.Request["id"]);
                                proJectPlanClassModel.PlanClassName = context.Request["planclassname"];
                                proJectPlanClassModel.Remark = context.Request["remark"];
                                if (projectPlanSql.EditProJectPlanClass(proJectPlanClassModel))
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = true;
                                    jsonMsg.data = "修改成功";
                                }
                                else
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = false;
                                    jsonMsg.data = "修改失败";
                                }
                                break;
                            case "delprojectplanclass"://删除计划任务大类信息
                                ArrayID = context.Request["arrayid"];
                                if (projectPlanSql.DelProJectPlanClass(ArrayID))
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = true;
                                    jsonMsg.data = "删除成功";
                                }
                                else
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = false;
                                    jsonMsg.data = "删除失败";
                                }
                                break;
                            #endregion
                            #region 项目打卡操作模块
                            case "uploadfile"://上传打卡附件
                                if (hfc.Count > 0)
                                {
                                    hpf = context.Request.Files[0];
                                }
                                punchCardFileModel.GUID = Guid.NewGuid().ToString();
                                date = DateTime.Now.ToString("yyyyMMddHHmmss");
                                if (hfc.Count > 0)
                                {
                                    var fileName = hpf.FileName.Insert(hpf.FileName.LastIndexOf('.'), "-" + date);
                                    string tmpGUID = proJectPlanPunchCardModel.GUID;
                                    var filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/ReportTemplate/" + punchCardFileModel.GUID + ""), fileName);
                                    string directoryName = Path.GetDirectoryName(filePath);
                                    if (!Directory.Exists(directoryName))
                                    {
                                        Directory.CreateDirectory(directoryName);
                                    }
                                    bool errormsg = true;
                                    string errormsgstr = "";
                                    try
                                    {
                                        hpf.SaveAs(filePath);
                                    }
                                    catch (Exception ex)
                                    {
                                        errormsg = false;
                                        errormsgstr = ex.Message;
                                    }
                                    if (errormsg)
                                    {
                                        string savetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        punchCardFileModel.FileName = fileName;
                                        //punchCardFileModel.FilePath = "~/ReportTemplate/" + fileName;
                                        punchCardFileModel.FilePath = "http://192.168.1.102/SZRiverSys/" + "ReportTemplate/" + punchCardFileModel.GUID + "/" + hpf.FileName;

                                        punchCardFileModel.CreateDate = savetime;
                                        punchCardFileModel.FileType = Path.GetExtension(hpf.FileName).ToLower().Replace(".", "");
                                        punchCardFileModel.MsgGUID = tmpGUID;

                                        //存储文件列表
                                        if (projectPlanSql.AddRM_PunchCardFile(punchCardFileModel))
                                        {
                                            jsonMsg.code = 0;
                                            jsonMsg.msg = tmpGUID;
                                            jsonMsg.data = "上传图片成功";
                                        }
                                        else
                                        {
                                            jsonMsg.code = 1;
                                            jsonMsg.msg = tmpGUID;
                                            jsonMsg.data = "上传图片失败";
                                        }
                                    }
                                }
                                break;
                            case "savepunchcardinfo"://上传打卡信息
                                proJectPlanPunchCardModel.ProJectPlanID = context.Request["projectplanid"];
                                proJectPlanPunchCardModel.PunchCardDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                proJectPlanPunchCardModel.PunchCardPerson = userModel.userName;
                                proJectPlanPunchCardModel.TodayPlanStatus = int.Parse(context.Request["todayplanstatus"]);
                                proJectPlanPunchCardModel.GUID = context.Request["guid"];
                                if (projectPlanSql.AddPunchCardInfo(proJectPlanPunchCardModel))//存储消息信息
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = true;
                                    jsonMsg.data = "打卡信息上传成功";
                                }
                                else
                                {
                                    jsonMsg.code = 1;
                                    jsonMsg.msg = false;
                                    jsonMsg.data = "打卡信息上传失败";
                                }
                                break;
                            case "todayispunchcard"://判断当天打开与否
                                ProJectPlanID = context.Request["projectplanid"];
                                if (projectPlanSql.TodayIsPunchCard(ProJectPlanID))
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = true;
                                    jsonMsg.data = "今天已经打卡";
                                }
                                else
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = false;
                                    jsonMsg.data = "今天未打卡";
                                }
                                break;
                            case "addpunchcardinfo"://添加打卡记录
                                if (hfc.Count > 0)
                                {
                                    hpf = context.Request.Files[0];
                                }
                                proJectPlanPunchCardModel.ProJectPlanID = context.Request["projectplanid"];

                                proJectPlanPunchCardModel.PunchCardDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                proJectPlanPunchCardModel.PunchCardPerson = userModel.userName;
                                proJectPlanPunchCardModel.TodayPlanStatus = int.Parse(context.Request["todayplanstatus"]);
                                proJectPlanPunchCardModel.GUID = context.Request["guid"];
                                punchCardFileModel.GUID = Guid.NewGuid().ToString();
                                date = DateTime.Now.ToString("yyyyMMddHHmmss");
                                if (hfc.Count > 0)
                                {
                                    var fileName = hpf.FileName.Insert(hpf.FileName.LastIndexOf('.'), "-" + date);
                                    string tmpGUID = proJectPlanPunchCardModel.GUID;
                                    var filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/ReportTemplate/" + punchCardFileModel.GUID + ""), fileName);
                                    string directoryName = Path.GetDirectoryName(filePath);
                                    if (!Directory.Exists(directoryName))
                                    {
                                        Directory.CreateDirectory(directoryName);
                                    }
                                    bool errormsg = true;
                                    string errormsgstr = "";
                                    try
                                    {
                                        hpf.SaveAs(filePath);
                                    }
                                    catch (Exception ex)
                                    {
                                        errormsg = false;
                                        errormsgstr = ex.Message;
                                    }
                                    if (errormsg)
                                    {
                                        string savetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        if (!projectPlanSql.ValidateExistGUID(tmpGUID))
                                        {
                                            //存在即代表多文件上传
                                            projectPlanSql.AddPunchCardInfo(proJectPlanPunchCardModel);//存储消息信息
                                        }
                                        punchCardFileModel.FileName = fileName;
                                        //punchCardFileModel.FilePath = "~/ReportTemplate/" + fileName;
                                        punchCardFileModel.FilePath = "http://192.168.1.102/SZRiverSys/" + "ReportTemplate/" + punchCardFileModel.GUID + "/" + hpf.FileName;

                                        punchCardFileModel.CreateDate = savetime;
                                        punchCardFileModel.FileType = Path.GetExtension(hpf.FileName).ToLower().Replace(".", "");
                                        punchCardFileModel.MsgGUID = tmpGUID;

                                        //存储文件列表
                                        projectPlanSql.AddRM_PunchCardFile(punchCardFileModel);
                                        jsonMsg.code = 0;
                                        jsonMsg.msg = true;
                                        jsonMsg.data = "打卡成功";
                                    }
                                    else
                                    {
                                        jsonMsg.code = 0;
                                        jsonMsg.msg = true;
                                        jsonMsg.data = "文件上传失败,失败原因: " + errormsgstr;
                                    }
                                }
                                else
                                {
                                    //存在即代表多文件上传
                                    projectPlanSql.AddPunchCardInfo(proJectPlanPunchCardModel);//存储消息信息
                                    projectPlanSql.AddRM_PunchCardFile(punchCardFileModel);
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = true;
                                    jsonMsg.data = "打卡成功";
                                };
                                #region 更新项目计划表的PlanStatus
                                //获取打卡的总次数
                                int tmpPunchCardNum = 0;
                                DataTable tmpSqlPunchCard = projectPlanSql.GetPunchCardInfo(proJectPlanPunchCardModel.ProJectPlanID);
                                DataTable tmpSqlCount = projectPlanSql.GetProjectInfoPlanByID(int.Parse(proJectPlanPunchCardModel.ProJectPlanID));
                                if (tmpSqlPunchCard != null && tmpSqlPunchCard.Rows.Count > 0)
                                {
                                    tmpPunchCardNum = dt.Rows.Count;
                                }
                                if (tmpSqlCount != null && tmpSqlCount.Rows.Count > 0)
                                {
                                    if (tmpSqlCount.Rows[0]["PlanStatus"].ToString() != "1")
                                    {
                                        //如果打卡的次数小于计划的总次数的话，则将状态改为进行中
                                        if (tmpPunchCardNum < int.Parse(tmpSqlCount.Rows[0]["Count"].ToString()))
                                        {
                                            projectPlanSql.UpdatePlanStatus(2, proJectPlanPunchCardModel.ProJectPlanID);
                                        }
                                    }
                                }
                                #endregion
                                break;
                            case "getpunchcardinfo"://获取某个计划的打卡详情记录
                                ProJectPlanID = context.Request["projectplanid"];
                                dt = projectPlanSql.GetPunchCardInfo(ProJectPlanID);
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    proJectPlanPunchCardModels = DataTableToEntity<ProJectPlanPunchCardModel>.ConvertToModel(dt);
                                }
                                jsonMsg.code = 0;
                                jsonMsg.msg = true;
                                jsonMsg.data = proJectPlanPunchCardModels;
                                break;
                            case "updatepunchcardstatus"://审批更新当天打卡记录状态
                                proJectPlanPunchCardModel.ID = int.Parse(context.Request["id"]);
                                proJectPlanPunchCardModel.ApprovalCreate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                proJectPlanPunchCardModel.ApprovalStatus = int.Parse(context.Request["approvalstatus"]);
                                proJectPlanPunchCardModel.ApprovalUser = userModel.userName;
                                if (projectPlanSql.UpdatePunchCardStatus(proJectPlanPunchCardModel))
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = true;
                                    jsonMsg.data = "审批成功";
                                }
                                else
                                {
                                    jsonMsg.code = 0;
                                    jsonMsg.msg = false;
                                    jsonMsg.data = "审批失败";
                                }
                                break;
                            case "getmyapprovalinfo"://我要审批的项目计划信息
                                dt = projectPlanSql.GetMyApprovalInfo(userModel);
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    proJectPlanPunchCardModels = DataTableToEntity<ProJectPlanPunchCardModel>.ConvertToModel(dt);
                                }
                                jsonMsg.code = 0;
                                jsonMsg.msg = true;
                                jsonMsg.count = proJectPlanPunchCardModels.Count;
                                jsonMsg.data = proJectPlanPunchCardModels.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                                break;
                            case "approvalinfocount"://我要审批的项目计划信息总数
                                dt = projectPlanSql.GetMyApprovalInfo(userModel);
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    proJectPlanPunchCardModels = DataTableToEntity<ProJectPlanPunchCardModel>.ConvertToModel(dt);
                                }
                                jsonMsg.code = 0;
                                jsonMsg.msg = true;
                                jsonMsg.count = proJectPlanPunchCardModels.Count;
                                jsonMsg.data = proJectPlanPunchCardModels;
                                break;
                            case "getfileimgbyguid"://查找某个打卡记录的附件
                                string guid = context.Request["guid"];
                                //根据打卡记录表的GUID找到所有附件
                                dt = projectPlanSql.GetfileimgbyGUID(guid);
                                if (dt.Rows.Count > 0)
                                {
                                    punchCardFileModels = DataTableToEntity<PunchCardFileModel>.ConvertToModel(dt);
                                }
                                jsonMsg.code = 0;
                                jsonMsg.msg = true;
                                jsonMsg.count = punchCardFileModels.Count;
                                jsonMsg.data = punchCardFileModels;
                                break;
                            #endregion;
                            #region 进度统计
                            case "datareport"://项目进度统计
                                string time = context.Request["time"];
                                ProjectID = context.Request["projectid"];
                                type = context.Request["type"];
                                dt = projectPlanSql.GetProjectPlanInfo("", ProjectID, type);
                                //获取该项目下的项目计划
                                if (dt.Rows.Count > 0)
                                {
                                    projectPlanModels = DataTableToEntity<ProjectPlanModel>.ConvertToModel(dt);
                                    projectDataReportModel.ProjectPlanCount = projectPlanModels.Count;//总计划数
                                    DataRow[] drs2 = dt.Select("PlanStatus = '0' ");
                                    DataRow[] drs3 = dt.Select("PlanStatus = '3' ");
                                    projectDataReportModel.CompletedCount = drs2.Length;//已完成计划数
                                    projectDataReportModel.PunchCardingCount = drs3.Length;//进行中计划数
                                    projectDataReportModel.Unfinished = projectPlanModels.Count - (drs2.Length + drs3.Length);//未完成计划数
                                    projectDataReportModel.DataList = projectPlanModels;//计划表
                                    foreach (ProjectPlanModel item in projectPlanModels)
                                    {
                                        //获取打卡次数
                                        dt = projectPlanSql.GetPunchCardInfo(item.ID.ToString(), type, time);
                                        item.PlanIsOKCount = dt.Rows.Count;//完成的的次数
                                        DateTime tsTmpBenginDateTimed = Convert.ToDateTime(item.BeginDate);
                                        DateTime tsTmpEndDateTimed = Convert.ToDateTime(time);
                                        if (dt.Rows.Count > 0)
                                        {
                                            if (type.Equals("d"))
                                            {
                                                //计算时间差
                                                TimeSpan tsTmpd = new TimeSpan();//时间差 
                                                tsTmpd = tsTmpEndDateTimed.Subtract(tsTmpBenginDateTimed);
                                                item.PlanCount = tsTmpd.Days / int.Parse(dt.Rows[0]["Frequencyt_Value"].ToString());//项目实际完成次数
                                                item.ProjectSchedule = item.PlanIsOKCount / item.PlanCount * 100;//完成进度
                                            }
                                            else if (type.Equals("m"))
                                            {
                                                //计算时间差  
                                                int Month = (tsTmpEndDateTimed.Year - tsTmpBenginDateTimed.Year) * 12 + (tsTmpEndDateTimed.Month - tsTmpBenginDateTimed.Month);
                                                item.PlanCount = Month / int.Parse(dt.Rows[0]["Frequencyt_Value"].ToString());//项目实际完成次数
                                                item.ProjectSchedule = item.PlanIsOKCount / item.PlanCount * 100;//完成进度
                                            }
                                            else if (type.Equals("y"))
                                            {
                                                //计算时间差  
                                                int Year = tsTmpEndDateTimed.Year - tsTmpBenginDateTimed.Year;
                                                item.PlanCount = Year / int.Parse(dt.Rows[0]["Frequencyt_Value"].ToString());//项目实际完成次数
                                                item.ProjectSchedule = item.PlanIsOKCount / item.PlanCount * 100;//完成进度
                                            }
                                        }
                                    }
                                }
                                jsonMsg.code = 0;
                                jsonMsg.msg = projectDataReportModel;
                                jsonMsg.data = projectDataReportModel.DataList;
                                break;
                                #endregion
                        }
                    }
                    else
                    {
                        jsonMsg.code = -1;
                        jsonMsg.msg = false;
                        jsonMsg.data = "用户登录失效";
                    }
                }

            }
            catch (Exception)
            {
                jsonMsg.code = 1;
                jsonMsg.msg = false;
                jsonMsg.data = "服务器程序错误";
            }
            finally
            {
                System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                context.Response.ContentType = "text/plain; charset=utf-8";
                context.Response.Write(js.Serialize(jsonMsg));//构造json数据格式
                context.Response.End();
                context.Response.Close();
            }

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