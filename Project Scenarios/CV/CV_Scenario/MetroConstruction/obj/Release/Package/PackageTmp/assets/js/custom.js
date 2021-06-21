/* Menu */

/*
 * jQuery canvasMenu Menus Plugin v0.1.5 (2013-09-07)
 * https://github.com/tefra/canvasMenu
 *
 * Copyright (c) 2013 Chris T (@tefra)
 * BSD - https://github.com/tefra/canvasMenu/blob/master/LICENSE-BSD
 */
/* (function($) {

	"use strict";
        


	/**
	 * Plugin Constructor. Every menu must have a unique id which will either
	 * be the actual id attribute or its index in the page.
	 *
	 * @param {Element} el
	 * @param {Object} options
	 * @param {Integer} idx
	 * @returns {Object} Plugin Instance
	 */
	/* var Plugin = function(el, options, idx) {
		this.el = el;
		this.$el = $(el);
		this.options = options;
		this.uuid = this.$el.attr('id') ? this.$el.attr('id') : idx;
		this.state = {};
		this.init();
		return this;
	};

	/**
	 * Plugin methods
	 */
	/* Plugin.prototype = {
		/**
		 * Load cookie, assign a unique data-index attribute to
		 * all sub-menus and show|hide them according to cookie
		 * or based on the parent open class. Find all parent li > a
		 * links add the carent if it's on and attach the event click
		 * to them.
		 
		init: function() {
			var self = this;
			self._load();
			self.$el.find('ul').each(function(idx) {
				var sub = $(this);
				sub.attr('data-index', idx);
				if (self.options.save && self.state.hasOwnProperty(idx)) {
					sub.parent().addClass(self.options.openClass);
					sub.show();
				} else if (sub.parent().hasClass(self.options.openClass)) {
					sub.show();
					self.state[idx] = 1;
				} else {
					sub.hide();
				}
			});

			if (self.options.caret) {
				self.$el.find("li:has(ul) > a").append(self.options.caret);
			}

			var links = self.$el.find("li > a");
			links.on('click', function(event) {
				event.stopPropagation();
				var sub = $(this).next();
				sub = sub.length > 0 ? sub : false;
				self.options.onClickBefore.call(this, event, sub);
				if (sub) {
					event.preventDefault();
					self._toggle(sub, sub.is(":hidden"));
					self._save();
				} else {
					if (self.options.accordion) {
						var allowed = self.state = self._parents($(this));
						self.$el.find('ul').filter(':visible').each(function() {
							var sub = $(this),
								idx = sub.attr('data-index');

							if (!allowed.hasOwnProperty(idx)) {
								self._toggle(sub, false);
							}
						});
						self._save();
					}
				}
				self.options.onClickAfter.call(this, event, sub);
			});
		},
		/**
		 * Accepts a JQuery Element and a boolean flag. If flag is false it removes the `open` css
		 * class from the parent li and slides up the sub-menu. If flag is open it adds the `open`
		 * css class to the parent li and slides down the menu. If accordion mode is on all
		 * sub-menus except the direct parent tree will close. Internally an object with the menus
		 * states is maintained for later save duty.
		 *
		 * @param {Element} sub
		 * @param {Boolean} open
		 
		_toggle: function(sub, open) {
			var self = this,
				idx = sub.attr('data-index'),
				parent = sub.parent();

			self.options.onToggleBefore.call(this, sub, open);
			if (open) {
				parent.addClass(self.options.openClass);
				sub.slideDown(self.options.slide);
				self.state[idx] = 1;

				if (self.options.accordion) {
					var allowed = self.state = self._parents(sub);
					allowed[idx] = self.state[idx] = 1;

					self.$el.find('ul').filter(':visible').each(function() {
						var sub = $(this),
							idx = sub.attr('data-index');

						if (!allowed.hasOwnProperty(idx)) {
							self._toggle(sub, false);
						}
					});
				}
			} else {
				parent.removeClass(self.options.openClass);
				sub.slideUp(self.options.slide);
				self.state[idx] = 0;
			}
			self.options.onToggleAfter.call(this, sub, open);
		},
		/**
		 * Returns all parents of a sub-menu. When obj is true It returns an object with indexes for
		 * keys and the elements as values, if obj is false the object is filled with the value `1`.
		 *
		 * @since v0.1.2
		 * @param {Element} sub
		 * @param {Boolean} obj
		 * @returns {Object}
		 
		_parents: function(sub, obj) {
			var result = {},
				parent = sub.parent(),
				parents = parent.parents('ul');

			parents.each(function() {
				var par = $(this),
					idx = par.attr('data-index');

				if (!idx) {
					return false;
				}
				result[idx] = obj ? par : 1;
			});
			return result;
		},
		/**
		 * If `save` option is on the internal object that keeps track of the sub-menus states is
		 * saved with a cookie. For size reasons only the open sub-menus indexes are stored.		 *
		 
		_save: function() {
			if (this.options.save) {
				var save = {};
				for (var key in this.state) {
					if (this.state[key] === 1) {
						save[key] = 1;
					}
				}
				cookie[this.uuid] = this.state = save;
				$.cookie(this.options.cookie.name, JSON.stringify(cookie), this.options.cookie);
			}
		},
		/**
		 * If `save` option is on it reads the cookie data. The cookie contains data for all
		 * canvasMenu menus so the read happens only once and stored in the global `cookie` var.
		 
		_load: function() {
			if (this.options.save) {
				if (cookie === null) {
					var data = $.cookie(this.options.cookie.name);
					cookie = (data) ? JSON.parse(data) : {};
				}
				this.state = cookie.hasOwnProperty(this.uuid) ? cookie[this.uuid] : {};
			}
		},
		/**
		 * Public method toggle to manually show|hide sub-menus. If no indexes are provided all
		 * items will be toggled. You can pass sub-menus indexes as regular params. eg:
		 * canvasMenu('toggle', true, 1, 2, 3, 4, 5);
		 *
		 * Since v0.1.2 it will also open parents when providing sub-menu indexes.
		 *
		 * @param {Boolean} open
		 
		toggle: function(open) {
			var self = this,
				length = arguments.length;

			if (length <= 1) {
				self.$el.find('ul').each(function() {
					var sub = $(this);
					self._toggle(sub, open);
				});
			} else {
				var idx,
					list = {},
					args = Array.prototype.slice.call(arguments, 1);
				length--;

				for (var i = 0; i < length; i++) {
					idx = args[i];
					var sub = self.$el.find('ul[data-index="' + idx + '"]').first();
					if (sub) {
						list[idx] = sub;
						if (open) {
							var parents = self._parents(sub, true);
							for (var pIdx in parents) {
								if (!list.hasOwnProperty(pIdx)) {
									list[pIdx] = parents[pIdx];
								}
							}
						}
					}
				}

				for (idx in list) {
					self._toggle(list[idx], open);
				}
			}
			self._save();
		},
		/**
		 * Removes instance from JQuery data cache and unbinds events.
		 
		destroy: function() {
			$.removeData(this.$el);
			this.$el.find("li:has(ul) > a").unbind('click');
		}
	};

	/**
	 * A JQuery plugin wrapper for canvasMenu. It prevents from multiple instances and also handles
	 * public methods calls. If we attempt to call a public method on an element that doesn't have
	 * a canvasMenu instance, one will be created for it with the default options.
	 *
	 * @param {Object|String} options
	 
	$.fn.canvasMenu = function(options) {
		if (typeof options === 'string' && options.charAt(0) !== '_' && options !== 'init') {
			var callback = true,
				args = Array.prototype.slice.call(arguments, 1);
		} else {
			options = $.extend({}, $.fn.canvasMenu.defaults, options || {});
			if (!$.cookie) {
				options.save = false;
			}
		}
		return this.each(function(idx) {
			var $this = $(this),
				obj = $this.data('canvasMenu');

			if (!obj) {
				obj = new Plugin(this, callback ? $.fn.canvasMenu.defaults : options, idx);
				$this.data('canvasMenu', obj);
			}
			if (callback) {
				obj[options].apply(obj, args);
			}
		});
	};
	/**
	 * Global var holding all canvasMenu menus open states
	 *
	 * @type {Object}
	 
	var cookie = null;

	/**
	 * Default canvasMenu options
	 *
	 * @type {Object}
	 
	$.fn.canvasMenu.defaults = {
		caret: '<span class="sub-icon"></span>',
		accordion: false,
		openClass: 'open',
		save: true,
		cookie: {
			name: 'canvasMenu',
			expires: false,
			path: '/'
		},
		slide: {
			duration: 400,
			easing: 'swing'
		},
		onClickBefore: $.noop,
		onClickAfter: $.noop,
		onToggleBefore: $.noop,
		onToggleAfter: $.noop
	};
})(jQuery);

*/

