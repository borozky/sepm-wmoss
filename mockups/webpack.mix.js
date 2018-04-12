let mix = require('laravel-mix');

mix.autoload({
    jquery: ['$','jQuery', 'window.jQuery']
})
.react("src/js/app.js", "public/js")
.sass("src/scss/app.scss", "public/css")
.setPublicPath('public')
.sourceMaps();