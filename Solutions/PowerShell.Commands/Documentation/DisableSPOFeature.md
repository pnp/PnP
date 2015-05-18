#Disable-SPOFeature
*Topic automatically generated on: 2015-05-04*

Disables a feature
##Syntax
```powershell
Disable-SPOFeature [-Force [<SwitchParameter>]] [-Scope <FeatureScope>] -Identity <GuidPipeBind>```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Force|SwitchParameter|False|Forcibly disable the feature.
Identity|GuidPipeBind|True|The id of the feature to disable.
Scope|FeatureScope|False|
##Examples

###Example 1
    PS:> Disable-SPOnlineFeature -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe


###Example 2
    PS:> Disable-SPOnlineFeature -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe -Force


###Example 3
    PS:> Disable-SPOnlineFeature -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe -Scope Web

<!-- Ref: B9AED0F99404EB9F5F27F5A7300A2EE1 -->