$(document).ready(function () {

	//Navigation Menu Slider
	$('.nav-expander').on('click', function (e) {
		e.preventDefault();
		$('body').toggleClass('nav-expanded');
	});
	$('.closebtn').on('click', function (e) {
		e.preventDefault();
		$('body').removeClass('nav-expanded');
	});


	// Initialize canvasMenu with default options
	/* $(".main-menu").canvasMenu({
		caret: '<span class="sub-icon"></span>',
		accordion: false,
		openClass: 'open',
		save: true,
		cookie: {
			name: 'canvasMenu',
			expires: false,
			path: '/'
		},
		slide: {
			duration: 300,
			easing: 'swing'
		}
	}); */


});



/* scroll-to-top */

$('.return-to-top').click(function() {      // When arrow is clicked
    $('body,html').animate({
        scrollTop : 0                       // Scroll to top of body
    }, 500);
});


/* Products Starts */


	// box 1
	$('.top-wrapper-blk.pdt-cat-blk .banner-contents a').mouseover(function () {
		$('.top-wrapper-blk.pdt-cat-blk .pdt-brand-blk').css('opacity', '0');
	});
	$('.top-wrapper-blk.pdt-cat-blk .banner-contents a').mouseout(function () {
		$('.top-wrapper-blk.pdt-cat-blk .pdt-brand-blk').css('opacity', '1');
	});
	// nbox 1
	$('.doccure-link').mouseover(function () {
		$('.doccure-blk').css('opacity', '1');
	});
	$('.doccure-link').mouseout(function () {
		$('.doccure-blk').css('opacity', '0');
	});
	// nbox 2
	$('.hms-link').mouseover(function () {
		$('.hms-blk').css('opacity', '1');
	});
	$('.hms-link').mouseout(function () {
		$('.hms-blk').css('opacity', '0');
	});
	// nbox 3
	$('.evirtual-link').mouseover(function () {
		$('.evirtual-blk').css('opacity', '1');
	});
	$('.evirtual-link').mouseout(function () {
		$('.evirtual-blk').css('opacity', '0');
	});
	
	// box 1
	$('.mentoring-link').mouseover(function () {
		$('.mentori-blk').css('opacity', '1');
	});
	$('.mentoring-link').mouseout(function () {
		$('.mentori-blk').css('opacity', '0');
	});

	// box 1
	$('.laundry-link').mouseover(function () {
		$('.laundry-blk').css('opacity', '1');
	});
	$('.laundry-link').mouseout(function () {
		$('.laundry-blk').css('opacity', '0');
	});

	// box 1
	$('.hrms-link').mouseover(function () {
		$('.hrms-blk').css('opacity', '1');
	});
	$('.hrms-link').mouseout(function () {
		$('.hrms-blk').css('opacity', '0');
	});

	// box 1
	$('.chat-link').mouseover(function () {
		$('.chat-blk').css('opacity', '1');
	});
	$('.chat-link').mouseout(function () {
		$('.chat-blk').css('opacity', '0');
	});

	// box 1
	$('.gigs-link').mouseover(function () {
		$('.gigs-blk').css('opacity', '1');
	});
	$('.gigs-link').mouseout(function () {
		$('.gigs-blk').css('opacity', '0');
	});


	// box 1
	$('.fnd-link').mouseover(function () {
		$('.fnd-blk').css('opacity', '1');
	});
	$('.fnd-link').mouseout(function () {
		$('.fnd-blk').css('opacity', '0');
	});

	// box 1
	$('.servpro-link').mouseover(function () {
		$('.servpro-blk').css('opacity', '1');
	});
	$('.servpro-link').mouseout(function () {
		$('.servpro-blk').css('opacity', '0');
	});

	// box 1
	$('.survey-link').mouseover(function () {
		$('.survey-blk').css('opacity', '1');
	});
	$('.survey-link').mouseout(function () {
		$('.survey-blk').css('opacity', '0');
	});

	// box 1
	$('.dreamkids-link').mouseover(function () {
		$('.dreamkids-blk').css('opacity', '1');
	});
	$('.dreamkids-link').mouseout(function () {
		$('.dreamkids-blk').css('opacity', '0');
	});
										
