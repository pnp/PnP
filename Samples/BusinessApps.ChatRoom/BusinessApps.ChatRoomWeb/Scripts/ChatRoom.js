function StartHub() {
    var chatHub = $.connection.chatHub;

    chatHub.client.pushMessage = function (userName, photoUrl, timeStamp, message) {
        ReceiveMessage(userName, photoUrl, timeStamp, message);
    };

    chatHub.client.joinRoom = function (userName) {
        ReceiveJoinRoom(userName);
    };

    chatHub.client.leaveRoom = function (userName) {
        ReceiveLeaveRoom(userName);
    };

    $.connection.hub.start();
}

function SendMessage() {
    var textBox = $("#message-box textarea");

    if (textBox.val() != "") {
        $.ajax({
            type: "GET",
            url: "/Home/SendMessage" + location.search + "&message=" + textBox.val(),
            cache: false
        });

        textBox.val("");
    }

    textBox.focus();
}

function ReceiveMessage(userName, photoUrl, timeStamp, message) {
    var localTimeStamp = new Date(timeStamp + " UTC");

    AddMessage(
        '<div class=\'message\'>' +
            '<img class=\'message-sender-image\' src=\'' + photoUrl + '\' alt=\'Photo of ' + userName + '\'/>' +
            '<div class=\'message-right\'>' +
                '<div class=\'message-sender\'>' + userName + '</div>' +
                '<div class=\'message-timestamp\'>' + localTimeStamp.toLocaleTimeString() + '</div>' +
                '<div class=\'message-content\'>' + message + '</div>' +
            '</div>' +
        '</div>'
        );
}

function ReceiveJoinRoom(userName) {
    AddMessage(
            '<div class=\'system-message\'>' +
                userName + " has joined the room." +
            '</div>'
            );
}

function ReceiveLeaveRoom(userName) {
    AddMessage(
            '<div class=\'system-message\'>' +
                userName + " has left the room." +
            '</div>'
            );
}

function JoinRoom() {
    $.ajax({
        type: "GET",
        url: "/Home/JoinRoom" + location.search,
        cache: false
    });
}

function LeaveRoom() {
    $.ajax({
        type: "GET",
        url: "/Home/LeaveRoom" + location.search,
        cache: false
    });
}

function AddMessage(html) {
    $('#message-window').append(html);

    $("#message-window").animate({
        scrollTop: $("#message-window")[0].scrollHeight
    });
}

function KeyUp(e) {
    if (e.keyCode === 13 || e.which === 13) {
        SendMessage();
    }
}

function Ping() {
    $.ajax({
        type: "GET",
        url: "/Home/Ping" + location.search,
        cache: false
    });
}

StartHub();

setInterval(function () { Ping() }, 20000);

window.onload = function () { JoinRoom() }
window.onunload = function () { LeaveRoom() }