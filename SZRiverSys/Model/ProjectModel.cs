using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SZRiverSys.Model
{
    public class ProjectModel
    {
        public ProjectModel()
        {
            this.ProJectNo = DateTime.Now.ToString("yyyyMMddHHmmss");
            this.CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.BeginDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.EndDate = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 唯一ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProJectName { get; set; }

        /// <summary>
        /// 项目编号
        /// </summary>
        public string ProJectNo { get; set; }


        /// <summary>
        /// 项目编号
        /// </summary>
        public string ProjectTeamMembers { get; set; }

        /// <summary>
        /// 负责人名称
        /// </summary>
        public string PM { get; set; }

        /// <summary>
        /// 负责人电话
        /// </summary>
        public string PM_Tel { get; set; }

        /// <summary>
        /// 负责人职位
        /// </summary>
        public string PM_Job { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }


        /// <summary>
        /// 修改时间
        /// </summary>
        public string ModifyDate { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifyUser { get; set; }


        /// <summary>
        /// 开始时间
        /// </summary>
        public string BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// 项目位置
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 甲方单位
        /// </summary>
        public string PartyAUnit { get; set; }

        /// <summary>
        /// 甲方负责人姓名
        /// </summary>
        public string PartyAPersonName { get; set; }

        /// <summary>
        /// 甲方负责人电话
        /// </summary>
        public string PartyAPersonTel { get; set; }

        /// <summary>
        /// 甲方负责人职务
        /// </summary>
        public string PartyAPersonJob { get; set; }

        /// <summary>
        /// 乙方单位
        /// </summary>
        public string PartyBUnit { get; set; }

        /// <summary>
        /// 乙方负责人姓名
        /// </summary>
        public string PartyBPersonName { get; set; }

        /// <summary>
        /// 乙方负责人电话
        /// </summary>
        public string PartyBPersonTel { get; set; }

        /// <summary>
        /// 乙方负责人职务
        /// </summary>
        public string PartyBPersonJob { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public Project7001HTML Operate { get; set; }//角色
    }


    public class ProjecUsrClass
    {
        public int UserID { get; set; }
        public string userName { get; set; }
        public string tel { get; set; }
        public string EmailAddress { get; set; }
        public string Company { get; set; }
        public string UserSex { get; set; }
        public string Age { get; set; }
        public string remark { get; set; }
        public string usercode { get; set; }
    }
}