/* Products ends */

// Testimonials Rotation
  var owl = $('.testimonials .owl-carousel');
                                owl.owlCarousel({
                                    items: 1,
                                    loop: true,
                                    margin: 10,
                                    autoplay: true,
                                    autoplayTimeout: 3000,
                                    autoplayHoverPause: true
                                }); 
								
jQuery(function($) {
 var path = window.location.href; // because the 'href' property of the DOM element is the absolute path
 $('ul.sub-nav-block a, ul.main-menu a').each(function() {
  if (this.href === path) {
   $(this).addClass('active');
  }
 });
});

              $(".carousel").on("touchstart", function(event){
        var xClick = event.originalEvent.touches[0].pageX;
    $(this).one("touchmove", function(event){
        var xMove = event.originalEvent.touches[0].pageX;
        if( Math.floor(xClick - xMove) > 5 ){
            $(this).carousel('next');
        }
        else if( Math.floor(xClick - xMove) < -5 ){
            $(this).carousel('prev');
        }
    });
    $(".carousel").on("touchend", function(){
            $(this).off("touchmove");
    });
});


function openNav() {
    document.getElementById("mySidenav").style.width = "100%";
}

function closeNav() {
    document.getElementById("mySidenav").style.width = "0";
}


/* Products starts */

if ($(window).width() < 768) {

$.fn.reverseChildren = function () {
    return this.each(function () {
        var $this = $(this);
        $this.children().each(function () {
            $this.prepend(this)
        });
    });
};
$('.reverse-div').reverseChildren();


}

