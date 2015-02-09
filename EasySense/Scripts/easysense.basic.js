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
	$("[data-content]").click(function () {
	    $("[data-toggle='" + CurrentNotification + "']").val($(this).attr("data-content"));
	    $("#" + CurrentNotification).slideUp(200);
	    CurrentNotification = null;
	});
});

$(document).on('click', function (e) {
    if (CurrentNotification == null) return;
    if ($(e.target).attr("data-toggle") == CurrentNotification) return;
    if ($(e.target).attr("id") == CurrentNotification) return;
    if ($(e.target).parents('[data-toggle="' + CurrentNotification + '"]').length > 0) return;
    if ($(e.target).parents('#' + CurrentNotification).length > 0) return;
    if ($(".xdsoft_datetimepicker").length > 0 && $(e.target).parents('.xdsoft_datetimepicker')) return;
    $("#" + CurrentNotification).slideUp(200);
    $("a[data-toggle='"+CurrentNotification+"']").removeClass("es-block-menu-active");
    CurrentNotification = null;
});

$(document).on('click', function (e) {
    if ($(".show").length <= 0) return;
    if ($(e.target).hasClass("es-dialog")) return;
    if ($(e.target).parents(".es-dialog").length > 0) return;
    if ($(e.target).parents(".es-customer").length > 0) return;
    if ($(e.target).parents(".fc-event-inner").length > 0) return;
    $(".es-dialog").removeClass("show");
});

function CloseToggle()
{
    $("#" + CurrentNotification).slideUp(200);
    $("a[data-toggle='" + CurrentNotification + "']").removeClass("es-block-menu-active");
    CurrentNotification = null;
}

$(document).keyup(function (e) {
    var code = e.keyCode ? e.keyCode : e.which;
    if (code == 27)
        CloseToggle();
});

$(".es-menu-item a").click(function () {
    ShowLoading();
});

function DayCountOfMonth(year, month) {
    var list = [null, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
    if (year % 4 == 0 && year % 100 != 0)
        list[2] = 29;
    return list[month];
}

function DayOfWeek(date)
{
    return "日一二三四五六".charAt(date.getDay());
}

function mm(theyear, weekcount) {
    var year = theyear;
    var week = weekcount;
    if (year == "" || week == "") return;

    var d = new Date(year, 0, 1);
    d.setDate(parseInt("1065432".charAt(d.getDay())) + week * 7);
    var fe = getFirstAndEnd(d);
    return fe.first.format("yyyy-MM-dd");
}

function mml(theyear, weekcount) {
    var year = theyear;
    var week = weekcount;
    if (year == "" || week == "") return;

    var d = new Date(year, 0, 1);
    d.setDate(parseInt("1065432".charAt(d.getDay())) + week * 7);
    var fe = getFirstAndEnd(d);
    return fe.end.format("yyyy-MM-dd");
}

Date.prototype.getWeek = function (flag) {
    var first = new Date(this.getFullYear(), 0, 1);
    var n = parseInt("1065432".charAt(first.getDay()));
    n = this.getTime() - first.getTime() - n * 24 * 60 * 60 * 1000;
    n = Math.ceil(n / (7 * 24 * 60 * 60 * 1000));
    return (flag == true && first.getDay() != 1) ? (n + 1) : n;
};
Date.prototype.format = function (format) {
    var o = {
        "M+": this.getMonth() + 1,   //month   
        "d+": this.getDate(),         //day   
        "h+": this.getHours(),       //hour   
        "m+": this.getMinutes(),   //minute   
        "s+": this.getSeconds(),   //second   
        "q+": Math.floor((this.getMonth() + 3) / 3),     //quarter   
        "S": this.getMilliseconds()   //millisecond   
    }
    if (/(y+)/.test(format)) format = format.replace(RegExp.$1,
        (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o) if (new RegExp("(" + k + ")").test(format))
        format = format.replace(RegExp.$1,
            RegExp.$1.length == 1 ? o[k] :
                ("00" + o[k]).substr(("" + o[k]).length));
    return format;
};

function getFirstAndEnd(d) {
    var w = d.getDay(), n = 24 * 60 * 60 * 1000;
    var first = new Date(d.getTime() - parseInt("6012345".charAt(w)) * n);
    var end = new Date(d.getTime() + parseInt("0654321".charAt(w)) * n);
    return { first: first, end: end };
}