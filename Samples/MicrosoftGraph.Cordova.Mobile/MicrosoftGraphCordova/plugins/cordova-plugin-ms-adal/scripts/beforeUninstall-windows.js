#!/usr/bin/env node

// Copyright (c) Microsoft Open Technologies, Inc.  All rights reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module.exports = function (ctx) {
    var shell = ctx.requireCordovaModule('shelljs');
    var path = ctx.requireCordovaModule('path');
    var fs = ctx.requireCordovaModule('fs');
    var helperPluginId = 'cordova-plugin-ms-adal-sso';

    // Removing references from .projitems
    var projitems = shell.ls(path.join(ctx.opts.projectRoot, 'platforms/windows/*.projitems'))[0];
    var referenceRe = /(<ItemGroup Condition="!?\$\(MSBuildProjectFullPath\.EndsWith\('\.Phone\.jsproj'\)\)">\s*<Reference Include="Microsoft\.IdentityModel\.Clients\.ActiveDirectory">[\s\S]*?<\/ItemGroup>)/i;

    // Removing 2 reference groups
    shell.sed('-i', referenceRe, '', projitems);
    shell.sed('-i', referenceRe, '', projitems);
    console.log('Removed 2 refereces from projitems');

    // Removing helper plugin as we added it manually
    var ssoPluginInstallPath = path.join(ctx.opts.projectRoot, 'plugins', helperPluginId);
    var ssoPluginDepEnabled = fs.existsSync(ssoPluginInstallPath);
    
    if (ssoPluginDepEnabled) {
        console.log('Removing SSO helper plugin');

        var plugmanInstallOpts = {
            plugins_dir: path.join(ctx.opts.projectRoot, 'plugins'),
            platform: 'windows',
            project: path.join(ctx.opts.projectRoot, 'platforms', 'windows')
        };

        var plugman = ctx.requireCordovaModule('../plugman/plugman');

        plugman.uninstall(plugmanInstallOpts.platform, plugmanInstallOpts.project, 
            helperPluginId, plugmanInstallOpts.plugins_dir);
    }
};