/* hire a developer starts */

            $('.reverse-align .services-slider').slick({
                autoplay: false,
                prevArrow: $('.prev-rev'),
                nextArrow: $('.next-rev'),
                dots: false,
                infinite: true,
                autoPlay: false,
                slidesToShow: 2,
                rows: 1,
                centerMode: false,
                cssEase: 'linear',
                variableWidth: false,
                variableHeight: false,
                autoplaySpeed: 3000,
                slidesToScroll: 2,
                responsive: [
                    {
                        breakpoint: 1024,
                        settings: {
                            slidesToShow: 2,
                            slidesToScroll: 2,
                            infinite: true,
                            dots: true
                        }
                    },
                    {
                        breakpoint: 910,
                        settings: {
                            slidesToShow: 1,
                            slidesToScroll: 1
                        }
                    },
                    {
                        breakpoint: 767,
                        settings: {
                            slidesToShow: 1,
                            slidesToScroll: 1
                        }
                    }
                ]
            });
    
    
            $('.cat-first-child .services-slider').slick({
                autoplay: false,
                prevArrow: $('.prev'),
                nextArrow: $('.next'),
                dots: false,
                infinite: true,
                autoPlay: false,
                slidesToShow: 2,
                rows: 1,
                centerMode: false,
                cssEase: 'linear',
                variableWidth: false,
                variableHeight: false,
                autoplaySpeed: 3000,
                slidesToScroll: 2,
                responsive: [
                    {
                        breakpoint: 1024,
                        settings: {
                            slidesToShow: 2,
                            slidesToScroll: 2,
                            infinite: true,
                            dots: true
                        }
                    },
                    {
                        breakpoint: 910,
                        settings: {
                            slidesToShow: 1,
                            slidesToScroll: 1
                        }
                    },
                    {
                        breakpoint: 767,
                        settings: {
                            slidesToShow: 1,
                            slidesToScroll: 1
                        }
                    }
                ]
            });     
    
    
     $('.cat-last-child .services-slider').slick({
                autoplay: false,
                prevArrow: $('.cat-last-child .prev'),
                nextArrow: $('.cat-last-child .next'),
                dots: false,
                infinite: true,
                autoPlay: false,
                slidesToShow: 2,
                rows: 1,
                centerMode: false,
                cssEase: 'linear',
                variableWidth: false,
                variableHeight: false,
                autoplaySpeed: 3000,
                slidesToScroll: 2,
                responsive: [
                    {
                        breakpoint: 1024,
                        settings: {
                            slidesToShow: 2,
                            slidesToScroll: 2,
                            infinite: true,
                            dots: true
                        }
                    },
                    {
                        breakpoint: 910,
                        settings: {
                            slidesToShow: 1,
                            slidesToScroll: 1
                        }
                    },
                    {
                        breakpoint: 767,
                        settings: {
                            slidesToShow: 1,
                            slidesToScroll: 1
                        }
                    }
                ]
            });             
    
