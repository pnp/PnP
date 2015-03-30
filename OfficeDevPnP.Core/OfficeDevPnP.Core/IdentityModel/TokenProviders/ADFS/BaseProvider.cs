using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

namespace OfficeDevPnP.Core.IdentityModel.TokenProviders.ADFS
{
    /// <summary>
    /// Base class for active SAML based authentication
    /// </summary>
    public class BaseProvider
    {
        /// <summary>
        /// Transforms the retrieved SAML token into a FedAuth cookie value by calling into the SharePoint STS
        /// </summary>
        /// <param name="samlToken">SAML token obtained via active authentication to ADFS</param>
        /// <param name="samlSite">Url of the SAML secured SharePoint site</param>
        /// <param name="relyingPartyIdentifier">Identifier of the ADFS relying party that we're hitting</param>
        /// <returns>The FedAuth cookie value</returns>
        internal string TransformSamlTokenToFedAuth(string samlToken, string samlSite, string relyingPartyIdentifier)
        {
            samlToken = WrapInSoapMessage(samlToken, relyingPartyIdentifier);

            string samlServer = samlSite.EndsWith("/") ? samlSite : samlSite + "/";
            Uri samlServerRoot = new Uri(samlServer);

            var sharepointSite = new
            {
                Wctx = samlServer + "_layouts/Authenticate.aspx?Source=%2F",
                Wtrealm = samlServer,
                Wreply = String.Format("{0}://{1}/_trust/", samlServerRoot.Scheme, samlServerRoot.Host)
            };

            string stringData = String.Format("wa=wsignin1.0&wctx={0}&wresult={1}", HttpUtility.UrlEncode(sharepointSite.Wctx), HttpUtility.UrlEncode(samlToken));

            HttpWebRequest sharepointRequest = WebRequest.Create(sharepointSite.Wreply) as HttpWebRequest;
            sharepointRequest.Method = "POST";
            sharepointRequest.ContentType = "application/x-www-form-urlencoded";
            sharepointRequest.CookieContainer = new CookieContainer();
            sharepointRequest.AllowAutoRedirect = false; // This is important

            Stream newStream = sharepointRequest.GetRequestStream();
            byte[] data = Encoding.UTF8.GetBytes(stringData);
            newStream.Write(data, 0, data.Length);
            newStream.Close();

            string fedAuthCookieValue;
            using (HttpWebResponse webResponse = (HttpWebResponse)sharepointRequest.GetResponse())
            {
                fedAuthCookieValue = webResponse.Cookies["FedAuth"].Value;
            }

            return fedAuthCookieValue;
        }

