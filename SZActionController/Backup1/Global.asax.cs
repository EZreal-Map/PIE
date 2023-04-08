using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using cn.ctgu.webgis.Config;
using cn.ctgu.webgis.Base.PMSManager.Component;
using cn.ctgu.webgis.Utils;

namespace ActionController
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            try
            {
                // 配置application对象,在启动的时候加载log4net配置.
                log4net.Config.DOMConfigurator.Configure();

                PMSConfig.InitPMSConfig(this.Context.Request.PhysicalApplicationPath);
                ObjectFactory.InitObjectFactory(this.Context.Request.PhysicalApplicationPath);
                ConvertBeanUtils.Init();
                reConfigDatasource.InitConfigDatasource(this.Context.Request.PhysicalApplicationPath);

            }
            catch (Exception ex)
            {
                throw new Exception("数据字典初始化失败，请重新启动WWW服务！\n（原因:" + ex.Message + "）");
            }
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}