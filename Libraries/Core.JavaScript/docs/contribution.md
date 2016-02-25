#PnP JavaScript Core#
##Contribution Guide##

This is a work in progress and very much dependent on community input. To have your voice heard please join us in the Yammer group!

### General Guidelines ###

1. We are currently **targeting es5** for the largest compatibility possible.
2. **Leave tslint on** - any pull requests that don't pass tslint will not be accepted as-is. If there is a setting causing you frustration please let's discuss. We would rather not have tslint be a hindrance, rather a guide to help our code remain consistent in form.
3. **Write tests** - if you create a new class/module/function please add test coverage along side. This is usually done by creating a *.test.ts file in the same directory. So if you create foo.ts we would expect a corresponding foo.test.ts.
4. “use strict”; - include this directive at the top of all your files. This helps catch some [common issues](http://stackoverflow.com/questions/1335851/what-does-use-strict-do-in-javascript-and-what-is-the-reasoning-behind-it) and throws errors in situations that would otherwise be allowed.

### Looking for Issues? ###

Please check the [issues list](https://github.com/OfficeDev/PnP/issues) and look for the label "JS-SIG" which will be used in conjunction with the standard labels.