# ECM.DocumentLibraries #

### Summary ###
This sample scenario shows how one can implement Document Library templates using a provider hosted application instead of the feature framework or sandbox solutions.

*Notice*: This sample uses [PnP Core Nuget package](https://github.com/OfficeDev/PnP-sites-core) for the needed API operations.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
ECM.DocumentLibraries | Frank Marasco (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.1  | August 5th 2015 | Nuget update
1.0  | August 4th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Overview #
This sample scenario shows how one can implement Document Library templates using a provider hosted application instead of the feature framework or sandbox solutions.  This sample demonstrates creating Site columns, Site Content Types, Creation of Taxonomy fields, the removal of the Default Document Content Type from the created library, and versioning settings of the library. To make this application available to all site collections, you can deploy the add-in a tenant scope application, which would make this application available in all site collections.

![Add-in UI with new document library view](http://i.imgur.com/F5TTvik.png)  

# Permissions #
AppOnly Permissions are used in this solution

- SiteCollection: **FullControl**
- Taxonomy: **Read**

# Set Up  #
Since this assumes that the Term Store is available and a Term Group named “Enterprise” is created as well as a Term set named “Classification” is already created as depicted below

![Taxonomy store](http://i.imgur.com/HjDiVEX.png)
 
# Adding Fields #
Now when working with Taxonomy fields from a user point of view,  it looks like there is only one field type that creates only a single taxonomy field in SharePoint; but there is actual two fields. With that said, if you go and delete the Taxonomy Column, the hidden field still exists and you will review an exception when trying to recreate the taxonomy field. This hidden field will have an internal name of the GUID assigned to the Taxonomy Field that you are creating. 

## Taxonomy Field  ##

![Taxonomy field ID in Visual Studio](http://i.imgur.com/FFJWPgN.png)

## Taxonomy Hidden Field  ##

![Taxonomy hidden field in Visual Studio](http://i.imgur.com/XNF8SST.png)

In order to handle this situation and prevent an error doing your testing we should delete the hidden Taxonomy field if an exception occurs.

```C#
public static Field CreateTaxonomyField(this Web web, Guid id, string internalName, string displayName, string group, string mmsGroupName, string mmsTermSetName)
{
    try
    {
        var _field = web.CreateField(id, internalName, "TaxonomyFieldType", true, displayName, group, "ShowField=\"Term1033\"");
        web.WireUpTaxonomyField(id, mmsGroupName, mmsTermSetName);
        _field.Update();
        web.Context.ExecuteQuery();

        return _field;
    }
    catch(Exception)
    {
        ///If there is an exception the hidden field might be present
        FieldCollection _fields = web.Fields;
        web.Context.Load(_fields, fc => fc.Include(f => f.Id, f => f.InternalName));
        web.Context.ExecuteQuery();
        var _hiddenField = id.ToString().Replace("-", "");
  
        var _field = _fields.FirstOrDefault(f => f.InternalName == _hiddenField);
        if(_field != null)
        {
            _field.DeleteObject();
            web.Context.ExecuteQuery();
        }
        throw;
    }
}
```

To create the fields and content types the below code leverages OfficeDevPnP.Core.  We chose to create the fields and content types programmatically, this gives you greater control of adding new fields, maintenance, and as well as gives you more control to implement localized versions of your fields. 

```C#
//Check the fields
if (!ctx.Web.FieldExistsById(FLD_CLASSIFICATION_ID))
{
    ctx.Web.CreateTaxonomyField(FLD_CLASSIFICATION_ID, 
	FLD_CLASSIFICATION_INTERNAL_NAME, 
	FLD_CLASSIFICATION_DISPLAY_NAME, 
	FIELDS_GROUP_NAME, 
	TAXONOMY_GROUP, 
	TAXONOMY_TERMSET_CLASSIFICATION_NAME);
}

//check the content type
if (!ctx.Web.ContentTypeExistsById(CONTOSODOCUMENT_CT_ID))
{
    ctx.Web.CreateContentType(CONTOSODOCUMENT_CT_NAME, 
    CT_DESC, 
	CONTOSODOCUMENT_CT_ID, 
    CT_GROUP);
}
```

# Create Document Library #
To create a document library we use the following code. We are again, leveraging core to provide this functionality. The following code will create the library, enable versioning and remove the default Document content type. 

```C#
private void CreateLibrary(ClientContext ctx, Library library, string associateContentTypeID) 
{
    if (!ctx.Web.ListExists(library.Title))
    {
       ctx.Web.AddList(ListTemplateType.DocumentLibrary, library.Title, false);
       List _list = ctx.Web.GetListByTitle(library.Title);

       if(!string.IsNullOrEmpty(library.Description)) 
       {
       	_list.Description = library.Description;
       }

       if(library.VerisioningEnabled) {
          _list.EnableVersioning = true;
       }

       _list.ContentTypesEnabled = true;
       _list.Update();
       ctx.Web.AddContentTypeToListById(library.Title, associateContentTypeID, true);
       
       //we are going to remove the default Document Content Type
       _list.RemoveContentTypeByName(ContentTypeManager.DEFAULT_DOCUMENT_CT_NAME);
       ctx.Web.Context.ExecuteQuery();
    }
}
```

Notice the RemoveContentTypeByName member. This is an extension that will remove the default Document content type in the list.

```C#
public static void RemoveContentTypeByName(this List list, string contentTypeName)
{
    if(string.IsNullOrEmpty(contentTypeName)) 
    {
        throw new ArgumentException(string.Format(Constants.EXCEPTION_MSG_INVALID_ARG, "contentTypeName"));
    }

    ContentTypeCollection _cts = list.ContentTypes;
    list.Context.Load(_cts);

    IEnumerable<ContentType> _results = list.Context.LoadQuery<ContentType>(_cts.Where(item => item.Name == contentTypeName));
    list.Context.ExecuteQuery();

    ContentType _ct = _results.FirstOrDefault();
    if (_ct != null)
    {
        _ct.DeleteObject();
        list.Update();
        list.Context.ExecuteQuery();
    }
}
```

# User Validation #
Now, if a user tries to access the provider hosted application, the sample solution will validate if the user has manage list permission. 

```C#
var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
using (var ctx = spContext.CreateUserClientContextForSPHost())
{
    BasePermissions perms = new BasePermissions();
    perms.Set(PermissionKind.ManageLists);
    ClientResult<bool> _permResult = ctx.Web.DoesUserHavePermissions(perms);
    ctx.ExecuteQuery();
    return _permResult.Value;
}
```

![UI when there's no permissions for library creation](http://i.imgur.com/A4a7tbs.png)

## Dependencies ##
- 	Microsoft.SharePoint.Client
-   Microsoft.SharePoint.Client.Runtime
-   Microsoft.SharePoint.Client.Taxonomy
-   [Setting up provider hosted add-in to Windows Azure for Office365 tenant](http://blogs.msdn.com/b/vesku/archive/2013/11/25/setting-up-provider-hosted-app-to-windows-azure-for-office365-tenant.aspx)


<img src="https://telemetry.sharepointpnp.com/pnp/samples/ECM.DocumentLibraries" />