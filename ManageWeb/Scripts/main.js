/// <reference path="jquery-1.9.1.js" />

var libjs = {
    lastid: 0
};

libjs.closedialog = function (id) {
    $('#' + id).modal('hide');
}

libjs.showdialog = function (title, content, btntext, callback) {
    libjs.lastid++;
    var lastid = libjs.lastid;
    var templa = '<div class="modal fade" id="dialog_@dialogid" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">' +
'     <div class="modal-dialog">' +
'         <div class="modal-content">' +
'             <div class="modal-header">' +
'                 <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>' +
'                <h4 class="modal-title" id="myModalLabel">@title</h4>' +
'            </div>' +
'            <div class="modal-body" id="m_value">@content</div>' +
'            <div class="modal-footer">' +
'                <button type="button" class="btn btn-default" id="closebtn_@dialogid">关闭</button>' +
 '               <button type="button" class="btn btn-primary" id="okbtn_@dialogid">@buttontext</button>' +
'           </div>' +
 '       </div>' +
    '    </div>' +
' </div> ';
    templa = templa.replace(/@dialogid/g, lastid).replace("@buttontext", btntext).replace("@title", title).replace("@content", content);
    var d = $(templa);
    $(document.body).append(d);
    $("#closebtn_" + lastid).click(function () {
        $('#dialog_' + lastid).modal('hide');
    });
    $("#okbtn_" + lastid).click(function () {
        var cc = callback("dialog_" + lastid);
        if (cc) {
            $('#dialog_' + lastid).modal('hide');
        }
    });
    $('#dialog_' + lastid).modal({
        keyboard: true
    })
    $('#dialog_' + lastid).on('hidden.bs.modal', function () {
        $('#dialog_' + lastid).remove();
    })
    return "dialog_" + lastid;
}

libjs.showselect = function (ty, callback, data) {
    var url = "/comm/select?type=" + ty;
    $.ajax({
        url: url,
        data: data,
        success: function (data) {
            var diaid = libjs.showdialog("选择", data, "确定", function (id) {
                if ($("#" + id + " .active").length == 0) {
                    alert("请选择");
                    return false;
                }
                var val = $("#" + id + " .active").data("item");
                var text = $("#" + id + " .active").data("text");
                var ty = $("#" + id + " .active").data("type");
                callback(ty, text, val);
                return true;
            })
            $("#" + diaid + " li").click(function () {
                $("#" + diaid + " li").removeClass("active");
                $(this).addClass("active");
                $(this).data("type", ty);
            })
        }
    });
}

libjs.getserverproject = function (pid, target, tags) {
    if (!pid)
        return;
    $.ajax({
        url: '/cmd/getserverproject',
        data: { projectid: pid, tags: tags },
        type: 'post',
        success: function (data) {
            var html = '<ul class="list-group"> ';
            for (var i = 0; i < data.data.length; i++) {
                html += '<li class="list-group-item"><input type="checkbox" name="serverprojectid" checked="checked" class="serverproitem" data-id="' + data.data[i].ServerProjectId + '" />' + data.data[i].Title + '</li>';
            }
            html += '</ul>';
            $(target).html(html);
        }
    });
}

libjs.getservers = function (target) {
    $.ajax({
        url: '/cmd/getservers',
        data: {},
        type: 'post',
        success: function (data) {
            var html = '<ul class="list-group"> ';
            for (var i = 0; i < data.data.length; i++) {
                html += '<li class="list-group-item"><input type="checkbox" checked="checked" class="container_servers" data-id="' + data.data[i].ServerId + '" />' + data.data[i].ServerName + '</li>';
            }
            html += '</ul>';
            $(target).html(html);
        }
    });
}


var libcusservice = {};
libcusservice.index_selectcus = function () {

    libjs.showselect('customer', function (ty, txt, v) {
        $("#search_cusid").val(v);
    })
}
libcusservice.edit_selectcus = function () {
    libjs.showselect('customer', function (ty, txt, v) {
        $("#Show_CusId").val(v + "-" + txt);
        $("#CusId").val(v);
    });
}
var libtask = {};
libtask.index_setstate = function (taskid, newstate) {
    $.ajax({
        url: '/taskdll/SettaskState',
        data: { taskid: taskid, newstate: newstate },
        type: 'post',
        success: function (data) {
            if (data.code > 0) {
                alert(data.msg);
                location.reload();
            }
            else {
                alert(data.msg);
            }
        }
    });
}

var libcusproject = {};
libcusproject.index_selectcus = function () {
    libjs.showselect('customer', function (ty, txt, v) {
        $("#search_cusid").val(v);
    })
}
libcusproject.index_selectproject = function () {
    libjs.showselect('project', function (ty, txt, v) {
        $("#search_projectid").val(v);
    });
}
libcusproject.index_selectserver = function () {
    libjs.showselect('server', function (ty, txt, v) {
        $("#search_serverid").val(v);
    });
}

