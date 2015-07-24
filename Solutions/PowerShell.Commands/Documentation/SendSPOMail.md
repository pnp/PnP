#Send-SPOMail
*Topic automatically generated on: 2015-06-11*

Sends an email using the Office 365 SMTP Service
##Syntax
```powershell
Send-SPOMail [-Server <String>] -From <String> -Password <String> -To <String[]> [-Cc <String[]>] -Subject <String> -Body <String> [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Body|String|True||
|Cc|String[]|False||
|From|String|True||
|Password|String|True||
|Server|String|False||
|Subject|String|True||
|To|String[]|True||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
<!-- Ref: B8BC3C088E01664FAE07A355FA758AE0 -->