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
		$(this).css("left", nav.offset().left);
	});
	$("[data-toggle]").each(function(){
		var nav = $(this);
		nav.click(function(){
			$("#" + nav.attr("data-toggle")).slideDown(200);
		});
	});
	$("[data-toggle]").click(function(){
		CurrentNotification = $(this).attr("data-toggle");
	});
});

$(document).on('click', function (e) {
	if(CurrentNotification == null) return;
    if ($(e.target).parents('[data-toggle="'+CurrentNotification+'"]').length > 0) return;
    if ($(e.target).parents('#'+CurrentNotification).length > 0) return;
    $("#"+CurrentNotification).slideUp(200);
    CurrentNotification = null;
});