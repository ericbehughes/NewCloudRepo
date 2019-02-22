//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.Web.Administration;
//using Microsoft.WindowsAzure;
//using Microsoft.WindowsAzure.Diagnostics;
//using Microsoft.WindowsAzure.ServiceRuntime;

//namespace WebRole1
//{
//    public class WebRole : RoleEntryPoint
//    {
//        public override bool OnStart()
//        {
//            using (ServerManager serverManager = new ServerManager())
//            {
//                var iisSiteName = System.Web.Hosting.HostingEnvironment.SiteName;
//                Configuration config = null;
//                List<string> allowedIps = new List<string>();
//                // running webrole1
//                if (!RoleEnvironment.IsAvailable)
//                {
//                    var virtualPath = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
//                    config = serverManager.GetWebConfiguration(iisSiteName, virtualPath);
//                }
//                // running local cloud service
//                else if (RoleEnvironment.IsEmulated)
//                {
//                    var roleName = RoleEnvironment.CurrentRoleInstance.Role.Name;
//                    allowedIps = RoleEnvironment.GetConfigurationSettingValue("AllowedIps").Split(',').ToList();
//                    //TODO: find a way to get Default Web Site Dynamically
//                    config = serverManager.GetWebConfiguration("Default Web Site", roleName);
//                }
//                //running azure 
//                else
//                {
//                    var siteId = RoleEnvironment.CurrentRoleInstance.Id;
//                    allowedIps = RoleEnvironment.GetConfigurationSettingValue("AllowedIps").Split(',').ToList();
//                    config = serverManager.GetWebConfiguration(siteId + "_Web");
//                }

//                ConfigurationSection ipSecuritySection = config.GetSection("system.webServer/security/ipSecurity");
//                ConfigurationElementCollection ipSecurityCollection = ipSecuritySection.GetCollection();
//                ipSecurityCollection.Clear();
//                allowedIps.Add("127.0.0.1");
//                foreach (var allowedIp in allowedIps)
//                {
//                    ConfigurationElement addElement = ipSecurityCollection.CreateElement("add");
//                    addElement["ipAddress"] = allowedIp;
//                    addElement["allowed"] = true;
//                    ipSecurityCollection.Add(addElement);
//                }

//                serverManager.CommitChanges();
//            }
//            return base.OnStart();
//        }
//    }
//}