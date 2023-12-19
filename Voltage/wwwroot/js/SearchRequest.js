const approval_div = document.getElementById("approval_div"),
    denial_div = document.getElementById("denial_div");

//#region Events

async function friendshipRequest(name) {
    let result = await FetchApiPost('/RequestApi/FriendshipRequest', name);

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
    let result = await FetchApiPost('/RequestApi/CancelRequest', name),
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
    let result = await FetchApiPost('/RequestApi/AcceptRequest', name);

    if (result) friendBtn(name);
    else friendshipRequestBtn(name);
}

async function declineRequest(name) {
    await FetchApiPost('/RequestApi/DeclineRequest', name);
    friendshipRequestBtn(name);
}

async function removeFriend(name) {
    let result = await FetchApiPost('/RequestApi/RemoveFriend', name)

    console.log(result);

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

async function getUserInfo(name) {
    return await fetchApiGet(`/UserInfo/GetId?name=${name}`);
}

async function getUser(name) {
    return await fetchApiGet(`/UserInfo/GetUser?name=${name}`)
}

function pendingBtn(name) {
    let btn = document.getElementById(`btnId${name}`);
    btn.onclick = _ => pendingRequest(name);
    btn.setAttribute('data-bs-toggle', 'modal');
    btn.setAttribute('data-bs-target', '#modal-danger');
    btn.innerHTML = `<img width="16"  src="https://img.icons8.com/office/16/hourglass-sand-top.png" 
                          alt="hourglass-sand-top"/> Pending...`;
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
    let btn = document.getElementById(`btnId${name}`);
    btn.onclick = '';
    btn.innerHTML = '';
    btn.removeAttribute('data-bs-toggle');
    btn.removeAttribute('data-bs-target');
    btn.innerHTML = `<button style="width:45px; heigth: 18px;  margin:0, 9px, 0, 0" class="btn btn-success" onclick="acceptRequest('${name}')">Accept</button> <button style="width:45px; heigth: 18px; margin:0, 0, 0, 9px" class="btn btn-danger" onclick="declineRequest('${name}')">Decline</button>`;
}

function friendBtn(name) {
    let btn = document.getElementById(`btnId${name}`);
    btn.innerHTML = '';
    btn.onclick = _ => removeRequest(name);
    btn.setAttribute('data-bs-toggle', 'modal');
    btn.setAttribute('data-bs-target', '#modal-danger');
    btn.innerHTML = '<img width="18" src="https://img.icons8.com/external-justicon-lineal-color-justicon/64/external-friend-notifications-justicon-lineal-color-justicon.png" alt="external-friend-notifications-justicon-lineal-color-justicon"/> Friend...';
}

async function sendRequestSignal(sender, user, status) {
    connection.invoke("SendRequest", sender, user, status)
        .catch(err => console.error(err.toString()));
}

//#endregion