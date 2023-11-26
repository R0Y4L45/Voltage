﻿let date = recUserId = recUserName = null,
    overChatBubble = document.getElementById("overChatBubbles"),
    list = document.getElementById("messagesList"), count;

overChatBubble.addEventListener("dragover", (event) => {
    event.preventDefault();
});

overChatBubble.addEventListener("drop", (event) => {
    event.preventDefault();
    handleFileDrop(event.dataTransfer.files);
});

function handleFileDrop(files) {
    for (const file of files) {
        if (file.type.startsWith('image/')) 
            handleImageDrop(file);
         else if (file.type === 'application/zip') 
            handleZipDrop(file);
          else
            console.log('Unsupported file type: ', file.type);
        
    }
}

function handleImageDrop(imageFile) {
    const reader = new FileReader();

    reader.onload = (event) => {
        const imageUrl = event.target.result;
        sendMessageWithImage(imageUrl);
    };

    reader.readAsDataURL(imageFile);
}

function handleZipDrop(zipFile) {
    console.log('Zip file dropped: ', zipFile.name);
}


function sendMessageWithImage(imageUrl) {
    const message = 'Sent an image';
    const sender = curUserName;
    date = new Date();

    MessageSaver(message, sender, recUserId);   

    connection.invoke("SendToUser", recUserId, message).catch(err => console.error(err.toString()));

    MessageCreater(`<img src="${imageUrl}" />`, 'justify-content-end', curUserName, date.getHours() + ':' + date.getMinutes().toString().padStart(2, '0'));
    overChatBubble.scrollTop = overChatBubble.scrollHeight;
}



//#region SignalR Connection and its events

connection.on("ReceiveMessage", (user, message, createdTime) => {
    date = new Date(createdTime)
    if (curUserName !== user) {
        MessageCreater(message, '', user, date.getHours().toString() + ':' + date.getMinutes().toString().padStart(2, '0'));
        overChatBubble.scrollTop = overChatBubble.scrollHeight;
    }
});

//#endregion

//#region Events

document.getElementById("messageInput").addEventListener("keypress", async event => {
    if (event.key === "Enter")
        await SendMessage(event)
});
async function ClickToUser(username) {
    recUserName = username
    recUserId = await GetUserId(username);
    count = 9;
    document.querySelector(".chat-bubbles").innerHTML = '';
    (await GetMessageList(username + ' ' + count)).forEach(message => {
        date = new Date(message.createdTime);

        MessageCreater(message.content, message.sender == curUserName ? 'justify-content-end' : '',
            message.sender, date.getHours().toString() + ':' + date.getMinutes().toString().padStart(2, '0'))
        overChatBubble.scrollTop = overChatBubble.scrollHeight;
    });
}

overChatBubble.addEventListener("scroll", async _ => {
    if (overChatBubble.scrollTop === 0) {
        count += 9;
        (await GetMessageList(recUserName + ' ' + count)).forEach(message => {
            date = new Date(message.createdTime);

            MessageCreater(message.content, message.sender == curUserName ? 'justify-content-end' : '',
                message.sender, date.getHours().toString() + ':' + date.getMinutes().toString().padStart(2, '0'))
        });
    }
});

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

//#endregion

//#region HelperMethods
async function SendMessage(event) {
    let message = document.getElementById("messageInput").value;
    date = new Date();

    if (recUserId !== null)
        if ((await MessageSaver(message, curUserId, recUserId)) !== 0) {
            connection.invoke("SendToUser", recUserId, message).catch(err => console.error(err.toString()));

            MessageCreater(message, 'justify-content-end', curUserName, date.getHours() + ':' + date.getMinutes().toString().padStart(2, '0'));
            overChatBubble.scrollTop = overChatBubble.scrollHeight;

            document.getElementById("messageInput").value = '';
            event.preventDefault();
        }
}
function MessageCreater(content, style, sender, date) {
    let chatBubbles = document.querySelector(".chat-bubbles"),
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
}

//#endregion