var libserverproject = {};
libserverproject.index_selectproject = function () {
    libjs.showselect('project', function (ty, txt, v) {
        $("#search_projectinfo").val(txt);
    });
}
libserverproject.index_selectserver = function () {
    libjs.showselect('server', function (ty, txt, v) {
        $("#search_serverinfo").val(txt);
    });
}

libserverproject.edit_selectserver = function () {
    libjs.showselect('server', function (ty, txt, v) {
        $("#ServerId").val(v);
        $("#show_serverid").text(v + " " + txt);
    });
}

libserverproject.edit_selectproject = function () {
    libjs.showselect('project', function (ty, text, val) {
        $("#ProjectId").val(val);
        $("#show_projectid").text(val + " " + text);
    });
}

var libcmd = {};
libcmd.index_selectcus = function () {
    libjs.showselect('customer', function (ty, txt, v) {
        $("#search_cusid").val(v);
    })
}
libcmd.index_selectproject = function () {
    libjs.showselect('project', function (ty, txt, v) {
        $("#search_projectid").val(v);
    });
}
libcmd.index_selectserver = function () {
    libjs.showselect('server', function (ty, txt, v) {
        $("#search_serverid").val(v);
    });
}

var libworkitem = {};
libworkitem.index_selectmanager1 = function () {
    libjs.showselect('manager', function (ty, txt, v) {
        $("#distributeuserid").val(v);
    })
}
libworkitem.index_selectmanager2 = function () {
    libjs.showselect('manager', function (ty, txt, v) {
        $("#createuserid").val(v);
    });
}
libworkitem.index_delete = function (e) {
    if (confirm("确定要删除吗？")) {
        $.ajax({
            url: '/workitem/delete',
            type: 'post',
            data: { workitemid: $(e).data("id") },
            success: function (data) {
                if (data.code > 0)
                    $(e).parents('tr').remove();
                else {
                    alert(data.msg);
                }
            }
        });
    }
}
libworkitem.detail_exec_click = function () {
    $('#myModal-exec').modal('show');
}
libworkitem.detail_exec = function (e) {
    $.ajax({
        url: '/workitem/DistributeExec',
        type: 'post',
        data: { workitemid: $(e).data("id"), actualtime: $("#exec-actualtime").val(), workRemark: $("#exec-content").val() },
        success: function (data) {
            if (data.code > 0) {
                alert("执行成功！");
                location.href = '/workitem/detail?workitemid=' + $(e).data("id");
            } else {
                alert(data.msg);
            }
        }
    });
}

var libfeedback = {};
libfeedback.index_precheck = function (e) {
    var id = $(e).data('id');
    $("#checktype").val("3");
    $("#btn_check").data("id", id);
    $('#myModal-check').modal('show');
}
libfeedback.edit_selectcus = function () {
    libjs.showselect('customer', function (ty, txt, v) {
        $("#cusId").val(v);
        $("#CusName").val(txt);
        $("#Show_cus").val(v + " " + txt);
    })
}
libfeedback.index_check = function () {
    var feedbackid = $("#btn_check").data("id");
    var remark = $("#check-remark").val();
    var checktype = $("#checktype").val();
    $.ajax({
        url: '/feedback/check',
        type: 'post',
        data: { feedbackid: feedbackid, checktype: checktype, checkremark: remark },
        success: function (data) {
            if (data.code < 0) {
                alert(data.msg);
            }
            else if (data.code == 1) {
                alert("审核成功");
                location.reload();
            } else if (data.code == 2) {
                alert("审核成功");
                location.href = data.data;
                //window.open(data.data, "_blank", "", false);
            }
        }
    });
}


var libcustomer = {};
libcustomer.index_delete = function (e) {
    if (!confirm("确定要删除吗?"))
        return;
    $.ajax({
        url: '/customer/Delete',
        type: 'post',
        data: { cusid: $(e).data('id') },
        success: function (d) {
            if (d.code > 0) {
                $(e).parents('tr').addClass('danger');
                alert('删除成功');
                setTimeout(function () {
                    $(e).parents('tr').hide();
                }, 500);
            } else {
                alert(d.msg);
            }
        }
    })
}

var libmanager = {};
libmanager.index_delete = function (e) {
    if (!confirm("你确定要删除吗?"))
        return;
    $.ajax({
        url: '/manager/DeleteManager',
        type: 'post',
        data: { managerid: $(e).data('id') },
        success: function (d) {
            if (d.code > 0) {
                $(e).parents('tr').addClass('danger');
                alert('删除成功');
                setTimeout(function () {
                    $(e).parents('tr').hide();
                }, 500);
            } else {
                alert(d.msg);
            }
        }
    })
}