/* hire a developer ends */

 

                                    $('.pdt-blk-1').waypoint(function () {
                                        $('.pdt-detail-1, .rollout-anim-1').addClass('qodef-appeared');
                                    }, {offset: '50%'});
                                    $('.pdt-blk-1').waypoint(function () {
                                        $('.rollout-anim-1').addClass('shadow-anim');
                                    }, {offset: '50%'});
                                    $('.pdt-blk-2').waypoint(function () {
                                        $('.pdt-detail-2, .rollout-anim-2').addClass('qodef-appeared');
                                    }, {offset: '50%'});
                                    $('.pdt-blk-2').waypoint(function () {
                                        $('.rollout-anim-2').addClass('shadow-anim');
                                    }, {offset: '50%'});
                                    $('.pdt-blk-3').waypoint(function () {
                                        $('.pdt-detail-3, .rollout-anim-3').addClass('qodef-appeared');
                                    }, {offset: '50%'});
                                    $('.pdt-blk-3').waypoint(function () {
                                        $('.rollout-anim-3').addClass('shadow-anim');
                                    }, {offset: '50%'});
                                    $('.pdt-blk-4').waypoint(function () {
                                        $('.pdt-detail-4, .rollout-anim-4').addClass('qodef-appeared');
                                    }, {offset: '50%'});
                                    $('.pdt-blk-4').waypoint(function () {
                                        $('.rollout-anim-4').addClass('shadow-anim');
                                    }, {offset: '50%'});
                                    $('.pdt-blk-5').waypoint(function () {
                                        $('.pdt-detail-5, .rollout-anim-5').addClass('qodef-appeared');
                                    }, {offset: '50%'});
                                    $('.pdt-blk-5').waypoint(function () {
                                        $('.rollout-anim-5').addClass('shadow-anim');
                                    }, {offset: '50%'});

                                    (function () {
                                        // setup your carousels as you normally would using JS
                                        // or via data attributes according to the documentation
                                        // https://getbootstrap.com/javascript/#carousel
                                        $('#carouselonebyone').carousel({interval: false});
                                    }());


                                    (function () {
                                        $('.carousel-thumbcustom .item').each(function () {
                                            var itemToClone = $(this);

                                            for (var i = 1; i < 4; i++) {
                                                itemToClone = itemToClone.next();

                                                // wrap around if at end of item collection
                                                if (!itemToClone.length) {
                                                    itemToClone = $(this).siblings(':first');
                                                }

                                                // grab item, clone, add marker class, add to collection
                                                itemToClone.children(':first-child').clone()
                                                        .addClass("cloneditem-" + (i))
                                                        .appendTo($(this));
                                            }
                                        });
                                    }());

