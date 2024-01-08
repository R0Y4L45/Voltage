let date = recUserId = recUserName = null,
    overChatBubble = document.getElementById("overChatBubbles"),
    fileDropArea = document.getElementById("fileDropArea"),
    fileInput = document.getElementById("fileInput"),
    fileDescription = document.getElementById("fileDescription"),
    list = document.getElementById("messagesList"),
    count, messageSection = document.getElementById("MessageSection"),
    animationArea = document.getElementById("animationtext"),isBckButtonPressed = false,
    obj = {
        userName: '',
        skip: 0
    },
    usernameFlag, firstObj = null;


/*overChatBubble.addEventListener("dragover", (event) => {
    event.preventDefault();
    showFileDropArea();
});

overChatBubble.addEventListener("dragleave", () => {
    hideFileDropArea();
});


overChatBubble.addEventListener("drop", (event) => {
    event.preventDefault();
    const files = event.dataTransfer.files;
    handleFileDrop(files, event.clientX, event.clientY);
    hideFileDropArea();
});


fileDropArea.addEventListener("drop", (event) => {
    event.preventDefault();
    hideFileDropArea();
    const files = event.dataTransfer.files;
    handleFileDrop(files);
});
function showFileDropArea() {
    fileDropArea.style.display = "block";
}

function hideFileDropArea() {
    fileDropArea.style.display = "none";
}


fileDropArea.addEventListener("dragleave", () => {
    fileDropArea.style.border = "2px dashed #ccc";
});



fileDropArea.addEventListener("drop", (event) => {
    event.preventDefault();
    fileDropArea.style.border = "2px dashed #ccc";
    handleFileDrop(event.dataTransfer.files);
});

function handleFileDrop(files) {
    for (const file of files) {
        if (file.type.startsWith('image/')) {
            handleImageDrop(file);
        } else if (file.type === 'application/zip') {
            handleZipDrop(file);
        } else {
            console.log('Unsupported file type: ', file.type);
        }
    }
}
function handleImageDrop(imageFile) {
    const reader = new FileReader();

    reader.onload = (event) => {
        const imageUrl = event.target.result;
        const img = new Image();
        img.src = imageUrl;
        img.style.maxWidth = '100%';
        img.style.maxHeight = '100%';

        fileDropArea.innerHTML = '';
        fileDropArea.appendChild(img);
    };

    reader.readAsDataURL(imageFile);
}


function handleImageDrop(imageFile) {
    const reader = new FileReader();

    reader.onload = (event) => {
        const imageUrl = event.target.result;
        fileDropArea.innerHTML = `<img src="${imageUrl}" style="max-width: 100%; max-height: 100%;">`;
    };

    reader.readAsDataURL(imageFile);
}

function handleZipDrop(zipFile) {
    console.log('Zip file dropped: ', zipFile.name);
    fileDropArea.innerHTML = `<p>Selected file: ${zipFile.name}</p>`;
}

function handleSend() {
    const description = fileDescription.value;
    console.log('Description:', description);
}*/



//#region SignalR Connection and its events

connection.on("ReceiveMessage", (user, message, createdTime) => {
    date = new Date(createdTime);
    console.log(date);
    if (curUserName !== user) {
        messageCreater(message, '', user, date, true, '');

        overChatBubble.scrollTop = overChatBubble.scrollHeight;
    }
});

//#endregion

//#region Events


function backFriendList() {
    const messageFriendList = document.getElementById("MessageFriendList"),
        messageAreas1 = document.getElementById("messageAreas1");
    messageAreas1.classList.add('displaynone');
    messageFriendList.classList.remove('displaynone');

    isBckButtonPressed = true;
}


function showMessagesClick() {
    const messageFriendList = document.getElementById("MessageFriendList"),
        messageAreas1 = document.getElementById("messageAreas1");

    messageSection.classList.remove("displaynone");
    animationArea.style.display = 'none';


    // if user logged from the phone and does not have to expand the window//
    if (window.innerWidth <= 993) {
        messageFriendList.classList.add('displaynone');
        messageFriendList.classList.remove('col-12', 'col-lg-5', 'col-xl-3', 'border-end');
        messageAreas1.classList.remove('col-12', 'col-lg-7', 'col-xl-9', 'd-flex', 'flex-column', 'hide-on-small-screen');
        messageAreas1.classList.add('col-12', 'col-lg-7', 'col-xl-12', 'd-flex', 'flex-column');
        animationArea.classList.add("displaynone");
        if (isBckButtonPressed) {
            messageAreas1.classList.remove('displaynone');
        }
    }


    window.addEventListener('resize', () => {
        if (window.innerWidth <= 993) {
            messageFriendList.classList.add('displaynone');
            messageFriendList.classList.remove('col-12', 'col-lg-5', 'col-xl-3', 'border-end');
            messageAreas1.classList.remove('col-12', 'col-lg-7', 'col-xl-9', 'd-flex', 'flex-column', 'hide-on-small-screen');
            messageAreas1.classList.add('col-12', 'col-lg-7', 'col-xl-12', 'd-flex', 'flex-column');
            animationArea.classList.add("displaynone");
            if (isBckButtonPressed) {
                messageAreas1.classList.remove('displaynone');
            }
        }
        else {
            messageFriendList.classList.add('col-12', 'col-lg-5', 'col-xl-3', 'border-end');
            messageFriendList.classList.remove('displaynone');
            messageAreas1.classList.remove('col-12', 'col-lg-7', 'col-xl-12', 'd-flex', 'flex-column');
            messageAreas1.classList.add('col-12', 'col-lg-7', 'col-xl-9', 'd-flex', 'flex-column', 'hide-on-small-screen');
        }    
    });
}

