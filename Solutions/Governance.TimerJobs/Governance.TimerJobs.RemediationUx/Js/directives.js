'use strict';

var spmanageDirectives = angular.module('spmanageDirectives', []);

spmanageDirectives.directive('ngConfirmClick', [
        function () {
            return {
                link: function (scope, element, attr) {                    
                    var msg = attr.ngConfirmClick || "Are you sure?";
                    var confirmedAction = attr.confirmedClick;
                    var cancelAction = attr.cancelClick;
                    element.bind('click', function (event) {
                        var trigger = scope.$eval(attr.confirmCondition);
                        if (!trigger)
                            return;
                        if (window.confirm(msg)) {
                            scope.$apply(confirmedAction)
                        }
                        else {
                            scope.$apply(cancelAction)
                        }
                    });
                }
            };
        }])