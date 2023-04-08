using System;
using System.Text;

using cn.ctgu.webgis.Base;
using cn.ctgu.webgis.Accounts.Business;

namespace cn.ctgu.webgis.WebUtils
{
    /// <summary>
    /// RightControl 的摘要说明。
    /// </summary>
    public abstract class RightControl
    {
        public static bool CheckPermission(System.Web.UI.Page page)
        {
            
            //SitePrincipal userInfo = (SitePrincipal)page.Session["UserInfo"];

            SitePrincipal userInfo = page.Session["UserInfo"] as SitePrincipal;
            if (userInfo == null)
            {
                page.Response.Write("<h3>用户没有登录或HTTP会话超时，请点击下面链接重新登录！<h3><p>" +
                    "<a href=\"javascript:document.location.href='../../../index.htm';opener.top.close();\" " +
                    "target=\"_blank\">进入登录页面</a>");
                page.Response.End();
                return false;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("<script language=\"javascript\">\n");
            sb.Append("<!--\n");
            sb.Append("var userId = \"" + userInfo.Identity.Name + "\";\n");
            sb.Append("//-->\n");
            sb.Append("</script>\n");
            page.RegisterClientScriptBlock("taskConfigs", sb.ToString());

            return true;
        }
    }
}
