# Using the client side object model against SharePoint with ADFS as trusted identity token issuer #
The PnP Core AuthManager.cs class supports creating a ClientContext object that can be used to perform CSOM requests against a SharePoint web application that's using ADFS as trusted identity token issuer.

Below schema shows how this work:

![](http://i.imgur.com/0unVI8h.png)

1. The application (can be a web add-in, console application,...) first tries to authenticate the against ADFS using a username and password combination. The endpoint that's being used is the trust/13/usernamemixed endpoint which is available and enabled by default on ADFS v2.0.
2. The ADFS server authenticates the user and returns a SAML token to the application. If the proxy endpoint was hit the proxy will communicate with the ADFS federation server to process the authentication request
3. The application will wrap the SAML token in a WS-Federation message and will send it to the SharePoint Security Token Service (STS) by hitting the /trust/ url. The response of this action contains a FedAuth cookie
4. In all CSOM request the FedAuth cookie is inserted during the call which makes the CSOM calls succeed

To use all of this in your code is very easy as you can see in this sample:

```C#
string samlSite = "https://saml.set1.bertonline.info/sites/bert";

OfficeDevPnP.Core.AuthenticationManager am = new OfficeDevPnP.Core.AuthenticationManager();
ClientContext ctx = am.GetADFSUserNameMixedAuthenticatedContext(samlSite, "administrator", "pwd", "domain", "sts.set1.bertonline.info", "urn:sharepoint:saml");

FieldCollection fields = ctx.Web.Fields;
IEnumerable<Field> results = ctx.LoadQuery<Field>(fields.Where(item => item.Hidden != false));
ctx.ExecuteQuery();

foreach (Field field in results)
{
    Console.WriteLine("{0} - {1}", field.Id, field.InternalName);
}
```

The below chapters contain a bit more details for each of the above steps.

## Authenticate against ADFS ##
The authentication against ADFS is done using the UsernameMixed class:

```C#
UsernameMixed adfsTokenProvider = new UsernameMixed();
var token = adfsTokenProvider.RequestToken(userName, password, userNameMixed, relyingPartyIdentifier);
```

The RequestToken method is shown below:

```C#
private GenericXmlSecurityToken RequestToken(string userName, string passWord, Uri userNameMixed, string relyingPartyIdentifier)
{
    var factory = new WSTrustChannelFactory(new UserNameWSTrustBinding(SecurityMode.TransportWithMessageCredential), new EndpointAddress(userNameMixed));

    factory.TrustVersion = TrustVersion.WSTrust13;
    // Hookup the user and password 
    factory.Credentials.UserName.UserName = userName;
    factory.Credentials.UserName.Password = passWord;

    var requestSecurityToken = new RequestSecurityToken
    {
        RequestType = RequestTypes.Issue,
        AppliesTo = new EndpointReference(relyingPartyIdentifier),
        KeyType = KeyTypes.Bearer
    };

    IWSTrustChannelContract channel = factory.CreateChannel();
    GenericXmlSecurityToken genericToken = channel.Issue(requestSecurityToken) as GenericXmlSecurityToken;

    return genericToken;
}
```

## The ADFS server handles the authentication and returns a SAML token ##
This requires that the trust/13/usernamemixed endpoint is enabled at ADFS, which should be the case in a default setup:

![](http://i.imgur.com/CMEcRpO.png)

## Wrap the SAML token in a WS-Fed token and send to the SharePoint STS ##
Below method is responsible for wrapping the SAML token and hitting the SharePoint STS to obtain a FedAuth cookie:

```C#
internal string TransformSamlTokenToFedAuth(string samlToken, string samlSite)
{
    samlToken = WrapInSoapMessage(samlToken);

    string samlServer = samlSite.EndsWith("/") ? samlSite : samlSite + "/";
    Uri samlServerRoot = new Uri(samlServer);

    var sharepointSite = new
    {
        Wctx = samlServer + "_layouts/Authenticate.aspx?Source=%2F",
        Wtrealm = samlServer,
        Wreply = String.Format("{0}://{1}/_trust/", samlServerRoot.Scheme, samlServerRoot.Host)
    };

    string stringData = String.Format("wa=wsignin1.0&wctx={0}&wresult={1}", HttpUtility.UrlEncode(sharepointSite.Wctx), HttpUtility.UrlEncode(samlToken));

    HttpWebRequest sharepointRequest = HttpWebRequest.Create(sharepointSite.Wreply) as HttpWebRequest;
    sharepointRequest.Method = "POST";
    sharepointRequest.ContentType = "application/x-www-form-urlencoded";
    sharepointRequest.CookieContainer = new CookieContainer();
    sharepointRequest.AllowAutoRedirect = false; // This is important
    Stream newStream = sharepointRequest.GetRequestStream();

    byte[] data = Encoding.UTF8.GetBytes(stringData);
    newStream.Write(data, 0, data.Length);
    newStream.Close();
    HttpWebResponse webResponse = sharepointRequest.GetResponse() as HttpWebResponse;
    return webResponse.Cookies["FedAuth"].Value;
}
```

## Insert the FedAuth cookie in all CSOM requests ##
Final step is the insertion of the FedAuth cookie when we make a CSOM request. This is done via the following code:

```C#
ClientContext clientContext = new ClientContext(siteUrl);
clientContext.ExecutingWebRequest += clientContext_ExecutingWebRequest;

private void clientContext_ExecutingWebRequest(object sender, WebRequestEventArgs e)
{
    e.WebRequestExecutor.WebRequest.CookieContainer = fedAuth;
}
```


