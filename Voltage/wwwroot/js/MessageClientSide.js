let date = recUserId = recUserName = null,
    overChatBubble = document.getElementById("overChatBubbles"),
    fileDropArea = document.getElementById("fileDropArea"),
    fileInput = document.getElementById("fileInput"),
    userStatus = document.getElementById("userStatus"),
    fileDescription = document.getElementById("fileDescription"),
    list = document.getElementById("messagesList"),
    messageSection = document.getElementById("MessageSection"),
    animationArea = document.getElementById("animationtext"), isBckButtonPressed = false,
    obj = {
        userName: '',
        skip: 0
    },
    usernameFlag, typingTimeout,
    onlineUsers = [], keySaverArr = [], readMessages = [],
    isEndOfMessages = false, lastMessageRead = false, isTyping = false;





//#region SignalR Connection and its events




function showNotification(sender, message, messageKey) {
    if (Notification.permission === 'granted') {
        if (!readMessages.includes(messageKey)) {
            let notification = new Notification('New Message from: ' + sender, {
                body: message,
                icon: '/dist/img/logos/logo-white.svg',
                vibrate:[200,100,200],
            });
            console.log("1 ci :", notification);
        }
    } else if (Notification.permission !== 'denied') {
        Notification.requestPermission().then(function (permission) {
            if (permission === 'granted') {
                if (!readMessages.includes(messageKey)) {
                    let notification = new Notification('New Message from:' + sender, {
                        body: message,
                        icon: '/dist/img/logos/logo-white.svg',
                        vibrate: [200, 100, 200],
                    });
                    console.log("2 ci :", notification);
                }
            }
        });
    }
}

connection.on("ReceiveTypingNotification", function (sender, messageText) {
    let typingIndicator = document.getElementById("typingIndicator");
    typingIndicator.textContent = sender + " печатает...";

    setTimeout(function () {
        typingIndicator.textContent = "";
    }, 3000);
});

function sendTypingNotification() {
    let messageText = getMessageText(); 
    connection.invoke("SendTypingNotification", curUserName, messageText).catch(err => console.error(err.toString()));
}




//overChatBubble.addEventListener("scroll", () => {
//    readMessages = [...new Set(keySaverArr)];
//    console.log(readMessages);
//});


//Messages Receiver
connection.on("ReceiveMessage", (user, message, createdTime) => {
    playNotificationSound();
    showNotification(user, message);
    let msgObj = { //new message obj created to send function
        sender: user,
        receiver: curUserName,
        content: message,
        createdTime: new Date(createdTime)
    };

    if (curUserName !== user) {
        messageCreater(msgObj, '', true);
        overChatBubble.scrollTop = overChatBubble.scrollHeight;
    }
});


connection.on("UserConnected", (userName) => {
    console.log(userName + " connected");
    if (!onlineUsers.includes(userName)) {
        onlineUsers.push(userName);
        if (userName === usernameFlag) 
            updateOnlineStatus(userName, true);
    }
    
});

connection.on("UserDisconnected", (userName) => {
    console.log(userName + " disconnected");
    const index = onlineUsers.indexOf(userName);
    if (index !== -1) {
        onlineUsers.splice(index, 1);
        if (userName === usernameFlag) 
            updateOnlineStatus(userName, false);
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

function playNotificationSound() {
    var audio = new Audio('/staticMusic/msgnotfmusic.mp3');
    console.log(audio);
    audio.play();
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

            if (isBckButtonPressed) messageAreas1.classList.remove('displaynone');
        }
        else {
            messageFriendList.classList.add('col-12', 'col-lg-5', 'col-xl-3', 'border-end');
            messageFriendList.classList.remove('displaynone');
            messageAreas1.classList.remove('col-12', 'col-lg-7', 'col-xl-12', 'd-flex', 'flex-column');
            messageAreas1.classList.add('col-12', 'col-lg-7', 'col-xl-9', 'd-flex', 'flex-column', 'hide-on-small-screen');
        }
    });
}



async function clickToUser(username) {
    if (window.innerWidth <= 993) usernameFlag = '';

    if (username !== usernameFlag) {
        overChatBubble.removeEventListener('scroll', scrollLoad);

        let chatHeader = document.getElementById("chatHeader"),
            avatarElement = chatHeader.querySelector('.avatar'),
            usernameElement = chatHeader.querySelector('.col-auto.ms-2 h4'),
            clickedElement = document.querySelector(`[data-user-name="${username}"]`),
            userPhoto = clickedElement.getAttribute('data-user-photo'),msgArr;



        showMessagesClick();

        document.querySelector(".chat-bubbles").innerHTML = '';
        avatarElement.style.backgroundImage = `url('${userPhoto}')`;
        usernameElement.textContent = username;

        updateOnlineStatus(username, checkUserOnline(username));

        recUserName = username
        recUserId = await getUserInfo(username);

        obj.userName = username;
        obj.skip = 0;
        isEndOfMessages = false;
        keySaverArr = [];

        //takeMessages return dictionary which contains keys and messages array
        msgArr = convert(await takeMessages(obj)); // Convert method convert Object to Dictionary
        stylishDiv.innerHTML = msgArr.length > 0 ? msgArr[msgArr.length - 1].key : '';
        showStylishDiv();

        msgArr.forEach(item => {
            let prmtr = '', flag = true;
            keySaverArr.push(item.key);

            if (msgArr[msgArr.length - 1] === item)
                flag = false;

            for (var i = 0; i < item.value.length; i++) {
                if (i === item.value.length - 1 && flag)
                    prmtr = item.key;

                messageCreater(item.value[i], prmtr)
            }
        });
        //let lastMessage = msgArr[msgArr.length - 1];
        //let lastMessageKey = lastMessage.key;
        //let lastMessageRead = readMessages.includes(lastMessageKey);
        //console.log('lastMessage',lastMessage);
        //console.log('lastMessageKey',lastMessageKey);
        //console.log('lastMessageRead',lastMessageRead);
        //
        //if (lastMessageRead) {
        //    console.log("last msg readed.");
        //} else {
        //    console.log("last msg readed.");
        //}



        overChatBubble.scrollTop = overChatBubble.scrollHeight;
        usernameFlag = username;

        overChatBubble.addEventListener('scroll', scrollLoad);
    }
}

