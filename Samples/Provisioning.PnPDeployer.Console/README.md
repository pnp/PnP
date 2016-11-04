# PNP.Deployer #

### Summary ###
`PNP.Deployer.exe` is a console application that makes it easy to deploy artifacts to SharePoint OnPremise/Online. Based on the [PnP Provisioning Engine](https://github.com/OfficeDev/PnP-Guidance/blob/551b9f6a66cf94058ba5497e310d519647afb20c/articles/Introducing-the-PnP-Provisioning-Engine.md), it wraps the engine's main functionnalities and provides a new layer responsible for handling [tokens](#tokens-accross-any-files), [authentication](#authentication-made-simple), [sequences](#sequences-for-a-configurable-deployment) and [logging](#easy-logging). Provide the `PnP templates`, define `sequences` in which you want the templates to be executed, specify whether you want to deploy everything `OnPrem` or `Online`, and your good to go.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
The projects within the Visual Studio solution have a dependency on the `PnP Core library`. By default, they are all configured for using the [SharePoint PnP Core library for SharePoint 2013](#sequences-for-a-configurable-deployment) nugget package, but feel free to change the nugget package for the one that suits your needs :
* [SharePoint PnP Core library for SharePoint 2013 (SharePointPnPCore2013)](https://www.nuget.org/packages/SharePointPnPCore2013)
* [SharePoint PnP Core library for SharePoint 2016 (SharePointPnPCore2016)](https://www.nuget.org/packages/SharePointPnPCore2016)
* [SharePoint PnP Core library for SharePoint Online (SharePointPnPCoreOnline)](https://www.nuget.org/packages/SharePointPnPCoreOnline)

### Solution ###
Solution | Author(s)
---------|----------
PNP.Deployer | Simon-Pierre Plante

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | September 7th 2016 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

<hr>

# How it works #

Place the `PNP.Deployer` package in a location of your choice and configure the `PNP.Deployer.exe.config` file located in the `bin\release` folder.

<img src="http://i63.tinypic.com/2ahshty.png" alt="" />

Provide a package with the following elements and call the deployer with the proper parameters: 

* The [artifact(s)](#files) that needs to be deployed (page layouts, images, css, js, etc)
* The [template(s)](#templates) referencing those previous artifacts (XML templates based on the [PnP Schema](https://github.com/OfficeDev/PnP-Provisioning-Schema))
* A [sequences](#sequences-file) file that tells the deployer the templates to apply in the desired order
* Optionnaly, a [tokens](#tokens-file) file used for "tokenizing" the whole working directory before deploying

<img src="http://i63.tinypic.com/35krjoj.png" alt="" />

<br>
# Features #

### Command line parser for a better user experience ###
The console application uses the [Command Line Parser Library](https://commandline.codeplex.com) in order to provide named arguments, a custom '--help' interface, and userfriendly command line argument exceptions.

<img src="http://i67.tinypic.com/bhinna.jpg" width="500" alt="Help Screen" />

<img src="http://i67.tinypic.com/29ynuww.jpg" width="500" alt="Help Sceen On Error" />

### Tokens accross any files ###
The [PnP Provisioning Engine](https://github.com/OfficeDev/PnP-Guidance/blob/551b9f6a66cf94058ba5497e310d519647afb20c/articles/Introducing-the-PnP-Provisioning-Engine.md) already supports tokens within template files, but what if you need tokens accross static files such as `CSS` files or a simple `Page Layout`? The deployer uses a `Tokenizer` that copies the whole working directory and generates a `tokenized` version of it (MyDirectory_Tokenized), which becomes the final working directory used by the deployer. The fact that tokens can be used accross the whole working directory makes the `Tokenizer` really powerfull, allowing the user to use tokens in any static files that aren't necessarily loaded in memory by the deployer.

```xml
<tokensConfiguration>
  <tokens>
    <token key="PortalUrl" value="http://company.sharepoint.com" />
    <token key="HubUrl" value="http://company.sharepoint.com/Hub" />
  </tokens>
</tokensConfiguration>
```

```css
.item {
   background-image: url('{{HubUrl}}/SiteAssets/Images/item-background.png');
}
```

### Authentication made simple ###
No need to handle the different types of authentication methods for `on-premise` and `online` environments, the deployer will automatically use the current user's credentials or prompt for a specific user's credential based on the specified `Environment` and `PromptCredentials` parameters.

### Sequences for a configurable deployment ###
`Sequences` makes it easy to orchestrate the different templates and their firing order. The `xml` syntax allows the user to easily `ignore` a specific sequence or a specific template within a sequence. 

```xml
<sequencesConfiguration>
  <sequences>
    <sequence name="SiteFields" description="..." webUrl="http://yoursite.sharepoint.com" ignore="false">
      <templates>
        <template name="TextFields" path="Templates/TextFields.xml" ignore="false" />
        <template name="ChoiceFields" path="Templates/ChoiceFields.xml" ignore="false" />
      </templates>
    </sequence>
    <sequence name="SiteContentTypes" description="..." webUrl="http://yoursite.sharepoint.com" ignore="false">
      <templates>
        <template name="ContentTypes" path="Templates/ContentTypes.xml" ignore="false" />
      </templates>
    </sequence>
  </sequences>
</sequencesConfiguration>
```

### Easy logging ###
The deployer uses [NLog](http://nlog-project.org/) for logging, which provides an easy way to configure the different output sources and their properties to the user's liking.

<img src="http://i65.tinypic.com/15o6wau.jpg" width="700" alt="Logging example" />

Customize the different output sources and the overall behavior of the logging engine simply by altering the provided `NLog.config` file, following the [NLog configuration file documentation](https://github.com/NLog/NLog/wiki/Configuration-file).

<img src="http://i67.tinypic.com/28rgmpy.jpg" width="700" alt="NLog.config" />

### Supports ".pnp" packages ###
While supporting regular `.xml` templates, the deployer also supports the new `.pnp` open xml format. Specify a `.pnp` package just like a standard template within the [sequences.xml](#sequences-for-a-configurable-deployment) file and everything within the `.pnp` package will be deployed.

```xml
...
<templates>
  <template name="My XML Template" path="Templates/MyXmlTemplate.xml" ignore="false" />
  <template name="My PnP Package" path="Templates/MyPnPPackage.pnp" ignore="false" />
</templates>
...
```

<br>
# Project Structure #

<img src="http://i65.tinypic.com/359n5ag.jpg" alt="" />


<br>
# Getting Started #

### 1 - Configuring the deployer ###
The configuration of the deployer is stored within the `PNP.Deployer.exe.config` file located in the `bin\release` folder. The \<appSettings\> section of the configuration file stores 4 kinds of information :
* `clientSequencesFile` : 
    - The name of the sequences file that the deployer needs to look for (relative to the `WorkingDirectory` specified by the caller)
* `clientTokensFile`
    - The name of the tokens file that the deployer needs to look for (relative to the `WorkingDirectory` specified by the caller)
* `clientIgnoredFolders`
    - The path of the folders that can be ignored by the deployer in order to speed up the tokenizing process. The different paths must be delimited by a pipe ("|") and must be relative to the `WorkingDirectory` specified by the caller. This feature can be usefull for instance when deploying a package with large non-production-sub-folders on a development environment, for example a NPM package with a large `node_modules` sub folder
* `token-*`
    - The default tokens will be available for any package deployed by the deployer, and can be added to the \<appSettings\> section by adding entries with keys that are prefixed with `token-` followed by the name of the token that will become available within the client packages

```xml
<appSettings>
  <!-- =================================================================================== -->
  <!-- The path of the sequences file, relative to the client's working directory          -->
  <!-- =================================================================================== -->
  <add key="clientSequencesFile" value="Sequences.xml" />

  <!-- =================================================================================== -->
  <!-- The path of the tokens file, relative to the client's working directory             -->
  <!-- =================================================================================== -->
  <add key="clientTokensFile" value="Tokens.xml" />

  <!-- =================================================================================== -->
  <!-- Folders ignored by the deployer, relative to the client's working directory (x|y|z) -->
  <!-- =================================================================================== -->
  <add key="clientIgnoredFolders" value="node_modules" />
    
  <!-- =================================================================================== -->
  <!-- Default tokens used by the tokenizer (Must be prefixed by 'token-')                 -->
  <!-- =================================================================================== -->
  <add key="token-Token1" value="Value of 'Token1'" />
  <add key="token-Token2" value="Value of 'Token2'" />
  <add key="token-Token3" value="Value of 'Token3'" />
</appSettings>
```

<br>
### 2 - Configuring the client package ###

#### Files ####
The files can be organized as the user whishes, with as much folders as needed, as long as they are properly referenced by the `templates`. File references within `templates` are always relative to the `WorkingDirectory` specified while calling the deployer.

#### Templates ####
The templates can be organized as the user whishes, with as much folders as needed, as long as they are properly referenced by the `sequences` file. Template references within `sequences` are always relative to the `WorkingDirectory` specified while calling the deployer.

#### Sequences file ####
The `sequences` file must reflect the deployer's configuration by having the same name and being at the same location, which is once again relative to the `WorkingDirectory` specified while calling the deployer.

```xml
<sequencesConfiguration>
  <sequences>
    <sequence name="SiteFields" description="Deploys the site fields" webUrl="http://spptechnologies.sharepoint.com" ignore="false">
      <templates>
        <template name="TextFields" path="Templates/TextFields.xml" ignore="false" />
        <template name="ChoiceFields" path="Templates/ChoiceFields.xml" ignore="false" />
      </templates>
    </sequence>
    <sequence name="SiteContentTypes" description="Deploys the site content types" webUrl="http://spptechnologies.sharepoint.com" ignore="false">
      <templates>
        <template name="ContentTypes" path="Templates/ContentTypes.xml" ignore="false" />
      </templates>
    </sequence>
  </sequences>
</sequencesConfiguration>
```

#### Tokens file ####
While being optional, in order for the `tokens` file to be recognised, it also needs to reflect the deployer's configuration by having the same name and being at the same location, which is once again relative to the `WorkingDirectory` specified while calling the deployer.

```xml
<tokensConfiguration>
  <tokens>
    <token key="MyToken1" value="Value1" />
    <token key="MyToken2" value="Value2" />
    <token key="MyToken3" value="Value3" />
  </tokens>
</tokensConfiguration>
```

<br>
### 3 - Calling the deployer ###
Once the deployer is in place and the client package is ready, simply call the deployer using the following syntax :

```powershell
.\[...]\bin\release\PNP.Deployer.exe --WorkingDirectory "[...]\MyPackage" --Environment "OnPrem|Online" [--PromptCredentials]
```

  * --WorkingDirectory
    - (Required) The full path of the package that needs to be deployed
  * --Environment
    - (Required) Whether the deployment occurs on a "OnPrem" or "Online" environment
  * --PromptCredentials
    - (Optionnal) Prompt for credentials to deploy under a specific account (Always prompts when "Online")

Another option is to use the benefits of an environment variable in order to avoid having to specify the absolute path to the deployer's exe, allowing the user to use this shorter alternative :

```powershell
PNP.Deployer.exe --WorkingDirectory "[...]\MyPackage" --Environment "OnPrem|Online" [--PromptCredentials]
```


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Provisioning.PnPDeployer.Console" />
