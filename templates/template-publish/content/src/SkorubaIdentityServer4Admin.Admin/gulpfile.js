var gulp = require('gulp');
var concat = require('gulp-concat');
var uglify = require('gulp-uglify');
var sass = require('gulp-sass');
var minifyCSS = require('gulp-minify-css');
var del = require('del');

var distFolder = './wwwroot/dist/';
var jsFolder = `${distFolder}js/`;
var cssFolder = `${distFolder}css/`;

gulp.task('scripts', function () {
	gulp
		.src([
			"./node_modules/jquery/dist/jquery.js",
			"./node_modules/jquery-validation/dist/jquery.validate.js",
			"./node_modules/jquery-validation-unobtrusive/jquery.validate.unobtrusive.js",
			"./node_modules/popper.js/dist/umd/popper.js",
			"./node_modules/bootstrap/dist/js/bootstrap.js",
			"./node_modules/holderjs/holder.js",
			"./node_modules/knockout/build/output/knockout-latest.js",
			"./node_modules/toastr/toastr.js",
			"./node_modules/moment/min/moment.min.js",
			"./node_modules/tempusdominus-bootstrap-4/build/js/tempusdominus-bootstrap-4.js",
			"./Scripts/App/components/Menu.js",
			"./Scripts/App/components/Picker.es5.js",
			"./Scripts/App/components/Theme.js",
			"./Scripts/App/helpers/FormMvcHelpers.js",
			"./Scripts/App/helpers/jsontree.min.js",
			"./Scripts/App/helpers/DateTimeHelpers.js",
			"./Scripts/App/pages/ErrorsLog.es5.js"
		])
		.pipe(concat('bundle.min.js'))
		.pipe(uglify())
		.pipe(gulp.dest(jsFolder));
});

gulp.task('styles', ['sass', 'sass:min'], function () {
	gulp
		.src([
			"./node_modules/bootstrap/dist/css/bootstrap.css",
			"./node_modules/toastr/build/toastr.css",
			"./node_modules/open-iconic/font/css/open-iconic-bootstrap.css",
			"./node_modules/font-awesome/css/font-awesome.css",
			"./node_modules/tempusdominus-bootstrap-4/build/css/tempusdominus-bootstrap-4.css",
			"./Styles/controls/jsontree.css"
		])
		.pipe(minifyCSS())
		.pipe(concat('bundle.min.css'))
		.pipe(gulp.dest(cssFolder));
});

gulp.task('fonts', function () {
	return gulp.src([
		'./node_modules/font-awesome/fonts/**', './node_modules/open-iconic/font/fonts/**'])
		.pipe(gulp.dest(`${distFolder}fonts/`));
});

gulp.task('sass:min', function () {
	gulp
		.src('Styles/web.scss')
		.pipe(sass().on('error', sass.logError))
		.pipe(minifyCSS())
		.pipe(concat('web.min.css'))
		.pipe(gulp.dest(cssFolder));
});

gulp.task('sass', function () {
	gulp
		.src('Styles/web.scss')
		.pipe(sass().on('error', sass.logError))
		.pipe(gulp.dest(cssFolder));
});

gulp.task('clean', function () {
	return del(`${distFolder}**`, { force: true });
});

gulp.task('build', ['styles', 'scripts']);
