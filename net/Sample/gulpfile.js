/// <binding BeforeBuild='copy-js' />

var gulp = require('gulp');

gulp.task('copy-js', function() {
    gulp.src("../../js/*.js").pipe(gulp.dest("wwwroot/lib"));
});
