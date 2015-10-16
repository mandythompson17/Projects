if ($('ViewBag').Title !== "Home Page") {
    $('#main-nav').addClass('nothome');
}

/*--Blog Background image--*/
if ($('ViewBag').Title == "Blog Index") {
    $('.backstretch').remove();
    $.backstretch('/assets/900.jpg');
}

/*--Filter BlogPosts--*/

if ($('#Category') == "Professional") {
    $('.blog-post').addClass('professional');
}

if ($('#Category') == "Professional") {
    $('.blog-post').addClass('professional');
}

$('#filter-posts a').click(function (e) {
    e.preventDefault();

    $('#filter-posts li').removeClass('active');
    $(this).parent('li').addClass('active');

    var category = $(this).attr('data-filter');

    $('.blog-post').each(function () {
        if ($(this).is(category)) {
            $(this).removeClass('filtered');
        }
        else {
            $(this).addClass('filtered');
        }

        $('#blogpost-container').masonry('reload');
    });

    scrollSpyRefresh();
    waypointsRefresh();
});