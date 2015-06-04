using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Data
{
    public abstract class AbstractModule
    {
        protected AbstractModule()
        {

        }
        public string ConnectionString
        {
            get;
            set;
        }

        public string Container
        {
            get;
            set;
        }
    }
}
