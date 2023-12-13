const approval_div = document.getElementById("approval_div"),
    denial_div = document.getElementById("denial_div");

//#region Events

async function friendshipRequest(name) {
    if (await FetchApiPost('FriendshipRequest', name)) {
        let btn = document.getElementById(`btnId${name}`);
        btn.onclick = _ => pendingRequest(name);
        btn.setAttribute('data-bs-toggle', 'modal');
        btn.setAttribute('data-bs-target', '#modal-danger');
        btn.innerHTML = `<img width="16"  src="https://img.icons8.com/office/16/hourglass-sand-top.png" 
                          alt="hourglass-sand-top"/> Pending...`;

        let user = await getUser(name);
        if (user.id != null)
            connection.invoke("SendRequest", user, 'request')
                .catch(err => console.error(err.toString()));
    }
    else {
        window.alert("U can't send friendship request. Please refresh page and try again..)");
    }
}

async function pendingRequest(name) {
    create_modal_a_tags(cancelRequest, name, 'Return Back', 'Cancel Request')
}

async function cancelRequest(name) {
    if (await FetchApiPost('CancelRequest', name)) {
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

        let user = await getUser(name);
        console.log(user);
        if (user.id != null)
            connection.invoke("SendRequest", user, 'cancelled')
                .catch(err => console.error(err.toString()));
    }
    else
        window.alert("U can't cancel friendship request. Please refresh page and try again..)");
}

async function removeRequest(name) {
    create_modal_a_tags(removeFriend, name, 'Return Back', 'Remove Friend');
}

async function acceptRequest(name) {
    if (await FetchApiPost('AcceptRequest', name)) {
        let btn = document.getElementById(`btnId${name}`);
        btn.innerHTML = '';
        btn.onclick = _ => removeRequest(name);
        btn.setAttribute('data-bs-toggle', 'modal');
        btn.setAttribute('data-bs-target', '#modal-danger');
        btn.innerHTML = '<img width="18" src="https://img.icons8.com/external-justicon-lineal-color-justicon/64/external-friend-notifications-justicon-lineal-color-justicon.png" alt="external-friend-notifications-justicon-lineal-color-justicon"/> Friend...';
    }
    else
        window.alert("U can't accept friendship request. Please refresh page and try again..)");
}

async function declineRequest(name) {
    if (await FetchApiPost('DeclineRequest', name)) {
        let btn = document.getElementById(`btnId${name}`);
        btn.innerHTML = '';
        btn.onclick = _ => friendshipRequest(name);
        btn.removeAttribute('data-bs-toggle');
        btn.removeAttribute('data-bs-target');
        btn.innerHTML = `<svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-user-plus" width="24"
            viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round"
            stroke-linejoin="round">
            <path stroke="none" d="M0 0h24v24H0z" fill="none" />
            <path d="M8 7a4 4 0 1 0 8 0a4 4 0 0 0 -8 0" /><path d="M16 19h6" />
            <path d="M19 16v6" /><path d="M6 21v-2a4 4 0 0 1 4 -4h4" /></svg> Send Friendship`;
    }
    else
        window.alert("U can't decline friendship request. Please refresh page and try again..)");
}

async function removeFriend(name) {
    if (await FetchApiPost('RemoveFriend', name)) {
        let btn = document.getElementById(`btnId${name}`);
        btn.onclick = _ => friendshipRequest(name);
        btn.removeAttribute('data-bs-toggle');
        btn.removeAttribute('data-bs-target');
        btn.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-user-plus" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none" /><path d="M8 7a4 4 0 1 0 8 0a4 4 0 0 0 -8 0" /><path d="M16 19h6" /><path d="M19 16v6" /><path d="M6 21v-2a4 4 0 0 1 4 -4h4" /></svg> Send Friendship';
    }
    else
        window.alert("U can't remove friendship. Please refresh page and try again..)");
}

//#endregion

//#region Helper_Funtions

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
    return await FetchApiPost('GetUser', name);
}

//#endregion