/* jshint jquery: true */
/* global SPListmanager */
/* global SP */

(function () {
    'use strict';

    SPListmanager.context = SP.ClientContext.get_current();
    SPListmanager.user = SPListmanager.context.get_web().get_currentUser();


    $(document).ready(function () {

        var HTMLWORKINPROGRESS = "<div style='margin:auto;width:500px;height:500px;margin-top:200px;'><h3>Working on it...</h3><img src='{0}/_layouts/images/loading.gif' alt='loading' /> This shouln't take long.<div>";
        $(".show-workinprogress").html(HTMLWORKINPROGRESS.replace("{0}", SPListmanager.hostweburl));

        SPListmanager.hostweburl =
            decodeURIComponent(
                SPListmanager.GetQueryStringParameter('SPHostUrl')
        );
        SPListmanager.appweburl =
            decodeURIComponent(
                SPListmanager.GetQueryStringParameter('SPAppWebUrl')
         );
        SPListmanager.ListDetails.listID =
            decodeURIComponent(
                SPListmanager.GetQueryStringParameter('List')
        );

        SPListmanager.scriptbase = SPListmanager.hostweburl + '/_layouts/15/';
        SPListmanager.imagesbase = SPListmanager.hostweburl + '/_layouts/images/';

        $.getScript(SPListmanager.scriptbase + 'SP.Runtime.js',
           function () {
               $.getScript(SPListmanager.scriptbase + 'SP.js',
                   function () { $.getScript(SPListmanager.scriptbase + 'SP.RequestExecutor.js', SPListmanager.ListDetails.init); }
               );
           }
        );
    });
}());