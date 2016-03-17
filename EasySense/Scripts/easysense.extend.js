var page = 0;
var lock = false;
var order = "asc";
var orderby = null;

function showOrHide(id) {
    var div = $("#" + id);
    if (div) {
        var hided = div.is(":hidden");
        if (hided) {
            div.show();
        } else {
            div.hide();
        }
    }
}

function notNull(v) {
    if (v) {
        return v;
    } else {
        return '';
    }
}

//
function __closeDialog() {
    try {
        var _CurrentNotification = 'CustomerSelect';
        $("#" + _CurrentNotification).slideUp(200);
        $("a[data-toggle='" + _CurrentNotification + "']").removeClass("es-block-menu-active");
    } catch (e) { }
}

function searchCustomers(enterpriseID, idOfDiv) {
    document.getElementById(idOfDiv).innerHTML = '<small>正在搜索联系人......</small>';
    $.getJSON("/Enterprise/SearchCustomer", {
        EnterpriseID: enterpriseID
    }, function (data) {
        var rows = new Array();
        for (var i = 0; i < data.length; i++) {
            var record = data[i];
            //
            var row = '<a href="javascript:;" onclick="$(\'#CustomerName\').val(\'' + notNull(record.Name) + '\'); $(\'#txtTel\').val(\'' + notNull(record.Tel) + '\'); $(\'#txtPhone\').val(\'' + notNull(record.Phone) + '\'); $(\'#txtEmail\').val(\'' + notNull(record.OfficeEmail) + '\'); $(\'#txtBrand\').val(\'' + notNull(record.DepartmentName) + '\'); __closeDialog();">' + notNull(record.Name) + '</a>';
            //
            rows.push(row);
        }
        document.getElementById(idOfDiv).innerHTML = rows.join('<hr />\n');
    });
}

$("[data-col]").click(function () {
    page = 0;
    $("img[src='/Images/asc.png']").remove();
    $("img[src='/Images/desc.png']").remove();
    if (order == "asc") order = "desc";
    else order = "asc";
    if (orderby == null || orderby != $(this).attr("data-col"))
        orderby = $(this).attr("data-col");
    $(this).append('<img src="/Images/' + order + '.png" />');

    if ($("#lstProjects").length > 0) {
        $("#lstProjects").html("");
        Load();
    }
    if ($("#lstCustomers").length > 0) {
        $("#lstCustomers").html("");
        lock = false;
        LoadCustomer($("#lstCustomers").attr('enterpriseID'));
    }
});

function Load(targetPageNo)
{
    if (lock) return;
    lock = true;    
    LoadProjects(targetPageNo);
}

function LoadBills(ProjectID) {
    if ($("#lstBills").length <= 0) return;
    ShowLoading();
    $.getJSON("/Project/SearchBills", {
        ProjectID: ProjectID,
        Type: $("#selectType").val(),
        Begin: $("#txtBegin").val(),
        End: $("#txtEnd").val()
    }, function (data) {
        var trs = new Array();
        for (var i = 0; i < data.length; i++) {
            var record = data[i];
            //
            var id = record.ID;
            var checkboxTd = '<td style="text-align:center;"><input type="checkbox" name="__checkItem" idOfBill="' + record.ID + '" onchange="check_item()"/></td>';
            var idTd = '<td style="text-align:center;">' + record.RefNum + '</td>';
            var tr = "<tr id='" + id + "'>" + checkboxTd + idTd + "<td>" + id + "  " + record.Title + "</td><td>" + (record.Charge ? record.Charge : "N/A") + "</td><td ondblclick='editType(this, \"" + id + "\")'>" + record.Type + "</td><td ondblclick='editHint(this, \"" + id + "\")'>" + notNull(record.Hint) + "</td><td ondblclick='editPlan(this, \"" + id + "\")'>￥" + record.Plan + "</td><td ondblclick='editActual(this, \"" + id + "\")'>￥" + record.Actual + "</td><td>" + record.Time + "</td><td><a href='javascript:EditBill(\"" + id + "\")'>编辑</a> <a href='javascript:DeleteBill(\"" + id + "\")'>删除</a></td></tr>";
            //
            trs.push(tr);
        }
        document.getElementById("lstBills").innerHTML = trs.join('\n');
        check_item();

        HideLoading();
    });
}

