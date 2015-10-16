using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Data.AppSettings
{
    public interface IAppSettingsFactory
    {
        IAppSettingsManager GetManager();
    }
}
