let connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").withAutomaticReconnect().build();

/*document.getElementById("sendButton").disabled = true;*/

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + ": " + msg;
    var data = JSON.stringify(user);

    var recevier = document.getElementById("receiverId").value;
    if (recevier == '') {
        fetch('user/GetUserId', {
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

    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
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
