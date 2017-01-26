namespace OfficeDevPnP.Core.Framework.Authentication
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Options;
    using OfficeDevPnP.Core.Framework.Authentication.Events;

    /// <summary>
    /// Creates an instance and sets default values of the Authentication Options for the middleware
    /// </summary>
    public class SharePointAuthenticationOptions : 
        RemoteAuthenticationOptions,
        IOptions<SharePointAuthenticationOptions>
    {
        /// <summary>
        /// Sets default options.
        /// </summary>
        public SharePointAuthenticationOptions()
        {
            // Sets automatic challenge to default.
            AutomaticAuthenticate = SharePointAuthenticationDefaults.AutomaticAuthenticate;
            AutomaticChallenge = SharePointAuthenticationDefaults.AutomaticChallenge;
            AuthenticationScheme = SharePointAuthenticationDefaults.AuthenticationScheme;
        }

        /// <summary>
        /// Gets or sets if HTTPS is required for the metadata address or authority.
        /// The default is true. This should be disabled only in development environments.
        /// </summary>
        public bool RequireHttpsMetadata { get; set; } = true;

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
        /// If set, the SP Authentication middleware will also call the SignIn method for the provided 
        /// Cookie authentication scheme
        /// </summary>
        public string CookieAuthenticationScheme { get; set; }

        /// <summary>  
        /// The object provided by the application to process events raised by the SharePoint authentication middleware.  
        /// The application may implement the interface fully, or it may create an instance of AuthenticationEvents  
        /// and assign delegates only to the events it wants to process.  
        /// </summary>  
        public ISharePointAuthenticationEvents SharePointAuthenticationEvents { get; set; } = new SharePointAuthenticationEvents();

        public SharePointAuthenticationOptions Value
        {
            get
            {
                return this;
            }
        }
    }
}