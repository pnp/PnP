#Set-SPOTheme
*Topic automatically generated on: 2015-08-04*

Sets the theme of the current web.
##Syntax
```powershell
Set-SPOTheme [-ColorPaletteUrl [<String>]] [-FontSchemeUrl [<String>]] [-BackgroundImageUrl [<String>]] [-ShareGenerated [<SwitchParameter>]] [-Web [<WebPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
BackgroundImageUrl|String|False|
ColorPaletteUrl|String|False|
FontSchemeUrl|String|False|
ShareGenerated|SwitchParameter|False|
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    PS:> Set-SPOTheme -ColorPaletteUrl /_catalogs/theme/15/company.spcolor

