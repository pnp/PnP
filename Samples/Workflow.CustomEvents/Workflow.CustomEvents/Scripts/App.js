'use strict';

var context = SP.ClientContext.get_current();

// ---------- Redirect from page ----------
function redirFromCurrentPage(redirectUrl) {
    window.location = redirectUrl;
}

// ---------- Returns an associative array (object) of URL params ----------
function GetUrlParams() {
    var urlParams = null;
    if (urlParams == null) {
        urlParams = {};
        var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (m, key, value) {
            urlParams[key] = decodeURIComponent(value);
        });
    }
    return urlParams;
}

function handleException(err) {
    alert(err);
    console.log(err);
}