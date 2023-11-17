let connection = curUserName = curUserId = recUserId = null, list = document.getElementById("messagesList"), author, date, avatarUrl;

//#region SignalR Connection and its events

//Connection Created
connection = new signalR.HubConnectionBuilder().withUrl("/messageHub").withAutomaticReconnect().build()

//Connection Started
connection.start().then(() => {
    connection.invoke("GetUserName").then(user => curUserName = user);
    connection.invoke("GetConnectionId").then(id => curUserId = id);
}).catch(err => console.error(err.toString()));

//Message Received
connection.on("ReceiveMessage", (user, message, createdTime) => {
    if (curUserId !== user)
        MessageCreater(message, "message recipient", user, createdTime);
});

//#endregion

//#region Events

//document.getElementById("sendToUser").addEventListener("click", async event => {
//    await SendMessage(event)
//});

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

    (await FetchGetList(username)).forEach(message => message.sender == curUserId
        ? MessageCreater(message.sender + ': ' + message.content, "message sender", message.createdTime)
        : MessageCreater(message.sender + ': ' + message.content, "message recipient", message.createdTime)
    );
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

        MessageCreater(message, "message sender", curUserName, Date.now());

        document.getElementById("messageInput").value = '';
        event.preventDefault();
    }
}

function MessageCreater(message, style, author, date) {
    let chatBubbles = document.querySelector(".chat-bubbles");
    if (!chatBubbles) {
        //elem yoxdsa yaradr
        chatBubbles = document.createElement("div");
        chatBubbles.classList.add("chat-bubbles");
        document.body.appendChild(chatBubbles);
    }

    let chatItem = document.createElement("div");
    chatItem.classList.add("chat-item");
    chatItem.innerHTML = `
        <div class="row align-items-end justify-content-end">
            <div class="col col-lg-6">
                <div class="chat-bubble ${style}">
                    <div class="chat-bubble-title">
                        <div class="row">
                            <div class="col chat-bubble-author">${author}</div>
                            <div class="col-auto chat-bubble-date">${date}</div>
                        </div>
                    </div>
                    <div class="chat-bubble-body">
                        <p>${message}</p>
                    </div>
                </div>
            </div>
        </div>
    `;

    chatBubbles.appendChild(chatItem);
}

//#endregion