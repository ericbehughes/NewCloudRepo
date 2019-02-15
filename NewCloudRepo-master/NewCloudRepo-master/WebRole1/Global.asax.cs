using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Web.Administration;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace WebRole1
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            using (ServerManager serverManager = new ServerManager())
            {
                Configuration config = serverManager.GetWebConfiguration("WebRole1_IN_0_Web");
                var siteName = RoleEnvironment.CurrentRoleInstance.Role.Name;
                //Configuration config = serverManager.GetWebConfiguration("Default Web Site", siteName);
                ConfigurationSection ipSecuritySection = config.GetSection("system.webServer/security/ipSecurity");
                ConfigurationElementCollection ipSecurityCollection = ipSecuritySection.GetCollection();
                ConfigurationElement addElement = ipSecurityCollection.CreateElement("add");

                //get allowed ips
                var allowedIps = RoleEnvironment.GetConfigurationSettingValue("AllowedIps").Split(',');
                foreach (var allowedIp in allowedIps)
                {
                    addElement["ipAddress"] = allowedIp;
                    addElement["allowed"] = true;
                    try
                    {
                        ipSecurityCollection.Add(addElement);
                    }
                    catch
                    {

                    }

                }
                serverManager.CommitChanges();
            }
        }
    }
}
