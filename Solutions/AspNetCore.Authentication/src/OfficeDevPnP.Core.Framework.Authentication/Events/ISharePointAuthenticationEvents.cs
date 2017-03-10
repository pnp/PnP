namespace OfficeDevPnP.Core.Framework.Authentication.Events
{
    using System.Threading.Tasks;

    public interface ISharePointAuthenticationEvents
    {
        /// <summary>  
        /// Invoked when the Active Directory authentication process has succeeded and authenticated the user.  
        /// </summary>  
        Task AuthenticationSucceeded(AuthenticationSucceededContext context);  
    
        /// <summary>  
        /// Invoked when the authentication handshaking failed and the user is not authenticated. 
        /// </summary>  
        Task AuthenticationFailed(AuthenticationFailedContext context);
    }
}
