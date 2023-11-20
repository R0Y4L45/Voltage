let date = recUserId = null, list = document.getElementById("messagesList")

//#region SignalR Connection and its events

connection.on("ReceiveMessage", (user, message, createdTime) => {
    date = new Date(createdTime)
    if (curUserName !== user)
        MessageCreater(message, '', user, date.getHours().toString() + ':' + date.getMinutes().toString().padStart(2, '0'));
});

//#endregion

//#region Events

document.getElementById("messageInput").addEventListener("keypress", async event => {
    if (event.key === "Enter")
        await SendMessage(event)
});
async function ClickToUser(username) {
    recUserId = await GetUserId(username);

    document.querySelector(".chat-bubbles").innerHTML = '';
    (await GetMessageList(username)).forEach(message => {
        date = new Date(message.createdTime);

        MessageCreater(message.content, message.sender == curUserName ? 'justify-content-end' : '',
            message.sender, date.getHours().toString() + ':' + date.getMinutes().toString().padStart(2, '0'))
    });
}

//#endregion

//#region Api's
async function GetUserId(username) { 
    return await FetchApiPost('GetUserId', username);
}

async function GetMessageList(receiver) {
    return await FetchApiPost('TakeMessages', receiver);
}

async function MessageSaver(message, sender, receiver) {
    let object = {
        Content: message,
        Sender: sender,
        Receiver: receiver
    };

    return await FetchApiPost('MessageSaver', object);
}
async function FetchApiPost(methodName, object) {
    return await fetch(methodName, {
        method: 'Post',
        headers: {
            'Content-type': 'application/json'
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
    date = new Date();

    if (recUserId !== null)
        if ((await MessageSaver(message, curUserId, recUserId)) !== 0) {
            connection.invoke("SendToUser", recUserId, message).catch(err => console.error(err.toString()));
            MessageCreater(message, 'justify-content-end', curUserName, date.getHours() + ':' + date.getMinutes().toString().padStart(2, '0'));
            document.getElementById("messageInput").value = '';
            event.preventDefault();
        }
}
function MessageCreater(content, style, sender, date) {
    let chatBubbles = document.querySelector(".chat-bubbles"),
        overChatBubble = document.getElementById("overChatBubbles"),
        chatItem = document.createElement("div");

    chatItem.classList.add("chat-item");
    chatItem.innerHTML = `
        <div class="row align-items-end ${style}">
            <div class="col col-lg-6">
                <div class="chat-bubble">
                    <div class="chat-bubble-title">
                        <div class="row">
                            <div class="col chat-bubble-author">${sender}</div>
                            <div class="col-auto chat-bubble-date">${date} </div>
                        </div>
                    </div>
                    <div class="chat-bubble-body">
                        <p>${content}</p>
                    </div>
                </div>
            </div>
        </div>
    `;

    chatBubbles.appendChild(chatItem);
    overChatBubble.scrollTop = overChatBubble.scrollHeight;
}

//#endregion