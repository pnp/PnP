
//// variable used for cross site CSOM calls
//var context;
//// peoplePicker variable needs to be globally scoped as the generated html contains JS that will call into functions of this class
//var peoplePicker;

var $app = {

    onStartPromise: null,
    getContextPromise: null,
    spContext: null,

    // using jQuery promises to load a single shared spcontext across a client application
    withSPContext: function (action) {

        if ($app.getContextPromise == null) {
            $app.getContextPromise = $.Deferred(function (def) {

                var hostUrl = $app.getUrlParamByName('SPHostUrl');
                var appWebUrl = $app.getUrlParamByName('SPAppWebUrl');
                var scriptbase = hostUrl + '/_layouts/15/';

                $.getScript(scriptbase + 'SP.Runtime.js').done(function () {
                    $.getScript(scriptbase + 'SP.js').done(function () {
                        $.getScript(scriptbase + 'SP.RequestExecutor.js').done(function () {

                            $app.spContext = new SP.ClientContext(appWebUrl);
                            var factory = new SP.ProxyWebRequestExecutorFactory(appWebUrl);
                            $app.spContext.set_webRequestExecutorFactory(factory);

                            def.resolveWith($app.spContext, [$app.spContext]);
                        });
                    });
                });

            }).promise();
        }

        if ($.isFunction(action)) {
            $app.getContextPromise.done(action);
        }

        return $app.getContextPromise;
    },

    getUrlParamByName: function (name) {
        name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
        var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
        results = regex.exec(location.search);
        return results == null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
    },

    getCtxCallback: function (context, method) {

        var args = [].slice.call(arguments).slice(2);

        return function () {
            method.apply(context, args);
        }
    },

    defaultProgramErrorHandler: function (jqXHR, textStatus) {
        textStatus = '' + textStatus;
        console.error(textStatus);
        alert('There was a problem executing your request. Please try again. If the problem persists please relaunch the app or refresh the page.');
    },

    appendSPQueryToUrl: function (/*string*/ url) {

        // we already have the SPHostUrl param from somewhere else, just give back the url
        if (url.indexOf('SPHostUrl=') > -1) {
            return url;
        }

        // add the required parameters
        url += url.indexOf('?') > -1 ? '&' : '?';
        url += 'SPHostUrl=' + encodeURIComponent($app.getUrlParamByName('SPHostUrl'));
        url += '&SPAppWebUrl=' + encodeURIComponent($app.getUrlParamByName('SPAppWebUrl'));
        url += '&SPLanguage=' + encodeURIComponent($app.getUrlParamByName('SPLanguage'));
        url += '&SPClientTag=' + encodeURIComponent($app.getUrlParamByName('SPClientTag'));
        url += '&SPProductNumber=' + encodeURIComponent($app.getUrlParamByName('SPProductNumber'));

        return url;
    },

    getAuthorityFromUrl: function (/*string*/ url) {
        if (url) {
            var match = /^(?:https:\/\/|http:\/\/|\/\/)([^\/\?#]+)(?:\/|#|$|\?)/i.exec(url);
            if (match) {
                return match[1].toUpperCase();
            }
        }
        return null;
    },

    ensureContextQueryString: function () {

        // remove the redirect flag
        var SPHasRedirectedToSharePointParam = "&SPHasRedirectedToSharePoint=1";
        var queryString = window.location.search;
        if (queryString.indexOf(SPHasRedirectedToSharePointParam) >= 0) {
            window.location.search = queryString.replace(SPHasRedirectedToSharePointParam, "");
        }

        $app.ensureSPHostUrlInLinks($('a'));
    },

    ensureSPHostUrlInLinks: function (/*jquery*/ parentNode) {

        var currentAuthority = $app.getAuthorityFromUrl(window.location.href);

        parentNode.filter(function () {
            var authority = $app.getAuthorityFromUrl(this.href);
            if (!authority && /^#|:/.test(this.href)) {
                // Filters out anchors and urls with other unsupported protocols.
                return false;
            }
            return authority != null && authority.toUpperCase() == currentAuthority;
        }).each(function () {
            this.href = $app.appendSPQueryToUrl(this.href);
        });
    },

    onStart: function (/*function()*/ onStartFunc) {

        if ($app.onStartPromise == null) {
            $app.onStartPromise = $.when($.Deferred(function (d) { $(d.resolve); }).then($.Deferred(function (d) { $app.ensureContextQueryString(); d.resolve(); }))).promise();
        }

        if ($.isFunction(onStartFunc)) {
            $app.onStartPromise.done(onStartFunc);
        }

        return $app.onStartPromise;
    },

    combinePaths: function () {
        var parts = [];
        for (var i = 0; i < arguments.length; i++) {
            parts.push(arguments[i].replace(/^[\\|\/]/, '').replace(/[\\|\/]$/, ''));
        }
        return parts.join("/").replace(/\\/, '/');
    }
};


