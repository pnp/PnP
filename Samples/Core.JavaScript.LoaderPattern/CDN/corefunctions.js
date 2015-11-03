// just an example of loading a common script library ahead of later files
// name this anything you want, but remember it will be in the global namespace
// you could also use any number of available js module frameworks
var $core = {

    // allows for the calling pattern getCtxCallback(thisobj, method, methodarg1, methodarg2, etc...)
    getCtxCallback: function (context, method) {
        var args = [].slice.call(arguments).slice(2);
        return function () { method.apply(context, args); }
    },

    // tests if a url param exists
    urlParamExists: function (name) {
        name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
        var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
        return regex.test(location.search);
    },

    // gets a url param value by name
    getUrlParamByName: function (name) {
        name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
        var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
        results = regex.exec(location.search);
        return results == null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
    },

    // gets a url param by name and attempts to parse a bool value
    getUrlParamBoolByName: function (name) {
        var p = $core.getUrlParamByName(name);
        return !(p == '' || /[false|0]/i.test(p));
    },

    // inserts the string s into the string target as the index specified by index
    stringInsert: function (target, index, s) {
        if (index > 0) {
            return target.substring(0, index) + s + target.substring(index, target.length);
        }
        return s + target;
    },

    loadStyleSheet: function (path) {
        $('<link>').appendTo('head').attr({ type: 'text/css', rel: 'stylesheet' }).attr('href', path);
    }
};
