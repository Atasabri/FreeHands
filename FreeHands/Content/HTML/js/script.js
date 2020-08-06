

jQuery(document).ready(function($) {  

  /* ///////////////////////////////////////////////////////////////////// 
  // 1 - Preloader
  /////////////////////////////////////////////////////////////////////*/
  
  $(window).load(function(){
    $('#preloader').fadeOut('slow',function(){$(this).remove();});
  });



  /* ///////////////////////////////////////////////////////////////////// 
  // 2 - General
  /////////////////////////////////////////////////////////////////////*/

  //woo effect
  new WOW().init();
  
  //tooltip
  $('#back-to-top').tooltip();
  $('#menu-toggle').tooltip();
  


  // Set Sticky Header
  var $sticky = $('.sticky');
  $(window).scroll(function(){
    var scroll = $(window).scrollTop();
    if (scroll >= 100) $sticky.addClass('fixed');
    else $sticky.removeClass('fixed');
  });



  //back to top
  var $backToTop = $('#back-to-top').fadeIn();

  $(window).scroll(function () {
      if ($(this).scrollTop() > 600) {
          $backToTop.fadeIn();
      } else {
          $backToTop.fadeOut();
      }
  });
  // scroll body to 0px on click
  $backToTop.click(function () {
      $backToTop.tooltip('hide');
      $('body,html').animate({
          scrollTop: 0
      }, 800);
      return false;
  });


  //Testimonial
  $("#review").owlCarousel({
    slideSpeed : 300,
    paginationSpeed : 400,
    singleItem:true,
  });


  //Async Video iframe loading
  setTimeout(function(){ 
    $('.async-iframe').each(function(){
      $(this).attr('src', $(this).data('src'));
    });
  }, 1000);




  /* ///////////////////////////////////////////////////////////////////// 
  // 3 - Sidebar menu Controls
  /////////////////////////////////////////////////////////////////////*/

  $("#menu-close").click(function(e) {
      e.preventDefault();
      $("#sidebar-wrapper").toggleClass("active");
  });

  // Opens the sidebar menu
  $("#menu-toggle").click(function(e) {
      e.preventDefault();
      $("#sidebar-wrapper").toggleClass("active");
  });




  /* ///////////////////////////////////////////////////////////////////// 
  // 4 - Scrolls to the selected menu item on the page
  /////////////////////////////////////////////////////////////////////*/

  $(function() {
      $('a[href*=#]:not([href=#])').click(function() {
          if (location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') || location.hostname == this.hostname) {

              var target = $(this.hash);
              target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
              if (target.length) {
                  $('html,body').stop().animate({
                      scrollTop: target.offset().top - 75
                  }, 1000);
                  return false;
              }
          }
      });
  });




  /* ///////////////////////////////////////////////////////////////////// 
  // 5 - Carousel
  /////////////////////////////////////////////////////////////////////*/

  $('#screenshots > a').nivoLightbox({effect: 'fadeScale'});

  var owl = $("#screenshots");
 
  owl.owlCarousel({
    autoPlay: false,
    pagination: false,
    stopOnHover: true,
  });
 

  // Custom Navigation Events
  $(".next").click(function(){
    owl.trigger('owl.next');
  })
  $(".prev").click(function(){
    owl.trigger('owl.prev');
  })



  /* ///////////////////////////////////////////////////////////////////// 
  // 5 - Mailchimp
  /////////////////////////////////////////////////////////////////////*/

  $('.mailchimp').ajaxChimp({
      callback: mailchimpCallback,
      url: "http://technextit.us3.list-manage.com/subscribe/post?u=9e945cce9a6497869d2d3cd79&amp;id=a3aff233b6", //Replace this with your own mailchimp post URL. Don't remove the "". Just paste the url inside "" You can find your POST URL from http://kb.mailchimp.com/lists/signup-forms/host-your-own-signup-forms.  
  });

  function mailchimpCallback(resp) {
       if (resp.result === 'success') {
          $('.subscription-success').html('<i class="icon_check_alt2"></i><br/>' + resp.msg).fadeIn(1000);
          $('.subscription-error').fadeOut(500);
          
      } else if(resp.result === 'error') {
          $('.subscription-error').html('<i class="icon_close_alt2"></i><br/>' + resp.msg).fadeIn(1000);
      }  
  }




  /* ///////////////////////////////////////////////////////////////////// 
  // 5 - Contact Form
  /////////////////////////////////////////////////////////////////////*/
 
}); //end document ready
