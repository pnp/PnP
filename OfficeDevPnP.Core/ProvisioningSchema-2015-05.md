
#PnP Provisioning Schema
----------
*Topic automatically generated on 29/04/2015*

##Namespace
The namespace of the PnP Provisioning Schema is:

http://schemas.dev.office.com/PnP/2015/05/ProvisioningSchema

All the elements have to be declared with that namespace reference.

##Root Elements
Here follows the list of root elements available in the PnP Provisioning Schema.
  
<a name="provisioning"></a>
###Provisioning


```xml
<Provisioning>
   <Preferences />
   <Templates />
   <Sequence />
   <ImportSequence />
</Provisioning>
```


Here follow the available child elements for the Provisioning element.


Element|Type|Description
-------|----|-----------
Preferences|[Preferences](#preferences)|The mandatory section of preferences for the current provisioning definition.
Templates|[Templates](#templates)|An optional section made of provisioning templates.
Sequence|[Sequence](#sequence)|An optional section made of provisioning sequences, which can include Sites, Site Collections, Taxonomies, Provisioning Templates, etc.
ImportSequence|[ImportSequence](#importsequence)|Imports sequences from an external file. All current properties should be sent to that file.
<a name="provisioningtemplate"></a>
###ProvisioningTemplate
Represents the root element of the SharePoint Provisioning Template

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


Element|Type|Description
-------|----|-----------
SitePolicy|xsd:string|The Site Policy of the Provisioning Template, optional element
PropertyBagEntries|[PropertyBagEntries](#propertybagentries)|The Property Bag entries of the Provisioning Template, optional collection of elements
Security|[Security](#security)|The Security Groups Members of the Provisioning Template, optional collection of elements
SiteFields|[SiteFields](#sitefields)|The Site Columns of the Provisioning Template, optional element
ContentTypes|[ContentTypes](#contenttypes)|The Content Types of the Provisioning Template, optional element
Lists|[Lists](#lists)|The Lists instances of the Provisioning Template, optional element
Features|[Features](#features)|The Features (Site or Web) to activate or deactivate while applying the Provisioning Template, optional collection of elements
CustomActions|[CustomActions](#customactions)|The Custom Actions (Site or Web) to provision with the Provisioning Template, optional element
Files|[Files](#files)|The Files to provision into the target Site through the Provisioning Template, optional element
Pages|[Pages](#pages)|The Pages to provision into the target Site through the Provisioning Template, optional element
TermGroups|[TermGroups](#termgroups)|The TermGroups element allows provisioning one or more TermGroups into the target Site, optional element
ComposedLook|[ComposedLook](#composedlook)|The ComposedLook for the Provisioning Template, optional element
Providers|[Providers](#providers)|The Extensiblity Providers to invoke while applying the Provisioning Template, optional collection of elements

Here follow the available attributes for the ProvisioningTemplate element.


Attibute|Type|Description
--------|----|-----------
ID|xsd:ID|The ID of the Provisioning Template, required attribute
Version|xsd:decimal|The Version of the Provisioning Template, required attribute


##Child Elements and Complex Types
Here follows the list of all the other child elements and complex types that can be used in the PnP Provisioning Schema.
<a name="preferences"></a>
###Preferences
General settings for a Provisioning file.

```xml
<Preferences
      Version="xsd:string"
      Author="xsd:string"
      Generator="xsd:string">
   <Parameters />
</Preferences>
```


Here follow the available child elements for the Preferences element.


Element|Type|Description
-------|----|-----------
Parameters|[Parameters](#parameters)|Definition of parameters that can be used as replacement within templates and provisioning objects.

Here follow the available attributes for the Preferences element.


Attibute|Type|Description
--------|----|-----------
Version|xsd:string|Optional version number.
Author|xsd:string|Optional Author name
Generator|xsd:string|Optional Name of tool generating this file
<a name="parameters"></a>
###Parameters
Definition of parameters that can be used as replacement within templates and provisioning objects.

```xml
<Parameters>
   <Parameter />
</Parameters>
```


Here follow the available child elements for the  element.


Element|Type|Description
-------|----|-----------
Parameter|[Parameter](#parameter)|A Parameter that can be used as a replacement within templates and provisioning objects.
<a name="templates"></a>
###Templates
SharePoint Templates, which can be inline or references to external files

```xml
<Templates
      ID="xsd:ID">
   <ProvisioningTemplateFile />
   <ProvisioningTemplateReference />
   <ProvisioningTemplate />
</Templates>
```


Here follow the available child elements for the Templates element.


Element|Type|Description
-------|----|-----------
ProvisioningTemplateFile|[ProvisioningTemplateFile](#provisioningtemplatefile)|Reference to an external template file, which will be based on the current schema but will focus only on the SharePointProvisioningTemplate section.
ProvisioningTemplateReference|[ProvisioningTemplateReference](#provisioningtemplatereference)|Reference to another template by ID.
ProvisioningTemplate|[ProvisioningTemplate](#provisioningtemplate)|

Here follow the available attributes for the Templates element.


Attibute|Type|Description
--------|----|-----------
ID|xsd:ID|A unique identifier of the Templates collection, optional attribute
<a name="propertybagentries"></a>
###PropertyBagEntries
The Property Bag entries of the Provisioning Template, optional collection of elements

```xml
<PropertyBagEntries>
   <PropertyBagEntry />
</PropertyBagEntries>
```


Here follow the available child elements for the  element.


Element|Type|Description
-------|----|-----------
PropertyBagEntry|[StringDictionaryItem](#stringdictionaryitem)|
<a name="security"></a>
###Security
The Security Groups Members of the Provisioning Template, optional collection of elements

```xml
<Security>
   <AdditionalAdministrators />
   <AdditionalOwners />
   <AdditionalMembers />
   <AdditionalVisitors />
</Security>
```


Here follow the available child elements for the  element.


Element|Type|Description
-------|----|-----------
AdditionalAdministrators|[UsersList](#userslist)|List of additional Administrators for the Site, optional collection of elements
AdditionalOwners|[UsersList](#userslist)|List of additional Owners for the Site, optional collection of elements
AdditionalMembers|[UsersList](#userslist)|List of additional Members for the Site, optional collection of elements
AdditionalVisitors|[UsersList](#userslist)|List of additional Visitors for the Site, optional collection of elements
<a name="sitefields"></a>
###SiteFields
The Site Columns of the Provisioning Template, optional element

```xml
<SiteFields>
   <!-- Any other XML content -->
</SiteFields>
```

<a name="contenttypes"></a>
###ContentTypes
The Content Types of the Provisioning Template, optional element

```xml
<ContentTypes>
   <ContentType />
</ContentTypes>
```


Here follow the available child elements for the  element.


Element|Type|Description
-------|----|-----------
ContentType|[ContentType](#contenttype)|
<a name="lists"></a>
###Lists
The Lists instances of the Provisioning Template, optional element

```xml
<Lists>
   <ListInstance />
</Lists>
```


Here follow the available child elements for the  element.


Element|Type|Description
-------|----|-----------
ListInstance|[ListInstance](#listinstance)|
<a name="features"></a>
###Features
The Features (Site or Web) to activate or deactivate while applying the Provisioning Template, optional collection of elements

```xml
<Features>
   <SiteFeatures />
   <WebFeatures />
</Features>
```


Here follow the available child elements for the  element.


Element|Type|Description
-------|----|-----------
SiteFeatures|[FeaturesList](#featureslist)|The Site Features to activate or deactivate while applying the Provisioning Template, optional collection of elements
WebFeatures|[FeaturesList](#featureslist)|The Web Features to activate or deactivate while applying the Provisioning Template, optional collection of elements
<a name="customactions"></a>
###CustomActions
The Custom Actions (Site or Web) to provision with the Provisioning Template, optional element

```xml
<CustomActions>
   <SiteCustomActions />
   <WebCustomActions />
</CustomActions>
```


Here follow the available child elements for the  element.


Element|Type|Description
-------|----|-----------
SiteCustomActions|[CustomActionsList](#customactionslist)|The Site Custom Actions to provision while applying the Provisioning Template, optional element
WebCustomActions|[CustomActionsList](#customactionslist)|The Web Custom Actions to provision while applying the Provisioning Template, optional element
<a name="files"></a>
###Files
The Files to provision into the target Site through the Provisioning Template, optional element

```xml
<Files>
   <File />
</Files>
```


Here follow the available child elements for the  element.


Element|Type|Description
-------|----|-----------
File|[File](#file)|
<a name="pages"></a>
###Pages
The Pages to provision into the target Site through the Provisioning Template, optional element

```xml
<Pages>
   <Page />
</Pages>
```


Here follow the available child elements for the  element.


Element|Type|Description
-------|----|-----------
Page|[Page](#page)|
<a name="termgroups"></a>
###TermGroups
The TermGroups element allows provisioning one or more TermGroups into the target Site, optional element

```xml
<TermGroups>
   <TermGroup />
</TermGroups>
```


Here follow the available child elements for the  element.


Element|Type|Description
-------|----|-----------
TermGroup|[TermGroup](#termgroup)|The TermGroup element to provision into the target Site through the Provisioning Template, optional element
<a name="providers"></a>
###Providers
The Extensiblity Providers to invoke while applying the Provisioning Template, optional collection of elements

```xml
<Providers>
   <Provider />
</Providers>
```


Here follow the available child elements for the  element.


Element|Type|Description
-------|----|-----------
Provider|[Provider](#provider)|
<a name="stringdictionaryitem"></a>
###StringDictionaryItem
Defines a StringDictionary element

```xml
<StringDictionaryItem
      Key="xsd:string"
      Value="xsd:string">
</StringDictionaryItem>
```


Here follow the available attributes for the StringDictionaryItem element.


Attibute|Type|Description
--------|----|-----------
Key|xsd:string|The Key of the property to store in the StringDictionary, required attribute
Value|xsd:string|The Value of the property to store in the StringDictionary, required attribute
<a name="userslist"></a>
###UsersList
List of Users for the Site Security, collection of elements

```xml
<UsersList>
   <User />
</UsersList>
```


Here follow the available child elements for the UsersList element.


Element|Type|Description
-------|----|-----------
User|[User](#user)|
<a name="user"></a>
###User
The base abstract type for a User element

```xml
<User
      Name="xsd:string">
</User>
```


Here follow the available attributes for the User element.


Attibute|Type|Description
--------|----|-----------
Name|xsd:string|The Name of the User, required attribute
<a name="listinstance"></a>
###ListInstance
Defines a ListInstance element

```xml
<ListInstance
      Title="xsd:string"
      Description="xsd:string"
      DocumentTemplate="xsd:string"
      OnQuickLaunch="xsd:boolean"
      TemplateType="xsd:int"
      Url="xsd:string"
      EnableVersioning="xsd:boolean"
      EnableMinorVersions="xsd:boolean"
      EnableModeration="xsd:boolean"
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


Element|Type|Description
-------|----|-----------
ContentTypeBindings|[ContentTypeBindings](#contenttypebindings)|The ContentTypeBindings entries of the List Instance, optional collection of elements
Views|[Views](#views)|The Views entries of the List Instance, optional collection of elements
Fields|[Fields](#fields)|The Fields entries of the List Instance, optional collection of elements
FieldRefs|[FieldRefs](#fieldrefs)|The FieldRefs entries of the List Instance, optional collection of elements
DataRows|[DataRows](#datarows)|

Here follow the available attributes for the ListInstance element.


Attibute|Type|Description
--------|----|-----------
Title|xsd:string|The Title of the List Instance, required attribute
Description|xsd:string|The Description of the List Instance, optional attribute
DocumentTemplate|xsd:string|The DocumentTemplate of the List Instance, optional attribute
OnQuickLaunch|xsd:boolean|The OnQuickLaunch flag for the List Instance, optional attribute
TemplateType|xsd:int|The TemplateType of the List Instance, required attribute Values available here: https://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.listtemplatetype.aspx
Url|xsd:string|The Url of the List Instance, required attribute
EnableVersioning|xsd:boolean|The EnableVersioning flag for the List Instance, optional attribute
EnableMinorVersions|xsd:boolean|The EnableMinorVersions flag for the List Instance, optional attribute
EnableModeration|xsd:boolean|The EnableModeration flag for the List Instance, optional attribute
MinorVersionLimit|xsd:int|The MinorVersionLimit for versions history for the List Instance, optional attribute
MaxVersionLimit|xsd:int|The MaxVersionLimit for versions history for the List Instance, optional attribute
RemoveExistingContentTypes|xsd:boolean|The RemoveExistingContentTypes flag for the List Instance, optional attribute
TemplateFeatureID|GUID|The TemplateFeatureID for the feature on which the List Instance is based, optional attribute
ContentTypesEnabled|xsd:boolean|The ContentTypesEnabled flag for the List Instance, optional attribute
Hidden|xsd:boolean|The Hidden flag for the List Instance, optional attribute
EnableAttachments|xsd:boolean|The EnableAttachments flag for the List Instance, optional attribute
EnableFolderCreation|xsd:boolean|The EnableFolderCreation flag for the List Instance, optional attribute
<a name="contenttypebindings"></a>
###ContentTypeBindings
The ContentTypeBindings entries of the List Instance, optional collection of elements

```xml
<ContentTypeBindings>
   <ContentTypeBinding />
</ContentTypeBindings>
```


Here follow the available child elements for the  element.


Element|Type|Description
-------|----|-----------
ContentTypeBinding|[ContentTypeBinding](#contenttypebinding)|
<a name="views"></a>
###Views
The Views entries of the List Instance, optional collection of elements

```xml
<Views
      RemoveExistingViews="xsd:boolean">
   <!-- Any other XML content -->
</Views>
```


Here follow the available attributes for the  element.


Attibute|Type|Description
--------|----|-----------
RemoveExistingViews|xsd:boolean|A flag to declare if the existing views of the List Instance have to be removed, befire adding the custom views, optional attribute
<a name="fields"></a>
###Fields
The Fields entries of the List Instance, optional collection of elements

```xml
<Fields>
   <!-- Any other XML content -->
</Fields>
```

<a name="fieldrefs"></a>
###FieldRefs
The FieldRefs entries of the List Instance, optional collection of elements

```xml
<FieldRefs>
   <FieldRef />
</FieldRefs>
```


Here follow the available child elements for the  element.


Element|Type|Description
-------|----|-----------
FieldRef|[ListInstanceFieldRef](#listinstancefieldref)|
<a name="datarows"></a>
###DataRows


```xml
<DataRows>
   <DataRow />
</DataRows>
```


Here follow the available child elements for the  element.


Element|Type|Description
-------|----|-----------
DataRow|[DataRow](#datarow)|
<a name="datavalue"></a>
###DataValue
The DataValue of a single field of a row to insert into a target ListInstance

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
Defines a content type

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


Element|Type|Description
-------|----|-----------
FieldRefs|[FieldRefs](#fieldrefs)|The FieldRefs entries of the List Instance, optional collection of elements
DocumentTemplate|[DocumentTemplate](#documenttemplate)|Specifies the document template for the content type. This is the file which SharePoint Foundation opens as a template when a user requests a new item of this content type.

Here follow the available attributes for the ContentType element.


Attibute|Type|Description
--------|----|-----------
ID|ContentTypeId|The value of the content type ID, required attribute
Name|xsd:string|The name of the content type, required attribute
Description|xsd:string|The description of the content type, optional attribute
Group|xsd:string|The group of the content type, optional attribute
Hidden|xsd:boolean|Optional Boolean. True to define the content type as hidden. If you define a content type as hidden, SharePoint Foundation does not display that content type on the New button in list views.
Sealed|xsd:boolean|Optional Boolean. True to prevent changes to this content type. You cannot change the value of this attribute through the user interface, but you can change it in code if you have sufficient rights. You must have site collection administrator rights to unseal a content type.
ReadOnly|xsd:boolean|Optional Boolean. TRUE to specify that the content type cannot be edited without explicitly removing the read-only setting. This can be done either in the user interface or in code.
Overwrite|xsd:boolean|Optional Boolean. TRUE to overwrite an existing content type with the same ID.
<a name="fieldrefs"></a>
###FieldRefs
The FieldRefs entries of the List Instance, optional collection of elements

```xml
<FieldRefs>
   <FieldRef />
</FieldRefs>
```


Here follow the available child elements for the  element.


Element|Type|Description
-------|----|-----------
FieldRef|[ContentTypeFieldRef](#contenttypefieldref)|
<a name="documenttemplate"></a>
###DocumentTemplate
Specifies the document template for the content type. This is the file which SharePoint Foundation opens as a template when a user requests a new item of this content type.

```xml
<DocumentTemplate
      TargetName="xsd:string">
</DocumentTemplate>
```


Here follow the available attributes for the  element.


Attibute|Type|Description
--------|----|-----------
TargetName|xsd:string|The value of the content type ID, required attribute
<a name="contenttypebinding"></a>
###ContentTypeBinding
Defines the binding between a ListInstance and a ContentType

```xml
<ContentTypeBinding
      ContentTypeID="pnp:ContentTypeId"
      Default="xsd:boolean">
</ContentTypeBinding>
```


Here follow the available attributes for the ContentTypeBinding element.


Attibute|Type|Description
--------|----|-----------
ContentTypeID|ContentTypeId|The value of the ContentTypeID to bind, required attribute
Default|xsd:boolean|Declares if the Content Type should be the default Content Type in the list or library, optional attribute
<a name="featureslist"></a>
###FeaturesList
Defines a collection of elements of type Feature

```xml
<FeaturesList>
   <Feature />
</FeaturesList>
```


Here follow the available child elements for the FeaturesList element.


Element|Type|Description
-------|----|-----------
Feature|[Feature](#feature)|
<a name="feature"></a>
###Feature
Defines a single Site or Web Feature, which will be activated or deactivated while applying the Provisioning Template

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
ID|GUID|The unique ID of the Feature, required attribute
Deactivate|xsd:boolean|Defines if the feature has to be deactivated or activated while applying the Provisioning Template, optional attribute
Description|xsd:string|The Description of the feature, optional attribute
<a name="listinstancefieldref"></a>
###ListInstanceFieldRef
Defines the binding between a ListInstance and a Field

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
ID|GUID|The value of the field ID to bind, required attribute
Name|xsd:string|The name of the field used in the field reference. This is for reference/readibility only.
Required|xsd:boolean|The Required flag for the field to bind, optional attribute
Hidden|xsd:boolean|The Hidden flag for the field to bind, optional attribute
DisplayName|xsd:string|The display name of the field to bind, only applicable to fields that will be added to lists, optional attribute
<a name="contenttypefieldref"></a>
###ContentTypeFieldRef
Defines the binding between a ContentType and a Field

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
ID|GUID|The value of the field ID to bind, required attribute
Name|xsd:string|The name of the field used in the field reference. This is for reference/readibility only.
Required|xsd:boolean|The Required flag for the field to bind, optional attribute
Hidden|xsd:boolean|The Hidden flag for the field to bind, optional attribute
<a name="customactionslist"></a>
###CustomActionsList
Defines a collection of elements of type CustomAction

```xml
<CustomActionsList>
   <CustomAction />
</CustomActionsList>
```


Here follow the available child elements for the CustomActionsList element.


Element|Type|Description
-------|----|-----------
CustomAction|[CustomAction](#customaction)|
<a name="customaction"></a>
###CustomAction
Defines a Custom Action, which will be provisioned while applying the Provisioning Template

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
Name|xsd:string|The Name of the CustomAction, required attribute
Description|xsd:string|The Description of the CustomAction, optional attribute
Group|xsd:string|The Group of the CustomAction, optional attribute
Location|xsd:string|The Location of the CustomAction, required attribute
Title|xsd:string|The Title of the CustomAction, required attribute
Sequence|xsd:int|The Sequence of the CustomAction, optional attribute
Rights|xsd:int|The Rights for the CustomAction, based on values from Microsoft.SharePoint.Client.BasePermissions, optional attribute
Url|xsd:string|The URL of the CustomAction, optional attribute
Enabled|xsd:boolean|The Enabled flag for the CustomAction, optional attribute
ScriptBlock|xsd:string|The ScriptBlock of the CustomAction, optional attribute
ImageUrl|xsd:string|The ImageUrl of the CustomAction, optional attribute
ScriptSrc|xsd:string|The ScriptSrc of the CustomAction, optional attribute
<a name="fileproperties"></a>
###FileProperties
A collection of File Properties

```xml
<FileProperties>
   <Property />
</FileProperties>
```


Here follow the available child elements for the FileProperties element.


Element|Type|Description
-------|----|-----------
Property|[StringDictionaryItem](#stringdictionaryitem)|
<a name="file"></a>
###File
Defines a File element, to describe a file that will be provisioned into the target Site

```xml
<File
      Src="xsd:string"
      Folder="xsd:string"
      Overwrite="xsd:boolean">
   <Properties />
   <WebParts />
</File>
```


Here follow the available child elements for the File element.


Element|Type|Description
-------|----|-----------
Properties|[FileProperties](#fileproperties)|The File Properties, optional collection of elements
WebParts|[WebParts](#webparts)|The webparts to add to the page, optional collection of elements

Here follow the available attributes for the File element.


Attibute|Type|Description
--------|----|-----------
Src|xsd:string|The Src of the File, required attribute
Folder|xsd:string|The TargetFolder of the File, required attribute
Overwrite|xsd:boolean|The Overwrite flag for the File, optional attribute
<a name="webparts"></a>
###WebParts
The webparts to add to the page, optional collection of elements

```xml
<WebParts>
   <WebPart />
</WebParts>
```


Here follow the available child elements for the  element.


Element|Type|Description
-------|----|-----------
WebPart|[WebPartPageWebPart](#webpartpagewebpart)|
<a name="page"></a>
###Page
Defines a Page element, to describe a page that will be provisioned into the target Site. Because of the Layout attribute, the assumption is made that you're referring/creating a WikiPage.

```xml
<Page
      Url="xsd:string"
      Overwrite="xsd:boolean"
      Layout="pnp:WikiPageLayout">
   <WebParts />
</Page>
```


Here follow the available child elements for the Page element.


Element|Type|Description
-------|----|-----------
WebParts|[WebParts](#webparts)|The webparts to add to the page, optional collection of elements

Here follow the available attributes for the Page element.


Attibute|Type|Description
--------|----|-----------
Url|xsd:string|Required: The server relative url of the page, supports tokens
Overwrite|xsd:boolean|Optional: if set, overwrites an existing page in the case of a wikipage.
Layout|WikiPageLayout|Required: Defines the layout of the wikipage
<a name="webparts"></a>
###WebParts
The webparts to add to the page, optional collection of elements

```xml
<WebParts>
   <WebPart />
</WebParts>
```


Here follow the available child elements for the  element.


Element|Type|Description
-------|----|-----------
WebPart|[WikiPageWebPart](#wikipagewebpart)|
<a name="wikipagewebpart"></a>
###WikiPageWebPart
Defines a WebPart to be added to a WikiPage

```xml
<WikiPageWebPart
      Title="xsd:string"
      Row="xsd:int"
      Column="xsd:int">
   <Contents />
</WikiPageWebPart>
```


Here follow the available child elements for the WikiPageWebPart element.


Element|Type|Description
-------|----|-----------
Contents|xsd:string|Required: Defines the WebPart XML

Here follow the available attributes for the WikiPageWebPart element.


Attibute|Type|Description
--------|----|-----------
Title|xsd:string|Required: Defines the title of the WebPart
Row|xsd:int|Required: Defines the row to add the WebPart to
Column|xsd:int|Required: Defines the column to add the WebPart to
<a name="webpartpagewebpart"></a>
###WebPartPageWebPart
Defines a webpart to be added to a WebPart Page

```xml
<WebPartPageWebPart
      Title="xsd:string"
      Zone="xsd:string"
      Order="xsd:int">
   <Contents />
</WebPartPageWebPart>
```


Here follow the available child elements for the WebPartPageWebPart element.


Element|Type|Description
-------|----|-----------
Contents|xsd:string|Required: Defines the WebPart XML

Here follow the available attributes for the WebPartPageWebPart element.


Attibute|Type|Description
--------|----|-----------
Title|xsd:string|Required: Defines the title of the WebPart
Zone|xsd:string|Required: defines the zone of a WebPart Page to add the webpart to
Order|xsd:int|Required: defines the index of the WebPart in the zone
<a name="composedlook"></a>
###ComposedLook
Defines a ComposedLook element

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
Name|xsd:string|The Name of the ComposedLook, required attribute
ColorFile|xsd:string|The ColorFile of the ComposedLook, required attribute
FontFile|xsd:string|The FontFile of the ComposedLook, required attribute
BackgroundFile|xsd:string|The BackgroundFile of the ComposedLook, optional attribute
MasterPage|xsd:string|The MasterPage of the ComposedLook, required attribute
SiteLogo|xsd:string|The SiteLogo of the ComposedLook, optional attribute
AlternateCSS|xsd:string|The AlternateCSS of the ComposedLook, optional attribute
Version|xsd:int|The Version of the ComposedLook, optional attribute
<a name="provider"></a>
###Provider
Defines an Extensibility Provider

```xml
<Provider
      Enabled="xsd:boolean"
      HandlerType="xsd:string">
   <Configuration />
</Provider>
```


Here follow the available child elements for the Provider element.


Element|Type|Description
-------|----|-----------
Configuration|[Configuration](#configuration)|Defines an optional configuration section for the Extensibility Provider. The configuration section can be any XML

Here follow the available attributes for the Provider element.


Attibute|Type|Description
--------|----|-----------
Enabled|xsd:boolean|Defines whether the Extensibility Provider is enabled or not, optional attribute
HandlerType|xsd:string|The type of the handler. It can be a FQN of a .NET type, the URL of a node.js file, or whatever else, required attribute
<a name="configuration"></a>
###Configuration
Defines an optional configuration section for the Extensibility Provider. The configuration section can be any XML

```xml
<Configuration>
   <!-- Any other XML content -->
</Configuration>
```

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
File|xsd:string|Absolute or relative path to the file.
ID|xsd:ID|ID of the referenced template
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
ID|xsd:IDREF|ID of the referenced template
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


Element|Type|Description
-------|----|-----------
SiteCollection|[SiteCollection](#sitecollection)|
Site|[Site](#site)|
TermStore|[TermStore](#termstore)|
Extensions|[Extensions](#extensions)|

Here follow the available attributes for the Sequence element.


Attibute|Type|Description
--------|----|-----------
SequenceType||Instructions to the Provisioning engine on how the Containers within the Sequence can be provisioned.
ID|xsd:ID|A unique identifier of the Sequence
<a name="sitecollection"></a>
###SiteCollection
Defines a SiteCollection that will be created into the target tenant/farm

```xml
<SiteCollection
      Url="pnp:ReplaceableString">
   <Templates />
</SiteCollection>
```


Here follow the available child elements for the SiteCollection element.


Element|Type|Description
-------|----|-----------
Templates|[Templates](#templates)|

Here follow the available attributes for the SiteCollection element.


Attibute|Type|Description
--------|----|-----------
Url|ReplaceableString|Absolute Url to the site
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


Element|Type|Description
-------|----|-----------
Templates|[Templates](#templates)|

Here follow the available attributes for the Site element.


Attibute|Type|Description
--------|----|-----------
UseSamePermissionsAsParentSite|xsd:boolean|
Url|ReplaceableString|Relative Url to the site
<a name="termstore"></a>
###TermStore
A TermStore to use for provisioning of TermGroups

```xml
<TermStore
      Scope="">
   <TermGroup />
</TermStore>
```


Here follow the available child elements for the TermStore element.


Element|Type|Description
-------|----|-----------
TermGroup|[TermGroup](#termgroup)|The TermGroup element to provision into the target TermStore through, optional element

Here follow the available attributes for the TermStore element.


Attibute|Type|Description
--------|----|-----------
Scope||The scope of the term store.
<a name="termgroup"></a>
###TermGroup
A TermGroup to use for provisioning of TermSets and Terms

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
Name|xsd:string|
ID|GUID|
<a name="termsetitem"></a>
###TermSetItem
Base type for TermSets and Terms

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
Owner|xsd:string|
Description|xsd:string|
IsAvailableForTagging|xsd:boolean|
<a name="termset"></a>
###TermSet
A TermSet to provision

```xml
<TermSet
      Language="xsd:int"
      IsOpenForTermCreation="xsd:boolean">
</TermSet>
```


Here follow the available attributes for the TermSet element.


Attibute|Type|Description
--------|----|-----------
Language|xsd:int|
IsOpenForTermCreation|xsd:boolean|
<a name="term"></a>
###Term
A Term to provision into a TermSet or a hyerarchical Term

```xml
<Term
      Language="xsd:int"
      CustomSortOrder="xsd:int">
</Term>
```


Here follow the available attributes for the Term element.


Attibute|Type|Description
--------|----|-----------
Language|xsd:int|
CustomSortOrder|xsd:int|
<a name="taxonomyitemproperties"></a>
###TaxonomyItemProperties
A collection of Term Properties

```xml
<TaxonomyItemProperties>
   <Property />
</TaxonomyItemProperties>
```


Here follow the available child elements for the TaxonomyItemProperties element.


Element|Type|Description
-------|----|-----------
Property|[StringDictionaryItem](#stringdictionaryitem)|
<a name="termlabels"></a>
###TermLabels
A collection of Term Labels, in order to support multi-language terms

```xml
<TermLabels>
   <Label />
</TermLabels>
```


Here follow the available child elements for the TermLabels element.


Element|Type|Description
-------|----|-----------
Label|[Label](#label)|
<a name="label"></a>
###Label


```xml
<Label
      Language="xsd:int"
      Value="xsd:string"
      IsDefaultForLanguage="xsd:boolean">
</Label>
```


Here follow the available attributes for the  element.


Attibute|Type|Description
--------|----|-----------
Language|xsd:int|
Value|xsd:string|
IsDefaultForLanguage|xsd:boolean|
<a name="termsets"></a>
###TermSets
A collection of TermSets to provision

```xml
<TermSets>
   <TermSet />
</TermSets>
```


Here follow the available child elements for the TermSets element.


Element|Type|Description
-------|----|-----------
TermSet|[TermSet](#termset)|
<a name="extensions"></a>
###Extensions
Extensions are custom XML elements and instructions that can be extensions of this default schema or vendor or engine specific extensions.

```xml
<Extensions>
   <!-- Any other XML content -->
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
File|xsd:string|Absolute or relative path to the file.
