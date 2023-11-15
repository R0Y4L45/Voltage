let connection = curUser = curUserId = recUserId = null, list = document.getElementById("messagesList");

//#region SignalR Connection and its events 

connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").withAutomaticReconnect().build()

connection.on("ReceiveMessage", (user, message) => {
    let encodedMsg = user + ": " + message;

    if (curUserId !== recUserId)
        MessageCreater(encodedMsg, "message recipient");
});

connection.start().then(() => {
    connection.invoke("GetUserName").then(user => {
        document.getElementById("connectionId").innerText = user;
        curUser = user;
    });

    connection.invoke("GetConnectionId").then(id => curUserId = id);
}).catch(err => console.error(err.toString()));

//#endregion

//#region Events
document.getElementById("sendButton").addEventListener("click", function (event) {
    let message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", message).catch(err => console.error(err.toString()));

    event.preventDefault();
});

document.getElementById("sendToUser").addEventListener("click", async event => {
    await SendMessage(event)
});

document.getElementById("messageInput").addEventListener("keypress", async event => {
    if (event.key === "Enter")
        await SendMessage(event)
});

//#endregion

//#region Api's
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
        ? MessageCreater(i.sender + ': ' + i.content, "message sender")
        : MessageCreater(i.sender + ': ' + i.content, "message recipient"));

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

//#endregion

//#region HelperMethods
async function SendMessage(event) {
    let message = document.getElementById("messageInput").value;
    if (recUserId !== null) {
        connection.invoke("SendToUser", recUserId, message).catch(err => console.error(err.toString()));

        await MessageSaver(message, curUserId, recUserId);
        MessageCreater(curUser + ": " + message, "message sender");

        document.getElementById("messageInput").value = '';
        event.preventDefault();
    }
}

function MessageCreater(message, style) {
    let li = document.createElement("li"), arr = document.getElementById("messagesList");
    li.textContent = message;
    let a = style.split(' ');
    a.forEach(s => {
        if (message.length > 20)
            li.classList.add("long-message")

        li.classList.add(s)
    });

    arr.appendChild(li);
}
//#endregion