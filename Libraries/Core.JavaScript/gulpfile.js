"use strict";

//******************************************************************************
//* DEPENDENCIES
//******************************************************************************
var gulp = require("gulp"),
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
    browserSync = require('browser-sync').create();
    
//******************************************************************************
//* LINT
//******************************************************************************
gulp.task("lint", function () {
    return gulp.src([
        "src/**/**.ts",
        "tests/**/**.test.ts"
    ])
        .pipe(tslint({}))
        .pipe(tslint.report("verbose"));
});

//******************************************************************************
//* BUILD, placing files in output - used when testing
//******************************************************************************
gulp.task("update-defs", function () {

    var tsProject = tsc.createProject("tsconfig.json");

    return gulp.src([
        "src/**/**.ts",
        "typings/main.d.ts/"
    ])
        .pipe(tsc(tsProject))
        .dts.pipe(gulp.dest('typings/project'));
});

gulp.task("build-app", ["update-defs"], function () {

    var tsProject = tsc.createProject("tsconfig.json");

    return gulp.src([
        "src/**/**.ts",
        "typings/main.d.ts/",
        "typings/project/**/*.d.ts"
    ])
        .pipe(tsc(tsProject))
        .js.pipe(gulp.dest('output'));
});

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

gulp.task("build", function (cb) {
    runSequence(["build-app"], cb);
});

//******************************************************************************
//* TEST
//******************************************************************************
gulp.task("istanbul:hook", function () {
    return gulp.src(['output/**/*.js', '!output/mocks/**/*.js', '!output/**/*.test.js'])
    // Covering files
        .pipe(istanbul())
    // Force `require` to return covered files
        .pipe(istanbul.hookRequire());
});

gulp.task("test", ["lint", "build", "istanbul:hook"], function () {
    return gulp.src('output/**/*.test.js')
        .pipe(mocha({ ui: 'bdd' }))
        .pipe(istanbul.writeReports());
});

//******************************************************************************
//* do the build that places the output in the server-root/scripts folder
//******************************************************************************
gulp.task("build-serve", function () {

    var outputFolder = "server-root/scripts";

    var tsBundleProject = tsc.createProject("tsconfig-serve.json");

    return gulp.src([
        "src/**/**.ts",
        "typings/main.d.ts/",
        "typings/project/**/*.d.ts"
    ])
        .pipe(tsc(tsBundleProject))
        .js.pipe(gulp.dest(outputFolder));
});

//******************************************************************************
//* do the build that places the output in the dist folder
//******************************************************************************
gulp.task("package", function () {

    var outputFolder = "dist";

    var tsBundleProject = tsc.createProject("tsconfig-package.json");

    return gulp.src([
        "src/**/**.ts",
        "typings/main.d.ts/",
        "typings/project/**/*.d.ts",
        '!src/**/*.test.ts'
    ])
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
