using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Configuration
{
    public class ModulesSection : ConfigurationSection
    {
        [ConfigurationProperty("Modules")]
        public ModuleElementCollection Modules
        {
            get { return ((ModuleElementCollection)(base["Modules"])); }
            set { base["Modules"] = value; }
        }
    }
}
