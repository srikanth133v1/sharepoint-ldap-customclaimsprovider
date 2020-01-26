using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Extranet.ClaimsProvider
{
    public class LDAPUser
    {
        public string DisplayName { get; set; }
        public string GivenName { get; set; }
        public string SurName { get; set; }
        public string Mail { get; set; }
        public string sAMAccountName { get; set; }
    }
}