/* products ends */


/* New hrms starts */

          

$(document).ready(function(){
	$('.package').click(function(){
		var cost = $(this).attr("cost");
		$('.pricetab a.active').removeClass("active");
		$(this).addClass("active");
		$(this).closest('.pricingcol').find('.basiccntr .priceamt span.cost').html(cost);
	});

	$('.pricingcol .basiccntr .pricecurrency a').click(function() {
		var currency = $(this).attr("currency");
		$('.pricingcol .basiccntr .pricecurrency a.active').removeClass();
		$(this).addClass("active " + currency);
		$(this).parents(".basiccntr").find('.priceamt #currency-holder').removeClass();
		$(this).parents(".basiccntr").find('.priceamt #currency-holder').addClass( "fa fa-" + currency );
	});

	$('.pricetab .package:first').click();
	$('.pricingcol .basiccntr .pricecurrency a:first').click();
});

 var wow = new WOW({
                boxClass: 'animate', // animated element css class (default is wow)
                animateClass: 'animated', // animation css class (default is animated)
                offset: 100, // distance to the element when triggering the animation (default is 0)
                mobile: false        // trigger animations on mobile devices (true is default)
            });
            wow.init();


        (function ($) {

            'use strict';

            $(document).on('show.bs.tab', '.nav-tabs-responsive [data-toggle="tab"]', function (e) {
                var $target = $(e.target);
                var $tabs = $target.closest('.nav-tabs-responsive');
                var $current = $target.closest('li');
                var $parent = $current.closest('li.dropdown');
                $current = $parent.length > 0 ? $parent : $current;
                var $next = $current.next();
                var $prev = $current.prev();
                var updateDropdownMenu = function ($el, position) {
                    $el
                      .find('.dropdown-menu')
                      .removeClass('pull-xs-left pull-xs-center pull-xs-right')
                      .addClass('pull-xs-' + position);
                };

                $tabs.find('>li').removeClass('next prev');
                $prev.addClass('prev');
                $next.addClass('next');

                updateDropdownMenu($prev, 'left');
                updateDropdownMenu($current, 'center');
                updateDropdownMenu($next, 'right');
            });

        })(jQuery);

/* New hrms ends */


/* Services starts */

        $('.main.serv-cat-details .services-slider').slick({
            autoplay: false,
            prevArrow: $('.main-prev'),
            nextArrow: $('.main-next'),
            dots: false,
            infinite: true,
            autoPlay: false,
            slidesToShow: 3,
            rows: 3,
            centerMode: false,
            cssEase: 'linear',
            variableWidth: false,
            variableHeight: false,
            autoplaySpeed: 3000,
            slidesToScroll: 3,
            responsive: [
                {
                    breakpoint: 1024,
                    settings: {
                        slidesToShow: 9,
                        slidesToScroll: 9,
                        infinite: true,
                        dots: true
                    }
                },
                {
                    breakpoint: 910,
                    settings: {
                        slidesToShow: 3,
                        slidesToScroll: 3
                    }
                },
                {
                    breakpoint: 767,
                    settings: {
                        slidesToShow: 3,
                        slidesToScroll: 3
                    }
                }
            ]
        });     

        $('.rev.serv-cat-details .services-slider').slick({
            autoplay: false,
            prevArrow: $('.prev-rev'),
            nextArrow: $('.next-rev'),
            dots: false,
            infinite: true,
            autoPlay: false,
            slidesToShow: 3,
            rows: 3,
            centerMode: false,
            cssEase: 'linear',
            variableWidth: false,
            variableHeight: false,
            autoplaySpeed: 3000,
            slidesToScroll: 3,
            responsive: [
                {
                    breakpoint: 1024,
                    settings: {
                        slidesToShow: 9,
                        slidesToScroll: 9,
                        infinite: true,
                        dots: true
                    }
                },
                {
                    breakpoint: 910,
                    settings: {
                        slidesToShow: 3,
                        slidesToScroll: 3
                    }
                },
                {
                    breakpoint: 767,
                    settings: {
                        slidesToShow: 3,
                        slidesToScroll: 3
                    }
                }
            ]
        });
		
		
