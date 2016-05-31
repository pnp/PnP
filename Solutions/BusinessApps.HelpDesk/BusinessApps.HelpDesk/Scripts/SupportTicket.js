function DisplaySupportTicket(supportTicketID) {
    $.ajax({
        type: "GET",
        url: "/SupportTicket/GetSupportTicketDetails?supportTicketID=" + supportTicketID,
        cache: false,
        error: function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        },
        success: function (data) {
            $("#dialog").html(data);
            $("#dialog").dialog({
                width: 700,
                height: 500,
                title: "Support Ticket Details"
            });
        }
    });
}


function CloseTicket(supportTicketID) {
    $.ajax({
        type: "POST",
        url: "/SupportTicket/CloseSupportTicket?supportTicketID=" + supportTicketID,
        cache: false,
        error: function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        },
        success: function (data) {
            $("#dialog").dialog("close");
        }
    });
}