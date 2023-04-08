using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
namespace SZRiverSys.SQL
{
    public class MaterialSql
    {
        #region 人员基本信息管理
        /// <summary>
        /// 获取巡河人员基本信息列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="search"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public DataTable GetPatrolUserList(int pageindex, int pagesize, string search, out int total)
        {
            string where = "1=1";
            if (search != "" && search != null)
            {
                where += " and Name like '" + search + "%'";
            }
            DataSet ds = new DataSet();
            ds = DBHelper.DBHelperMsSql.GetByPage(@"New_PatrolUserInfo",//查询的表
                                                    "*",//显示的字段
                                                    "ID",
                                                    pagesize,
                                                    pageindex,
                                                    where,
                                                    "ID",
                                                    out total);
            return ds.Tables[0];
        }
        #endregion

        #region 车辆管理

        #endregion


        #region 物资管理
        /// <summary>
        /// 获取物资分页列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="search"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public DataTable GetMaterialList(int pageindex, int pagesize, string search, string type, out int total)
        {
            string where = "1=1 and Type='" + type + "'";
            if (search != "" && search != null)
            {
                where += " and Name like '" + search + "%'";
            }
            DataSet ds = new DataSet();
            ds = DBHelper.DBHelperMsSql.GetByPage(@"RM_MaterialInfo",//查询的表
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
        /// 获取物资分类
        /// </summary>
        /// <returns></returns>
        public DataTable GetMaterialType()
        {
            string tmpsql = "select * from [dbo].[RM_MaterialType]";
            DataTable dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 获取单个物资
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetMaterialByID(string id)
        {
            DataTable dt = new DataTable();
            string tmpsql = "select * from [dbo].[RM_MaterialInfo] where ID=" + id + "";
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 添加物资信息
        /// </summary>
        /// <param name="de"></param>
        /// <returns></returns>
        public bool AddMaterialInfo(Model.MaterialModel de)
        {
            string date = DateTime.Now.ToLocalTime().ToString();
            string tmpsql = string.Format(@"insert into RM_MaterialInfo(Type,Name,Number,SavePlace,Manager,Subordinate,longitude,latitude,projectid,state,CreateTime) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',getdate())", de.Type, de.Name, de.Number,de.SavePlace, de.Manager, de.Subordinate, de.longitude,de.latitude,de.projectid,de.state);
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
        /// 修改物资信息
        /// </summary>
        /// <param name="de"></param>
        /// <returns></returns>
        public bool EditMaterial(Model.MaterialModel de)
        {
            string tmpsql = string.Format(@"update [dbo].[RM_MaterialInfo] set Name='{0}',Number='{1}',SavePlace='{2}',Manager='{3}',longitude='{4}',latitude='{5}',state='{6}',Subordinate='{7}' where ID={8}", de.Name, de.Number, de.SavePlace, de.Manager,de.longitude,de.latitude,de.state, de.Subordinate,de.ID);
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
        /// 删除物资
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Del_MaterialInfo(string id)
        {
            string tmpsql = "delete RM_MaterialInfo where ID in(" + id + ")";
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
        /// 添加物资使用记录
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool AddMaterUserLog(Model.MaterialUserLog m)
        {
            string tmpsql = string.Format("insert into [dbo].[RM_MaterialUserLog](MID,LogTime,UserId,[Content]) values('{0}',GETDATE(),'{1}','{2}');",m.MID,m.UserId,m.Content);
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
        /// 获取物资的使用记录
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public DataTable GetMaterUserLog(string ID)
        {
            string tmpsql = "select a.*,b.userName from [dbo].[RM_MaterialUserLog] a left join Sys_Accounts_Users b on a.UserId = b.UserID where MID = '"+ID+"'";
            DataTable dt=DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        #endregion
        #region 其他分页管理

        /// <summary>
        /// 设备分页列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="search"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public DataTable GetDevicelList(int pageindex, int pagesize, string search, out int total)
        {
            string where = "1=1";
            if (search != "" && search != null)
            {
                where += " and DeviceName like '" + search + "%'";
            }
            DataSet ds = new DataSet();
            ds = DBHelper.DBHelperMsSql.GetByPage(@"New_DeviceInfo",//查询的表
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
        /// 车辆分页列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="search"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public DataTable GetCarList(int pageindex, int pagesize, string search, out int total)
        {
            string where = "1=1";
            if (search != "" && search != null)
            {
                where += " and CarNumber like '" + search + "%'";
            }
            DataSet ds = new DataSet();
            ds = DBHelper.DBHelperMsSql.GetByPage(@"New_Carinfo",//查询的表
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
        /// 获取办公室分页列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="search"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public DataTable GetOfficeList(int pageindex, int pagesize, string search, out int total)
        {
            string where = "1=1";
            if (search != "" && search != null)
            {
                where += " and Name like '" + search + "%'";
            }
            DataSet ds = new DataSet();
            ds = DBHelper.DBHelperMsSql.GetByPage(@"New_OfficeInfo",//查询的表
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
        /// 办公室分页列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="search"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public DataTable GetDormitoryList(int pageindex, int pagesize, string search, out int total)
        {
            string where = "1=1";
            if (search != "" && search != null)
            {
                where += " and Name like '" + search + "%'";
            }
            DataSet ds = new DataSet();
            ds = DBHelper.DBHelperMsSql.GetByPage(@"New_DormitoryInfo",//查询的表
                                                    "*",//显示的字段
                                                    "ID",
                                                    pagesize,
                                                    pageindex,
                                                    where,
                                                    "ID",
                                                    out total);
            return ds.Tables[0];
        }
        #endregion

        #region  新增物资操作
        /// <summary>
        /// 添加人员信息
        /// </summary>
        /// <param name="de"></param>
        /// <returns></returns>
        public bool AddPatrolUserInfo(Model.PatrolUserInfo de)
        {
            string tmpsql = string.Format(@"insert into New_PatrolUserInfo(Name,sex,age,NativePlace,Education,address) values('{0}','{1}','{2}','{3}','{4}','{5}')", de.Name, de.sex, de.age, de.NativePlace, de.Education, de.address);
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
        /// 添加车辆
        /// </summary>
        /// <param name="de"></param>
        /// <returns></returns>
        public bool AddCarInfo(Model.CarinfoModel de)
        {
            string tmpsql = string.Format(@"insert into New_Carinfo(CarNumber,CarType,brand,CarName,Remark,CreateTime,Propertyer,[User],Kilometres) values('{0}','{1}','{2}','{3}','{4}',getdate(),'{5}','{6}','{7}')", de.CarNumber, de.CarType, de.brand, de.CarName, de.Remark, de.Propertyer, de.User, de.Kilometres);
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
        /// 添加设备信息
        /// </summary>
        /// <param name="de"></param>
        /// <returns></returns>
        public bool AddDeviceInfo(Model.DeviceInfoModel de)
        {
            string date = DateTime.Now.ToLocalTime().ToString();
            string tmpsql = string.Format(@"insert into New_DeviceInfo(DeviceType,DeviceName,Number,SavePlace,Manager,UserLogin,Subordinate) values('{0}','{1}','{2}','{3}','{4}','{6}','{5}')", de.DeviceType, de.DeviceName, de.Number, de.SavePlace, de.Manager, de.Subordinate,date);
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
        /// 添加宿舍信息
        /// </summary>
        /// <param name="de"></param>
        /// <returns></returns>
        public bool AddDormitoryInfo(Model.DormitoryInfoModel de)
        {
            string tmpsql = string.Format(@"insert into New_DormitoryInfo(Name,Area,OfficerNumber,Manager,Rent) values('{0}','{1}','{2}','{3}','{4}')", de.Name, de.Area, de.OfficerNumber, de.Manager, de.Rent);
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
        /// 添加办公室信息
        /// </summary>
        /// <param name="de"></param>
        /// <returns></returns>
        public bool AddOfficeInfo(Model.OfficeInfo de)
        {
            string tmpsql = string.Format(@"insert into New_OfficeInfo(Name,Area,OfficerNumber,Manager,Rent) values('{0}','{1}','{2}','{3}','{4}')", de.Name, de.Area, de.OfficerNumber,de.Manager, de.Rent);
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

        #endregion

        #region 获取单个信息操作
        /// <summary>
        /// 获取单个设备信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetDeviceByID(string id)
        {
            DataTable dt = new DataTable();
            string tmpsql = "select * from [dbo].[New_DeviceInfo] where ID=" + id + "";
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 获取单个巡河人员
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetPatrolUserByID(string id)
        {
            DataTable dt = new DataTable();
            string tmpsql = "select * from [dbo].[New_PatrolUserInfo] where ID=" + id + "";
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 获取单个车辆信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetCarInfoByID(string id)
        {
            DataTable dt = new DataTable();
            string tmpsql = "select * from [dbo].[New_Carinfo] where ID=" + id + "";
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        /// <summary>
        /// 获取单个宿舍信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetDormitoryByID(string id)
        {
            DataTable dt = new DataTable();
            string tmpsql = "select * from [dbo].[New_DormitoryInfo] where ID=" + id + "";
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
  
        /// <summary>
        /// 获取单个办公室
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetOfficeInfoByID(string id)
        {
            DataTable dt = new DataTable();
            string tmpsql = "select * from [dbo].[New_OfficeInfo] where ID=" + id + "";
            dt = DBHelper.DBHelperMsSql.ExecuteDataTable(tmpsql);
            return dt;
        }
        #endregion
        #region 修改操作
        /// <summary>
        /// 修改设备信息
        /// </summary>
        /// <returns></returns>
        public bool EditDevice(Model.DeviceInfoModel de)
        {
            string tmpsql = string.Format(@"update [dbo].[New_DeviceInfo] set DeviceType='{0}',DeviceName='{1}',Number='{2}',SavePlace='{3}',Manager='{4}',UserLogin='{5}',Subordinate='{6}' where ID={7}", de.DeviceType, de.DeviceName, de.Number, de.SavePlace, de.Manager,de.UserLogin,de.Subordinate,de.ID);
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
        /// 修改巡河人员信息
        /// </summary>
        /// <param name="de"></param>
        /// <returns></returns>
        public bool EditPatrolUser(Model.PatrolUserInfo de)
        {
            string tmpsql = string.Format(@"update [dbo].[New_PatrolUserInfo] set Name='{0}',sex='{1}',age='{2}',NativePlace='{3}',Education='{4}',address='{5}' where ID={6}", de.Name, de.sex, de.age, de.NativePlace, de.Education, de.address, de.ID);
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
        /// 修改车辆信息
        /// </summary>
        /// <param name="de"></param>
        /// <returns></returns>
        public bool EditCarInfo(Model.CarinfoModel de)
        {
            string tmpsql = string.Format(@"update [dbo].[New_Carinfo] set CarNumber='{0}',CarType='{1}',brand='{2}',CarName='{3}',Remark='{4}',Propertyer='{5}',User='{6}',Kilometres='{7}' where ID={8}", de.CarNumber, de.CarType, de.brand, de.CarName, de.Remark, de.Propertyer, de.User,de.Kilometres,de.ID);
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
        /// 修改宿舍信息
        /// </summary>
        /// <param name="de"></param>
        /// <returns></returns>
        public bool EditDormitory(Model.DormitoryInfoModel de)
        {
            string tmpsql = string.Format(@"update [dbo].[New_DormitoryInfo] set Name='{0}',Area='{1}',OfficerNumber='{2}',Manager='{3}',Rent='{4}' where ID={5}", de.Name, de.Area, de.OfficerNumber, de.Manager, de.Rent, de.ID);
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
        /// 修改办公室信息
        /// </summary>
        /// <param name="de"></param>
        /// <returns></returns>
        public bool EditOfficeInfo(Model.OfficeInfo de)
        {
            string tmpsql = string.Format(@"update [dbo].[New_OfficeInfo] set Name='{0}',Area='{1}',OfficerNumber='{2}',Manager='{3}',Rent='{4}' where ID={5}", de.Name, de.Area, de.OfficerNumber, de.Manager, de.Rent, de.ID);
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

        #region 删除操作
        /// <summary>
        /// 删除设备信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Del_Device(string id)
        {
            string tmpsql = "delete New_DeviceInfo where ID in(" + id + ")";
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
        /// 删除车辆信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Del_Car(string id)
        {
            string tmpsql = "delete New_Carinfo where ID in(" + id + ")";
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
        /// 删除宿舍信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Del_DormitoryInfo(string id)
        {
            string tmpsql = "delete New_DormitoryInfo where ID in(" + id + ")";
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
        /// 删除办公室信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Del_OfficeInfo(string id)
        {
            string tmpsql = "delete New_OfficeInfo where ID in(" + id + ")";
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
       
        public bool Del_PatrolUserInfo(string id)
        {
            string tmpsql = "delete New_PatrolUserInfo where ID in(" + id + ")";
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