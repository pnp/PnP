#Set-SPOTheme
*Topic automatically generated on: 2015-06-11*

Sets the theme of the current web.
##Syntax
```powershell
Set-SPOTheme [-ColorPaletteUrl <String>] [-FontSchemeUrl <String>] [-BackgroundImageUrl <String>] [-ShareGenerated [<SwitchParameter>]] [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|BackgroundImageUrl|String|False||
|ColorPaletteUrl|String|False||
|FontSchemeUrl|String|False||
|ShareGenerated|SwitchParameter|False||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    PS:> Set-SPOTheme -ColorPaletteUrl /_catalogs/theme/15/company.spcolor

<!-- Ref: 616A57CB884B27F2BAD1DD8F4F0352DF -->