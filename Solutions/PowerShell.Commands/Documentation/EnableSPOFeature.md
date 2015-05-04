#Enable-SPOFeature
*Topic automatically generated on: 2015-05-04*

Enables a feature
##Syntax
```powershell
Enable-SPOFeature [-Force [<SwitchParameter>]] [-Scope <FeatureScope>] [-Sandboxed [<SwitchParameter>]] -Identity <GuidPipeBind>```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Force|SwitchParameter|False|Forcibly enable the feature.
Identity|GuidPipeBind|True|The id of the feature to enable.
Sandboxed|SwitchParameter|False|Specify this parameter if the feature you're trying to active is part of a sandboxed solution.
Scope|FeatureScope|False|
##Examples

###Example 1
    PS:> Enable-SPOFeature -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe -Scope Web


###Example 2
    PS:> Enable-SPOFeature -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe


###Example 3
    PS:> Enable-SPOFeature -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe -Force

<!-- Ref: 873A48DD2C42F4022339AF0C798A6C5E -->