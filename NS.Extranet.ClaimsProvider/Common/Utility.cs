using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.SharePoint;
using NS.Extranet.ClaimsProvider.Common;

namespace NS.Extranet.ClaimsProvider.Commmon
{
    public class Utility
    {
        private static string GetTenantPropertyBagValue(string key)
        {
            string value = string.Empty;
            SPSite currentSite = SPContext.Current.Site;

            SPSiteSubscription sub = currentSite.SiteSubscription;
            Guid tenantAdminSiteId;
            bool isSuccess = SPTenantAdmin.TryGetTenantAdministrationSiteGuid(sub,
                out tenantAdminSiteId);
            if (isSuccess)
            {
                SPSite tenantAdminSite
                    = sub.GetSites(currentSite.WebApplication).Where(s => s.ID == tenantAdminSiteId) as SPSite;
                //SPSite tenantAdminSite=  currentSite.WebApplication.Sites[tenantAdminSiteId];
                if (null != tenantAdminSite)
                {
                    SPWeb rootWeb = tenantAdminSite.RootWeb;
                    if (rootWeb.AllProperties.ContainsKey(key))
                    {
                        value = Convert.ToString(rootWeb.GetProperty(key));
                    }
                }
            }

            return value;
        }
        public static string GetTenantPropertyBagValue(string key, bool isElevated)
        {
            string value = string.Empty;
            if (isElevated)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    value = GetTenantPropertyBagValue(key);
                });
            }
            else
            {
                value = GetTenantPropertyBagValue(key);
            }
            return value;
        }
        private static string GetPropertyBagValue(string key)
        {
            string value = string.Empty;
            SPSite currentSite = SPContext.Current.Site;
            SPWeb rootWeb = currentSite.RootWeb;
            if (rootWeb.AllProperties.ContainsKey(key))
            {
                value = Convert.ToString(rootWeb.GetProperty(key));
            }
            return value;
        }
        public static string GetPropertyBagValue(string key, bool isElevated)
        {
            string value = string.Empty;
            if (isElevated)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    value = GetPropertyBagValue(key);
                });
            }
            else
            {
                value = GetPropertyBagValue(key);
            }
            return value;
        }

        public static List<string> SplitAndAppendOU(string sourcePaths, string suffix)
        {
            List<string> ldapPaths = new List<string>();
            if (!string.IsNullOrEmpty(sourcePaths))
            {
                string[] ouNames = sourcePaths.Split(new string[] { Constants.DELIMITER_SEMI_COLON },
                    StringSplitOptions.RemoveEmptyEntries);
                foreach (var ouName in ouNames)
                {
                    ldapPaths.Add(string.Concat(string.Format(Constants.OU_NAME,
                        ouName.Trim()),
                        suffix));
                }
            }
            else
            {
                ldapPaths.Add(suffix);
            }
            return ldapPaths;
        }

    }
}
