let mix = require('laravel-mix');

mix
.react("wwwroot/src/js/app.js", "wwwroot/js/site.js")
.sass("wwwroot/src/scss/app.scss", "wwwroot/css/site.css")
.setPublicPath('wwwroot')
.autoload({
    jquery: ["$", "jQuery", "jquery"]
})
.sourceMaps();





