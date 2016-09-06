# Dynamically request permissions for an add-in #

### Summary ###
This sample shows how to dynamically request permissions for an add-in from any web site using an OAuth code.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Core.DynamicPermissions | Kirk Evans (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | April 26th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# SCENARIO: DYNAMICALLY REQUEST ADD-IN PERMISSIONS #
This scenario shows how an add-in can be used to dynamically request permissions to access SharePoint resources from any web site.

![App UI with Connect button](http://i.imgur.com/7Dnd75t.png)

After entering the URL for the SharePoint site and clicking the Connect button, the user is redirected to sign into Office 365.

![Sign-in to Office 365](http://i.imgur.com/zYs8EDJ.png)

Once signed in, the user is prompted to trust the add-in.

![Trust dialog for add-in permissions](http://i.imgur.com/psJXcqu.png)

### Note:###
This type of add-in can only be run by users who have Manage permissions to the resources the add-in wants to access, because only they have sufficient rights to grant the add-in the permissions that it requests. For example, if an add-in requests only Read permission to a website, a user who has Read, but not Manage, rights to the website cannot run the add-in.  For more information, see http://msdn.microsoft.com/en-us/library/office/jj687470.aspx. 

This happens because the add-in redirects the user to a page in SharePoint, OAuthAuthorize,aspx, passing a client ID, requested permission scope, and the response type as “code”.

```
https://kirke.sharepoint.com/sites/dev/_layouts/15/OAuthAuthorize.aspx?IsDlg=1&client_id=2bb3c34a-b043-4c6c-adda-2e9634f24c3d&scope=Web.Manage&response_type=code
```

This URL is formed by using TokenHelper to determine the appropriate redirect URL simply by passing the URL of the SharePoint site with the desired permissions.

```C#
_response.Redirect(TokenHelper.GetAuthorizationUrl(hostUrl, "Web.Manage"));
```

Notice the client ID in the above URL example. This indicates the client ID must be known in advance.  If the add-in is registered through the Seller Dashboard, then any SharePoint online site can be used. If the add-in is not registered through the Seller Dashboard, then you first must register the client ID using appregnew.aspx in the SharePoint site prior to calling. The resulting add-in Principal includes the callback URL. The client ID must exist in the web.config for the add-in.

### Note: ###
To be able to call into SharePoint, this type of add-in must first be registered through the Seller Dashboard or the appregnew.aspx page. For more information about registering apps via the Seller Dashboard or appregnew.aspx, see Guidelines for registering apps for SharePoint 2013.  For more information, see http://msdn.microsoft.com/en-us/library/office/jj687470.aspx.

TokenHelper will read the client ID from web.config and append it to the URL.

As a demonstration, I register the add-in using AppRegNew.aspx.  The client ID identifies the add-in, and the Redirect URL provides the URL to redirect the browser to once the permissions are granted.

![The App Id and Title page. The App Id field contains a GUID. The Title contains DynamicPermissions. The App Domain contains localhost. The Redirect URL contains https://localhost:44363/Home/Callback](http://i.imgur.com/FGkEat5.png)

Once the add-in has been granted permissions, it redirects to the registered Redirect URL (https://localhost:44363/Home/Callback) and passes an authorization code.  This code is handed in the Home controller in the Callback action:

```C#
public ActionResult Callback(string code)
{
    TokenRepository repository = new TokenRepository(Request, Response);
    repository.Callback(code);
    return RedirectToAction("Index");
}
```

Notice the code that is passed.  Once this method receives the code, we use TokenHelper.GetAccessToken to obtain an OAuth access token based on the returned code.

```C#
string refreshToken = TokenHelper.GetAccessToken(request.QueryString["code"], "00000003-0000-0ff1-ce00-000000000000", targetUri.Authority, TokenHelper.GetRealmFromTargetUrl(targetUri), new Uri(request.Url.GetLeftPart(UriPartial.Path))).RefreshToken;
```

Use the refresh token and access token in order to create a ClientContext with the client side object model. The add-in principal was registered in AppRegNew.aspx without any permissions (note the empty permissions XML block above), but was able to successfully request Manage permissions.

The add-in can now obtain the SharePoint site’s title (notice the “Successfully connected to” in the screen shot below shows the site’s title).

![The text in the image: Successfully connected to Dev. Now that you dynamically requested permissions, test it out by creating a list. A text box contains the text, A Test List. Followed by a button, Create List.](http://i.imgur.com/Kk8As9F.png)

To test that our provider-hosted add-in actually was granted Manage permissions for the web, we can create a new list by providing the list title.  Click Create List and we see the new list is created.

![The text in the image: Lists in Dev. A Test List. App Packages.](http://i.imgur.com/yUFp74h.png)


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.DynamicPermissions" />