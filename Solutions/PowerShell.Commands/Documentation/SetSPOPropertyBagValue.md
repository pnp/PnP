#Set-SPOPropertyBagValue
*Topic automatically generated on: 2015-08-04*

Sets a property bag value
##Syntax
```powershell
Set-SPOPropertyBagValue -Key [<String>] -Value [<String>] -Indexed [<SwitchParameter>] -Value [<String>] [-Web [<WebPipeBind>]]
```
&nbsp;

```powershell
Set-SPOPropertyBagValue -Key [<String>] -Value [<String>] [-Folder [<String>]] -Value [<String>] [-Web [<WebPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Folder|String|False|Site relative url of the folder. See examples for use.
Indexed|SwitchParameter|True|
Key|String|True|
Value|String|True|
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    PS:> Set-SPOPropertyBagValue -Key MyKey -Value MyValue
This sets or adds a value to the current web property bag

###Example 2
    PS:> Set-SPOPropertyBagValue -Key MyKey -Value MyValue -Folder /
This sets or adds a value to the root folder of the current web

###Example 3
    PS:> Set-SPOPropertyBagValue -Key MyKey -Value MyValue -Folder /MyFolder
This sets or adds a value to the folder MyFolder which is located in the root folder of the current web
