'use strict';

function getQueryStringParameter(param) {
    var params = document.URL.split("?")[1].split("&");
    
    for (var i = 0; i < params.length; i = i + 1) {
        var singleParam = params[i].split("=");
        if (singleParam[0] == param) {
            return singleParam[1];
        }
    }
}