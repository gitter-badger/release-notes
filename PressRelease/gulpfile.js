"use strict";

var gulp = require("gulp"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    htmlmin = require("gulp-htmlmin"),
    uglify = require("gulp-uglify"),
    sourcemaps = require("gulp-sourcemaps"),
    merge = require("merge-stream"),
    del = require("del"),
    bundleconfig = require("./bundleconfig.json");

var regex = {
    css: /\.css$/,
    html: /\.(html|htm)$/,
    js: /\.js$/
};

gulp.task("build", ["bundle", "min"]);
gulp.task("bundle", ["bundle:js", "bundle:css", "bundle:html"]);
gulp.task("min", ["min:js", "min:css", "min:html"]);

gulp.task("bundle:js", function () {
    var tasks = getBundles(regex.js).map(function (bundle) {
        if (bundle.minifyOnly) return [];
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(sourcemaps.init())
            .pipe(concat(bundle.outputFileName))
            .pipe(sourcemaps.write("."))
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
});

gulp.task("bundle:css", function () {
    var tasks = getBundles(regex.css).map(function (bundle) {
        if (bundle.minifyOnly) return [];
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(sourcemaps.init())
            .pipe(concat(bundle.outputFileName))
            .pipe(sourcemaps.write("."))
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
});

gulp.task("bundle:html", function () {
    var tasks = getBundles(regex.html).map(function (bundle) {
        if (bundle.minifyOnly) return [];
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(concat(bundle.outputFileName))
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
});

gulp.task("min:js", function () {
    var tasks = getBundles(regex.js).map(function (bundle) {
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(sourcemaps.init())
            .pipe(concat(makeMinified(bundle.outputFileName)))
            .pipe(uglify())
            .pipe(sourcemaps.write("."))
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
});

gulp.task("min:css", function () {
    var tasks = getBundles(regex.css).map(function (bundle) {
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(sourcemaps.init())
            .pipe(concat(makeMinified(bundle.outputFileName)))
            .pipe(cssmin())
            .pipe(sourcemaps.write("."))
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
});

gulp.task("min:html", function () {
    var tasks = getBundles(regex.html).map(function (bundle) {
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(concat(makeMinified(bundle.outputFileName)))
            .pipe(htmlmin({ collapseWhitespace: true, minifyCSS: true, minifyJS: true }))
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
});

gulp.task("clean", function () {
    var files = bundleconfig.map(function (bundle) {
        return bundle.outputFileName;
    });
    files.push("wwwroot/**/*.map");
    files.push("wwwroot/**/*.min.*");

    return del(files);
});

gulp.task("watch", function () {
    getBundles(regex.js).forEach(function (bundle) {
        gulp.watch(bundle.inputFiles, ["bundle:js", "min:js"]);
    });

    getBundles(regex.css).forEach(function (bundle) {
        gulp.watch(bundle.inputFiles, ["bundle:css", "min:css"]);
    });

    getBundles(regex.html).forEach(function (bundle) {
        gulp.watch(bundle.inputFiles, ["bundle:html", "min:html"]);
    });
});

function getBundles(regexPattern, minify) {
    return bundleconfig.filter(function (bundle) {
        bundle.minifyOnly = bundle.outputFileName.indexOf(".min.") >= 0;
        return regexPattern.test(bundle.outputFileName);
    });
}

function makeMinified(path) {
    var minLoc = path.lastIndexOf(".min.");
    if (minLoc >= 0) return path;

    var extLoc = path.lastIndexOf(".");
    return path.slice(0, extLoc) + ".min" + path.slice(extLoc);
}