        /// <summary>
        /// Wrap SAML token in RequestSecurityTokenResponse soap message
        /// </summary>
        /// <param name="stsResponse">SAML token obtained via active authentication to ADFS</param>
        /// <param name="relyingPartyIdentifier">Identifier of the ADFS relying party that we're hitting</param>
        /// <returns>RequestSecurityTokenResponse soap message</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Xml.XmlDocument.CreateTextNode(System.String)")]
        private string WrapInSoapMessage(string stsResponse, string relyingPartyIdentifier)
        {
            XmlDocument samlAssertion = new XmlDocument();
            samlAssertion.PreserveWhitespace = true;
            samlAssertion.LoadXml(stsResponse);

            //Select the book node with the matching attribute value.
            String notBefore = samlAssertion.DocumentElement.FirstChild.Attributes["NotBefore"].Value;
            String notOnOrAfter = samlAssertion.DocumentElement.FirstChild.Attributes["NotOnOrAfter"].Value;

            XmlDocument soapMessage = new XmlDocument();
            XmlElement soapEnvelope = soapMessage.CreateElement("t", "RequestSecurityTokenResponse", "http://schemas.xmlsoap.org/ws/2005/02/trust");
            soapMessage.AppendChild(soapEnvelope);
            XmlElement lifeTime = soapMessage.CreateElement("t", "Lifetime", soapMessage.DocumentElement.NamespaceURI);
            soapEnvelope.AppendChild(lifeTime);
            XmlElement created = soapMessage.CreateElement("wsu", "Created", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
            XmlText createdValue = soapMessage.CreateTextNode(notBefore);
            created.AppendChild(createdValue);
            lifeTime.AppendChild(created);
            XmlElement expires = soapMessage.CreateElement("wsu", "Expires", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
            XmlText expiresValue = soapMessage.CreateTextNode(notOnOrAfter);
            expires.AppendChild(expiresValue);
            lifeTime.AppendChild(expires);
            XmlElement appliesTo = soapMessage.CreateElement("wsp", "AppliesTo", "http://schemas.xmlsoap.org/ws/2004/09/policy");
            soapEnvelope.AppendChild(appliesTo);
            XmlElement endPointReference = soapMessage.CreateElement("wsa", "EndpointReference", "http://www.w3.org/2005/08/addressing");
            appliesTo.AppendChild(endPointReference);
            XmlElement address = soapMessage.CreateElement("wsa", "Address", endPointReference.NamespaceURI);
            XmlText addressValue = soapMessage.CreateTextNode(relyingPartyIdentifier);
            address.AppendChild(addressValue);
            endPointReference.AppendChild(address);
            XmlElement requestedSecurityToken = soapMessage.CreateElement("t", "RequestedSecurityToken", soapMessage.DocumentElement.NamespaceURI);
            XmlNode samlToken = soapMessage.ImportNode(samlAssertion.DocumentElement, true);
            requestedSecurityToken.AppendChild(samlToken);
            soapEnvelope.AppendChild(requestedSecurityToken);
            XmlElement tokenType = soapMessage.CreateElement("t", "TokenType", soapMessage.DocumentElement.NamespaceURI);
            XmlText tokenTypeValue = soapMessage.CreateTextNode("urn:oasis:names:tc:SAML:1.0:assertion");
            tokenType.AppendChild(tokenTypeValue);
            soapEnvelope.AppendChild(tokenType);
            XmlElement requestType = soapMessage.CreateElement("t", "RequestType", soapMessage.DocumentElement.NamespaceURI);
            XmlText requestTypeValue = soapMessage.CreateTextNode("http://schemas.xmlsoap.org/ws/2005/02/trust/Issue");
            requestType.AppendChild(requestTypeValue);
            soapEnvelope.AppendChild(requestType);
            XmlElement keyType = soapMessage.CreateElement("t", "KeyType", soapMessage.DocumentElement.NamespaceURI);
            XmlText keyTypeValue = soapMessage.CreateTextNode("http://schemas.xmlsoap.org/ws/2005/05/identity/NoProofKey");
            keyType.AppendChild(keyTypeValue);
            soapEnvelope.AppendChild(keyType);

            return soapMessage.OuterXml;
        }

        /// <summary>
        /// Returns the DateTime when then received saml token will expire
        /// </summary>
        /// <param name="stsResponse">saml token</param>
        /// <returns>DateTime holding the expiration date. Defaults to DateTime.MinValue if there's no valid datetime in the saml token</returns>
        internal DateTime SamlTokenExpiresOn(string stsResponse)
        {
            XmlDocument samlAssertion = new XmlDocument();
            samlAssertion.PreserveWhitespace = true;
            samlAssertion.LoadXml(stsResponse);

            String notOnOrAfter = samlAssertion.DocumentElement.FirstChild.Attributes["NotOnOrAfter"].Value;
            DateTime toDate = DateTime.MinValue;
            if (DateTime.TryParse(notOnOrAfter, out toDate))
            {
                return toDate;
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Returns the SAML token life time
        /// </summary>
        /// <param name="stsResponse">saml token</param>
        /// <returns>TimeSpan holding the token lifetime. Defaults to TimeSpan.Zero is case of problems</returns>
        internal TimeSpan SamlTokenlifeTime(string stsResponse)
        {
            XmlDocument samlAssertion = new XmlDocument();
            samlAssertion.PreserveWhitespace = true;
            samlAssertion.LoadXml(stsResponse);

            String notOnOrAfter = samlAssertion.DocumentElement.FirstChild.Attributes["NotOnOrAfter"].Value;
            String notBefore = samlAssertion.DocumentElement.FirstChild.Attributes["NotBefore"].Value;

            DateTime toDate = DateTime.MinValue;
            if (DateTime.TryParse(notOnOrAfter, out toDate))
            {
                DateTime fromDate = DateTime.MinValue;
                if (DateTime.TryParse(notBefore, out fromDate))
                {
                    return toDate - fromDate;
                }
            }

            return TimeSpan.Zero;
        }

    }
}