function LoadCustomer(enterpriseID)
{
    if (lock) return;
    lock = true;

    if ($("#lstCustomers").length <= 0) return;
    ShowLoading();
    $.getJSON("/Enterprise/SearchCustomer", {
        Order: order,
        OrderBy: orderby,
        EnterpriseID: enterpriseID
    }, function (data) {
        //
        var trs = new Array();
        for (var i = 0; i < data.length; i++) {
            var record = data[i];
            //
            var tr = '<tr ondblclick="EditCustomer(' + record.ID + ')">';
            tr += '<td>' + notNull(record.DepartmentName) + '</td>';
            tr += '<td>' + notNull(record.ProductCategory) + '</td>';
            tr += '<td>' + notNull(record.ProductName) + '</td>';
            tr += '<td>' + notNull(record.Name) + '</td>';
            tr += '<td>' + notNull(record.Sex) + '</td>';
            tr += '<td>' + notNull(record.Position) + '</td>';
            tr += '<td>' + notNull(record.Tel) + '</td>';
            tr += '<td>' + notNull(record.Fax) + '</td>';
            tr += '<td>' + notNull(record.Phone) + '</td>';
            tr += '<td>' + notNull(record.OfficeEmail) + '</td>';
            tr += '<td>' + notNull(record.Email) + '</td>';
            tr += '<td>' + notNull(record.QQ) + '</td>';
            tr += '<td>' + notNull(record.Wechat) + '</td>';
            tr += '<td>' + notNull(record.Birthday) + '</td>';
            tr += '<td>' + notNull(record.Hint) + '</td>';
            //
            var Projects = record.Projects;
            if (Projects == null || Projects == '' || Projects.length == 0) {
                tr += '<td><p>无</p></td>';
            } else {
                tr += '<td>';
                for (var k = 0; k < Projects.length; k++) {
                    var project = Projects[k];
                    tr += '<p><a target="_blank" href="/Project/Show/' + project.ID + '" onclick="setTimeout(function () { $(\'.es-dialog\').removeClass(\'show\') }, 1000)">' + notNull(project.Title) + '</a></p>';
                }
                tr += '</td>';
            }
            //
            tr += '<td><a href="javascript:EditCustomer(' + record.ID + ')">编辑</a></td>';
            tr += '</tr>';
            //
            trs.push(tr);
        }
        document.getElementById("lstCustomers").innerHTML = trs.join('\n');
        //
        lock = false;
        HideLoading();
    });
}

