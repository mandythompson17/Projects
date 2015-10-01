$.backstretch('assets/header-bg.jpg');

$('body').append('<img class="preload-image" src="assets/contact-bg.jpg" style="display:none;"/>');

$('#about').waypoint(function(direction){

	if($('.preload-image').length){$('.preload-image').remove();}
	
	$('.backstretch').remove();

	if (direction=='down'){
		$.backstretch('assets/contact-bg.jpg');
	}else{
		$.backstretch('assets/header-bg.jpg');
	}
});

