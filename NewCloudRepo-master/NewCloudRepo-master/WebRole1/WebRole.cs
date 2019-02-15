using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Web.Administration;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace WebRole1
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.
            using (ServerManager serverManager = new ServerManager())
            {
                //Configuration config = serverManager.GetWebConfiguration("WebRole1_IN_0_Web");
                var siteName =RoleEnvironment.CurrentRoleInstance.Role.Name;
               Configuration config = serverManager.GetWebConfiguration("Default Web Site", siteName);
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
            return base.OnStart();
        }
    }
}
