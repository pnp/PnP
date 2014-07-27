using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contoso.Core
{
    public class EmailSender
    {
        public int Port
        {
            get;
            set;
        }

        public string FromAddress
        {
            get;
            set;
        }

        public string ToAddress
        {
            get;
            set;
        }

        public string Host
        {
            get;
            set;
        }
    }
}
