function DisplayEmail(messageId) {
    $.ajax({
        type: "GET",
        url: "/Email/EmailDetails?messageId=" + messageId,
        cache: false,
        error: function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        },
        success: function (data) {
            $("#dialog").html(data);
            $("#dialog").dialog({
                width: 700,
                height: 500,
                title: "Message Details"
            });
        }
    });
}

function Assign(messageId, subject, body) {
    var title = escape($("#email-message-subject").text());
    var description = escape($("#email-message-body").text());
    var assignedTo = $(".user-dropdown li.selected").attr("data-user");

    $.ajax({
        type: "POST",
        url: "/SupportTicket/AssignSupportTicket?messageID=" + messageId,
        data: { 
            "description": description,
            "title": title,
            "assignedTo": assignedTo
        },
        cache: false,
        error: function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        },
        success: function (data) {
            $("#dialog").dialog("close");
        }
    });
}

function Discard(messageId) {
    $.ajax({
        type: "POST",
        url: "/Email/DiscardEmail?messageId=" + messageId,
        cache: false,
        error: function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        },
        success: function (data) {
            $("#dialog").dialog("close");
        }
    });
}