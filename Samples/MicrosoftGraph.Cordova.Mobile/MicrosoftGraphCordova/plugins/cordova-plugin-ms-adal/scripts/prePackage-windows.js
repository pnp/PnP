#!/usr/bin/env node

// Copyright (c) Microsoft Open Technologies, Inc.  All rights reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module.exports = function (ctx) {
    var shell = ctx.requireCordovaModule('shelljs');
    var path = ctx.requireCordovaModule('path');
    var fs = ctx.requireCordovaModule('fs');
    var helperPluginId = 'cordova-plugin-ms-adal-sso';

    // Read config.xml -> extract adal-use-corporate-network variable value; default it to false
    var useCorporateNetwork = false;
    var configXml = shell.ls(path.join(ctx.opts.projectRoot, 'platforms/windows/config.xml'))[0];
    var pluginXml = shell.ls(path.join(ctx.opts.projectRoot, 'plugins/cordova-plugin-ms-adal/plugin.xml'))[0];

    var rePreferenceValue = /<preference\s+name="adal-use-corporate-network"\s+value="(.+)"\s*\/>/i;
    var preferenceValue = shell.grep(rePreferenceValue, configXml);

    var result = rePreferenceValue.exec(preferenceValue);
    if(result !== null) {
        var match = result[1];

        useCorporateNetwork = match.toUpperCase() === 'TRUE';
    }

    var ssoPluginInstallPath = path.join(ctx.opts.projectRoot, 'plugins', helperPluginId);
    var ssoPluginDepEnabled = fs.existsSync(ssoPluginInstallPath);

    var ssoPluginPath = path.join(ctx.opts.projectRoot, 'plugins/cordova-plugin-ms-adal/src/windows/sso');

    var plugmanInstallOpts = {
        plugins_dir: path.join(ctx.opts.projectRoot, 'plugins'),
        platform: 'windows',
        project: path.join(ctx.opts.projectRoot, 'platforms', 'windows')
    };

    if(useCorporateNetwork === true) {
        // If adal-use-corporate-network is true, check if we have enabled SSO plugin dependency
        //  If yes, then it should be already added, no action needed
        //  If no - enable it and manually install the dependent plugin
        if(!ssoPluginDepEnabled) {
            console.log('useCorporateNetwork: ' + useCorporateNetwork);
            console.log('Adding SSO helper plugin');

            // Enabling dependency
            var plugman = ctx.requireCordovaModule('../plugman/plugman');

            plugman.install(plugmanInstallOpts.platform, plugmanInstallOpts.project, 
                ssoPluginPath, plugmanInstallOpts.plugins_dir);
        }
    } else {
        // If adal-use-corporate-network is false, check if we have disabled SSO plugin dependency
        //  If yes, then it should be already removed, no action needed
        //  If no - disable it and manually uninstall the dependent plugin
        if(ssoPluginDepEnabled) {
            console.log('useCorporateNetwork: ' + useCorporateNetwork);
            console.log('Removing SSO helper plugin');

            // Removing dependency
            var plugman = ctx.requireCordovaModule('../plugman/plugman');

            plugman.uninstall(plugmanInstallOpts.platform, plugmanInstallOpts.project, 
                helperPluginId, plugmanInstallOpts.plugins_dir);
        }
    }
};
