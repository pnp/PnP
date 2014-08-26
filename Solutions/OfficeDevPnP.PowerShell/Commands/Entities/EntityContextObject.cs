using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.PowerShell.Commands.Entities
{
    public class EntityContextObject<T>
    {
        internal T _contextObject;

        private T ContextObject
        {
            set
            {
                _contextObject = value;
            }
        }

        public T GetContextObject() { return _contextObject; }
    }
}
