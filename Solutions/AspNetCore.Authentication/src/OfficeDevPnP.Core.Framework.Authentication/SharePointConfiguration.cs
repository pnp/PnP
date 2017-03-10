namespace OfficeDevPnP.Core.Framework.Authentication
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// SharePointConfiguration has base set of configuration fields needed for authenticating against SharePoint.
    /// It is used by the TokenHelper to send HTTP calls 
    /// </summary>
    public class SharePointConfiguration
    {
        /// <summary>
        /// Gets or sets ClientId.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets ClientSecret.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets IssuerId.
        /// </summary>
        public string IssuerId { get; set; }

        /// <summary>
        /// Gets or sets HostedAppHostNameOverride.
        /// </summary>
        public string HostedAppHostNameOverride { get; set; }

        /// <summary>
        /// Gets or sets HostedAppHostName.
        /// </summary>
        public string HostedAppHostName { get; set; }

        /// <summary>
        /// Gets or sets SecondaryClientSecret.
        /// </summary>
        public string SecondaryClientSecret { get; set; }

        /// <summary>
        /// Gets or sets Realm.
        /// </summary>
        public string Realm { get; set; }

        /// <summary>
        /// Gets or sets ClientSigningCertificatePath.
        /// </summary>
        public string ClientSigningCertificatePath { get; set; }

        /// <summary>
        /// Gets or sets ClientSigningCertificatePassword.
        /// </summary>
        public string ClientSigningCertificatePassword { get; set; }

        /// <summary>
        /// Creates new SharePointConfiguration from SharePointAuthenticationOptions.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static SharePointConfiguration GetFromSharePointAuthenticationOptions(SharePointAuthenticationOptions options)
        {
            return new SharePointConfiguration()
            {
                ClientId = options.ClientId,
                ClientSecret = options.ClientSecret,
                IssuerId = options.IssuerId,
                HostedAppHostNameOverride = options.HostedAppHostNameOverride,
                HostedAppHostName = options.HostedAppHostName,
                SecondaryClientSecret = options.SecondaryClientSecret,
                Realm = options.Realm,
                ClientSigningCertificatePath = options.ClientSigningCertificatePath,
                ClientSigningCertificatePassword = options.ClientSigningCertificatePassword
            };
        }

        /// <summary>
        /// Creates new SharePointConfiguration from IOptions<SharePointConfiguration>.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static SharePointConfiguration GetFromIOptions(IOptions<SharePointConfiguration> options)
        {
            return new SharePointConfiguration()
            {
                ClientId = options.Value.ClientId,
                ClientSecret = options.Value.ClientSecret,
                IssuerId = options.Value.IssuerId,
                HostedAppHostNameOverride = options.Value.HostedAppHostNameOverride,
                HostedAppHostName = options.Value.HostedAppHostName,
                SecondaryClientSecret = options.Value.SecondaryClientSecret,
                Realm = options.Value.Realm,
                ClientSigningCertificatePath = options.Value.ClientSigningCertificatePath,
                ClientSigningCertificatePassword = options.Value.ClientSigningCertificatePassword
            };
        }

        /// <summary>
        /// Creates new SharePointConfiguration from IConfiguration.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static SharePointConfiguration GetFromIConfiguration(IConfiguration configuration)
        {
            return new SharePointConfiguration()
            {
                ClientId = configuration["ClientId"],
                ClientSecret = configuration["ClientSecret"],
                IssuerId = configuration["IssuerId"],
                HostedAppHostNameOverride = configuration["HostedAppHostNameOverride"],
                HostedAppHostName = configuration["HostedAppHostName"],
                SecondaryClientSecret = configuration["SecondaryClientSecret"],
                Realm = configuration["Realm"],
                ClientSigningCertificatePath = configuration["ClientSigningCertificatePath"],
                ClientSigningCertificatePassword = configuration["ClientSigningCertificatePassword"]
            };
        }
    }
}