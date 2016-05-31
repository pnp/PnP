#!/usr/bin/env node

// Copyright (c) Microsoft Open Technologies, Inc.  All rights reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module.exports = function (ctx) {
    var shell = ctx.requireCordovaModule('shelljs');
    var path = ctx.requireCordovaModule('path');

    var projitems = shell.ls(path.join(ctx.opts.projectRoot, 'platforms/windows/*.projitems'))[0];
    console.log('Patching windows universal app .projitems file: ' + projitems);
    if (shell.grep(/Condition="\$\(MSBuildProjectFullPath\.EndsWith\(\'\.Phone\.jsproj\'\)\)"/, projitems).length === 0) {
        // Need to write dependencies to projitems instead of separate jsprojs because of MSBuild v.14 issue with Extensions in AppManifest.xml
        var preliminaryRe = /<\/Project>/i;
        var preliminarySubst = "    <ItemGroup>\n" +
            "        <Reference Include=\"Microsoft.IdentityModel.Clients.ActiveDirectory\">\n" +
            "            <HintPath>plugins\\cordova-plugin-ms-adal\\Microsoft.IdentityModel.Clients.ActiveDirectory.winmd</HintPath>\n" +
            "            <IsWinMDFile>true</IsWinMDFile>\n" +
            "        </Reference>\n" +
            "    </ItemGroup>\n" +
            "</Project>";

        // We need 2 item groups
        shell.sed('-i', preliminaryRe, preliminarySubst, projitems);
        shell.sed('-i', preliminaryRe, preliminarySubst, projitems);

        var re = /(<ItemGroup)(>)(\s*<Reference Include="Microsoft.IdentityModel.Clients.ActiveDirectory">\s*<HintPath>)(plugins\\cordova-plugin-ms-adal\\Microsoft.IdentityModel.Clients.ActiveDirectory.winmd)(<\/HintPath>)/i;
        var substPhone = '$1 Condition="$(MSBuildProjectFullPath.EndsWith(\'.Phone.jsproj\'))"$2$3..\\..\\plugins\\cordova-plugin-ms-adal\\src\\windows\\lib\\wpa\\Microsoft.IdentityModel.Clients.ActiveDirectory.winmd$5';
        var substWindows = '$1 Condition="!$(MSBuildProjectFullPath.EndsWith(\'.Phone.jsproj\'))"$2$3..\\..\\plugins\\cordova-plugin-ms-adal\\src\\windows\\lib\\netcore45\\Microsoft.IdentityModel.Clients.ActiveDirectory.winmd$5';

        shell.sed('-i', re, substPhone, projitems);
        shell.sed('-i', re, substWindows, projitems);
    } else {
        console.log('Already patched, skipping...');
    }
};
