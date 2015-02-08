#Enable-SPOFeature
*Topic last generated: 2015-02-08*

Enables a feature
##Syntax
    Enable-SPOFeature [-Force [<SwitchParameter>]] [-Scope [<FeatureScope>]] -Identity [<GuidPipeBind>]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Force|SwitchParameter|False|Forcibly enable the feature.
Identity|GuidPipeBind|True|The id of the feature to enable.
Scope|FeatureScope|False|
##Examples

###Example 1
    PS:> Enable-SPOnlineFeature -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe


###Example 2
    PS:> Enable-SPOnlineFeature -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe -Force


###Example 3
    PS:> Enable-SPOnlineFeature -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe -Scope Web

