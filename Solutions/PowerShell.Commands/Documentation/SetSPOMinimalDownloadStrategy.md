#Set-SPOMinimalDownloadStrategy
*Topic last generated: 2015-02-08*

Activates or deactivates the minimal downloading strategy.
##Syntax
    Set-SPOMinimalDownloadStrategy -On [<SwitchParameter>] [-Force [<SwitchParameter>]] [-Web [<WebPipeBind>]]

&nbsp;

    Set-SPOMinimalDownloadStrategy -Off [<SwitchParameter>] [-Force [<SwitchParameter>]] [-Web [<WebPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Force|SwitchParameter|False|
Off|SwitchParameter|True|
On|SwitchParameter|True|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
