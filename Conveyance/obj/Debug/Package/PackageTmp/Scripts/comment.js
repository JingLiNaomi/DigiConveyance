$(function () {
    //$(".commentSection").hide();
    $(".showComment").hide();
});

$('.showComment').click(function () {
    $(this).nextAll("div .commentSection").show();
    $(this).hide();
    $(this).next('.hideComment').show();
});

$('.hideComment').click(function () {
    $(this).nextAll("div .commentSection").hide();
    $(this).hide();
    $(this).prev('.showComment').show();
});