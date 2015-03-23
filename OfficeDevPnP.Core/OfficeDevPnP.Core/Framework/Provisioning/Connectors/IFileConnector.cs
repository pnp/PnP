using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Connectors
{
    public interface IFileConnector
    {
        List<string> GetFiles();

        List<string> GetFiles(string container);

        string GetFile(string fileName);

        string GetFile(string fileName, string container);

        Stream GetFileStream(string fileName);

        Stream GetFileStream(string fileName, string container);
    }
}
