using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NS.Extranet.ClaimsProvider
{
    [Serializable]
    public class ConfigEntry
    {
        [XmlElement]
        public string ConfigKey { get; set; }
        [XmlElement]
        public string ConfigValue { get; set; }
    }

    [Serializable]
    public class ConfigEntries
    {
        [XmlElement]
        public ConfigEntry[] ConfigEntry { get; set; }
    }

}
