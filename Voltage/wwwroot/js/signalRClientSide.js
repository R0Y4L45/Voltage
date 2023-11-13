let connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").withAutomaticReconnect().build(),
    curUser, curUserId, recUserId, list = document.getElementById("messagesList");

connection.on("ReceiveMessage", (user, message) => {
    let encodedMsg = user + ": " + message;

    if (curUserId !== recUserId)
        MessageCreater(encodedMsg, "left-item");
});

connection.start().then(() => {
    connection.invoke("GetUserName").then(user => {
        document.getElementById("connectionId").innerText = user;
        curUser = user;
    });

    connection.invoke("GetConnectionId").then(id => curUserId = id);
}).catch(err => console.error(err.toString()));

document.getElementById("sendButton").addEventListener("click", function (event) {
    let message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", message).catch(err => console.error(err.toString()));

    event.preventDefault();
});

document.getElementById("sendToUser").addEventListener("click", async event => {
    let message = document.getElementById("messageInput").value;
    connection.invoke("SendToUser", recUserId, message).catch(err => console.error(err.toString()));

    await MessageSaver(message, curUserId, recUserId);
    MessageCreater(curUser + ": " + message, "right-item");

    //Complete it
    //message.value = null;
    event.preventDefault();
});

async function ClickToUser(username) {
    recUserId = await fetch('GetUserId', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(username)
    })
        .then(response => response.json())
        .then(data => data)
        .catch(error => console.error(error));

    arr = document.getElementById("messagesList");
    arr.innerText = "";
    (await FetchGetList(username)).forEach(i => i.sender == curUser
        ? MessageCreater(i.sender + ': ' + i.content, "right-item")
        : MessageCreater(i.sender + ': ' + i.content, "left-item"));

    //Temporary created
    document.getElementById("receiverId").value = username;
}

async function FetchGetList(receiver) {
    return await fetch('TakeMessages', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(receiver)
    })
        .then(response => response.json())
        .then(data => data)
        .catch(error => console.error(error));
}

async function MessageSaver(message, sender, receiver) {
    let object = {
        Content: message,
        Sender: sender,
        Receiver: receiver
    };

    await fetch('AcceptMessage', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(object)
    })
        .then(response => response.json())
        .then(data => data)
        .catch(error => console.error(error));
}

function MessageCreater(message, style) {
    let li = document.createElement("li"), arr = document.getElementById("messagesList");
    li.textContent = message;
    li.classList.add(style);

    arr.appendChild(li);
}