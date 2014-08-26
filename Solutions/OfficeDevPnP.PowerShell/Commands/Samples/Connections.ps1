# Connecting with normal user credentials, if you don't specify credentials you will prompted for them
Connect-SPOnline -Url https://yourtenant.sharepoint.com 


# Connecting with specified credentials
$creds = Get-Credential
Connect-SPOnline -Url https://yourtenant.sharepoint.com -Credentials $creds


# Connecting with credentials that are stored in the credential manager. See readme.md
Connect-SPOnline -Url https://yourtenant.sharepoint.com -Credentials "LABEL"

# Connecting with appid and appsecret. If you don't specify the realm, a call will be made to the site to retrieve the realm info.
Connect-SPOnline -Url https://yourtenant.sharepoint.com -AppId e8a9a0ef-86a9-4871-ba5f-dbacbcd57e4c -AppSecret abmQDEw/x/PHX6stdhbJgDEF3pSZkN64sS63XDViBm60=

# Connecting with App Id and App Secret. If you don't specify the realm, a call will be made to the site to retrieve the realm info.
# Navigate to https://yourtenant.sharepoint.com/_layouts/appregnew.aspx. Click both 'generate' buttons, specify "localhost" as the hostname.
# After registering store both generated App Id and App Secret and navigate to https://yourtenant.sharepoint.com/_layouts/appinv.aspx
# Lookup the app by entering the App Id and paste a snippet in there to describe the requested security. An example is below:
#
#  <AppPermissionRequests AllowAppOnlyPolicy="true">
#    <AppPermissionRequest Scope="http://sharepoint/content/tenant" Right="FullControl" />
#  </AppPermissionRequests>
#
# Make notice of the AllowAppOnlyPolicy parameter which needs to be set to true.
# In the snippet above you request full control over the complete tenant. This is obviously not always needed. If you request tenant admin rights, make notice
# that only someone with Tenant Admin rights can enter this request.
#
Connect-SPOnline -Url https://yourtenant.sharepoint.com -AppId e8a9a0ef-86a9-4871-ba5f-dbacbcd57e4c -AppSecret abmQDEw/x/PHX6stdhbJgDEF3pSZkN64sS63XDViBm60= -Realm 939013d2-9450-4a4e-a63a-3f364233c6cd

