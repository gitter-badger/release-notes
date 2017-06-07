"use strict";

var gulp = require("gulp"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    htmlmin = require("gulp-htmlmin"),
    uglify = require("gulp-uglify"),
    merge = require("merge-stream"),
    del = require("del"),
    bundleconfig = require("./bundleconfig.json");

var regex = {
    css: /\.css$/,
    html: /\.(html|htm)$/,
    js: /\.js$/
};

gulp.task("bundle", ["bundle:js", "bundle:css", "bundle:html"]);
gulp.task("min", ["min:js", "min:css", "min:html"]);

gulp.task("bundle:js", function () {
    var tasks = getBundles(regex.js).map(function (bundle) {
        var bundleMinified = !bundle.minify || bundle.minify.enabled;
        if (bundleMinified) return [];
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(concat(bundle.outputFileName))
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
});

gulp.task("bundle:css", function () {
    var tasks = getBundles(regex.css).map(function (bundle) {
        var bundleMinified = !bundle.minify || bundle.minify.enabled;
        if (bundleMinified) return [];
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(concat(bundle.outputFileName))
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
});

gulp.task("bundle:html", function () {
    var tasks = getBundles(regex.html).map(function (bundle) {
        var bundleMinified = !bundle.minify || bundle.minify.enabled;
        if (bundleMinified) return [];
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(concat(bundle.outputFileName))
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
});

gulp.task("min:js", ["bundle:js"], function () {
    var tasks = getBundles(regex.js).map(function (bundle) {
        var bundleMinified = !bundle.minify || bundle.minify.enabled;
        if (!bundleMinified) return [];
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(concat(bundle.outputFileName))
            .pipe(uglify())
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
});

gulp.task("min:css", ["bundle:css"], function () {
    var tasks = getBundles(regex.css).map(function (bundle) {
        var bundleMinified = !bundle.minify || bundle.minify.enabled;
        if (!bundleMinified) return [];
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(concat(bundle.outputFileName))
            .pipe(cssmin())
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
});

gulp.task("min:html", ["bundle:html"], function () {
    var tasks = getBundles(regex.html).map(function (bundle) {
        var bundleMinified = !bundle.minify || bundle.minify.enabled;
        if (!bundleMinified) return [];
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(concat(bundle.outputFileName))
            .pipe(htmlmin({ collapseWhitespace: true, minifyCSS: true, minifyJS: true }))
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
});

gulp.task("clean", function () {
    var files = bundleconfig.map(function (bundle) {
        return bundle.outputFileName;
    });

    return del(files);
});

gulp.task("watch", function () {
    getBundles(regex.js).forEach(function (bundle) {
        gulp.watch(bundle.inputFiles, ["min:js"]);
    });

    getBundles(regex.css).forEach(function (bundle) {
        gulp.watch(bundle.inputFiles, ["min:css"]);
    });

    getBundles(regex.html).forEach(function (bundle) {
        gulp.watch(bundle.inputFiles, ["min:html"]);
    });
});

function getBundles(regexPattern, minify) {
    return bundleconfig.filter(function (bundle) {
        return regexPattern.test(bundle.outputFileName);
    });
}