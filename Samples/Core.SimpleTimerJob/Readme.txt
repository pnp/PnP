Alternative way to register the needed permissions.

1. Move to “_layouts/AppInv.aspx”.
2. Look up for client ID
3. Provide needed permissions

  <AppPermissionRequests AllowAppOnlyPolicy="true">
    <AppPermissionRequest Scope="http://sharepoint/content/sitecollection/web" Right="FullControl" />
  </AppPermissionRequests>