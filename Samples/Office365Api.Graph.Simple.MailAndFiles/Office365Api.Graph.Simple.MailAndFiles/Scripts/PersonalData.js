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
                emailHtml = "<div class='email-message'>" +
                                "<span class='email-text'>" + data[i].Subject.substr(0, 40) + "</span>" +
                                "<span class='email-timestamp'>" + data[i].SentTimestampString + "</span>" +
                            "</div>";

                $("#email-content").append(emailHtml);
            }
        }
    });
}

function GetFiles() {
    $.ajax({
        type: "GET",
        url: "/File",
        cache: false,
        error: function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        },
        success: function (data) {
            $("#file-content").html('');

            for (var i = 0; i < data.length; i++) {
                emailHtml = "<div class='email-message'>" +
                                "<span class='email-text'>" + data[i].FileName.substr(0, 40) + "</span>" +
                                "<span class='email-timestamp'>" + data[i].LastModifiedDateString + "</span>" +
                            "</div>";

                $("#file-content").append(emailHtml);
            }
        }
    });
}

window.onload = function () {
     GetEmails();
     GetFiles();

    setInterval('GetEmails()', 5000);
    setInterval('GetFiles()', 5000);
}