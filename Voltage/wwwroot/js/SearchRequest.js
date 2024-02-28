const approval_div = document.getElementById("approval_div"),
    denial_div = document.getElementById("denial_div");

//#region Events

async function friendshipRequest(name) {
    let result = await fetchApiPost('/RequestApi/FriendshipRequest', name);

    if (result === 0) {
        let user = await getUser(curUserName),
            sender = await getUserInfo(name);

        if (user.UserName !== null && sender !== null) {
            pendingBtn(name, pendingRequest);
            sendRequestSignal(sender, user, 'request');
        }
    }
    else acceptOrDeclineBtn(name);
}

async function pendingRequest(name) {
    create_modal_a_tags(cancelRequest, name, 'Return Back', 'Cancel Request')
}

async function cancelRequest(name) {
    let result = await fetchApiPost('/RequestApi/CancelRequest', name),
        sender = await getUserInfo(name);

    if (result === 0) {
        if (curUserName !== null && sender !== null) {
            friendshipRequestBtn(name);
            sendRequestSignal(sender, curUserName, 'cancelled');
        }
    }
    else if (result === 1) friendBtn(name);
    else if (result === 2) acceptOrDeclineBtn(name);
    else friendshipRequestBtn(name);
}

async function removeRequest(name) {
    create_modal_a_tags(removeFriend, name, 'Return Back', 'Remove Friend');
}

async function acceptRequest(name) {
    let result = await fetchApiPost('/RequestApi/AcceptRequest', name);

    if (result) {
        friendBtn(name);
        createAlert(true, "Succsesfuly Accepted");
    }
    else {
        friendshipRequestBtn(name);
        createAlert(false, "Oops! Something went wrong...");
    }
}

async function declineRequest(name) {
    const declineBtn = document.getElementById(`btnIdDecline${name}`);
    if (declineBtn && declineBtn.parentNode) {
        declineBtn.parentNode.removeChild(declineBtn);
    }
    await fetchApiPost('/RequestApi/DeclineRequest', name);
    friendshipRequestBtn(name);
}

async function removeFriend(name) {
    await fetchApiPost('/RequestApi/RemoveFriend', name);
    console.log("pressed")
    if (result) friendshipRequestBtn(name);
    else acceptOrDeclineBtn(name);
}

//#endregion

//#region Helper_Functions

function create_modal_a_tags(methodName, parameter, denialText, approvalText) {
    const approvalBtn = document.createElement('a'),
        denialBtn = document.createElement('a');

    approval_div.innerHTML = '';
    denial_div.innerHTML = '';

    approvalBtn.classList = "btn-danger btn w-100";
    denialBtn.classList = "btn w-100";
    approvalBtn.setAttribute("data-bs-dismiss", "modal");
    denialBtn.setAttribute("data-bs-dismiss", "modal");
    denialBtn.innerText = denialText
    approvalBtn.innerText = approvalText
    approvalBtn.onclick = _ => methodName(parameter);

    approval_div.appendChild(approvalBtn);
    denial_div.appendChild(denialBtn);
}

async function getUser(name) {
    return await fetchApiGet(`/UserInfo/GetUser?name=${name}`);
}

function pendingBtn(name) {
    let btn = document.getElementById(`btnId${name}`);
    btn.onclick = _ => pendingRequest(name);
    btn.setAttribute('data-bs-toggle', 'modal');
    btn.setAttribute('data-bs-target', '#modal-danger');
    btn.innerHTML = `<img id = "changingGif" width="16"  src="/staticPhotos/output-onlinegiftools.gif" alt="hourglass-sand-top"/> Pending...`;
}

function friendshipRequestBtn(name) {
    let btn = document.getElementById(`btnId${name}`);
    btn.removeAttribute('data-bs-toggle');
    btn.removeAttribute('data-bs-target');
    btn.onclick = _ => friendshipRequest(name);
    btn.innerHTML = `<svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-user-plus" width="24"
            viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round"
            stroke-linejoin="round">
            <path stroke="none" d="M0 0h24v24H0z" fill="none" />
            <path d="M8 7a4 4 0 1 0 8 0a4 4 0 0 0 -8 0" /><path d="M16 19h6" />
            <path d="M19 16v6" /><path d="M6 21v-2a4 4 0 0 1 4 -4h4" /></svg> Send Friendship`;
}

function acceptOrDeclineBtn(name) {
    let div = document.getElementById(`DivId`)
    div.innerHTML = '';
    div.removeAttribute('data-bs-toggle');
    div.removeAttribute('data-bs-target');
    div.innerHTML = `
        <a id="btnId${name}" onclick="acceptRequest('${name}')" class="card-btn">
                                <svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-circle-check-filled" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M17 3.34a10 10 0 1 1 -14.995 8.984l-.005 -.324l.005 -.324a10 10 0 0 1 14.995 -8.336zm-1.293 5.953a1 1 0 0 0 -1.32 -.083l-.094 .083l-3.293 3.292l-1.293 -1.292l-.094 -.083a1 1 0 0 0 -1.403 1.403l.083 .094l2 2l.094 .083a1 1 0 0 0 1.226 0l.094 -.083l4 -4l.083 -.094a1 1 0 0 0 -.083 -1.32z" stroke-width="0" fill="currentColor"/></svg>
                                <span style="margin-right: 5px;" ></span>
                                Accept
        </a>
        <a id="btnIdDecline${name}" onclick="declineRequest('${name}')" class="card-btn">
                                <svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-circle-x-filled" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M17 3.34a10 10 0 1 1 -14.995 8.984l-.005 -.324l.005 -.324a10 10 0 0 1 14.995 -8.336zm-6.489 5.8a1 1 0 0 0 -1.218 1.567l1.292 1.293l-1.292 1.293l-.083 .094a1 1 0 0 0 1.497 1.32l1.293 -1.292l1.293 1.292l.094 .083a1 1 0 0 0 1.32 -1.497l-1.292 -1.293l1.292 -1.293l.083 -.094a1 1 0 0 0 -1.497 -1.32l-1.293 1.292l-1.293 -1.292l-.094 -.083z" stroke-width="0" fill="currentColor" /></svg>
                                <span style="margin-right: 5px;" ></span>
                                Decline
        </a>
     `;
}

function friendBtn(name) {
    let btn = document.getElementById(`btnId${name}`);
    const declineBtn = document.getElementById(`btnIdDecline${name}`);
    if (declineBtn && declineBtn.parentNode) {
        declineBtn.parentNode.removeChild(declineBtn);
    }
    btn.innerHTML = '';
    btn.onclick = _ => removeRequest(name);
    btn.setAttribute('data-bs-toggle', 'modal');
    btn.setAttribute('data-bs-target', '#modal-danger');
    btn.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-user-check" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M8 7a4 4 0 1 0 8 0a4 4 0 0 0 -8 0" /><path d="M6 21v-2a4 4 0 0 1 4 -4h4" /><path d="M15 19l2 2l4 -4" /></svg> <span style="margin-right: 5px;" ></span> Successfully Added';
}

async function sendRequestSignal(sender, user, status) {
    connection.invoke("SendRequest", sender, user, status)
        .catch(err => console.error(err.toString()));
}

//#endregion
