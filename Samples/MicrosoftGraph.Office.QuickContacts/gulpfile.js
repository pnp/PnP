'use script';

var gulp = require('gulp');
var webserver = require('gulp-webserver');
var browserSync = require('browser-sync');

gulp.task('serve-static', function() {
    gulp.src('.')
        .pipe(webserver({
            https: true,
            port: '8443',
            host: 'localhost',
            directoryListing: false,
            fallback: 'index.html'
        }));
});

gulp.task('serve', () => {
    browserSync({
        notify: false,
        port: 8443,
        server: './',
        https: true
    });
});
