using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Office365Api.Helpers
{
    public abstract class BaseOffice365Helper
    {
        protected AuthenticationHelper AuthenticationHelper
        {
            get;
            private set;
        }

        public BaseOffice365Helper(AuthenticationHelper authenticationHelper)
        {
            this.AuthenticationHelper = authenticationHelper;
        }
    }
}
