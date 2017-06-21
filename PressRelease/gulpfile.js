"use strict";

var gulp = require("gulp"),
    pump = require('pump'),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    htmlmin = require("gulp-htmlmin"),
    uglify = require("gulp-uglify"),
    babel = require("gulp-babel"),
    sourcemaps = require("gulp-sourcemaps"),
    merge = require("merge-stream"),
    del = require("del"),
    bundleconfig = require("./bundleconfig.json");

var regex = {
    css: /\.css$/,
    html: /\.(html|htm)$/,
    js: /\.js$/
};

gulp.task("build", ["bundle", "min", "fonts"]);
gulp.task("bundle", ["bundle:js", "bundle:css", "bundle:html"]);
gulp.task("min", ["min:js", "min:css", "min:html"]);

gulp.task("fonts", ["fafonts", "bsfonts"]);

gulp.task("fafonts", function (cb) {
    pump([
        gulp.src("node_modules/font-awesome/fonts/*.*", { base: "node_modules/font-awesome/" }),
        gulp.dest("wwwroot")], cb);
});

gulp.task("bsfonts", function (cb) {
    pump([
        gulp.src("node_modules/bootstrap/dist/fonts/*.*", { base: "node_modules/bootstrap/dist/" }),
        gulp.dest("wwwroot")], cb);
});

gulp.task("bundle:js", function () {
    var tasks = getBundles(regex.js).map(function (bundle) {
        if (bundle.minifyOnly) return [];
        return pump([
            gulp.src(bundle.inputFiles, { base: "." }),
            sourcemaps.init(),
            babel(),
            concat(bundle.outputFileName),
            sourcemaps.write("."),
            gulp.dest(".")]);
    });
    return merge(tasks);
});

gulp.task("bundle:css", function () {
    var tasks = getBundles(regex.css).map(function (bundle) {
        if (bundle.minifyOnly) return [];
        return pump([
            gulp.src(bundle.inputFiles, { base: "." }),
            sourcemaps.init(),
            concat(bundle.outputFileName),
            sourcemaps.write("."),
            gulp.dest(".")]);
    });
    return merge(tasks);
});

gulp.task("bundle:html", function () {
    var tasks = getBundles(regex.html).map(function (bundle) {
        if (bundle.minifyOnly) return [];
        return pump([
            gulp.src(bundle.inputFiles, { base: "." }),
            concat(bundle.outputFileName),
            gulp.dest(".")]);
    });
    return merge(tasks);
});

gulp.task("min:js", function () {
    var tasks = getBundles(regex.js).map(function (bundle) {
        return pump([
            gulp.src(bundle.inputFiles, { base: "." }),
            sourcemaps.init(),
            babel(),
            concat(makeMinified(bundle.outputFileName)),
            uglify(),
            sourcemaps.write("."),
            gulp.dest(".")]);
    });
    return merge(tasks);
});

gulp.task("min:css", function () {
    var tasks = getBundles(regex.css).map(function (bundle) {
        return pump([
            gulp.src(bundle.inputFiles, { base: "." }),
            sourcemaps.init(),
            concat(makeMinified(bundle.outputFileName)),
            cssmin(),
            sourcemaps.write("."),
            gulp.dest(".")]);
    });
    return merge(tasks);
});

gulp.task("min:html", function () {
    var tasks = getBundles(regex.html).map(function (bundle) {
        return pump([
            gulp.src(bundle.inputFiles, { base: "." }),
            concat(makeMinified(bundle.outputFileName)),
            htmlmin({ collapseWhitespace: true, minifyCSS: true, minifyJS: true }),
            sourcemaps.write("."),
            gulp.dest(".")]);
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