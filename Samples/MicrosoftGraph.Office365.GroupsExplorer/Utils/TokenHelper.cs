using Microsoft.IdentityModel.Clients.ActiveDirectory;
using OfficeDevPnP.MSGraphAPIGroups.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OfficeDevPnP.MSGraphAPIGroups.Utils
{
	public class TokenHelper
	{
		public static async Task<string> GetAccessToken()
		{
			var signInUserId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
			var userObjectId = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

			AuthenticationContext authContext =
				new AuthenticationContext(SettingsHelper.Authority,
																		new ADALTokenCache(signInUserId));

			var authResult = await authContext.AcquireTokenSilentAsync(
							SettingsHelper.MSGraphResource,
							new ClientCredential(SettingsHelper.ClientId, SettingsHelper.AppKey),
							new UserIdentifier(userObjectId, UserIdentifierType.UniqueId));

			return authResult.AccessToken;

		}
	}
}