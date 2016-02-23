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
//* BUILD
//******************************************************************************
var tsProject = tsc.createProject("tsconfig.json");

gulp.task("update-definitions", function() {
    return gulp.src([
            "src/**/**.ts",
            "typings/main.d.ts/"
        ])
        .pipe(tsc(tsProject))
        .dts.pipe(gulp.dest('typings/project'));
});

gulp.task("build-app", ["update-definitions"], function () {
        return gulp.src([
            "src/**/**.ts",
            "typings/main.d.ts/",
            "typings/project/**/*.d.ts"
        ])
        .pipe(tsc(tsProject))
        .js.pipe(gulp.dest('output'));
});

var tsTestProject = tsc.createProject("tsconfig-tests.json");

gulp.task("build-test", function () {
    return gulp.src([
        "tests/**/*.ts",
        "typings/main.d.ts/"
    ])
        .pipe(tsc(tsTestProject))
        .js.pipe(gulp.dest("output/tests/"));
});

gulp.task("build", function (cb) {
    runSequence(["build-app", "build-test"], cb);
});

//******************************************************************************
//* TEST
//******************************************************************************
gulp.task("istanbul:hook", function () {
    return gulp.src(['output/**/*.js'])
    // Covering files
        .pipe(istanbul())
    // Force `require` to return covered files
        .pipe(istanbul.hookRequire());
});

gulp.task("test", ["build", "istanbul:hook"], function () {
    return gulp.src('output/tests/**/*.test.js')
        .pipe(mocha({ ui: 'bdd' }))
        .pipe(istanbul.writeReports());
});

//******************************************************************************
//* BUNDLE
//******************************************************************************
gulp.task("bundle", function () {

    var outputFolder = "dist/";

    return gulp.src('output/**/*.js')
        .pipe(gulp.dest(outputFolder));
});

//******************************************************************************
//* Copy files to local server
//******************************************************************************
gulp.task("updateserverroot", function () {

    var outputFolder = "server-root/scripts";

    return gulp.src('dist/**/*.js')
        .pipe(gulp.dest(outputFolder));
});

//******************************************************************************
//* Build Watch
//******************************************************************************
gulp.task("build-watch", function () {
    gulp.watch(["src/**/**.ts", "tests/**/*.ts"], ["lint", "build"]);
});

//******************************************************************************
//* DEV SERVER
//******************************************************************************
gulp.task("serve", ["default"], function () {

    browserSync.init({
        server: "./server-root"
    });

    gulp.watch(["src/**/**.ts", "tests/**/*.ts"], ["default"]);
    gulp.watch("server-root").on('change', browserSync.reload);
});

//******************************************************************************
//* DEFAULT
//******************************************************************************
gulp.task("default", function (cb) {
    runSequence("lint", "build", "test", "bundle", "updateserverroot", cb);
});
