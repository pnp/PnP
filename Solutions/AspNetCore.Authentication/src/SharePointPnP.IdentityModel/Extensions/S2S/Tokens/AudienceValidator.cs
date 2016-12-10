using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharePointPnP.IdentityModel.Extensions.S2S.Tokens
{
    internal static class AudienceValidator
    {
        public static void ValidateAudiences(System.Collections.Generic.IList<System.Uri> allowedAudiences, System.Collections.Generic.IList<System.Uri> tokenAudiences)
        {
            if (allowedAudiences == null)
            {
                throw new System.ArgumentNullException("allowedAudiences");
            }
            if (tokenAudiences == null)
            {
                throw new System.ArgumentNullException("tokenAudiences");
            }
            if (tokenAudiences.Count == 0)
            {
                throw new AudienceUriValidationFailedException("Audience URI validation failed. No token audiences were found.");
            }
            if (allowedAudiences.Count == 0)
            {
                throw new AudienceUriValidationFailedException("Audience URI validation failed. No allowed audiences are configured.");
            }
            foreach (System.Uri current in tokenAudiences)
            {
                if (current != null)
                {
                    foreach (System.Uri current2 in allowedAudiences)
                    {
                        if (System.Uri.Compare(current2, current, System.UriComponents.AbsoluteUri, System.UriFormat.Unescaped, System.StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            return;
                        }
                        System.Uri uri;
                        if (!current2.OriginalString.EndsWith("/") && System.Uri.TryCreate(current2.OriginalString + "/", System.UriKind.RelativeOrAbsolute, out uri) && System.Uri.Compare(uri, current, System.UriComponents.AbsoluteUri, System.UriFormat.Unescaped, System.StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            return;
                        }
                    }
                }
            }
            throw new AudienceUriValidationFailedException("Audience URI validation failed. Audience does not match.");
        }
    }
}
