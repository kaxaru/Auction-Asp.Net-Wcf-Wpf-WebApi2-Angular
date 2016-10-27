(function ($) {
    $("#lang").change(function () {
        window.location = $(this).val();
    });
    //fix lang dropdown


    //fast fix
    setInterval(function () {
        if ($(window).height() > $('.body-content').height() + $('footer').height() + 100)
            $('footer').addClass('footer_bottom')
        else
            $('footer').removeClass('footer_bottom')
    }, 1000);


    setTimeout(function () {
        $(".locale").parent().find("i.icon").removeClass('dropdown').addClass('world');
    }, 100);
})($)