var libworkdaily = {};
libworkdaily.index_presubmit = function () {
    var isok = true;
    if (isok && !$("#WorkTime").val()) {
        alert("请选择工作日期!");
        isok = false;
    }
    //if (isok && !$("#Summary").val()) {
    //    alert("请填写概要!");
    //    isok = false;
    //}
    if (isok && !$("#Content").val()) {
        alert("请填写工作内容!");
        isok = false;
    }
    return isok;
}
libworkdaily.index_delete = function (e) {
    if (!confirm("确定要删除吗?"))
        return;
    $.ajax({
        url: '/workdaily/Delete',
        type: 'post',
        data: { workdailyid: $(e).data('id') },
        success: function (d) {
            if (d.code > 0) {
                $(e).parents('tr').addClass('danger');
                alert('删除成功');
                setTimeout(function () {
                    $(e).parents('tr').hide();
                }, 500);
            } else {
                alert(d.msg);
            }
        }
    })
}
libworkdaily.index_datechange = function (e) {
    var date = $("#WorkTime").val();
    var WorkDailyId = $("#WorkDailyId").val();
    if (date) {
        $.ajax({
            url: '/workdaily/checkdate',
            type: 'post',
            data: { date: date, WorkDailyId: WorkDailyId },
            success: function (data) {
                if (data.code > 0) {
                    if (data.data > 0) {
                        $("#date_msg").html('<span class="label label-warning">该日期已存在提交记录</span><a href="/workdaily/edit?workdailyid=' + data.data + '" class="btn btn-sm btn-primary">去编辑</a>');
                    } else {
                        $("#date_msg").html('');
                    }
                }
                else {
                }
            }
        });
    } else {
        $("#date_msg").html('');
    }
}

libworkdaily.index_autobuild = function () {
    var date = $("#WorkTime").val();
    if (!date) {
        alert("请选择日期！");
        return;
    }

    $.ajax({
        url: '/workdaily/BuildDailyFromWork',
        type: 'post',
        data: { date: date },
        success: function (data) {
            if (data.code > 0) {
                $("#Content").val(data.data);
            }
            else {
                alert(data.msg);
            }
        }
    });
}
libworkdaily.index_selectmanager = function () {
    libjs.showselect('manager', function (ty, txt, v) {
        $("#search_managerid").val(v);
    })
}

libworkdaily.report_hasmore = function (e) {
    $.ajax({
        url: '/workdaily/Report',
        type: 'post',
        data: { begintime: $(e).data("begintime"), endtime: $(e).data("endtime"), groupid: $(e).data("groupid") },
        success: function (data) {

            $("#tb_fixheader").remove();
            $("#tb_fixheader_left").remove();

            $("#hasmore_tr").remove();
            $('.cc-workdaily-report table tbody').append(data);

            libworkdaily.report_scroll();
        }
    });
}

libworkdaily.report_scroll = function () {
    $(".cc-workdaily-report").scroll(function (e) {

        var setwidthtop = function () {
            $(".tbreport-main tr:first").children('td').each(function (i, k) {
                console.log($(k).width());
                $($("#tb_fixheader tr td")[i]).width($(this).width());
            });

        }
        var setwidthleft = function () {

            $(".tbreport-main tr").each(function (i, k) {
                $($("#tb_fixheader_left tr")[i]).children('th').width($(k).children('th:first').width());
                $($("#tb_fixheader_left tr")[i]).children('th').height($(k).children('th:first').height());
            });
        }
        var st = $('.cc-workdaily-report').scrollTop();
        if (st >= 0) {
            if ($("#tb_fixheader").length == 0) {
                var thtml = '<table id="tb_fixheader" class="table table-bordered" style="width:' + $(".tbreport-main").width() + 'px;"><tbody><tr class="report-top-head">';
                thtml += $(".cc-workdaily-report table tr:first").html();
                thtml += '</tr></tbody></table>';
                $(".tbreport-main").before(thtml);
            }
            $("#tb_fixheader").show();
            setwidthtop();

        } else {
            $("#tb_fixheader").hide();
        }

        var sl = $('.cc-workdaily-report').scrollLeft();
        if (sl >= 0) {
            if ($("#tb_fixheader_left").length == 0) {
                var thtml = '<table id="tb_fixheader_left" class="table table-bordered" style="width:' + $(".tbreport-main tr:first").children('th:first').attr('width') + ';"><tbody>';
                $(".tbreport-main tr").each(function (i, te) {
                    thtml += '<tr><th class="' + $($(te).children('th')[0]).attr('class') + '">' + $($(te).children('th')[0]).html() + '</th></tr>';
                });
                thtml += '</tbody></table>';
                $(".tbreport-main").before(thtml);
            }
            $("#tb_fixheader_left").show();
            setwidthleft();
        } else {
            $("#tb_fixheader_left").hide();
        }
        $("#tb_fixheader_left").css({
            //   top: st + "px",
            left: sl + 'px'
        });

        $("#tb_fixheader").css({
            top: st + "px"
            //   left: sl + 'px'
        });
        //console.log($('.cc-workdaily-report').scrollTop());
        // console.log(e);
    })
}
