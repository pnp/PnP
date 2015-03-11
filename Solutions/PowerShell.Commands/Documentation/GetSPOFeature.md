#Get-SPOFeature
*Topic automatically generated on: 2015-03-11*

Returns all or a specific feature
##Syntax
    Get-SPOFeature [-Scope [<FeatureScope>]] [-Web [<WebPipeBind>]] [-Identity [<FeaturePipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Identity|FeaturePipeBind|False|
Scope|FeatureScope|False|The scope of the feature. Defaults to Web.
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
##Examples

###Example 1
    PS:> Get-SPOFeature
This will return all web scoped features

###Example 2
    PS:> Get-SPOFeature -Scope Site
This will return all site scoped features

###Example 3
    PS:> Get-SPOFeature -Identity fb689d0e-eb99-4f13-beb3-86692fd39f22 -Scope Site
This will return a specific site scoped feature

###Example 4
    PS:> Get-SPOFeature -Identity fb689d0e-eb99-4f13-beb3-86692fd39f22
This will return a specific web scoped feature
