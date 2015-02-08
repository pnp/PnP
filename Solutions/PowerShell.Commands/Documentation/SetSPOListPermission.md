#Set-SPOListPermission
*Topic last generated: 2015-02-08*


##Syntax
    Set-SPOListPermission -Group [<GroupPipeBind>] -Identity [<ListPipeBind>] [-AddRole [<String>]] [-RemoveRole [<String>]] [-Web [<WebPipeBind>]]

&nbsp;

    Set-SPOListPermission -User [<String>] -Identity [<ListPipeBind>] [-AddRole [<String>]] [-RemoveRole [<String>]] [-Web [<WebPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
AddRole|String|False|
Group|GroupPipeBind|True|
Identity|ListPipeBind|True|
RemoveRole|String|False|
User|String|True|
Web|WebPipeBind|False|