$('.end.serv-cat-details .services-slider').slick({
            autoplay: false,
            prevArrow: $('.prev'),
            nextArrow: $('.next'),
            dots: false,
            infinite: true,
            autoPlay: false,
            slidesToShow: 3,
            rows: 3,
            centerMode: false,
            cssEase: 'linear',
            variableWidth: false,
            variableHeight: false,
            autoplaySpeed: 3000,
            slidesToScroll: 3,
            responsive: [
                {
                    breakpoint: 1024,
                    settings: {
                        slidesToShow: 9,
                        slidesToScroll: 9,
                        infinite: true,
                        dots: true
                    }
                },
                {
                    breakpoint: 910,
                    settings: {
                        slidesToShow: 3,
                        slidesToScroll: 3
                    }
                },
                {
                    breakpoint: 767,
                    settings: {
                        slidesToShow: 3,
                        slidesToScroll: 3
                    }
                }
            ]
        });             
		

/* Services ends */


/* solutions starts */

        $('.solutions-slick').slick({
            autoplay: true,
            arrows: false,
            dots: true,
            infinite: true,
            autoPlay: true,
            slidesToShow: 3,
            autoplaySpeed: 3000,
            slidesToScroll: 3,
            responsive: [
                {
                    breakpoint: 1024,
                    settings: {
                        slidesToShow: 1,
                        slidesToScroll: 1,
                        infinite: true,
                        dots: true
                    }
                },
                {
                    breakpoint: 910,
                    settings: {
                        slidesToShow: 2,
                        slidesToScroll: 2
                    }
                },
                {
                    breakpoint: 767,
                    settings: {
                        slidesToShow: 1,
                        slidesToScroll: 1
                    }
                }
            ]
        });
		
/* solutions ends */

/* dev pricing starts */

    function owl_Carousel(){
        if ( $('.blog_slide').length ){ 
            $('.blog_carousel').owlCarousel({
                loop: true,
                margin: 20,
                nav: true,
				dots: false,
                items: 3,  
                navText: ["<i class='flaticon-back'></i>","<i class='flaticon-next'></i>"],
                responsive: {
                    0: {
                        items: 2, 
                        margin: 0,
                    },
                    767: {
                        items: 2, 
                    },
                    1000: {
                        items: 3, 
                    },
                    1199: {
                        items: 3, 
                    },
                    1299: {
                        items: 3, 
                    },
                    1700: {
                        items: 3, 
                    }
                }
            })
        };
    }; 


     /*Function Calls*/ 
    owl_Carousel ();  
    
		
/* dev pricing ends */

/* mentor.ng slider autoplay */
$('#inner-slide').carousel({
pause: false,
interval: 2300
});


var wow = new WOW({
    boxClass: 'animate', // animate element css class (default is wow)
    animateClass: 'animate', // animation css class (default is animate)
    offset: 100, // distance to the element when triggering the animation (default is 0)
    mobile: false        // trigger animations on mobile devices (true is default)
});
wow.init();



$(window).scroll(function () {
    if ($(this).scrollTop() > 100) {
        $('.navbar-affixed-top.affix').addClass("affix-position");
    } else {
        $('.navbar-affixed-top.affix').removeClass("affix-position");
    }

});




 var screenshot = $(".screenshot-carousel");
    screenshot.owlCarousel({
        loop:true,
        margin:30,
        nav:false,
        dots:false,
        center:true,
        autoplay: true,
        autoplayTimeout: 8000,
        responsive:{
            0:{
                items:2,
            },
            767:{
                items:2,
            },
            768:{
                items:3,
            },
            992:{
                items:4,
            },
            1200:{
                items:5
            }
        }
    });
