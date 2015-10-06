using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Data.AppSettings
{
    public interface IAppSettingsManager
    {
        ICollection<AppSetting> GetAppSettings();
    }
}
