'use strict';

/* Filters */

angular.module('spmanageFilters', []).filter('encodeURIComponent', function () {
  return function(input) {
      return window.encodeURIComponent(input);
  };
});
