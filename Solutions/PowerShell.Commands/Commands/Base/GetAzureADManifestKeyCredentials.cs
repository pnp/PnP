using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Json;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands.Base
{
    [Cmdlet(VerbsCommon.Get, "SPOAzureADManifestKeyCredentials")]
    [CmdletHelp("Creates the JSON snippet that is required for the manifest json file for Azure WebApplication / WebAPI apps", Category = "Base Cmdlets")]
    [CmdletExample(
        Code = @"PS:> Get-SPOAzureADManifestKeyCredentials -CertPath .\mycert.cer",
        Remarks = "Output the JSON snippet which needs to be replaced in the application manifest file", 
        SortOrder = 1)]
    [CmdletExample(
        Code = @"PS:> Get-SPOAzureADManifestKeyCredentials -CertPath .\mycert.cer | Set-Clipboard",
        Remarks = "Output the JSON snippet which needs to be replaced in the application manifest file and copies it to the clipboard",
        SortOrder = 2)]
    public class GetAzureADManifestKeyCredentials : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public string CertPath;

        protected override void ProcessRecord()
        {
            if (!System.IO.Path.IsPathRooted(CertPath))
            {
                CertPath = System.IO.Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, CertPath);
            }
            var cert = new X509Certificate2(CertPath);

            var rawCert = cert.GetRawCertData();

            var base64Cert = Convert.ToBase64String(rawCert);

            var rawCertHash = cert.GetCertHash();

            var base64CertHash = Convert.ToBase64String(rawCertHash);

            var keyId = Guid.NewGuid().ToString();

            var output = string.Format("\"keyCredentials\": [\n\t{{\n\t\t\"customKeyIdentifier\": \"{0}\",\n\t\t\"keyId\": \"{1}\",\n\t\t\"type\": \"AsymmetricX509Cert\",\n\t\t\"usage\": \"Verify\",\n\t\t\"value\": \"{2}\"\n\t}}\n],", base64CertHash, keyId, base64Cert);

            WriteObject(output);

        }
    }
}
