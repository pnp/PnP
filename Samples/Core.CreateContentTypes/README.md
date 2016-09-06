# Creating Content Types #

### Summary ###
This sample shows how you can create site columns, content types and add then add the site columns to the content type. It will also explain the new localization features that have been introduced for Office 365 CSOM APIs.

For more information on this sample, please see Vesa Juvonen's thorough blog post: [http://blogs.msdn.com/b/vesku/archive/2014/02/28/ftc-to-cam-create-content-types-with-specific-ids-using-csom.aspx](http://blogs.msdn.com/b/vesku/archive/2014/02/28/ftc-to-cam-create-content-types-with-specific-ids-using-csom.aspx)

### Video Walkthrough ##
A comprehensive video of the solution can be found at [http://www.youtube.com/watch?v=w7i0gkqxzfg](http://www.youtube.com/watch?v=w7i0gkqxzfg "http://www.youtube.com/watch?v=w7i0gkqxzfg")

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D) - *Multilingual part not supported*
-  SharePoint 2013 on-premises - *Multilingual part not supported*

### Solution ###
Solution | Author(s)
---------|----------
Core.CreateContentTypes | Vesa Juvonen (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | February 28th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Creation of Content Types and Site Columns #
Below code snippet shows how to create a content type using the ContentTypeCreationInformation class. Also note that since SP1 you now can set the Id:

    ContentTypeCollection contentTypes = web.ContentTypes;
    cc.Load(contentTypes);
    cc.ExecuteQuery();
    
    foreach (var item in contentTypes)
    {
      if (item.StringId == "0x0101009189AB5D3D2647B580F011DA2F356FB2")
        return;
    }

    // Create a Content Type Information object
    ContentTypeCreationInformation newCt = new ContentTypeCreationInformation();
    // Set the name for the content type
    newCt.Name = "Contoso Document";
    //Inherit from oob document - 0x0101 and assign
    newCt.Id = "0x0101009189AB5D3D2647B580F011DA2F356FB2";
    // Set content type to be avaialble from specific group
    newCt.Group = "Contoso Content Types";
    // Create the content type
    ContentType myContentType = contentTypes.Add(newCt);
    cc.ExecuteQuery();
    
Using AddFieldAsXml you can add fields to the FieldCollection of a site collection:

    FieldCollection fields = web.Fields;
    cc.Load(fields);
    cc.ExecuteQuery();

    string FieldAsXML = @"<Field ID='{4F34B2ED-9CFF-4900-B091-4C0033F89944}' Name='ContosoString' DisplayName='Contoso String' Type='Text' Hidden='False' Group='Contoso Site Columns' Description='Contoso Text Field' />";
    Field fld = fields.AddFieldAsXml(FieldAsXML, true, AddFieldOptions.DefaultValue);
    cc.Load(fields);
    cc.Load(fld);
    cc.ExecuteQuery();


Finally the fields need to be linked to the content type which is done by using the FieldLinkCollection and FieldLinkCreationInformation classes as is shown in below sample:

    FieldCollection fields = web.Fields;
    Field fld = fields.GetByInternalNameOrTitle("ContosoString");
    cc.Load(fields);
    cc.Load(fld);
    cc.ExecuteQuery();

    FieldLinkCollection refFields = myContentType.FieldLinks;
    cc.Load(refFields);
    cc.ExecuteQuery();

    foreach (var item in refFields)
    {
      if (item.Name == "ContosoString")
        return;
    }

    // ref does nt
    FieldLinkCreationInformation link = new FieldLinkCreationInformation();
    link.Field = fld;
    myContentType.FieldLinks.Add(link);
    myContentType.Update(true);
    cc.ExecuteQuery();

# Localization of Content Types, Lists, and Site Titles #
**Notice** *This section is only available in CSOM 16 version assemblies, meaning that it only works in the Office 365 MT and in the Office 365 Dedicated vNext.* 

If needed you can localize the site title and site description using below code sample:

    web.TitleResource.SetValueForUICulture("fi-FI", "Kielikäännä minut");
    web.DescriptionResource.SetValueForUICulture("fi-FI", "Kielikäännetty saitti");

For a list the same approach can be used to localize the title and description:

    list.TitleResource.SetValueForUICulture("fi-FI", "Kielikäännä minut");
    list.DescriptionResource.SetValueForUICulture("fi-FI", "Tämä esimerkki näyttää miten voit kielikääntää listoja.");

For content types you have the option to localize the name and description while for the fields you can localize the title and description values:

    myContentType.NameResource.SetValueForUICulture("fi-FI", "Contoso Dokumentti");
    myContentType.DescriptionResource.SetValueForUICulture("fi-FI", "Tämä on geneerinen Contoso dokumentti.");

    fld.TitleResource.SetValueForUICulture("fi-FI", "Contoso Teksti");
    fld.DescriptionResource.SetValueForUICulture("fi-FI", "Tää on niiku Contoso metadatalle.");
 
 <img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.CreateContentTypes" />