$(window).load(function(){

    $('.client-carousel').slick({
        arrows:false,
        prevArrow:'<a data-slide="prev" class="left color-two" href="#carouselWork"><i class="fa fa-angle-left"></i></a>',
        nextArrow:'<a data-slide="next" class="right color-two" href="#carouselWork"><i class="fa fa-angle-right"></i></a>',
		slide: $('.client-carousel .carousel-item'),
        dots: false,

        infinite: true,
        easing:"easeInCubic",
        autoplay:true,
        autoplaySpeed:3000,
        speed: 700,
        draggable:true,
        slidesToShow: 5,
        slidesToScroll: 1,
        centerMode: false,
        variableWidth: false,
        responsive: [{

            breakpoint: 768,
            settings: {
                arrows:false,
              autoplaySpeed:3000,
                speed: 700,
                centerPadding:0,
                slidesToShow: 1,
                centerMode: true,
                variableWidth: false,
                infinite: true
            }
        }]
    });

});