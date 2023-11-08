let connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").withAutomaticReconnect().build();
connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + ": " + msg;


    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);

    AcceptMessage(msg, user, user);
});

connection.start().then(function () {
    connection.invoke("GetConnectionId").then(function (id) {
        document.getElementById("connectionId").innerText = id;
    });
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("sendToUser").addEventListener("click", function (event) {
    var receiverConnectionId = document.getElementById("receiverId").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendToUser", receiverConnectionId, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

function ClickToMessage(username) {
    let data = JSON.stringify(username);

    fetch('GetUserId', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: data
    })
        .then(response => response.json())
        .then(data => document.getElementById("receiverId").value = data)
        .catch(error => console.error(error));
}

function AcceptMessage(message, sender, receiver) {
    let object = {
        Message: message,
        Sender: sender,
        Receiver: receiver
    },
        data = JSON.stringify(object);

    fetch('AcceptMessage', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: data
    })
        .then(response => response.json())
        .then(data => Console.log(Json.stringify(data)))
        .catch(error => console.error(error));
}