function LoadProjects(targetPageNo)
{
    if ($("#lstProjects").length <= 0) return;
    ShowLoading();
    $.getJSON("/Project/Search", {
        Page: (targetPageNo != null)?targetPageNo:page,
        Order: order,
        OrderBy: orderby,
        Title: $("#txtTitle").val(),
        Status: $("#lstStatus").val(),
        Begin: $("#txtBegin").val(),
        End: $("#txtEnd").val(),
        InvoiceBegin: $("#txtInvoiceBegin").val(),
        InvoiceEnd: $("#txtInvoiceEnd").val(),
        EnterpriseID: $("#lstEnterpriseID").val()
    }, function (searchResult) {
        var data = searchResult.Data;
        var pager = searchResult.Pager;
        document.getElementById("__countOfRecords").innerHTML = pager.CountOfRecords;
        document.getElementById("__targetPageNo").innerHTML = pager.TargetPageNo;
        document.getElementById("__countOfPages").innerHTML = pager.CountOfPages;
        if (pager.ExistPrePage) {
            $("#__firstYes").show();
            $("#__firstNo").hide();
            //
            $("#__preYes").show();
            $("#__preNo").hide();
            //
            document.getElementById("__firstYes").onclick = function () { Load(1) }
            document.getElementById("__preYes").onclick = function () { Load(pager.PrePageNo) }
        } else {
            $("#__firstYes").hide();
            $("#__firstNo").show();
            //
            $("#__preYes").hide();
            $("#__preNo").show();
            //
            document.getElementById("__firstYes").onclick = function () {}
            document.getElementById("__preYes").onclick = function () {}
        }
        if (pager.ExistNextPage) {
            $("#__lastYes").show();
            $("#__lastNo").hide();
            //
            $("#__nextYes").show();
            $("#__nextNo").hide();
            //
            document.getElementById("__lastYes").onclick = function () { Load(pager.CountOfPages) }
            document.getElementById("__nextYes").onclick = function () { Load(pager.NextPageNo) }
        } else {
            $("#__lastYes").hide();
            $("#__lastNo").show();
            //
            $("#__nextYes").hide();
            $("#__nextNo").show();
            //
            document.getElementById("__lastYes").onclick = function () { }
            document.getElementById("__nextYes").onclick = function () { }
        }
        //
        var trs = new Array();
        var checkboxAll = document.getElementById("__checkboxAll");
        if (checkboxAll) {
            checkboxAll.checked = false;
        }
        for (var i = 0; i < data.length; i++)
        {
            var checkboxTd = '';
            if (checkboxAll) {
                checkboxTd = '<td style="text-align:center;"><input type="checkbox" name="__checkItem" idOfProject="' + data[i].ID + '" onchange="check_item()"/></td>';
            }
            var onclick = ' onclick = "window.location=\'\/Project\/Show\/' + data[i].ID + '\'"';
            trs.push('<tr>' + checkboxTd + '<td' + onclick + '>' + data[i].RefNum + '</td><td' + onclick + '>' + data[i].Owner + '</td><td' + onclick + '>' + data[i].Title + '</td><td' + onclick + '>￥' + data[i].Charge + '</td><td' + onclick + '>' + data[i].SignTime + '</td><td' + onclick + '>' + data[i].Product + '</td><td' + onclick + '>' + data[i].Enterprise + '</td><td' + onclick + '>' + data[i].Brand + '</td><td' + onclick + '>' + data[i].Customer + '</td><td' + onclick + '>' + data[i].Status + '</td><td' + onclick + '>' + data[i].InvoiceTime + '</td><td' + onclick + '>' + data[i].ChargeTime + '</td></tr>');
        }
        document.getElementById("lstProjects").innerHTML = trs.join('\n');
        page = pager.NextPageNo;
        //
        lock = false;
        HideLoading();
    });
}

function Search() {
    $("#SearchResult").html("<img src='/Images/loading_small.gif' />加载中...");
    $.getJSON("/Search", { Text: $("#txtSearchAll").val() }, function (data) {
        var str = "";
        if ($("#SearchResult").length > 0) {
            str += "<p class='es-notification-subtitle'>项目(" + data.Projects.length + ")</p>";
            if (data.Projects.length > 0) {
                $.each(data.Projects, function (key, value) {
                    str += "<p><a href='/Project/Show/" + value.ID + "'>" + value.Title + "</a></p>";
                });
            }
            str += "<p class='es-notification-subtitle'>客户(" + data.Enterprises.length + ")</p>";
            if (data.Enterprises.length > 0) {
                $.each(data.Enterprises, function (key, value) {
                    str += "<p><a href='/Enterprise/Show/" + value.ID + "'>" + value.Title + "</a></p>";
                });
            }
            str += "<p class='es-notification-subtitle'>名录(" + data.Customers.length + ")</p>";
            if (data.Customers.length > 0) {
                $.each(data.Customers, function (key, value) {
                    str += "<p><a href='/Enterprise/Show/" + value.EnterpriseID + "'>" + value.Name + " (" + value.Enterprise + ")</a></p>";
                });
            }
            str += "<p class='es-notification-subtitle'>员工(" + data.Users.length + ")</p>";
            if (data.Users.length > 0) {
                $.each(data.Users, function (key, value) {
                    str += "<p><img src='/User/Avatar/1' /><a href='#" + value.ID + "'>" + value.Name + "</a></p>";
                });
            }
            $("#SearchResult").html(str);
        }
    });
}

