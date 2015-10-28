'use strict';

// Create pipe for all hint messages from different modules
window.angular.hint = require('angular-hint-log');

// Load angular hint modules
require('angular-hint-controllers');
require('angular-hint-directives');
//require('angular-hint-dom');
require('angular-hint-events');
//require('angular-hint-interpolation');
require('angular-hint-modules');
require('angular-hint-scopes');

// List of all possible modules
// The default ng-hint behavior loads all modules
var allModules = [
  'ngHintControllers',
  'ngHintDirectives',
//  'ngHintDom',
  'ngHintEvents',
//  'ngHintInterpolation',
  'ngHintModules',
  'ngHintScopes'
];

var SEVERITY_WARNING = 2;

// Determine whether this run is by protractor.
// If protractor is running, the bootstrap will already be deferred.
// In this case `resumeBootstrap` should be patched to load the hint modules.
if (window.name === 'NG_DEFER_BOOTSTRAP!') {
    var originalResumeBootstrap;
    Object.defineProperty(angular, 'resumeBootstrap', {
        get: function () {
            return function (modules) {
                return originalResumeBootstrap.call(angular, modules.concat(loadModules()));
            };
        },
        set: function (resumeBootstrap) {
            originalResumeBootstrap = resumeBootstrap;
        }
    });
}
    //If this is not a test, defer bootstrapping
else {
    window.name = 'NG_DEFER_BOOTSTRAP!';

    // determine which modules to load and resume bootstrap
    document.addEventListener('DOMContentLoaded', maybeBootstrap);
}

function maybeBootstrap() {
    // we don't know if angular is loaded
    if (!angular.resumeBootstrap) {
        return setTimeout(maybeBootstrap, 1);
    }

    var modules = loadModules();
    angular.resumeBootstrap(modules);
}

function loadModules() {
    var modules = [], elt;

    if ((elt = document.querySelector('[ng-hint-include]'))) {
        modules = hintModulesFromElement(elt);
    } else if (elt = document.querySelector('[ng-hint-exclude]')) {
        modules = excludeModules(hintModulesFromElement(elt));
    } else if (document.querySelector('[ng-hint]')) {
        modules = allModules;
    } else {
        angular.hint.logMessage('General', 'ngHint is included on the page, but is not active because ' +
          'there is no `ng-hint` attribute present', SEVERITY_WARNING);
    }
    return modules;
}

function excludeModules(modulesToExclude) {
    return allModules.filter(function (module) {
        return modulesToExclude.indexOf(module) === -1;
    });
}

function hintModulesFromElement(elt) {
    var selectedModules = (elt.attributes['ng-hint-include'] ||
      elt.attributes['ng-hint-exclude']).value.split(' ');

    return selectedModules.map(hintModuleName).filter(function (name) {
        return (allModules.indexOf(name) > -1) ||
          angular.hint.logMessage('General', 'Module ' + name + ' could not be found', SEVERITY_WARNING);
    });
}

function hintModuleName(name) {
    return 'ngHint' + title(name);
}

function title(str) {
    return str[0].toUpperCase() + str.substr(1);
}

var LEVELS = [
  'error',
  'warning',
  'suggestion'
];

function flush() {
    var log = angular.hint.flush(),
        groups = Object.keys(log);

    groups.forEach(function (groupName) {
        var group = log[groupName];
        var header = 'Angular Hint: ' + groupName;

        console.groupCollapsed ?
            console.groupCollapsed(header) :
            console.log(header);

        LEVELS.forEach(function (level) {
            group[level] && logGroup(group[level], title(level));
        });
        console.groupEnd && console.groupEnd();
    });
}

setInterval(flush, 2)

angular.hint.onMessage = function () {
    setTimeout(flush, 2);
};

function logGroup(group, type) {
    console.group ? console.group(type) : console.log(type);
    for (var i = 0, ii = group.length; i < ii; i++) {
        console.log(group[i]);
    }
    console.group && console.groupEnd();
}

