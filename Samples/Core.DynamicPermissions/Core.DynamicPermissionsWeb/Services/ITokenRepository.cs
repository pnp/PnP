using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Contoso.Core.DynamicPermissionsWeb.Services
{
    public interface ITokenRepository
    {
        Uri GetHostUrl();
        bool IsConnectedToO365{get;}

        string GetSiteTitle();

        void CreateList(string title);
    }
}
