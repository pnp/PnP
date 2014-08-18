using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.SPOnline.Commands
{
    public class SPOContextObject<T>
    {
        internal T _contextObject;

        public T ContextObject
        {
            get
            {
                return _contextObject;
            }
        }
    }
}
