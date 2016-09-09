namespace OfficeDevPnP.Core.Framework.Authentication.Events
{
    using System;
    using System.Threading.Tasks;

    public class SharePointAuthenticationEvents : ISharePointAuthenticationEvents
    {
        /// <summary> 
        /// Invoked when the SharePoint authentication process has succeeded and authenticated the user. 
        /// </summary> 
        public Func<AuthenticationFailedContext, Task> OnAuthenticationFailed { get; set; } = context => Task.FromResult(0);

        /// <summary> 
        /// Invoked when the authentication handshaking failed and the user is not authenticated.
        /// </summary> 
        public Func<AuthenticationSucceededContext, Task> OnAuthenticationSucceeded { get; set; } = context => Task.FromResult(0);

        public virtual Task AuthenticationFailed(AuthenticationFailedContext context)
            => OnAuthenticationFailed(context);

        public virtual Task AuthenticationSucceeded(AuthenticationSucceededContext context)
            => OnAuthenticationSucceeded(context);
    }
}
