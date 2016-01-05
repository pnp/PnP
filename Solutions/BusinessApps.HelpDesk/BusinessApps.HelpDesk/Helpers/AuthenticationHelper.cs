using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace BusinessApps.HelpDesk.Helpers
{
    public class AuthenticationHelper
    {
        public ClientAssertionCertificate GetClientAssertionCertificate()
        {
            X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            certStore.Open(OpenFlags.ReadOnly);

            X509Certificate2 cert = certStore.Certificates.Find(X509FindType.FindByThumbprint, SettingsHelper.CertThumbprint, false)[0];

            ClientAssertionCertificate cac = new ClientAssertionCertificate(SettingsHelper.ClientId, cert);

            return cac;
        }

        public async Task<AuthenticationResult> GetToken(string resource)
        {
            ClientAssertionCertificate cac = GetClientAssertionCertificate();

            AuthenticationContext authContext = new AuthenticationContext(SettingsHelper.AzureADAuthority);
            AuthenticationResult authResult = await authContext.AcquireTokenAsync(resource, cac);

            return authResult;
        }
    }
}