document.getElementById("messageInput").addEventListener("keypress", async event => {
    if (event.key === "Enter")
        await sendMessage(event)
});
async function clickToUser(username) {
    if (window.innerWidth <= 993)
        usernameFlag = '';

    if (username !== usernameFlag) {
        overChatBubble.removeEventListener('scroll', scrollLoad);

        let chatHeader = document.getElementById("chatHeader"),
            avatarElement = chatHeader.querySelector('.avatar'),
            usernameElement = chatHeader.querySelector('.col-auto.ms-2 h4'),
            clickedElement = document.querySelector(`[data-user-name="${username}"]`),
            userPhoto = clickedElement.getAttribute('data-user-photo'),
            userStatus = clickedElement.getAttribute('data-user-status'),
            h5Element, first = true;

        showMessagesClick();

        document.querySelector(".chat-bubbles").innerHTML = '';
        avatarElement.style.backgroundImage = `url('${userPhoto}')`;
        usernameElement.textContent = username;

        h5Element = chatHeader.querySelector('.col-auto.ms-2 h5');
        h5Element.innerHTML = `${userStatus}<span class="badge bg-green badge-blink ms-1"></span>`;

        recUserName = username
        recUserId = await getUserInfo(username);

        obj.userName = username;
        obj.skip = 0;

        var arr = await getMessageList(obj);

        var groupedArr = arr.reduce((arr, item) => {
            const key = (new Date(item.createdTime)).toISOString().split('T')[0];

            if (!arr[key])
                arr[key] = [];

            arr[key].push(item);

            return arr;
        }, {});

        console.log(groupedArr);

        arr.reverse().forEach(message => {

            if (first) {
                firstObj = message;
                first = false;
            }

            messageCreater(message.content, message.sender == curUserName ? 'justify-content-end' : '',
                message.sender, new Date(message.createdTime), false)

            overChatBubble.scrollTop = overChatBubble.scrollHeight;
        });

        overChatBubble.addEventListener('scroll', scrollLoad);

        usernameFlag = username;
    }
}

let scrollLoad = (async _ => {
    if (overChatBubble.scrollTop === 0) {
        overChatBubble.removeEventListener('scroll', scrollLoad);
        let offsetHeight = 0, first = true;
        obj.userName = recUserName;
        obj.skip += 9;

        console.log('scroll');


        var arr = await getMessageList(obj);

        var reduced = arr.reduce((arr, item) => {
            const key = (new Date(item.createdTime)).toISOString().split('T')[0];

            if (!arr[key])
                arr[key] = [];

            arr[key].push(item);

            return arr;
        }, {});

        console.log('arr => ');
        var d = Object.entries(reduced).map(([key, value]) => ({ key, value }));
        console.log(d);
        console.log(d[0].key);
        console.log(d[0].value);

        arr.reverse().forEach(message => {
            date = new Date(message.createdTime); 
        
            offsetHeight += (messageCreater(message.content, message.sender == curUserName ? 'justify-content-end' : '', message.sender,
                date, false));
        });
        
        overChatBubble.scrollTop = offsetHeight;

        console.log('end');
        overChatBubble.addEventListener('scroll', scrollLoad);
    }
});

overChatBubble.addEventListener("scroll", scrollLoad);

//#endregion

//#region Api's

async function getMessageList(receiver) {
    return await fetchApiPost('/MessagesApi/TakeMessages', receiver);
}

async function messageSaver(message, sender, receiver) {
    let object = {
        Content: message,
        Sender: sender,
        Receiver: receiver
    };

    return await fetchApiPost('/MessagesApi/MessageSaver', object);
}

//#endregion

//#region HelperMethods
async function sendMessage(event) {
    let message = document.getElementById("messageInput").value;

    if (recUserId !== null && message.trim().length != 0)
        if ((await messageSaver(message, curUserId, recUserId)) !== 0) {
            connection.invoke("SendToUser", recUserId, message).catch(err => console.error(err.toString()));
          
            messageCreater(message, 'justify-content-end', curUserName, new Date(), true, 'chat-bubble-me');
          
          overChatBubble.scrollTop = overChatBubble.scrollHeight;

            document.getElementById("messageInput").value = '';
        }

    event.preventDefault();
}
      
function messageCreater(content, style, sender, date, flag, messageColor) {
    const chatBubbles = document.querySelector(".chat-bubbles"),
        chatItem = document.createElement("div");
    let dayOffset, weekOffset = Math.floor(dayOffset / 7),
        monthOffset = Math.floor(dayOffset / 30),
        yearOffset = Math.floor(dayOffset / 365),
        timeOffSet = (new Date()) - date;

    dayOffset = Math.floor(timeOffSet / (1000 * 60 * 60 * 24))

    chatItem.classList.add("chat-item");
    chatItem.innerHTML = `
        <div class="row align-items-end ${style}">
            <div class="col col-lg-6">
                <div class="chat-bubble ${messageColor}">
                    <div class="chat-bubble-title">
                        <div class="row">
                            <div class="col chat-bubble-author">${sender}</div>
                            <div class="col-auto chat-bubble-date">${date.getHours().toString() + ':' + date.getMinutes().toString().padStart(2, '0')} </div>
                        </div>
                    </div>
                    <div class="chat-bubble-body">
                        <p>${content}</p>
                    </div>
                </div>
            </div>
        </div>`;

    if (flag) chatBubbles.append(chatItem)
    else chatBubbles.prepend(chatItem);

    return chatItem.offsetHeight;
}



//#endregion