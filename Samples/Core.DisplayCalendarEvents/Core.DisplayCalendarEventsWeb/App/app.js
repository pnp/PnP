"use strict";

window.App = angular
    .module('App', [
        'index'
    ]);

App.config(['SharepointProvider', function (SharepointProvider) {
    var queryParameterString = (window.location.search[0] === '?') ? window.location.search.slice(1) : window.location.search;
    var queryParameters = deparam(queryParameterString);
    console.log("SharepointProvider config: queryParams: ", queryParameters);
    console.log("windows.__spUrls: ", window.__spUrls);

    SharepointProvider.setSiteCollectionUrl(queryParameters.SPHostUrl);
    //SharepointProvider.setAppWebUrl(window.__spUrls.spAppWebUrl);
}])