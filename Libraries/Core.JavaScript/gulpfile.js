"use strict";

//******************************************************************************
//* DEPENDENCIES
//******************************************************************************

var gulp = require("gulp"),
    print = require('gulp-print'),
    browserify = require("browserify"),
    src = require("vinyl-source-stream"),
    buffer = require("vinyl-buffer"),
    tslint = require("gulp-tslint"),
    tsc = require("gulp-typescript"),
    srcmaps = require("gulp-sourcemaps"),
    uglify = require("gulp-uglify"),
    runSequence = require("run-sequence"),
    mocha = require("gulp-mocha"),
    istanbul = require("gulp-istanbul"),
    merge = require('merge-stream'),
    browserSync = require('browser-sync').create(),
    minify = require('gulp-uglify'),
    tsify = require('tsify'),
    header = require('gulp-header');

//******************************************************************************
//* GLOBAL VARIABLES
//******************************************************************************

var TSTypings = {
    "RootFolder":'typings',
    "Main": 'typings/main.d.ts',
    "PnPRootFolder": 'typings/pnp',
    "PnPFiles": [
        'typings/pnp/*.d.ts', 
        'typings/pnp/**/*.d.ts'
    ]  
};

var TSCompiledOutput = {
    "RootFolder":'output',
    "JSCodeFiles" : [  
        'output/*.js',
        'output/**/*.js',
        '!output/*.test.js',
        '!output/**/*.test.js', 
    ],
    "JSTestFiles":  [  
        'output/*.test.js',
        'output/**/*.test.js', 
    ],
};

var TSWorkspace = {
    "RootFolder":'src',
    "PnPFile":"src/pnp.ts",
    "Files":  [ 
        'src/*.ts',
        'src/**/*.ts',   
    ]
}

var TSDist = {
    "RootFolder":'dist',
    "BundleFileName": "pnp.core.js",  
    "MinifyFileName": "pnp.core.min.js"
}
  
var tsProject = tsc.createProject("tsconfig.json");
var pkg = require("./package.json");

//******************************************************************************
//* LINT
//******************************************************************************

gulp.task("lint", function () {    
    return gulp.src(TSWorkspace.Files)
        .pipe(tslint({}))
        .pipe(tslint.report("verbose"));
});

//******************************************************************************
//* BUILD, placing files in compiled - used when testing
//******************************************************************************

gulp.task("build-typings", function () {    
    var src = TSWorkspace.Files;
    src.push(TSTypings.Main);
    
    return gulp.src(src)
        .pipe(tsc(tsProject))
        .dts.pipe(gulp.dest(TSTypings.PnPRootFolder))        
        .pipe(print());
});

gulp.task("build", ["lint", "build-typings"], function () {
    var src = TSWorkspace.Files;
    src = src.concat(TSTypings.PnPFiles);
    src.push(TSTypings.Main);

    gulp.src(src)
        .pipe(tsc(tsProject))
        .js.pipe(gulp.dest(TSCompiledOutput.RootFolder))
        .pipe(print());
});

//******************************************************************************
//* BUILD DIST FOLDER
//******************************************************************************

gulp.task("package", ["build"], function () {
    
    var bundler = null;

    var banner = [
        "/**",
        " * <%= pkg.name %> v.<%= pkg.version %> - <%= pkg.description %>",
        " * Copyright (c) 2016 <%= pkg.author %>",
        " * <%= pkg.license %>",
        " */", ""
    ].join("\n");
        
    bundler = browserify({debug: true});

    bundler.add(TSWorkspace.PnPFile)
                .plugin(tsify)
                .bundle()
                .pipe(src(TSDist.MinifyFileName))
                .pipe(buffer())
                .pipe(srcmaps.init({ loadMaps: true }))
                .pipe(uglify())
                .pipe(header(banner, { pkg : pkg } ))
                .pipe(srcmaps.write('./'))
                .pipe(gulp.dest(TSDist.RootFolder))
                .pipe(print());
    
    bundler = browserify({debug: true});
    
    bundler.add(TSWorkspace.PnPFile)
                .plugin(tsify)
                .bundle()
                .pipe(src(TSDist.BundleFileName))
                .pipe(buffer())
                .pipe(header(banner, { pkg : pkg } ))
                .pipe(gulp.dest(TSDist.RootFolder))
                .pipe(print());
});

//******************************************************************************
//* TEST
//******************************************************************************

gulp.task("istanbul:hook", function () {
    return gulp.src(['output/**/*.js', '!output/**/*.test.js'])
    // Covering files
        .pipe(istanbul())
    // Force `require` to return covered files
        .pipe(istanbul.hookRequire());
});

gulp.task("test", ["build", "istanbul:hook"], function () {
    return gulp.src(TSCompiledOutput.JSTestFiles)
        .pipe(mocha({ ui: 'bdd' }))
        .pipe(istanbul.writeReports());
});

//******************************************************************************
//* BUILD & COPY THE OUTPUT IN THE "SERVER-ROOT/SCRIPTS" FOLDER
//******************************************************************************

gulp.task("build-serve", ["lint", "build"], function () {
    var src = TSWorkspace.Files;
    src = src.concat(TSTypings.PnPFiles);
    src.push(TSTypings.Main);
    
    var outputFolder = "server-root/scripts";

    var tsBundleProject = tsc.createProject("tsconfig-serve.json");

    return gulp.src(src)
        .pipe(tsc(tsBundleProject))
        .js.pipe(gulp.dest(outputFolder));
});

//******************************************************************************
//* DEV SERVER
//******************************************************************************

gulp.task("serve", ["lint", "build-serve"], function () {

    browserSync.init({
        server: "./server-root"
    });

    gulp.watch(["src/**/**.ts"], ["lint", "build-serve"]);
    gulp.watch("server-root").on('change', browserSync.reload);
});

//******************************************************************************
//* DEFAULT
//******************************************************************************

gulp.task("default", function (cb) {
    runSequence("lint", "build", "test", cb);
});


//******************************************************************************
//* NOT USED
//******************************************************************************

// gulp.task("package", function () {
// 
//     var outputFolder = "dist";
// 
//     var tsBundleProject = tsc.createProject("tsconfig-package.json");
// 
//     return gulp.src([
//         "src/**/**.ts",
//         "typings/main.d.ts/",
//         "typings/project/**/*.d.ts",
//         '!src/**/*.test.ts'
//     ])
//         .pipe(tsc(tsBundleProject))
//         .js.pipe(gulp.dest(outputFolder));
// });

// gulp.task("build-test", function () {
// 
//     var tsTestProject = tsc.createProject("tsconfig.json");
// 
//     return gulp.src([
//         "src/tests/**/*.ts",
//         "typings/main.d.ts/"
//     ])
//         .pipe(tsc(tsTestProject))
//         .js.pipe(gulp.dest("output/tests"));
// });