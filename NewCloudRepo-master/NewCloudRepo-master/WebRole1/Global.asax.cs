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
                var iisSiteName = System.Web.Hosting.HostingEnvironment.SiteName;
                Configuration config = null;
                List<string> allowedIps = new List<string>();
                
                // running webrole1
                if (!RoleEnvironment.IsAvailable)
                {
                    var virtualPath = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
                    config = serverManager.GetWebConfiguration(iisSiteName, virtualPath);
                }
                // running local cloud service
                else if (RoleEnvironment.IsEmulated)
                {
                    var roleName = RoleEnvironment.CurrentRoleInstance.Role.Name;

                    allowedIps = RoleEnvironment.GetConfigurationSettingValue("AllowedIps")
                        .Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(str => str.Trim()).Where(s => s != string.Empty).ToList();
                    //TODO: find a way to get Default Web Site Dynamically
                    config = serverManager.GetWebConfiguration("Default Web Site", roleName);
                }
                //running azure 
                else
                {
                    var siteId = RoleEnvironment.CurrentRoleInstance.Id;
                    allowedIps = RoleEnvironment.GetConfigurationSettingValue("AllowedIps").Split(',').ToList();
                    config = serverManager.GetWebConfiguration(siteId + "_Web");
                }

                ConfigurationSection ipSecuritySection = config.GetSection("system.webServer/security/ipSecurity");
                ConfigurationElementCollection ipSecurityCollection = ipSecuritySection.GetCollection();    
                ipSecurityCollection.Clear();
                allowedIps.Add("127.0.0.1");
                foreach (var allowedIp in allowedIps)
                {
                    ConfigurationElement addElement = ipSecurityCollection.CreateElement("add");
                    var list = allowedIp.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(str => str.Trim()).Where(s => s != string.Empty).ToList();
                    addElement["ipAddress"] = list[0];
                    addElement["allowed"] = true;
                    if (list.Count > 1)
                        addElement["subnetMask"] = list[1];
                    ipSecurityCollection.Add(addElement);
                }

                serverManager.CommitChanges();
            }
        }
    }
}
