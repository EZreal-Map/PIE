using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using SZRiverSys.Model;
using SZRiverSys.CacheHelper;
using SZRiverSys.Comm;
namespace SZRiverSys.ServiceHandler
{
    /// <summary>
    /// MaterialManage 的摘要说明
    /// </summary>
    public class MaterialManage : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string msg = "{\"code\":1,\"success\":false,\"msg\":\"请求参数不对\"}";
            string method = context.Request["method"];
            SQL.MaterialSql sql = new SQL.MaterialSql();
            DataTable dt;
            JSON.JsonHelper Json = new JSON.JsonHelper();
            #region 实例化实体类
            List<MaterialModel> materialModels = new List<Model.MaterialModel>();//物资列表集合
            MaterialModel materialModel = new MaterialModel();
            #endregion

            #region  实例化权限模块
            Device2003HTML device2003HTML = new Device2003HTML();
            RolePowerHelper _ROLEPOWER = new RolePowerHelper();
            #endregion
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            string json = "";
            if (method != null)
            {
                string token = context.Request.Params["token"] == null ? "" : context.Request.Params["token"];
                Model.UserModel tmpUser = CacheHelper.CacheHelper.Get(token) as Model.UserModel;
                #region 物资管理接口
                if (CacheHelper.CacheHelper.IsContain(token))
                {
                    if (method == "getmateriallist")//获取物资分页列表
                    {
                        int pageindex = int.Parse(context.Request["pageindex"]);
                        int pagesize = int.Parse(context.Request["pagesize"]);
                        string search = context.Request["search"];
                        string type = context.Request["type"];
                        int total = 0;
                        dt = sql.GetMaterialList(pageindex, pagesize, search, type, out total);
                        device2003HTML = _ROLEPOWER.RULE_Device2003HTML(tmpUser.RoleID);
                        if (dt != null && dt.Rows.Count > 0)
                        {    
                            materialModels = DataTableToEntity<MaterialModel>.ConvertToModel(dt);
                            for (int i = 0; i < materialModels.Count; i++)
                            {
                                materialModels[i].Operate = device2003HTML;
                            }
                            msg = "{\"code\":0,\"success\":true,\"data\":" +js.Serialize(materialModels.Skip((pageindex - 1) * pagesize).Take(pagesize)) + ",\"total\":" + total + ",\"msg\":" +js.Serialize( device2003HTML) + "}";
                        }
                        else
                        {
                            msg = "{\"code\":0,\"success\":false,\"data\":[],\"msg\":" + js.Serialize(device2003HTML) + "}";
                        }
                    }
                    else if (method == "getmaterialtype")//获取物资类型
                    {
                        dt = sql.GetMaterialType();
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
                    else if (method == "addmaterialinfo")//添加物资信息接口
                    {
                        Model.MaterialModel m = new Model.MaterialModel();
                        m.Name = context.Request["Name"];
                        m.Type = context.Request["Type"];
                        m.Number = context.Request["Number"];
                        m.SavePlace = context.Request["SavePlace"];
                        m.Manager = context.Request["Manager"];
                        m.Subordinate = context.Request["Subordinate"];
                        m.projectid = tmpUser.projectid;
                        m.longitude = context.Request["longitude"];
                        m.latitude = context.Request["latitude"];
                        m.state = "";
                        if (sql.AddMaterialInfo(m))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"添加成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"添加失败\"}";
                        }
                    }
                    else if (method == "getonematerial")//获取单个物资
                    {
                        string ID = context.Request["ID"];
                        dt = sql.GetMaterialByID(ID);
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
                    else if (method == "editmaterial")//修改物资
                    {
                        Model.MaterialModel m = new Model.MaterialModel();
                        m.ID = context.Request["ID"];
                        m.Name = context.Request["Name"];
                        m.Number = context.Request["Number"];
                        m.SavePlace = context.Request["SavePlace"];
                        m.Manager = context.Request["Manager"];
                        m.Subordinate = context.Request["Subordinate"];
                        m.longitude = context.Request["longitude"];
                        m.latitude = context.Request["latitude"];
                        m.state = context.Request["state"];
                        if (sql.EditMaterial(m))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"修改成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"修改失败\"}";
                        }
                    }
                    else if (method == "delmaterial")//删除物资
                    {
                        string ID = context.Request["ID"];
                        if (sql.Del_MaterialInfo(ID))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"删除成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"删除失败\"}";
                        }
                    }
                    else if (method == "addmaterialuserlog")//添加物资使用记录
                    {
                        Model.MaterialUserLog m = new Model.MaterialUserLog();
                        m.MID = context.Request["MID"];
                        m.UserId = tmpUser.UserID.ToString();
                        m.Content = context.Request["Content"];
                        if (sql.AddMaterUserLog(m))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"添加成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"添加失败\"}";
                        }
                    }
                    else if (method == "getmaterialuserlog")//获取物资使用记录
                    {
                        string ID = context.Request["ID"];
                        dt = sql.GetMaterUserLog(ID);
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
                    #region 其他列表
                    else if (method == "getcarlist")//获取车辆列表
                    {
                        int pageindex = int.Parse(context.Request["pageindex"]);
                        int pagesize = int.Parse(context.Request["pagesize"]);
                        string search = context.Request["search"];
                        int total = 0;
                        dt = sql.GetCarList(pageindex, pagesize, search, out total);
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
                    else if (method == "getdevicelist")//获取设备列表
                    {
                        int pageindex = int.Parse(context.Request["pageindex"]);
                        int pagesize = int.Parse(context.Request["pagesize"]);
                        string search = context.Request["search"];
                        int total = 0;
                        dt = sql.GetDevicelList(pageindex, pagesize, search, out total);
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
                    else if (method == "getdormitorylist")//获取宿舍列表
                    {
                        int pageindex = int.Parse(context.Request["pageindex"]);
                        int pagesize = int.Parse(context.Request["pagesize"]);
                        string search = context.Request["search"];
                        int total = 0;
                        dt = sql.GetDormitoryList(pageindex, pagesize, search, out total);
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
                    else if (method == "getofficelist")//获取办公室信息列表
                    {
                        int pageindex = int.Parse(context.Request["pageindex"]);
                        int pagesize = int.Parse(context.Request["pagesize"]);
                        string search = context.Request["search"];
                        int total = 0;
                        dt = sql.GetOfficeList(pageindex, pagesize, search, out total);
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
                    #endregion
                    #region  添加设备信息
                    else if (method == "adddeviceinfo")
                    {
                        Model.DeviceInfoModel m = new Model.DeviceInfoModel();
                        m.DeviceName = context.Request["DeviceName"];
                        m.DeviceType = context.Request["DeviceType"];
                        m.Number = context.Request["Number"];
                        m.SavePlace = context.Request["SavePlace"];
                        m.Manager = context.Request["Manager"];
                        m.Subordinate = context.Request["Subordinate"];
                        if (sql.AddDeviceInfo(m))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"添加成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"添加失败\"}";
                        }
                    }
                    #endregion
                    #region   添加车辆信息
                    else if (method == "addcarinfo")
                    {
                        Model.CarinfoModel m = new Model.CarinfoModel();
                        m.CarNumber = context.Request["CarNumber"];
                        m.CarName = context.Request["CarName"];
                        m.brand = context.Request["brand"];
                        m.CarType = context.Request["CarType"];
                        m.Kilometres = context.Request["Kilometres"];
                        m.Propertyer = context.Request["Propertyer"];
                        m.Remark = context.Request["Remark"];
                        m.User = context.Request["User"];
                        if (sql.AddCarInfo(m))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"添加成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"添加失败\"}";
                        }
                    }
                    #endregion
                    #region 添加办公室,宿舍信息
                    else if (method == "addofficeinfo")
                    {
                        Model.OfficeInfo m = new Model.OfficeInfo();
                        m.Area = context.Request["Area"];
                        m.Manager = context.Request["Manager"];
                        m.Name = context.Request["Name"];
                        m.OfficerNumber = context.Request["OfficerNumber"];
                        m.Rent = context.Request["Rent"];
                        if (sql.AddOfficeInfo(m))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"添加成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"添加失败\"}";
                        }
                    }
                    else if (method == "adddormitory")
                    {
                        Model.DormitoryInfoModel m = new Model.DormitoryInfoModel();
                        m.Area = context.Request["Area"];
                        m.Manager = context.Request["Manager"];
                        m.Name = context.Request["Name"];
                        m.OfficerNumber = context.Request["OfficerNumber"];
                        m.Rent = context.Request["Rent"];
                        if (sql.AddDormitoryInfo(m))
                        {
                            msg = "{\"code\":0,\"success\":true,\"msg\":\"添加成功\"}";
                        }
                        else
                        {
                            msg = "{\"code\":1,\"success\":false,\"msg\":\"添加失败\"}";
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

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}