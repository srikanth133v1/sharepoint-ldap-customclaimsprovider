using NS.Extranet.ClaimsProvider.Commmon;
using NS.Extranet.ClaimsProvider.Common;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NS.Extranet.ClaimsProvider
{

    public class LDAPHelper
    {
        public static List<LDAPUser> Search(string ldapPath, string pattern)
        {
            List<LDAPUser> ret = new List<LDAPUser>();

            //Run with elevated privileges to get the context of the service account            
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                //TODO: Where to store the LDAP string?
                //using (DirectoryEntry entry = new DirectoryEntry(GetLdapPath()))
                //using (DirectoryEntry entry = new DirectoryEntry("LDAP://OU=PepsiFBA,OU=CustomersFBA,DC=ns,DC=com"))
                using (DirectoryEntry entry = new DirectoryEntry(Constants.LDAP_PREFIX + ldapPath))
                {
                    using (DirectorySearcher ds = new DirectorySearcher(entry))
                    {
                        ds.PropertiesToLoad.Add("displayName");
                        ds.PropertiesToLoad.Add("sAMAccountName");
                        ds.PropertiesToLoad.Add("givenName");
                        ds.PropertiesToLoad.Add("sn");
                        ds.PropertiesToLoad.Add("mail");

                        ds.Filter = "(|((displayName=" + pattern + "*)(sAMAccountName=" + pattern + "*)(givenName=" + pattern + "*)(sn=" + pattern + "*)(mail=" + pattern + "*)))";

                        SearchResultCollection results = ds.FindAll();

                        foreach (SearchResult result in results)
                        {
                            ret.Add(new LDAPUser
                            {
                                DisplayName = result.Properties["displayName"][0].ToString(),
                                sAMAccountName = result.Properties["sAMAccountName"][0].ToString(),
                                //GivenName = result.Properties["givenName"][0].ToString(),
                                //SurName = result.Properties["sn"][0].ToString(),
                                Mail = result.Properties["mail"][0].ToString(),
                                GivenName = result.Properties["sAMAccountName"][0].ToString(),
                                SurName = result.Properties["sAMAccountName"][0].ToString(),
                                //Mail = result.Properties["sAMAccountName"][0].ToString()
                            });
                        }
                    }
                }
            });
            return ret;

        }

        public static LDAPUser FindExact(string path, string pattern)
        {
            LDAPUser ret = null;

            //Run with elevated privileges to get the context of the service account
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                //TODO: Where to store the LDAP string?
                using (DirectoryEntry entry = new DirectoryEntry(Constants.LDAP_PREFIX + path))
                {

                    using (DirectorySearcher ds = new DirectorySearcher(entry))
                    {
                        ds.PropertiesToLoad.Add("displayName");
                        ds.PropertiesToLoad.Add("sAMAccountName");
                        ds.PropertiesToLoad.Add("givenName");
                        ds.PropertiesToLoad.Add("sn");
                        ds.PropertiesToLoad.Add("mail");

                        ds.Filter = "(|((displayName=" + pattern + ")(sAMAccountName=" + pattern + ")(givenName=" + pattern + ")(sn=" + pattern + ")(mail=" + pattern + ")))";

                        SearchResult result = ds.FindOne();
                        if (null != result)
                        {
                            ret = new LDAPUser
                            {
                                DisplayName = result.Properties["displayName"][0].ToString(),
                                sAMAccountName = result.Properties["sAMAccountName"][0].ToString(),
                                //GivenName = result.Properties["givenName"][0].ToString(),
                                //SurName = result.Properties["sn"][0].ToString(),
                                Mail = result.Properties["mail"][0].ToString(),
                                GivenName = result.Properties["sAMAccountName"][0].ToString(),
                                SurName = result.Properties["sAMAccountName"][0].ToString(),
                                //Mail = result.Properties["sAMAccountName"][0].ToString()
                            };
                        }
                    }
                }
            });
            return ret;

        }
        private static string GetLdapOUName(string key)
        {
            string configValue = string.Empty;
            if (string.IsNullOrEmpty(key))
            {
                return configValue;
            }
            key = key.ToLower();
            Dictionary<string, ConfigEntry> entries
                = new Dictionary<string, ConfigEntry>();
            using (FileStream fs = new FileStream(ConfigFileName, FileMode.Open))
            {
                XmlSerializer serializer
                    = new XmlSerializer(typeof(ConfigEntries));
                ConfigEntries entryColl = serializer.Deserialize(fs) as ConfigEntries;
                entries
                     = entryColl.ConfigEntry.ToDictionary(e => e.ConfigKey, e => e);
            }
            if (entries.ContainsKey(key))
            {
                configValue = Convert.ToString(entries[key].ConfigValue);
            }
            return configValue;
        }
        private static string ConfigFileName
        {
            get
            {
                return ConfigurationManager.AppSettings["ConfigFileName"];
            }
        }
        private static string GetLdapPath()
        {
            string ldapPath = "LDAP://OU={0},OU={1},DC={2},DC={3}";
            //TODO: Apply regex to get the tenant root url
            string rootWebUrl = SPContext.Current.Site.Url;
            return string.Format(ldapPath,
                GetLdapOUName(rootWebUrl),
                ConfigurationManager.AppSettings["TenantRootContainer"],
                ConfigurationManager.AppSettings["Domain"],
                ConfigurationManager.AppSettings["Forest"]);


        }

        public static List<LDAPUser> SearchMultipleOUs(string pattern)
        {
            List<LDAPUser> users = new List<LDAPUser>();
            string ldapValueInPropertyBag
                = Utility.GetPropertyBagValue(Constants.PROPERTY_BAG_KEY_NAME, true);
            List<string> ldapPaths
                = Utility.SplitAndAppendOU(ldapValueInPropertyBag, CustomLDAPBasePath);

            foreach (var path in ldapPaths)
            {
                users.AddRange(Search(path, pattern));
            }

            return users;
        }
        public static LDAPUser FindExactMultipleOUs(string pattern)
        {
            LDAPUser ret = null;
            string ldapValueInPropertyBag
                = Utility.GetPropertyBagValue(Constants.PROPERTY_BAG_KEY_NAME, true);
            List<string> ldapPaths
                = Utility.SplitAndAppendOU(ldapValueInPropertyBag, CustomLDAPBasePath);

            foreach (var path in ldapPaths)
            {
                ret = FindExact(path, pattern);
                if (ret != null)
                {
                    break;
                }
            }
            return ret;
        }
        public static string CustomLDAPBasePath
        {
            get
            {
                return ConfigurationManager.AppSettings[Constants.CUSTOM_LDAP_BASE_PATH];
            }
        }
    }
}
