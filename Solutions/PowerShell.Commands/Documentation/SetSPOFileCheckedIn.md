#Set-SPOFileCheckedIn
*Topic automatically generated on: 2015-08-04*

Checks in a file
##Syntax
```powershell
Set-SPOFileCheckedIn [-CheckinType [<CheckinType>]] [-Comment [<String>]] [-Web [<WebPipeBind>]] -Url [<String>]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
CheckinType|CheckinType|False|
Comment|String|False|
Url|String|True|
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
