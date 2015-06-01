#Add-SPOFile
*Topic automatically generated on: 2015-06-01*

Uploads a file to Web
##Syntax
```powershell
Add-SPOFile -Path <String> -Folder <String> [-Checkout [<SwitchParameter>]] [-Approve [<SwitchParameter>]] [-ApproveComment <String>] [-Publish [<SwitchParameter>]] [-PublishComment <String>] [-UseWebDav [<SwitchParameter>]] [-Web <WebPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Approve|SwitchParameter|False|Will auto approve the uploaded file.
ApproveComment|String|False|The comment added to the approval.
Checkout|SwitchParameter|False|If versioning is enabled, this will check out the file first if it exists, upload the file, then check it in again.
Folder|String|True|The destination folder in the site
Path|String|True|The local file path.
Publish|SwitchParameter|False|Will auto publish the file.
PublishComment|String|False|The comment added to the publish action.
UseWebDav|SwitchParameter|False|
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    
PS:> Add-SPOFile -Path c:\temp\company.master -Folder "_catalogs/masterpage
This will upload the file company.master to the masterpage catalog

###Example 2
    
PS:> Add-SPOFile -Path .\displaytemplate.html -Folder "_catalogs/masterpage/display templates/test
This will upload the file displaytemplate.html to the test folder in the display templates folder. If the test folder not exists it will create it.
<!-- Ref: B5A3BB58A27AA5151C8EBB7A6A1D6540 -->