$(document).ready(function () {
    $('#txtSearchAll').keyup(function (e) {
        if (e.keyCode == 13) {
            $("#btnAllSearch").click();
        }
    }); 

    $("#btnAllSearch").click(function () {
        Search();
    });

    function LoadEnterprise() {
        if ($("#aenterprise").length <= 0) return;
        ShowLoading();
        $("#aenterprise").html("");
        $("#benterprise").html("");
        $("#centerprise").html("");
        $("#denterprise").html("");
        $.getJSON("/Enterprise/Search", { Text: $("#txtEnterpriseSearch").val() }, function (data) {
            for (var i = 0; i < data.length; i++) {
                var html = '<div class="es-enterprise-item"><a href="/Enterprise/Show/' + data[i].ID + '" class="title">' + data[i].Title + '</a> <a href="javascript:Delete(' + data[i].ID + ')">删除</a></div>';
                var level = data[i].Level;
                if (level == "A")
                    $("#aenterprise").prepend(html);
                else if (level == "B")
                    $("#benterprise").prepend(html);
                else if (level == "C")
                    $("#centerprise").prepend(html);
                else if (level == "D")
                    $("#denterprise").prepend(html);
            }
            HideLoading();
        });
    }

    LoadEnterprise();

    $("#btnEnterpriseSearch").click(function () {
        LoadEnterprise();
    });

    Load();
});

$(window).scroll(function () {
    /*
    totalheight = parseFloat($(window).height()) + parseFloat($(window).scrollTop());
    if ($(document).height() <= totalheight) {
        Load();
    }
    */
});


function checkNullable(formId) {
    var form = document.getElementById('frmEditProject');
    var form = document.getElementById("frmEditProject");
    var illegal = false;
    $(form).find("input").each(function () {
        var element = $('#__' + $(this).attr("name") + 'Null');
        if ($.trim($(this).val()) == "" && $(this).attr("name") != "undefined" && !$(this).hasClass("nullable") && $(this).attr("placeholder")) {
            if (element) {
                element.show();
            }
            if (!illegal) {
                alert($(this).attr("placeholder") + "不能为空！");
                $(this).focus();
            }
            illegal = true;
        } else {
            if (element) {
                element.hide();
            }
        }
    });
    $(form).find("textarea").each(function () {
        var element = $('#__' + $(this).attr("name") + 'Null');
        if ($.trim($(this).val()) == "" && $(this).attr("name") != "undefined" && !$(this).hasClass("nullable") && $(this).attr("placeholder")) {
            if (element) {
                element.show();
            }
            if (!illegal) {
                alert($(this).attr("placeholder") + "不能为空！");
                $(this).focus();
            }
            illegal = true;
        } else {
            if (element) {
                element.hide();
            }
        }
    });
    $(form).find("select").each(function () {
        var element = $('#__' + $(this).attr("name") + 'Null');
        if ($.trim($(this).val()) == "" && $(this).attr("name") != "undefined" && !$(this).hasClass("nullable") && $(this).attr("placeholder")) {
            if (element) {
                element.show();
            }
            if (!illegal) {
                alert($(this).attr("placeholder") + "不能为空！");
                $(this).focus();
            }
            illegal = true;
        } else {
            if (element) {
                element.hide();
            }
        }
    });
    //
    if (illegal) {
        return false;
    } else {
        return true;
    }
}

$("form").submit(function (e) {
    $.each($(this).find("input[type='text']"), function (i, item) {
        if ($(item).val() == "" && $(item).attr("name") != "undefined" && !$(item).hasClass("nullable")) {
            HideLoading();
            if ($(item).attr("placeholder") != null && $(item).attr("placeholder") != "")
                alert($(item).attr("placeholder") + "不能为空！");
            else
                alert($(item).attr("name") + " 不能为空！");
            e.preventDefault();
            return false;
        }
        else {
            return true;
        }
    });
});