#Enable&#8209;SPOFeature
*Topic automatically generated on: 2015-03-12*

Enables a feature
##Syntax
```powershell
Enable&#8209;SPOFeature [-Force [<SwitchParameter>]] [-Scope [<FeatureScope>]] -Identity [<GuidPipeBind>]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Force|SwitchParameter|False|Forcibly enable the feature.
Identity|GuidPipeBind|True|The id of the feature to enable.
Scope|FeatureScope|False|
##Examples

###Example 1
    PS:> Enable-SPOnlineFeature -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe -Force


###Example 2
    PS:> Enable-SPOnlineFeature -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe


###Example 3
    PS:> Enable-SPOnlineFeature -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe -Scope Web

