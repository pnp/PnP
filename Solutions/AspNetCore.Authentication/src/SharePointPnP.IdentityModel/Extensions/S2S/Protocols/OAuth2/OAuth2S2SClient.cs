namespace SharePointPnP.IdentityModel.Extensions.S2S.Protocols.OAuth2
{
    using Microsoft.IdentityModel.SecurityTokenService;

    public class OAuth2S2SClient
    {
        public OAuth2Message Issue(string securityTokenServiceUrl, OAuth2AccessTokenRequest oauth2Request)
        {
            OAuth2WebRequest oAuth2WebRequest = new OAuth2WebRequest(securityTokenServiceUrl, oauth2Request);
            OAuth2Message result;
            try
            {
                System.Net.WebResponse response = oAuth2WebRequest.GetResponse();
                result = OAuth2MessageFactory.CreateFromEncodedResponse(new System.IO.StreamReader(response.GetResponseStream()));
            }
            catch (System.Exception innerException)
            {
                throw new RequestFailedException("Token request failed.", innerException);
            }
            return result;
        }
    }
}
