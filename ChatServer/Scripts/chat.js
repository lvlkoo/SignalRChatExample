$(function () {  
    //$("#displayname").val(prompt("Enter your name:", ""));
    $("#nicknamePromt").modal({ backdrop: "static", keyboard: false });
    $("#nicknamePromt").modal("show");
});

function startChatConnection() {
    // Reference the auto-generated proxy for the hub.  
    var chat = $.connection.chatHub;
    // Create a function that the hub can call back to display messages.
    chat.client.SendMessage = appendRecivedAnswer;
    // Create a function that the hub can call back to display new joined user.
    chat.client.Join = appendJoinedUser;
    // Create a function that the hub can call back to remove left user from users list.
    chat.client.Leave = removeLeftUser;
    // Get the user name and store it to prepend to messages.
    // Set initial focus to message input box.  
    $("#message").focus();
    // Start the connection.
    $.connection.hub.start().done(function () {
        var name = $("#displayname").val();
        chat.server.join(name);
        $("#sendmessage").click(function () {
            var message = $("#message").val();
            // Call the Send method on the hub. 
            chat.server.send(name, message);
            appendAnswerFromMe(name, message);
            // Clear text box and reset focus for next comment. 
            $("#message").val("").focus();
        });
        chat.server.getUsersList().done(function (result) {
            console.log(result);
            jQuery.each(result, function () {
                appendJoinedUser(this.Name, this.ConnectionId);
            });

        });

    });
}

function appendJoinedUser(name, id) {
    $("#users").append('<div id="' + id +'" class="user">' +
        '<div class="avatar">' +
        '<img src="http://bootdey.com/img/Content/avatar/avatar1.png" alt="User name">' +
        '<div class="status online"></div>' +
        "</div>" +
        '<div class="name">' + name + "</div>" +
        '<div class="mood">Online</div>' +
        "</div>");
}

function removeLeftUser(name, id) {
    $("#" + id).remove();
}

function appendRecivedAnswer(name, message) {
    var date = getDate();
    $("#messages").append('<div class="answer left">' +
        '<div class="avatar">' +
        '<img src="http://bootdey.com/img/Content/avatar/avatar1.png" alt="User name">' +
        '<div class="status online"></div>' +
        "</div>" +
        '<div class="name">' + name + "</div>" +
        '<div class="text">' + message + "</div>" +
        '<div class="time">' + date + "</div>" +
        "</div>");
}

function appendAnswerFromMe(name, message) {
    var date = getDate();
    $("#messages").append('<div class="answer right">' +
        '<div class="avatar">' +
        '<img src="http://bootdey.com/img/Content/avatar/avatar1.png" alt="User name">' +
        '<div class="status online"></div>' +
        "</div>" +
        '<div class="name">' + name + "</div>" +
        '<div class="text">' + message + "</div>" +
        '<div class="time">' + date + "</div>" +
        "</div>");
}

function getDate() {
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1;
    var yyyy = today.getFullYear();

    if (dd < 10) {
        dd = "0" + dd;
    }

    if (mm < 10) {
        mm = "0" + mm;
    }

    today = mm + "/" + dd + "/" + yyyy;
    return today;
}

// Html-encodes messages for display in the page.
function htmlEncode(value) {
    var encodedValue = $("<div />").text(value).html();
    return encodedValue;
}