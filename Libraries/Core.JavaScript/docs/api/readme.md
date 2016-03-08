#PnP JavaScript Core#
##API Reference##

This guide describes the public API surface of the Patterns and Practices core JavaScript library. If you see a problem, please submit an issue reporting it or a pull request updating it.

##Loading the Library##

###NPM###
Coming Soon!

###Bower###
Coming Soon!

##Importing the code for use:##

Once you have downloaded the library the next step is including it in your project. Here are some examples using common techniques:

###SystemJS###

If you are using the [SystemJS](https://github.com/systemjs/systemjs) the following example shows how to load the library. It assumes you have already loaded SystemJS into the page.

```JavaScript
System.import('/path/to/pnp.js').then(function (pnp) {
    pnp.logging.write('My first log message!');
});
```