#!/usr/bin/env node

// Copyright (c) Microsoft Open Technologies, Inc.  All rights reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

var CODE_SIGN_ENTITLEMENTS = 'CODE_SIGN_ENTITLEMENTS';

var ACTION_INSTALL = 1;
var ACTION_UNINSTALL = 2;

module.exports = function (ctx) {

    var action;

    if (ctx.hook == 'after_plugin_install') {
        console.log('Adding required keychain sharing capability (ADALiOS.entitlements)..');
        action = ACTION_INSTALL;
    } else if (ctx.hook == 'before_plugin_uninstall' || ctx.hook == 'before_plugin_rm') {
        action = ACTION_UNINSTALL;
        console.log('Removing keychain sharing capability (ADALiOS.entitlements)..');
    } else {
        // script is intended to be used only after plugin install and before uninstall
        return;
    };

    var fs = ctx.requireCordovaModule('fs');
    var path = ctx.requireCordovaModule('path');
    var xcode = ctx.requireCordovaModule('xcode');

    var deferral = new ctx.requireCordovaModule('q').defer();

    var platformRoot = path.join(ctx.opts.projectRoot, 'platforms', 'ios');

    // this api is required to clear internal cordova ios projects cache;
    // otherwise our changes will be overwritten by cached item
    var iosProjectFile =  null;
    try {
        // recent versions (4.0+) of cordova-ios with own platform api
        iosProjectFile = require(path.join(ctx.opts.projectRoot, 'platforms/ios/cordova/lib/projectFile'));
    } catch (ex) {
        // fallback to cordova-lib (shared platform functionality)
        iosProjectFile = ctx.requireCordovaModule('../plugman/platforms/ios');
    }

    fs.readdir(platformRoot, function (err, data) {
        if(err) {
            throw err;
        }

        var projFolder;
        var projName;

        // Find the project folder by looking for *.xcodeproj
        if(data && data.length) {
            data.forEach(function (folder) {
                if(folder.match(/\.xcodeproj$/)) {
                    projFolder = path.join(platformRoot, folder);
                    projName = path.basename(folder, '.xcodeproj');
                }
            });
        }

        if(!projFolder) {
            throw new Error("Could not find an .xcodeproj folder in: " + platformRoot);
        }

        var projectPath = path.join(projFolder, 'project.pbxproj');
        var xcodeProject = xcode.project(projectPath);

        var entitlementsFile = path.join(projName, "Resources/ADALiOS.entitlements");
        // escape path so that it works if project name contains whitespaces
        entitlementsFile = "\"" + entitlementsFile + "\"";

        console.log('Attempt to update xcode project: ' + projectPath);

        xcodeProject.parseSync();

        var buildConfig = xcodeProject.pbxXCBuildConfigurationSection();

        if (action == ACTION_INSTALL) {
            console.log('Adding reference to entitlements file ' + entitlementsFile);
            setbuildSettingsProp(buildConfig, projName, CODE_SIGN_ENTITLEMENTS, entitlementsFile);
        } else { // uninstall
            console.log('Removing entitlements from ' + CODE_SIGN_ENTITLEMENTS + ' section');
            setbuildSettingsProp(buildConfig, projName, CODE_SIGN_ENTITLEMENTS, null);
        }

        fs.writeFileSync(projectPath, xcodeProject.writeSync());

        if (iosProjectFile && iosProjectFile.purgeProjectFileCache) {
            console.log('Updating iOS projects cache...');
            iosProjectFile.purgeProjectFileCache(platformRoot);
        }

        console.log('Operation completed');
        deferral.resolve();
    });

    return deferral.promise;
};

function setbuildSettingsProp(projSection, projName, propName, value) {

    Object.keys(projSection).forEach(function (p) {
        // we check for PRODUCT_NAME here to skip CordovaLib
        // TODO better to test for "%projName%" or %projName%
        if (p == 'buildSettings' && projSection[p]['PRODUCT_NAME']) {
            console.log(propName + ' = ' + value);

            if (value !== null) {
                projSection[p][propName] = value;
            } else {
                delete projSection[p][propName];
            }
        } else if (typeof projSection[p] == 'object') {
            setbuildSettingsProp(projSection[p], projName, propName, value);
        }
    });
}
