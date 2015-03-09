#Send-SPOMail
*Topic automatically generated on: 2015-03-10*


##Syntax
    Send-SPOMail [-Server [<String>]] -From [<String>] -Password [<String>] -To [<String[]>] [-Cc [<String[]>]] -Subject [<String>] -Body [<String>] [-Web [<WebPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Body|String|True|
Cc|String[]|False|
From|String|True|
Password|String|True|
Server|String|False|
Subject|String|True|
To|String[]|True|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
