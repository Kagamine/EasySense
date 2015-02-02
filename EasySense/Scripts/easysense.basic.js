var CurrentNotification;

function ShowLoading()
{
	$("body").prepend('<div id="loading-bg"></div><div id="loading"><img src="/Images/loading.gif" /><p>请稍候</p></div>');
}

function HideLoading()
{
	$("#loading").remove();
	$("#loading-bg").remove();
}

$(document).ready(function(){
	$(".es-notification").each(function(){
		var nav = $("[data-toggle='" + $(this).attr("id")+"']");
		$(this).css("top", nav.offset().top+77);
		$(this).css("left", nav.offset().left-12);
	});
	$(".es-menu-extend").each(function () {
	    var nav = $("[data-toggle='" + $(this).attr("id") + "']");
	    $(this).css("top", nav.offset().top + 47);
	    $(this).css("left", nav.offset().left);
	});
	$(".es-block-menu a[data-toggle]").click(function () {
	    $(this).addClass("es-block-menu-active");
	});
	$("[data-toggle]").click(function () {
	    $("#" + CurrentNotification).slideUp(200);
	    $("#" + $(this).attr("data-toggle")).slideDown(200);
	    $("a[data-toggle='" + CurrentNotification + "']").removeClass("es-block-menu-active");
		CurrentNotification = $(this).attr("data-toggle");
	});
});

$(document).on('click', function (e) {
    if (CurrentNotification == null) return;
    if ($(e.target).attr("data-toggle") == CurrentNotification) return;
    if ($(e.target).attr("id") == CurrentNotification) return;
    if ($(e.target).parents('[data-toggle="'+CurrentNotification+'"]').length > 0) return;
    if ($(e.target).parents('#'+CurrentNotification).length > 0) return;
    $("#" + CurrentNotification).slideUp(200);
    $("a[data-toggle='"+CurrentNotification+"']").removeClass("es-block-menu-active");
    CurrentNotification = null;
});