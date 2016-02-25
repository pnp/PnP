function GetEmails() {
    $.ajax({
        type: "GET",
        url: "/Email",
        cache: false,
        error: function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        },
        success: function (data) {
            $("#email-content").html('');

            for (var i = 0; i < data.length; i++) {
                emailHtml = "<div class='email-message' onclick='DisplayEmail(\"" + data[i].MessageID + "\")'>" +
                                "<span class='email-text'>" + data[i].Subject.substr(0, 40) + "</span>" +
                                "<span class='email-timestamp'>" + new Date(data[i].SentTimestamp + " UTC").toLocaleString() + "</span>" +
                            "</div>";

                $("#email-content").append(emailHtml);
            }
        }
    });
}

function GetTickets(userEmailAddress) {
    $.ajax({
        type: "GET",
        url: "/SupportTicket/GetSupportTicketsForUser?userEmailAddress=" + userEmailAddress,
        cache: false,
        error: function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        },
        success: function (data) {
            $("#support-ticket-content").html('');

            for (var i = 0; i < data.length; i++) {
                ticketHtml = "<div class='support-ticket-message' onclick='DisplaySupportTicket(\"" + data[i].ID + "\")'>" +
                                "<span class='support-ticket-title'>" + data[i].Title.substr(0, 40) + "</span>" +
                                "<span class='support-ticket-status'>" + data[i].Status + "</span>" +
                            "</div>";

                $("#support-ticket-content").append(ticketHtml);
            }
        }
    });
}


function GetAnnouncements() {
    $.ajax({
        type: "GET",
        url: "/Announcement",
        cache: false,
        error: function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        },
        success: function (data) {
            $("#announcement-content").html('');

            for (var i = 0; i < data.length; i++) {
                var date = new Date(parseInt(data[i].Timestamp.replace("/Date(", "").replace(")/", ""), 10));

                html = "<div class='announcement'>" +
                            "<span class='announcement-title'>" + data[i].Title + "</span>" +
                            " - " + 
                            "<span class='accouncement-timestamp'>" + date.toLocaleString() + "</span>" +
                        "</div>";

                $("#announcement-content").append(html);
            }
        }
    });
}


window.onload = function () {
    GetEmails();
    GetTickets();
    GetAnnouncements();

    setInterval('GetEmails()', 5000);
    setInterval('GetTickets()', 5000);
    setInterval('GetAnnouncements()', 5000);
}