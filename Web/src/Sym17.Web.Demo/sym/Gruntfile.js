module.exports = function(grunt) {
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        sass: {
            options: {
                style: 'expanded'
            },
            dist: {
                files: {
                    'css/build/styles.css': 'css/scss/main.scss'
                }
            }
        },
        concat: {
            dist: {
                src: [  'js/jquery.min.js',
                'js/bootstrap.min.js',
                'js/wow.min.js',
                'js/jquery.placeholder.min.js',
                'js/smoothscroll.js',
                'js/hybrid.js',
                'js/particles.js',
                'hjs/stats.js',
                'js/typed.js',
                'js/slick-slider/slick.min.js',
                'js/slick-slider/slick.init.js',
                'js/main.js'
                ],

                dest: 'js/build.js'
            },
            css: {
                src:  [ "css/slick-slider/slick-theme.css",
                    "css/slick-slider/slick.css",
                    "css/animate.css",
                    "css/hybrid.css",
                    "css/build/styles.css"],
                dest: 'css/main.css'
            }
        },
        uglify: {
            options: {
                stripBanners: false
             },
            build: {
                src: 'js/build.js',
                dest: 'js/build.min.js'
            }
        },
        cssmin: {
            files: {
                 options: {
                        banner: '/* My minified CSS */'
                    },

                    files: {
                        'css/main.min.css' : ["css/bootstrap.min.css", "css/main.css"]
                    }
            }
        }
    });

    grunt.loadNpmTasks('grunt-contrib-sass');
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-contrib-cssmin');



    grunt.registerTask('default', ['sass', 'concat', 'uglify','cssmin']);

};