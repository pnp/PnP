# DocumentSetTemplate using CSOM #

### Summary ###
Demonstrates how to use DocumentSetTemplate using CSOM.

*Notice that there is a bug in the lanuguage controlling API with 2014 December CU. This will be fixed with future releases.*

### Applies to ###
-  Office 365 Multi Tenant (MT) - With upcoming CSOM package
-  Office 365 Dedicated (D) 
-  SharePoint 2013 on-premises

### Prerequisites ###
2015 March CU installed on the farm or new CSOM cloud re-distributable package (released around end of March 2015)

### Solution ###
Solution | Author(s)
---------|----------
Core.DocumentSetTemplate | Frank Chen

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | March 19th 2015 (to update) | Draft version

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Controlling regional settings and languages #
Sample shows simple API calls to control these required settings. 

![](http://i.imgur.com/dbXy4Cf.png)

## Controlling regional settings ##
You can control regional settings by using LocaleId property in the regional settings object. 
```C#
Web web = clientContext.Web;

clientContext.Load(web, w => w.ContentTypes, w => w.Fields);
clientContext.ExecuteQuery();
template = GetDocumentSetTemplate(clientContext);
foreach (ContentType ct in web.ContentTypes)
{
    //find out documentset and child content type
    if (ct.Name.IndexOf("document set", StringComparison.CurrentCultureIgnoreCase) != -1 ||
        DocumentSetTemplate.IsChildOfDocumentSetContentType(clientContext, ct).Value)
    {
        template = DocumentSetTemplate.GetDocumentSetTemplate(clientContext, ct);
        clientContext.Load(template, t => t.AllowedContentTypes, t => t.DefaultDocuments, t => t.SharedFields, t => t.WelcomePageFields);
        clientContext.ExecuteQuery();

        foreach (ContentTypeId ctId in template.AllowedContentTypes)
        {
            ContentType ctAllowed = clientContext.Web.ContentTypes.First(d => d.StringId == ctId.StringValue);
            if (ctAllowed != null)
                model.AllowedContentTypes.Add(new ContentTypeModel() { Id = ctId, Name = ctAllowed.Name });
        }

        foreach (Field field in template.SharedFields)
        {
            model.SharedFields.Add(new FieldModel() { Id = field.Id, Name = field.InternalName, Type = field.TypeDisplayName });
        }

        foreach (Field field in template.WelcomePageFields)
        {
            model.WelcomeFields.Add(new FieldModel() { Id = field.Id, Name = field.InternalName, Type = field.TypeDisplayName });
        }
        break;
    }
}
```

### Add ContentType to AllowedContentTypes ###

```C#
Web web = clientContext.Web;

clientContext.Load(web, w => w.ContentTypes, w => w.Fields);
clientContext.ExecuteQuery();

var query = from ct in web.ContentTypes
            where ct.Id.StringValue == model.SelectedStringId 
            select ct;
ContentType ctFound = query.First();

DocumentSetTemplate template = GetDocumentSetTemplate(clientContext);
if(template !=null)
{
    template.AllowedContentTypes.Add(ctFound.Id);
    template.Update(true);
    clientContext.Load(template);
    clientContext.ExecuteQuery();
}
```

### Remove ContentType to AllowedContentTypes ###

```C#
Web web = clientContext.Web;

clientContext.Load(web, w => w.ContentTypes, w => w.Fields);
clientContext.ExecuteQuery();

var query = from ct in web.ContentTypes
            where ct.Id.StringValue == id
            select ct;
ContentType ctFound = query.First();

DocumentSetTemplate template = GetDocumentSetTemplate(clientContext);
if (template != null)
{
    template.AllowedContentTypes.Remove(ctFound.Id);
    template.Update(true);
    clientContext.Load(template);
    clientContext.ExecuteQuery();
}
```