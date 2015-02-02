var page = 0;
var lock = false;
var order = "asc";
var orderby = null;

$("[data-col]").click(function () {
    $("img[src='/Images/asc.png']").remove();
    $("img[src='/Images/desc.png']").remove();
    if(order=="asc")order="desc";
    else order = "asc";
    if (orderby == null || orderby != $(this).attr("data-col"))
        orderby = $(this).attr("data-col");
    $(this).append('<img src="/Images/' + order + '.png" />');
});