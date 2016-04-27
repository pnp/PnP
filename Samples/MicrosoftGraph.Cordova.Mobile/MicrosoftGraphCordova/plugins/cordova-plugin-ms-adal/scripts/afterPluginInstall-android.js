#!/usr/bin/env node

// Copyright (c) Microsoft Open Technologies, Inc.  All rights reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module.exports = function (ctx) {
    var shell = ctx.requireCordovaModule('shelljs');
    var path = ctx.requireCordovaModule('path');
    var configFile = path.resolve(ctx.opts.projectRoot, 'config.xml');

    // check if minSdkReference is already set
    if (shell.grep('android-minSdkVersion', configFile)) {
        return;
    }
    // add required minSdkVersion to config
    shell.sed('-i',
        '</widget>',
        '    <preference name="android-minSdkVersion" value="14" />\n' + 
        '</widget>',
        configFile);
};
