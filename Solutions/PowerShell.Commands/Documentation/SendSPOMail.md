#Send-SPOMail
*Topic last generated: 2015-02-08*


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
Web|WebPipeBind|False|
