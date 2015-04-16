(function ($, RSVP, SP2013, EE, window, undefined) {
    "use strict";

    var app = window.angular.module('EmbeddedEditingApp', []);

    app.controller('IndexController', ['$scope', EE.Controllers.Index]);

})(jQuery, RSVP, SP2013, EE, this);