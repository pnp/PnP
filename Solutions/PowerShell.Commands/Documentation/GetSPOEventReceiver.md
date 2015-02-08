#Get-SPOEventReceiver
*Topic last generated: 2015-02-08*

Returns all or a specific event receiver
##Syntax
    Get-SPOEventReceiver [-List [<ListPipeBind>]] [-Identity [<GuidPipeBind>]] [-Web [<WebPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Identity|GuidPipeBind|False|
List|ListPipeBind|False|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
##Examples

###Example 1
    PS:> Get-SPOEventReceiver
This will return all registered event receivers on the current web

###Example 2
    PS:> Get-SPOEventReceiver -Identity fb689d0e-eb99-4f13-beb3-86692fd39f22
This will return a specific registered event receivers from the current web

###Example 3
    PS:> Get-SPOEventReceiver -List "ProjectList"
This will return all registered event receivers in the list with the name ProjectList

###Example 4
    PS:> Get-SPOEventReceiver -List "ProjectList" -Identity fb689d0e-eb99-4f13-beb3-86692fd39f22
This will return a specific registered event receiver in the list with the name ProjectList
