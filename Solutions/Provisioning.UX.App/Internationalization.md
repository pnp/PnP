# Translating the Provisioning UX Solution
How to add new translations to the Provisioning UX Solution.

 **Last modified:** September 2, 2015

 _**Applies to:** PnP Provisioning.UX.App_

 **In this article**

 [Angular Translate](#AngularTranslate)

 [Translation files](#Files)

## Angular Translate
<a name="AngularTranslate"> </a>

The Provisioning.UX.App uses the [Angular Translate](https://angular-translate.github.io/) AngularJS module for internationalization

## Translation Files
<a name="Files"> </a>
Translations are stored in static JavaScript files using JSON notation. They are located in the Provisioning.UX.AppWeb project under /scripts/i18n/_culture_.json. The _culture_ is read from the _SPLanguage_ query string attribute of the app, which represents the language settings for the Host Web. Fallback culture is en-US.

To add new translation files make a copy of the en-US.json file and change the name of it to the culture and start translating. 
