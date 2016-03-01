"use strict";

//******************************************************************************
//* DEPENDENCIES
//******************************************************************************

var gulp = require("gulp"),
    print = require('gulp-print'),    
    src = require("vinyl-source-stream"),
    buffer = require("vinyl-buffer"),
    srcmaps = require("gulp-sourcemaps"),
    header = require('gulp-header'),
    clean = require('gulp-clean'),
    
    runSequence = require("run-sequence"),    
    mocha = require("gulp-mocha"),
    istanbul = require("gulp-istanbul"), 
    
    tslint = require("gulp-tslint"),
    tsc = require("gulp-typescript"),
    tsify = require('tsify'),
    
    browserSync = require('browser-sync').create(),
    browserify = require("browserify"),
    uglify = require("gulp-uglify");

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
    "BundleFileName": "pnp.js",  
    "MinifyFileName": "pnp.min.js"
}

var PnPLocalServer = {
    "RootFolder":'server-root',
    "ScriptsRootFolder":'scripts'    
}
  
var tsProject = tsc.createProject("tsconfig.json");
var pkg = require("./package.json");

var banner = [
        "/**",
        " * <%= pkg.name %> v.<%= pkg.version %> - <%= pkg.description %>",
        " * Copyright (c) 2016 <%= pkg.author %>",
        " * <%= pkg.license %>",
        " */", ""
    ].join("\n");
    
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

gulp.task('clean', function () { 
  var directories = [];
  directories.push(TSCompiledOutput.RootFolder);
  directories.push(PnPLocalServer.RootFolder + "/" + PnPLocalServer.ScriptsRootFolder);
  
  return gulp.src(directories, {read: false})
    .pipe(clean());
});

gulp.task("build-typings", function () {    
    var src = TSWorkspace.Files;
    src.push(TSTypings.Main);
    
    return gulp.src(src)
        .pipe(tsc(tsProject))
        .dts.pipe(gulp.dest(TSTypings.PnPRootFolder));
});

gulp.task("build", ["lint", "build-typings", "clean"], function () {
    var src = TSWorkspace.Files;
    src = src.concat(TSTypings.PnPFiles);
    src.push(TSTypings.Main);

    return gulp.src(src)
        .pipe(tsc(tsProject))
        .js.pipe(gulp.dest(TSCompiledOutput.RootFolder))
        .pipe(print());
});

//******************************************************************************
//* BUILD DIST FOLDER
//******************************************************************************

function packageBundle()
{
    var bify = browserify({debug: true, standalone: 'PnP'});
    
    var stream = bify.add(TSWorkspace.PnPFile)
                    .plugin(tsify)
                    .bundle();
    
    console.log(TSDist.RootFolder + "/" + TSDist.BundleFileName);
    
    return stream
        .pipe(src(TSDist.BundleFileName))        
        .pipe(buffer())
        .pipe(header(banner, { pkg : pkg } ))
        .pipe(gulp.dest(TSDist.RootFolder));        
}

function packageBundleUglify()
{
    var bify = browserify({debug: true, standalone: 'PnP'});
    
    var stream = bify.add(TSWorkspace.PnPFile)
                    .plugin(tsify)
                    .bundle();
    
    console.log(TSDist.RootFolder + "/" + TSDist.MinifyFileName);
    console.log(TSDist.RootFolder + "/" + TSDist.MinifyFileName + ".map");
        
    return stream
        .pipe(src(TSDist.MinifyFileName))
        .pipe(buffer())
        .pipe(srcmaps.init({ loadMaps: true }))
        .pipe(uglify())
        .pipe(header(banner, { pkg : pkg } ))
        .pipe(srcmaps.write('./'))
        .pipe(gulp.dest(TSDist.RootFolder))
}

gulp.task("package", ["build"], function () {
           packageBundle();
           packageBundleUglify();
});

//******************************************************************************
//* TEST
//******************************************************************************

gulp.task("istanbul:hook", function () {
    return gulp.src(TSCompiledOutput.JSCodeFiles)
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

function setBrowserSync(buildServeTaskName)
{
    browserSync.init({
        server: PnPLocalServer.RootFolder
    });

    gulp.watch(TSWorkspace.Files, ["lint", buildServeTaskName]);
    gulp.watch(PnPLocalServer.RootFolder).on('change', browserSync.reload);
    gulp.watch(PnPLocalServer.RootFolder + "/" + PnPLocalServer.ScriptsRootFolder + "/**.js").on('change', browserSync.reload);
    gulp.watch(PnPLocalServer.RootFolder + "/" + PnPLocalServer.ScriptsRootFolder + "/**/**.js").on('change', browserSync.reload);
}

// DEV SERVE (DEPRECATED ? ANY NEED TO HAVE ALL FILES IN SERVER-ROOT/SCRIPTS ?)

gulp.task("build-serve", ["lint", "build"], function () {
    var src = TSWorkspace.Files;
    src = src.concat(TSTypings.PnPFiles);
    src.push(TSTypings.Main);
        
    var tsBundleProject = tsc.createProject("tsconfig-serve.json");

    return gulp.src(src)
        .pipe(tsc(tsBundleProject))
        .js.pipe(gulp.dest(PnPLocalServer.RootFolder + "/" + PnPLocalServer.ScriptsRootFolder));
});

gulp.task("serve", ["lint", "build-serve"], function () {    
    setBrowserSync("build-serve");
});

// DIST SERVE (BUNDLE WITH SOURCE MAP)

gulp.task("build-serve-dist", ["lint", "package"], function () {
    var distFiles = TSDist.RootFolder +  "/*.{js,map}"
    
    return gulp.src(distFiles)
        .pipe(gulp.dest(PnPLocalServer.RootFolder + "/" + PnPLocalServer.ScriptsRootFolder));
});

gulp.task("serve-dist", ["lint", "build-serve-dist"], function () {
    setBrowserSync("build-serve-dist");
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