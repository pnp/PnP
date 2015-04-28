#Set-SPOMinimalDownloadStrategy
*Topic automatically generated on: 2015-04-28*

Activates or deactivates the minimal downloading strategy.
##Syntax
```powershell
Set-SPOMinimalDownloadStrategy -On [<SwitchParameter>] [-Force [<SwitchParameter>]] [-Web [<WebPipeBind>]]
```
&nbsp;

```powershell
Set-SPOMinimalDownloadStrategy -Off [<SwitchParameter>] [-Force [<SwitchParameter>]] [-Web [<WebPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Force|SwitchParameter|False|
Off|SwitchParameter|True|
On|SwitchParameter|True|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
