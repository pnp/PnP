
#PnP Provisioning Schema
----------
*Topic automatically generated on 25/04/2015*

##Namespace
The namespace of the PnP Provisioning Schema is:

http://schemas.dev.office.com/PnP/2015/05/ProvisioningSchema

All the elements have to be declared with that namespace reference.

##Root Elements
Here follows the list of root elements available in the PnP Provisioning Schema.
  
<a name="provisioning"></a>
###Provisioning
Represents the root element of a Provisioning File.

```xml
<Provisioning>
   <Preferences />
   <Templates />
   <Sequence />
   <ImportSequence />
</Provisioning>
```


Here follow the available child elements for the Provisioning element.


Element|Description
-------|-----------
[Preferences](#preferences)|Mandatory section of preferences for the current provisioning definition.
[Templates](#templates)|Mandatory section of templates defined in the provisioning file.
[Sequence](#sequence)|Optional sequence of deployment actions to accomplis during the provisioning.
[ImportSequence](#importsequence)|Optional import of an external file with the sequence of deployment actions to accomplish during the provisioning. All current properties should be sent to that file.
<a name="provisioningtemplate"></a>
###ProvisioningTemplate
Represents the root element of the SharePoint Provisioning Template.

```xml
<ProvisioningTemplate
      ID="xsd:ID"
      Version="xsd:decimal">
   <SitePolicy />
   <PropertyBagEntries />
   <Security />
   <SiteFields />
   <ContentTypes />
   <Lists />
   <Features />
   <CustomActions />
   <Files />
   <Pages />
   <TermGroups />
   <ComposedLook />
   <Providers />
</ProvisioningTemplate>
```


Here follow the available child elements for the ProvisioningTemplate element.


Element|Description
-------|-----------
[SitePolicy](#sitepolicy)|The Site Policy of the Provisioning Template, optional element.
[PropertyBagEntries](#propertybagentries)|The Property Bag entries of the Provisioning Template, optional collection of elements.
[Security](#security)|The Security Groups Members of the Provisioning Template, optional collection of elements.
[SiteFields](#sitefields)|The Site Columns of the Provisioning Template, optional element.
[ContentTypes](#contenttypes)|The Content Types of the Provisioning Template, optional element.
[Lists](#lists)|The Lists instances of the Provisioning Template, optional element.
[Features](#features)|The Features (Site or Web) to activate or deactivate while applying the Provisioning Template, optional collection of elements.
[CustomActions](#customactions)|The Custom Actions (Site or Web) to provision with the Provisioning Template, optional element.
[Files](#files)|The Files to provision into the target Site through the Provisioning Template, optional element.
[Pages](#pages)|The Pages to provision into the target Site through the Provisioning Template, optional element.
[TermGroups](#termgroups)|The TermGroups element allows provisioning one or more TermGroups into the target Site, optional element.
[ComposedLook](#composedlook)|The ComposedLook for the Provisioning Template, optional element.
[Providers](#providers)|The Extensiblity Providers to invoke while applying the Provisioning Template, optional collection of elements.

Here follow the available attributes for the ProvisioningTemplate element.


Attibute|Type|Description
--------|----|-----------
ID|xsd:ID|The ID of the Provisioning Template, required attribute.
Version|xsd:decimal|The Version of the Provisioning Template, required attribute.


##Child Elements and Complex Types
Here follows the list of all the other child elements and complex types that can be used in the PnP Provisioning Schema.
<a name="preferences"></a>
###Preferences
General settings for the provisioning file.

```xml
<Preferences
      Version="xsd:string"
      Author="xsd:string"
      Generator="xsd:string">
   <Parameters />
</Preferences>
```


Here follow the available child elements for the Preferences element.


Element|Description
-------|-----------
[Parameters](#parameters)|Definition of parameters.

Here follow the available attributes for the Preferences element.


Attibute|Type|Description
--------|----|-----------
Version|xsd:string|The Version number of the file, optional attribute.
Author|xsd:string|The Author of the file, optional attribute.
Generator|xsd:string|The Name of the tool that generated the file, optional attribute.
<a name="templates"></a>
###Templates
SharePoint Templates, which can be inline or references to external files.

```xml
<Templates
      ID="xsd:ID">
   <ProvisioningTemplateFile />
   <ProvisioningTemplateReference />
   <ProvisioningTemplate />
</Templates>
```


Here follow the available child elements for the Templates element.


Element|Description
-------|-----------
[ProvisioningTemplateFile](#provisioningtemplatefile)|Reference to an external template file, which will be based on the current schema but will focus only on the ProvisioningTemplate section.
[ProvisioningTemplateReference](#provisioningtemplatereference)|Reference by ID to another template, which is still defined or referenced in the current file.
[ProvisioningTemplate](#provisioningtemplate)|

Here follow the available attributes for the Templates element.


Attibute|Type|Description
--------|----|-----------
ID|xsd:ID|A unique identifier of the Templates collection, optional attribute.
<a name="stringdictionaryitem"></a>
###StringDictionaryItem
Defines a StringDictionary element.

```xml
<StringDictionaryItem
      Key="xsd:string"
      Value="xsd:string">
</StringDictionaryItem>
```


Here follow the available attributes for the StringDictionaryItem element.


Attibute|Type|Description
--------|----|-----------
Key|xsd:string|The Key of the property to store in the StringDictionary, required attribute.
Value|xsd:string|The Value of the property to store in the StringDictionary, required attribute
<a name="userslist"></a>
###UsersList
List of Users for the Site Security, collection of elements.

```xml
<UsersList>
   <User />
</UsersList>
```


Here follow the available child elements for the UsersList element.


Element|Description
-------|-----------
[User](#user)|
<a name="user"></a>
###User
The base abstract type for a User element.

```xml
<User
      Name="xsd:string">
</User>
```


Here follow the available attributes for the User element.


Attibute|Type|Description
--------|----|-----------
Name|xsd:string|The Name of the User, required attribute.
<a name="listinstance"></a>
###ListInstance
Defines a ListInstance element.

```xml
<ListInstance
      Title="xsd:string"
      Description="xsd:string"
      DocumentTemplate="xsd:string"
      OnQuickLaunch="xsd:boolean"
      TemplateType="xsd:int"
      Url="xsd:string"
      EnableVersioning="xsd:boolean"
      MinorVersionLimit="xsd:int"
      MaxVersionLimit="xsd:int"
      RemoveExistingContentTypes="xsd:boolean"
      TemplateFeatureID="pnp:GUID"
      ContentTypesEnabled="xsd:boolean"
      Hidden="xsd:boolean"
      EnableAttachments="xsd:boolean"
      EnableFolderCreation="xsd:boolean">
   <ContentTypeBindings />
   <Views />
   <Fields />
   <FieldRefs />
   <DataRows />
</ListInstance>
```


Here follow the available child elements for the ListInstance element.


Element|Description
-------|-----------
[ContentTypeBindings](#contenttypebindings)|The ContentTypeBindings entries of the List Instance, optional collection of elements.
[Views](#views)|The Views entries of the List Instance, optional collection of elements.
[Fields](#fields)|The Fields entries of the List Instance, optional collection of elements.
[FieldRefs](#fieldrefs)|The FieldRefs entries of the List Instance, optional collection of elements.
[DataRows](#datarows)|A container of Data Rows, which will be added to the target List Instance, optional element.

Here follow the available attributes for the ListInstance element.


Attibute|Type|Description
--------|----|-----------
Title|xsd:string|The Title of the List Instance, required attribute.
Description|xsd:string|The Description of the List Instance, optional attribute.
DocumentTemplate|xsd:string|The DocumentTemplate of the List Instance, optional attribute.
OnQuickLaunch|xsd:boolean|The OnQuickLaunch flag for the List Instance, optional attribute.
TemplateType|xsd:int|The TemplateType of the List Instance, required attribute Values available here: https://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.listtemplatetype.aspx
Url|xsd:string|The Url of the List Instance, required attribute.
EnableVersioning|xsd:boolean|The EnableVersioning flag for the List Instance, optional attribute.
MinorVersionLimit|xsd:int|The MinorVersionLimit for versions history for the List Instance, optional attribute.
MaxVersionLimit|xsd:int|The MaxVersionLimit for versions history for the List Instance, optional attribute.
RemoveExistingContentTypes|xsd:boolean|The RemoveExistingContentTypes flag for the List Instance, optional attribute.
TemplateFeatureID|pnp:GUID|The TemplateFeatureID for the feature on which the List Instance is based, optional attribute.
ContentTypesEnabled|xsd:boolean|The ContentTypesEnabled flag for the List Instance, optional attribute.
Hidden|xsd:boolean|The Hidden flag for the List Instance, optional attribute.
EnableAttachments|xsd:boolean|The EnableAttachments flag for the List Instance, optional attribute.
EnableFolderCreation|xsd:boolean|The EnableFolderCreation flag for the List Instance, optional attribute.
<a name="datavalue"></a>
###DataValue
A single Data Value that represents a field of an item, which will be added to the target List Instance, at least one DataValue element is required for each DataRow element.

```xml
<DataValue
      FieldName="xsd:string">
</DataValue>
```


Here follow the available attributes for the DataValue element.


Attibute|Type|Description
--------|----|-----------
FieldName|xsd:string|
<a name="contenttype"></a>
###ContentType
Defines a Content Type, which will be added to the target site.

```xml
<ContentType
      ID="pnp:ContentTypeId"
      Name="xsd:string"
      Description="xsd:string"
      Group="xsd:string"
      Hidden="xsd:boolean"
      Sealed="xsd:boolean"
      ReadOnly="xsd:boolean"
      Overwrite="xsd:boolean">
   <FieldRefs />
   <DocumentTemplate />
</ContentType>
```


Here follow the available child elements for the ContentType element.


Element|Description
-------|-----------
[FieldRefs](#fieldrefs)|The FieldRefs entries of the Content Type, optional collection of elements.
[DocumentTemplate](#documenttemplate)|Specifies the document template for the content type. This is the file which SharePoint Foundation opens as a template when a user requests a new item of this content type.

Here follow the available attributes for the ContentType element.


Attibute|Type|Description
--------|----|-----------
ID|pnp:ContentTypeId|The value of the content type ID, required attribute.
Name|xsd:string|The name of the content type, required attribute.
Description|xsd:string|The description of the content type, optional attribute.
Group|xsd:string|The group of the content type, optional attribute.
Hidden|xsd:boolean|Optional Boolean. True to define the content type as hidden. If you define a content type as hidden, SharePoint Foundation does not display that content type on the New button in list views.
Sealed|xsd:boolean|Optional Boolean. True to prevent changes to this content type. You cannot change the value of this attribute through the user interface, but you can change it in code if you have sufficient rights. You must have site collection administrator rights to unseal a content type.
ReadOnly|xsd:boolean|Optional Boolean. True to specify that the content type cannot be edited without explicitly removing the read-only setting. This can be done either in the user interface or in code.
Overwrite|xsd:boolean|Optional Boolean. True to overwrite an existing content type with the same ID.
<a name="contenttypebinding"></a>
###ContentTypeBinding
Defines the binding between a ListInstance and a ContentType.

```xml
<ContentTypeBinding
      ContentTypeID="pnp:ContentTypeId"
      Default="xsd:boolean">
</ContentTypeBinding>
```


Here follow the available attributes for the ContentTypeBinding element.


Attibute|Type|Description
--------|----|-----------
ContentTypeID|pnp:ContentTypeId|The value of the ContentTypeID to bind, required attribute.
Default|xsd:boolean|Declares if the Content Type should be the default Content Type in the list or library, optional attribute. Default value False.
<a name="featureslist"></a>
###FeaturesList
Defines a collection of elements of type Feature.

```xml
<FeaturesList>
   <Feature />
</FeaturesList>
```


Here follow the available child elements for the FeaturesList element.


Element|Description
-------|-----------
[Feature](#feature)|
<a name="feature"></a>
###Feature
Defines a single Site or Web Feature, which will be activated or deactivated while applying the Provisioning Template.

```xml
<Feature
      ID="pnp:GUID"
      Deactivate="xsd:boolean"
      Description="xsd:string">
</Feature>
```


Here follow the available attributes for the Feature element.


Attibute|Type|Description
--------|----|-----------
ID|pnp:GUID|The unique ID of the Feature, required attribute
Deactivate|xsd:boolean|Defines if the feature has to be deactivated or activated while applying the Provisioning Template, optional attribute. The default value is False.
Description|xsd:string|The Description of the feature, optional attribute.
<a name="listinstancefieldref"></a>
###ListInstanceFieldRef
Defines the binding between a ListInstance and a Field.

```xml
<ListInstanceFieldRef
      ID="pnp:GUID"
      Name="xsd:string"
      Required="xsd:boolean"
      Hidden="xsd:boolean"
      DisplayName="xsd:string">
</ListInstanceFieldRef>
```


Here follow the available attributes for the ListInstanceFieldRef element.


Attibute|Type|Description
--------|----|-----------
ID|pnp:GUID|The value of the field ID to bind, required attribute.
Name|xsd:string|The name of the field used in the field reference. This is for reference/readibility only.
Required|xsd:boolean|The Required flag for the field to bind, optional attribute.
Hidden|xsd:boolean|The Hidden flag for the field to bind, optional attribute.
DisplayName|xsd:string|The display name of the field to bind, only applicable to fields that will be added to lists, optional attribute.
<a name="contenttypefieldref"></a>
###ContentTypeFieldRef
Defines the binding between a ContentType and a Field.

```xml
<ContentTypeFieldRef
      ID="pnp:GUID"
      Name="xsd:string"
      Required="xsd:boolean"
      Hidden="xsd:boolean">
</ContentTypeFieldRef>
```


Here follow the available attributes for the ContentTypeFieldRef element.


Attibute|Type|Description
--------|----|-----------
ID|pnp:GUID|The value of the field ID to bind, required attribute.
Name|xsd:string|The name of the field used in the field reference. This is for reference/readibility only.
Required|xsd:boolean|The Required flag for the field to bind, optional attribute.
Hidden|xsd:boolean|The Hidden flag for the field to bind, optional attribute.
<a name="customactionslist"></a>
###CustomActionsList
Defines a collection of elements of type CustomAction.

```xml
<CustomActionsList>
   <CustomAction />
</CustomActionsList>
```


Here follow the available child elements for the CustomActionsList element.


Element|Description
-------|-----------
[CustomAction](#customaction)|
<a name="customaction"></a>
###CustomAction
Defines a Custom Action, which will be provisioned while applying the Provisioning Template.

```xml
<CustomAction
      Name="xsd:string"
      Description="xsd:string"
      Group="xsd:string"
      Location="xsd:string"
      Title="xsd:string"
      Sequence="xsd:int"
      Rights="xsd:int"
      Url="xsd:string"
      Enabled="xsd:boolean"
      ScriptBlock="xsd:string"
      ImageUrl="xsd:string"
      ScriptSrc="xsd:string">
</CustomAction>
```


Here follow the available attributes for the CustomAction element.


Attibute|Type|Description
--------|----|-----------
Name|xsd:string|The Name of the CustomAction, required attribute.
Description|xsd:string|The Description of the CustomAction, optional attribute.
Group|xsd:string|The Group of the CustomAction, optional attribute.
Location|xsd:string|The Location of the CustomAction, required attribute.
Title|xsd:string|The Title of the CustomAction, required attribute.
Sequence|xsd:int|The Sequence of the CustomAction, optional attribute.
Rights|xsd:int|The Rights for the CustomAction, based on values from Microsoft.SharePoint.Client.BasePermissions, optional attribute.
Url|xsd:string|The URL of the CustomAction, optional attribute.
Enabled|xsd:boolean|The Enabled flag for the CustomAction, optional attribute.
ScriptBlock|xsd:string|The ScriptBlock of the CustomAction, optional attribute.
ImageUrl|xsd:string|The ImageUrl of the CustomAction, optional attribute.
ScriptSrc|xsd:string|The ScriptSrc of the CustomAction, optional attribute.
<a name="fileproperties"></a>
###FileProperties
A collection of File Properties.

```xml
<FileProperties>
   <Property />
</FileProperties>
```


Here follow the available child elements for the FileProperties element.


Element|Description
-------|-----------
[Property](#property)|
<a name="file"></a>
###File
Defines a File element, to describe a file that will be provisioned into the target Site.

```xml
<File
      Src="xsd:string"
      Folder="xsd:string"
      Overwrite="xsd:boolean"
      Create="xsd:boolean">
   <Properties />
   <WebParts />
</File>
```


Here follow the available child elements for the File element.


Element|Description
-------|-----------
[Properties](#properties)|The File Properties, optional collection of elements.
[WebParts](#webparts)|The WebParts to add to the Page, optional collection of elements.

Here follow the available attributes for the File element.


Attibute|Type|Description
--------|----|-----------
Src|xsd:string|The Src location of the File, required attribute.
Folder|xsd:string|The TargetFolder of the File, required attribute.
Overwrite|xsd:boolean|The Overwrite flag for the File, optional attribute.
Create|xsd:boolean|Optional: if set to false the file will not be created. Use to add WebParts to existing WebPart Pages.
<a name="page"></a>
###Page
Defines a WikiPage element, to describe a page that will be provisioned into the target Site. Because of the Layout attribute, the assumption is made that you're referring/creating a WikiPage.

```xml
<Page
      Url="xsd:string"
      Overwrite="xsd:boolean"
      Layout="pnp:WikiPageLayout">
   <WebParts />
</Page>
```


Here follow the available child elements for the Page element.


Element|Description
-------|-----------
[WebParts](#webparts)|The WebParts to add to the page, optional collection of elements.

Here follow the available attributes for the Page element.


Attibute|Type|Description
--------|----|-----------
Url|xsd:string|The server relative Url of the WikiPage, supports tokens, required attribute.
Overwrite|xsd:boolean|Defines whether to overwrite the WikiPage if it already exists, optional attribute. The default value is False.
Layout|pnp:WikiPageLayout|Defines the layout of the WikiPage, required attribute.
<a name="wikipagewebpart"></a>
###WikiPageWebPart
Defines a WebPart to be added to a WikiPage.

```xml
<WikiPageWebPart
      Title="xsd:string"
      Row="xsd:int"
      Column="xsd:int">
   <Contents />
</WikiPageWebPart>
```


Here follow the available child elements for the WikiPageWebPart element.


Element|Description
-------|-----------
[Contents](#contents)|Defines the WebPart XML, required element.

Here follow the available attributes for the WikiPageWebPart element.


Attibute|Type|Description
--------|----|-----------
Title|xsd:string|Defines the title of the WebPart, required attribute.
Row|xsd:int|Defines the Row to add the WebPart to, required attribute.
Column|xsd:int|Defines the Column to add the WebPart to, required attribute.
<a name="webpartpagewebpart"></a>
###WebPartPageWebPart
Defines a WebPart to be added to a WebPart Page.

```xml
<WebPartPageWebPart
      Title="xsd:string"
      Zone="xsd:string"
      Order="xsd:int">
   <Contents />
</WebPartPageWebPart>
```


Here follow the available child elements for the WebPartPageWebPart element.


Element|Description
-------|-----------
[Contents](#contents)|Defines the WebPart XML, required element.

Here follow the available attributes for the WebPartPageWebPart element.


Attibute|Type|Description
--------|----|-----------
Title|xsd:string|Defines the Title of the WebPart, required attribute.
Zone|xsd:string|Defines the Zone of a WebPart Page to add the webpart to, required attribute.
Order|xsd:int|Defines the index of the WebPart in the zone, required attribute.
<a name="composedlook"></a>
###ComposedLook
Defines a ComposedLook element.

```xml
<ComposedLook
      Name="xsd:string"
      ColorFile="xsd:string"
      FontFile="xsd:string"
      BackgroundFile="xsd:string"
      MasterPage="xsd:string"
      SiteLogo="xsd:string"
      AlternateCSS="xsd:string"
      Version="xsd:int">
</ComposedLook>
```


Here follow the available attributes for the ComposedLook element.


Attibute|Type|Description
--------|----|-----------
Name|xsd:string|The Name of the ComposedLook, required attribute.
ColorFile|xsd:string|The ColorFile of the ComposedLook, required attribute.
FontFile|xsd:string|The FontFile of the ComposedLook, required attribute.
BackgroundFile|xsd:string|The BackgroundFile of the ComposedLook, optional attribute.
MasterPage|xsd:string|The MasterPage of the ComposedLook, required attribute.
SiteLogo|xsd:string|The SiteLogo of the ComposedLook, optional attribute.
AlternateCSS|xsd:string|The AlternateCSS of the ComposedLook, optional attribute.
Version|xsd:int|The Version of the ComposedLook, optional attribute.
<a name="provider"></a>
###Provider
Defines an Extensibility Provider, which will be invoked at the end of the provisioning.

```xml
<Provider
      Enabled="xsd:boolean"
      HandlerType="xsd:string">
   <Configuration />
</Provider>
```


Here follow the available child elements for the Provider element.


Element|Description
-------|-----------
[Configuration](#configuration)|Defines an optional configuration section for the Extensibility Provider. The configuration section can be any XML.

Here follow the available attributes for the Provider element.


Attibute|Type|Description
--------|----|-----------
Enabled|xsd:boolean|Defines whether the Extensibility Provider is enabled or not, optional attribute. The default value is False.
HandlerType|xsd:string|The type of the handler. It can be a FQN of a .NET type, the URL of a node.js file, or whatever else, required attribute.
<a name="provisioningtemplatefile"></a>
###ProvisioningTemplateFile
An element that references an external file.

```xml
<ProvisioningTemplateFile
      File="xsd:string"
      ID="xsd:ID">
</ProvisioningTemplateFile>
```


Here follow the available attributes for the ProvisioningTemplateFile element.


Attibute|Type|Description
--------|----|-----------
File|xsd:string|The absolute or relative path to the file, required attribute.
ID|xsd:ID|The ID of the referenced template, required attribute.
<a name="provisioningtemplatereference"></a>
###ProvisioningTemplateReference
An element that references an external file.

```xml
<ProvisioningTemplateReference
      ID="xsd:IDREF">
</ProvisioningTemplateReference>
```


Here follow the available attributes for the ProvisioningTemplateReference element.


Attibute|Type|Description
--------|----|-----------
ID|xsd:IDREF|The ID of the referenced template, required attribute.
<a name="sequence"></a>
###Sequence
Each Provisioning file is split into a set of Sequence elements. The Sequence element groups the artefacts to be provisioned into groups. The Sequences must be evaluated by the provisioning engine in the order in which they appear.

```xml
<Sequence
      SequenceType=""
      ID="xsd:ID">
   <SiteCollection />
   <Site />
   <TermStore />
   <Extensions />
</Sequence>
```


Here follow the available child elements for the Sequence element.


Element|Description
-------|-----------
[SiteCollection](#sitecollection)|
[Site](#site)|
[TermStore](#termstore)|
[Extensions](#extensions)|

Here follow the available attributes for the Sequence element.


Attibute|Type|Description
--------|----|-----------
SequenceType||Instructions to the Provisioning engine on how the Containers within the Sequence can be provisioned.
ID|xsd:ID|A unique identifier of the Sequence, required attribute.
<a name="sitecollection"></a>
###SiteCollection
Defines a SiteCollection that will be created into the target tenant/farm.

```xml
<SiteCollection
      Url="pnp:ReplaceableString">
   <Templates />
</SiteCollection>
```


Here follow the available child elements for the SiteCollection element.


Element|Description
-------|-----------
[Templates](#templates)|

Here follow the available attributes for the SiteCollection element.


Attibute|Type|Description
--------|----|-----------
Url|pnp:ReplaceableString|Absolute Url to the site, required attribute.
<a name="site"></a>
###Site
Defines a Site that will be created into a target Site Collection

```xml
<Site
      UseSamePermissionsAsParentSite="xsd:boolean"
      Url="pnp:ReplaceableString">
   <Templates />
</Site>
```


Here follow the available child elements for the Site element.


Element|Description
-------|-----------
[Templates](#templates)|

Here follow the available attributes for the Site element.


Attibute|Type|Description
--------|----|-----------
UseSamePermissionsAsParentSite|xsd:boolean|Defines whether to use the same permisssions of the parent Site, optional attribute.
Url|pnp:ReplaceableString|Relative Url to the site, required attribute.
<a name="termstore"></a>
###TermStore
A TermStore to use for provisioning of TermGroups. It is supported on-premises only.

```xml
<TermStore
      Scope="">
   <TermGroup />
</TermStore>
```


Here follow the available child elements for the TermStore element.


Element|Description
-------|-----------
[TermGroup](#termgroup)|The TermGroup element to provision into the target TermStore through, optional element.

Here follow the available attributes for the TermStore element.


Attibute|Type|Description
--------|----|-----------
Scope||The scope of the term store, required attribute.
<a name="termgroup"></a>
###TermGroup
A TermGroup to use for provisioning of TermSets and Terms.

```xml
<TermGroup
      Description="xsd:string"
      Name="xsd:string"
      ID="pnp:GUID">
</TermGroup>
```


Here follow the available attributes for the TermGroup element.


Attibute|Type|Description
--------|----|-----------
Description|xsd:string|
Name|xsd:string|The Name of the TaxonomyItem, required attribute.
ID|pnp:GUID|The ID of the TaxonomyItem, optional attribute.
<a name="termsetitem"></a>
###TermSetItem
Base type for TermSets and Terms.

```xml
<TermSetItem
      Owner="xsd:string"
      Description="xsd:string"
      IsAvailableForTagging="xsd:boolean">
</TermSetItem>
```


Here follow the available attributes for the TermSetItem element.


Attibute|Type|Description
--------|----|-----------
Owner|xsd:string|The Owner of the TaxonomyItem, optional attribute.
Description|xsd:string|The Description of the TaxonomyItem, optional attribute.
IsAvailableForTagging|xsd:boolean|Defines whether the TaxonomyItem is available for tagging or not, optional attribute. The default value is True.
<a name="termset"></a>
###TermSet
A TermSet to provision.

```xml
<TermSet
      Language="xsd:int"
      IsOpenForTermCreation="xsd:boolean">
</TermSet>
```


Here follow the available attributes for the TermSet element.


Attibute|Type|Description
--------|----|-----------
Language|xsd:int|The Language of the TermSet, optional attribute.
IsOpenForTermCreation|xsd:boolean|Defines whether the TermSet is open for Term creation or not, optional attribute. The default value is False.
<a name="term"></a>
###Term
A Term to provision into a TermSet or a hyerarchical Term.

```xml
<Term
      Language="xsd:int"
      CustomSortOrder="xsd:int">
</Term>
```


Here follow the available attributes for the Term element.


Attibute|Type|Description
--------|----|-----------
Language|xsd:int|The Language of the Term, optional attribute.
CustomSortOrder|xsd:int|The Custom Sort Order of the Term, optional attribute. Use sequential numbers.
<a name="taxonomyitemproperties"></a>
###TaxonomyItemProperties
A collection of Term Properties, used for CustomProperties and LocalCustomProperties of a Term.

```xml
<TaxonomyItemProperties>
   <Property />
</TaxonomyItemProperties>
```


Here follow the available child elements for the TaxonomyItemProperties element.


Element|Description
-------|-----------
[Property](#property)|
<a name="termlabels"></a>
###TermLabels
A collection of Term Labels, in order to support multi-language terms.

```xml
<TermLabels>
   <Label />
</TermLabels>
```


Here follow the available child elements for the TermLabels element.


Element|Description
-------|-----------
[Label](#label)|
<a name="termsets"></a>
###TermSets
A collection of TermSets to provision.

```xml
<TermSets>
   <TermSet />
</TermSets>
```


Here follow the available child elements for the TermSets element.


Element|Description
-------|-----------
[TermSet](#termset)|A TermSet that will be provisioned within the collection of TermSets, optional collection of elements.
<a name="extensions"></a>
###Extensions
Extensions are custom XML elements and instructions that can be extensions of this default schema or vendor or engine specific extensions.

```xml
<Extensions>
</Extensions>
```

<a name="importsequence"></a>
###ImportSequence
Imports sequences from an external file. All current properties should be sent to that file.

```xml
<ImportSequence
      File="xsd:string">
</ImportSequence>
```


Here follow the available attributes for the ImportSequence element.


Attibute|Type|Description
--------|----|-----------
File|xsd:string|Absolute or relative path to the file, required attribute.
