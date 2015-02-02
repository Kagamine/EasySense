var page = 0;
var lock = false;
var order = "asc";
var orderby = null;

$("[data-col]").click(function () {
    $("img[src='/Images/asc.png']").remove();
    $("img[src='/Images/desc.png']").remove();
    if (order == "asc") order = "desc";
    else order = "asc";
    if (orderby == null || orderby != $(this).attr("data-col"))
        orderby = $(this).attr("data-col");
    $(this).append('<img src="/Images/' + order + '.png" />');
});

$(document).ready(function () {
    $("#btnAllSearch").click(function () {
        $.getJSON("/Search", { Text: $("#txtSearchAll").val() }, function (data) {
            //console.log(data);
            var str = "";
            if ($("#SearchResult").length > 0) {
                str += "<p class='es-notification-subtitle'>项目(" + data.Projects.length + ")</p>";
                if (data.Projects.length > 0) {
                    $.each(data.Projects, function (key, value) {
                        str += "<p><a href='/Project/Show?id=" + value.ID + "'>" + value.Title + "</a></p>";
                    });
                }
                str += "<p class='es-notification-subtitle'>客户(" + data.Enterprises.length + ")</p>";
                if (data.Enterprises.length > 0) {
                    $.each(data.Enterprises, function (key, value) {
                        str += "<p><a href='/Enterprise/Show?id=" + value.ID + "'>" + value.Title + "</a></p>";
                    });
                }
                str += "<p class='es-notification-subtitle'>名录(" + data.Customers.length + ")</p>";
                if (data.Customers.length > 0) {
                    $.each(data.Customers, function (key, value) {
                        str += "<p><a href='/Project/Show?id=" + value.ID + "'>" + value.Name + "</a></p>";
                    });
                }
                str += "<p class='es-notification-subtitle'>员工(" + data.Users.length + ")</p>";
                if (data.Users.length > 0) {
                    $.each(data.Users, function (key, value) {
                        str += "<p><img src='/Images/avatar.png' /><a href='/User/Show?id=" + value.ID + "'>" + value.Name + "</a></p>";
                    });
                }
                $("#SearchResult").html(str);
            }
        });
    });

    $("#btnEnterpriseSearch").click(function () {
        $.getJSON("/Enterprise/Search", { Text: $("#txtEnterpriseSearch").val() }, function (data) {
            var astr = "";
            var bstr = "";
            var cstr = "";
            var dstr = "";
            $.each(data, function (key, value) {
                if (value.Level == "A") {
                    astr+="<div class='es-enterprise-item'><a href='/Enterprise/Show?id="+value.ID+"' class='title'>"+value.Title+"</a> <a href='/Enterprise/Delete?id="+value.ID+"'>删除</a> <a href='/Enterprise/Edit?id="+value.ID+"'>编辑</a></div>";
                }
                if (value.Level == "B") {
                    bstr += "<div class='es-enterprise-item'><a href='/Enterprise/Show?id=" + value.ID + "' class='title'>" + value.Title + "</a> <a href='/Enterprise/Delete?id=" + value.ID + "'>删除</a> <a href='/Enterprise/Edit?id=" + value.ID + "'>编辑</a></div>";
                }
                if (value.Level == "C") {
                    cstr += "<div class='es-enterprise-item'><a href='/Enterprise/Show?id=" + value.ID + "' class='title'>" + value.Title + "</a> <a href='/Enterprise/Delete?id=" + value.ID + "'>删除</a> <a href='/Enterprise/Edit?id=" + value.ID + "'>编辑</a></div>";
                }
                if (value.Level == "D") {
                    dstr += "<div class='es-enterprise-item'><a href='/Enterprise/Show?id=" + value.ID + "' class='title'>" + value.Title + "</a> <a href='/Enterprise/Delete?id=" + value.ID + "'>删除</a> <a href='/Enterprise/Edit?id=" + value.ID + "'>编辑</a></div>";
                }
            });
            $("#aenterprise").html(astr);
            $("#benterprise").html(bstr);
            $("#centerprise").html(cstr);
            $("#denterprise").html(dstr);
        });
    });
});