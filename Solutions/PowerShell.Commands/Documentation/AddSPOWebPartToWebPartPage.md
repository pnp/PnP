#Add-SPOWebPartToWebPartPage
*Topic automatically generated on: 2015-07-08*

Adds a webpart to a web part page in a specified zone
##Syntax
```powershell
Add-SPOWebPartToWebPartPage -Path <String> -PageUrl <String> -ZoneId <String> -ZoneIndex <Int32> [-Web <WebPipeBind>]
```


```powershell
Add-SPOWebPartToWebPartPage -Xml <String> -PageUrl <String> -ZoneId <String> -ZoneIndex <Int32> [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|PageUrl|String|True|Server Relative Url of the page to add the webpart to.|
|Path|String|True|A path to a webpart file on a the file system.|
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
|Xml|String|True|A string containing the XML for the webpart.|
|ZoneId|String|True||
|ZoneIndex|Int32|True||
##Examples

###Example 1
    PS:> Add-SPOWebPartToWebPartPage -PageUrl "/sites/demo/sitepages/home.aspx" -Path "c:\myfiles\listview.webpart" -ZoneId "Header" -ZoneIndex 1 
This will add the webpart as defined by the XML in the listview.webpart file to the specified page in the specified zone and with the order index of 1

###Example 2
    PS:> Add-SPOWebPartToWebPartPage -PageUrl "/sites/demo/sitepages/home.aspx" -XML $webpart -ZoneId "Header" -ZoneIndex 1 
This will add the webpart as defined by the XML in the $webpart variable to the specified page in the specified zone and with the order index of 1
<!-- Ref: CF7E91E6DD1F2DB6DA2A1ADA824DF9AB -->