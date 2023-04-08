using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.SessionState;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;


using System.Collections;
using System.ComponentModel;
using System.Reflection;

using cn.ctgu.webgis.Base.PMSManager.Component;
using cn.ctgu.webgis.Data;
using cn.ctgu.webgis.Config;
/// <summary>
/// Upload handler for uploading files.
/// </summary>
public class Upload : IHttpHandler, IRequiresSessionState
{
    public Upload()
    {
    }

    #region IHttpHandler Members

    public bool IsReusable
    {
        get { return true; }
    }
    public const string MODULENAME = "Base";
    private static string types;
    public void ProcessRequest(HttpContext context)
    {

        string uploadPath = "ComponentUpload";
        string fileName = "";
        for (int j = 0; j < context.Request.Files.Count; j++)
        {
            HttpPostedFile uploadFile = context.Request.Files[j];
            if (uploadFile.ContentLength > 0)
            {
                fileName = uploadFile.FileName;
                string PhysicalPath = context.Server.MapPath(uploadPath);

                try
                {
                    uploadFile.SaveAs(Path.Combine(PhysicalPath, uploadFile.FileName));
                }
                catch (Exception e)
                {
                    context.Response.Write("{success:true,msg:'Possible file upload attack!'}");
                    return;
                }
                /*
                try
                {
                    Assembly assembly = Assembly.LoadFrom(Path.Combine(PhysicalPath, uploadFile.FileName));
                    string assemblyFullName = assembly.FullName;

                    if (ComponentManager.HaveExisted(assemblyFullName))
                    {
                        Response.Write("<script language='javascript'>alert(\"该组件已经注册！\");</script>");
                        return;
                    }
                    else
                    {
                        string func = "";// this.tb_func.Text.Trim();
                        int TypeId = Convert.ToInt32(context.Request.QueryString["id"]);
                        string storePath = "/" + assemblyFullName + ".dll";
                        ComponentManager.RegComponent(assemblyFullName, storePath, func, TypeId);

                        storePath = Server.MapPath("../ComponentBase" + Request.QueryString["path"].ToString() + storePath);
                        File.Move(Path.Combine(PhysicalPath, uploadFile.FileName), storePath);

                        compNo = Convert.ToInt32(SQLUtils.ExecuteScalar("pms", PMSConfig.GetCommand(MODULENAME, "GetCid")));

                        Type[] types_arr = assembly.GetTypes();
                        types = types_arr[0].FullName;
                        for (int i = 1; i < types_arr.Length; i++)
                        {
                            types += "#" + types_arr[i].FullName;
                        }

                    }
                }

                catch 
                {
                    Response.Write("<script language='javascript'>alert('注册组件失败！请检查您选择的数据格式是否正确！');</script>");
                    return;
                }
                 */
            }
            context.Response.Write("{success:true,msg:'文件已经上传成功.',file:'" + fileName + "'}");
        }
    }

    #endregion
}
