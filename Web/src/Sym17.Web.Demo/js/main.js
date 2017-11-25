

$.fn.exists = function(){return this.length>0;};

var agendaTags = []
    agendaCats = ["Strategy", "Marketing", "Developer", "Partner Theatre", "Getting Started"];

 function stickyHeader () {
    if ($('.top_menu').exists()) {
        $(window).scrollTop() > 40 ? $('.top_menu').addClass('header-sticky') : $('.top_menu').removeClass('header-sticky');
    }
};

function initAgendaCarousel() {
    $('.carousel.carousel--agenda').carousel({
       interval: false
    });

    initAgendaCarouselBindings();
};

function filterAgendaByCategory($selector) {
    var category = $selector.val();

    if ($selector.is(':checked')) {
       $('[data-agenda-items]').find('[data-category]').filter('[data-category="' + category + '"]').show();
    } else {
       $('[data-agenda-items]').find('[data-category]').filter('[data-category="' + category + '"]').hide();
    }
}

function collectAgendaCats($selector) {
    var category = $selector.val();

    if ($selector.is(':checked')) {
        agendaCats.push(category);
    } else {
        $.each(agendaCats, function(index){
            if (agendaCats[index] === category) {
                agendaCats.splice(index, 1);
            }
        });
    }
}

function collectAgendaTags($selector) {
    var tag = $selector.val();

    if ($selector.is(':checked')) {
        agendaTags.push(tag);
    } else {
        $.each(agendaTags, function(index){
           if (agendaTags[index] === tag) {
               agendaTags.splice(index, 1);
           }
        });
    }
}

function filterAgendaByTags(array) {
    $('[data-agenda-items]').find('[data-tags]').hide();

    $.each(array, function(index){
        $('[data-agenda-items]').find('[data-tags]').each(function(){
            if ($.inArray(array[index], $(this).attr('data-tags').split(', ')) != -1 && $.inArray($(this).attr('data-category'), agendaCats) != -1) {
                $(this).show();
            }
        });
    });

    if (array.length == 0) {
        $('[data-agenda-items]').find('[data-tags]').hide();

        $('[data-agenda-items]').find('[data-category]').each(function(){
            if ($.inArray($(this).attr('data-category'), agendaCats) != -1) {
                $(this).show();
            }
        });

    }
}

function initAgendaCarouselBindings() {
    $('.carousel.carousel--agenda .carousel-control.left').on('click', function(e){
        e.preventDefault();
        $('.carousel.carousel--agenda').carousel('prev');
    });

    $('.carousel.carousel--agenda .carousel-control.right').on('click', function(e){
        e.preventDefault();
        $('.carousel.carousel--agenda').carousel('next');
    });
}

function carouselNormalization(obj) {
	obj.each(function(){
		var items = $(this).find('.item'), //grab all slides
			heights = [], //create empty array to store height values
			tallest; //create variable to make note of the tallest slide

		if (items.length) {
			function normalizeHeights() {
				items.each(function() { //add heights to array
					heights.push($(this).height());
				});
				tallest = Math.max.apply(null, heights); //cache largest value
				items.each(function() {
					$(this).css('min-height', tallest + 'px');
				});
			};
			normalizeHeights();
			$(window).on('resize orientationchange', function() {
				tallest = 0, heights.length = 0; //reset vars
				items.each(function() {
					$(this).css('min-height', '0'); //reset min-height
				});
				normalizeHeights(); //run it again
			});

		}
	});

}

function typedjs() {

    $(".typed").each(function(){
        var $this = $(this);
        flag=true;
        setInterval(function() {
            if( flag){
                flag=false;

                $this.typed({
                    strings: $this.attr('data-elements').split(','),
                    typeSpeed: 100, // typing speed
                    backDelay: 3000, // pause before backspacing

                    callback: function () {
                        flag=true;
                        //destroy typed plugin here. See in the api if
                        //there is a method for doing that.

                    }
                });

            }

        }, 3000);
    });
}

function fadeInUp() {
    $('h1.fadeIn').addClass('fadeInUp')
}


function setAgendaWidth() {
    var width =  document.getElementById('agenda-wrapper').scrollWidth;
    
    $('.agenda-time').width(width);
    $('.agenda-table--full').width(width);
}

$(document).ready(function () {

    stickyHeader();
    typedjs();
    fadeInUp();
	carouselNormalization($('.carousel .carousel-inner'));

    if ($('.carousel.carousel--agenda').length) {
        initAgendaCarousel();
    }

    $('[data-agenda-list] input').prop('checked', true);
    $('[data-agenda-list] input').on('change', function(e) {
        collectAgendaCats($(e.currentTarget));
        filterAgendaByTags(agendaTags);
    });

    $('[data-agenda-tags] input').prop('checked', false);
    $('[data-agenda-tags] input').on('change', function(e) {
        collectAgendaTags($(e.currentTarget));
        filterAgendaByTags(agendaTags);
    });

    $('[data-agenda-tags] .agenda-tags__item--clear a').on('click', function(e) {
        e.preventDefault();
        $('[data-agenda-tags] input').prop('checked', false);
        $('[data-agenda-tags] input').trigger('change');
    });

    if ($('.carousel.carousel--agenda').length) {
        setAgendaWidth();
    }
});

$(window).on('scroll', function () {
    stickyHeader()
});

particlesJS("particles-js", {
    "particles": {
        "number": {
            "value": 50,
            "density": {
                "enable": true,
                "value_area": 800
            }
        },
        "color": {
            "value": "#ffffff"
        },
        "shape": {
            "type": "circle",
            "stroke": {

                "width": 0,
                "color": "#000000"
            },
            "polygon": {
                "nb_sides": 5
            },
            "image": {
                "src": "img/github.svg",
                "width": 100,
                "height": 100
            }
        },
        "opacity": {
            "value": 0.5,
            "random": false,
            "anim": {
                "enable": false,
                "speed": 0.3,
                "opacity_min": 0.1,
                "sync": false
            }
        },
        "size": {
            "value": 3,
            "random": true,
            "anim": {
                "enable": false,
                "speed": 10,
                "size_min": 0.1,
                "sync": false
            }
        },
        "line_linked": {
            "enable": true,
            "distance": 150,
            "color": "#ffffff",
            "opacity": 0.4,
            "width": 1
        },
        "move": {
            "enable": true,
            "speed": 3,
            "direction": "none",
            "random": false,
            "straight": false,
            "out_mode": "out",
            "bounce": false,
            "attract": {
                "enable": false,
                "rotateX": 600,
                "rotateY": 1200
            }
        }
    },
    "interactivity": {
        "detect_on": "canvas",
        "events": {
            "onhover": {
                "enable": true,
                "mode": "grab"
            },
            "onclick": {
                "enable": true,
                "mode": "push"
            },
            "resize": true
        },
        "modes": {
            "grab": {
                "distance": 140,
                "line_linked": {
                    "opacity": 1
                }
            },
            "bubble": {
                "distance": 400,
                "size": 40,
                "duration": 2,
                "opacity": 8,
                "speed": 3
            },
            "repulse": {
                "distance": 200,
                "duration": 0.4
            },
            "push": {
                "particles_nb": 4
            },
            "remove": {
                "particles_nb": 2
            }
        }
    },
    "retina_detect": true
});
