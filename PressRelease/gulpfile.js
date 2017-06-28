"use strict";

var gulp = require("gulp"),
    browserify = require("browserify"),
    babelify = require("babelify"),
    watchify = require("watchify"),
    gulpif = require("gulp-if"),
    plumber = require('gulp-plumber'),
    gutil = require('gulp-util'),
    source = require('vinyl-source-stream'),
    buffer = require('vinyl-buffer'),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    htmlmin = require("gulp-htmlmin"),
    uglify = require("gulp-uglify"),
    sourcemaps = require("gulp-sourcemaps"),
    merge = require("merge-stream"),
    del = require("del"),
    assign = require("lodash/assign"),
    bundleconfig = require("./bundleconfig.json");

var regex = {
    css: /\.css$/,
    html: /\.(html|htm)$/,
    js: /\.js$/
};

function browserifyShare(options) {
    options = options || {};

    let transforms = [];
    let plugins = [];
    transforms.push(
        ['babelify', { presets: ["env", "react"] }],
        ['browserify-css', { minify: options.minify, autoInject: true }]
    );

    if (options.minify) {
        transforms.push(
            ['envify', { global: true, 'NODE_ENV': 'production' }],
            [require('uglifyify'), { global: true }]
        );

        plugins.push(
            ['bundle-collapser/plugin']);
    }

    var browserifyOptions = assign({}, watchify.args, {
        entries: ['./Components/site.js'],
        debug: true,
        transform: transforms,
        plugin: plugins
    });

    var b = browserify(browserifyOptions);
    b.on('log', gutil.log);

    if (options.watch) {
        // if watch is enable, wrap this bundle inside watchify
        b = watchify(b);
        b.on('update', function () {
            bundleShare(b);
        });
    }
    return bundleShare(b, options);
}

function bundleShare(b, options) {
    return b.bundle()
        .on('error', function (err) {
            console.log(err.message);
            this.emit('end');
        })
        .pipe(plumber())
        .pipe(gulpif(options.minify, source('wwwroot/js/site.min.js'), source('wwwroot/js/site.js')))
        .pipe(buffer())
        .pipe(gulpif(options.minify, uglify({ compress: true, mangle: true })))
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(sourcemaps.write('./'))
        .pipe(gulp.dest('.'));
}

gulp.task("build", ["bundle", "min", "fonts"]);
gulp.task("bundle", ["bundle:app", "bundle:js", "bundle:css", "bundle:html"]);
gulp.task("min", ["min:app", "min:js", "min:css", "min:html"]);

gulp.task("fonts", ["fafonts", "bsfonts"]);

gulp.task("fafonts", function () {
    return gulp.src("node_modules/font-awesome/fonts/*.*", { base: "node_modules/font-awesome/" })
        .pipe(plumber())
        .pipe(gulp.dest("wwwroot"));
});

gulp.task("bsfonts", function () {
    return gulp.src("node_modules/bootstrap/dist/fonts/*.*", { base: "node_modules/bootstrap/dist/" })
        .pipe(plumber())
        .pipe(gulp.dest("wwwroot"));
});

gulp.task("bundle:app", function () {
    return browserifyShare({ minify: false });
});

gulp.task("bundle:js", function () {
    var tasks = getBundles(regex.js).map(function (bundle) {
        if (bundle.minifyOnly) return [];
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(plumber())
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
            .pipe(plumber())
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
            .pipe(plumber())
            .pipe(concat(bundle.outputFileName))
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
});

gulp.task("min:app", function () {
    return browserifyShare({ minify: true });
});

gulp.task("min:js", function () {
    var tasks = getBundles(regex.js).map(function (bundle) {
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(plumber())
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
            .pipe(plumber())
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
            .pipe(plumber())
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
    files.push("wwwroot/**/*.js");

    return del(files);
});

gulp.task("watch", function () {
    browserifyShare({ watch: true });

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