function updateOnlineStatus(username, isOnline) {
    console.log(username, isOnline);
    userStatus.innerHTML = isOnline ? `online<span class="badge bg-green badge-blink ms-1"></span>`
            : `offline<span class="badge bg-red badge-blink ms-1"></span>`;
}



function convert(obj) {
    return Object.keys(obj).map(key => ({
        key: key,
        value: obj[key]
    }));
}

let scrollLoad = (async _ => {
    if (overChatBubble.scrollTop === 0) {
        overChatBubble.removeEventListener('scroll', scrollLoad);

        let offsetHeight = 0, msgArr;
        obj.userName = recUserName;
        obj.skip += 9;

        msgArr = convert(await takeMessages(obj));
        if (msgArr.length === 0 && !isEndOfMessages) {
            messageCreater(null, keySaverArr[keySaverArr.length - 1]);
            isEndOfMessages = true;
        }

        msgArr.forEach(item => {

            let prmtr = '',
                isDateBeforeMsg = false,
                isDateAfterMsg = true;

            keySaverArr.push(item.key);

            if (msgArr[0] === item && keySaverArr[keySaverArr.length - 1] !== msgArr[0].key)
                isDateBeforeMsg = true;

            if (msgArr[msgArr.length - 1] === item)
                isDateAfterMsg = false;

            for (var i = 0; i < item.value.length; i++) {
                if (i === 0 && isDateBeforeMsg) {
                    console.log(isDateBeforeMsg);
                    messageCreater(null, keySaverArr[keySaverArr.length - 1]);
                }

                if (i === item.value.length - 1 && isDateAfterMsg)
                    prmtr = item.key;

                offsetHeight += messageCreater(item.value[i], prmtr);
            }
        });

        overChatBubble.scrollTop = offsetHeight;
        overChatBubble.addEventListener('scroll', scrollLoad);
    }
});

if (overChatBubble)overChatBubble.addEventListener("scroll", scrollLoad);

//#endregion

//#region Api's

async function takeMessages(receiver) {
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

function checkUserOnline(username) {
    return onlineUsers.includes(username);
}

async function sendMessage(event) {
    let message = document.getElementById("messageInput").value;

    if (recUserId !== null && message.trim().length != 0)
        if ((await messageSaver(message, curUserId, recUserId)) !== 0) {
            connection.invoke("SendToUser", recUserId, message).catch(err => console.error(err.toString()));
            let msgObj = { //new message obj created to send function
                sender: curUserName,
                receiver: recUserName,
                content: message,
                createdTime: new Date()
            };

            messageCreater(msgObj, '', true);
            overChatBubble.scrollTop = overChatBubble.scrollHeight;
            document.getElementById("messageInput").value = '';
        }

    event.preventDefault();
}

let stylishDiv = document.getElementById("stylishDiv");
function messageCreater(msgObj, msgGroupedDate, flag = false) {
    const chatBubbles = document.querySelector(".chat-bubbles"),
        chatItem = document.createElement("div");

    chatItem.classList.add("chat-item");
    if (msgObj !== null) {
        date = new Date(msgObj.createdTime);

        chatItem.innerHTML = `
        <div class="row align-items-end p ${msgObj.sender == curUserName ? 'justify-content-end' : ''}">
            ${msgGroupedDate !== '' ? `<p class='text-center'>${msgGroupedDate}</p>` : ''}
            <div class="col col-lg-6">
                <div class="chat-bubble ${msgObj.sender == curUserName ? 'chat-bubble-me' : ''}">
                    <div class="chat-bubble-title">
                        <div class="row">
                            <div class="col chat-bubble-author">${msgObj.sender}</div>
                            <div class="col-auto chat-bubble-date">${date.getHours().toString() + ':' + date.getMinutes().toString().padStart(2, '0')}</div>
                        </div>
                    </div>
                    <div class="chat-bubble-body">
                        <p>${msgObj.content}</p>
                    </div>
                </div>
            </div>
        </div>`;
    }
    else {
        chatItem.innerHTML = `
        <div class="row align-items-end">
            ${msgGroupedDate !== '' ? `<p id="groupedDateText" class='text-center'>${msgGroupedDate}</p>` : ''}
        </div>`
    }

    if (flag) chatBubbles.append(chatItem)
    else chatBubbles.prepend(chatItem);

    return chatItem.offsetHeight;
}

function showStylishDiv() {
    stylishDiv.classList.remove("hide");
    setTimeout(function () {
        stylishDiv.classList.add("hide");
    }, 1000);
}

if (overChatBubble) overChatBubble.addEventListener("scroll", showStylishDiv);

//#endregion