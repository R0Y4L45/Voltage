const approval_div = document.getElementById("approval_div"),
    denial_div = document.getElementById("denial_div");

async function friendshipRequest(name) {
    if (await FetchApiPost('FriendshipRequest', name)) {
        let btn = document.getElementById(`btnId${name}`);
        btn.onclick = _ => pendingRequest(name);
        btn.setAttribute('data-bs-toggle', 'modal');
        btn.setAttribute('data-bs-target', '#modal-danger');
        btn.innerHTML = '<img width="16" height="16" src="https://img.icons8.com/office/16/hourglass-sand-top.png" alt="hourglass-sand-top"/> Pending...';
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
        btn.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-user-plus" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none" /><path d="M8 7a4 4 0 1 0 8 0a4 4 0 0 0 -8 0" /><path d="M16 19h6" /><path d="M19 16v6" /><path d="M6 21v-2a4 4 0 0 1 4 -4h4" /></svg> Send Friendship';
    }
}

async function removeRequest(name) {
    create_modal_a_tags(removeFriend, name, 'Return Back', 'Remove Friend');
}

async function removeFriend(name) {
    if (await FetchApiPost('RemoveFriend', name)) {
        let btn = document.getElementById(`btnId${name}`);
        btn.onclick = _ => friendshipRequest(name);
        btn.removeAttribute('data-bs-toggle');
        btn.removeAttribute('data-bs-target');
        btn.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-user-plus" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none" /><path d="M8 7a4 4 0 1 0 8 0a4 4 0 0 0 -8 0" /><path d="M16 19h6" /><path d="M19 16v6" /><path d="M6 21v-2a4 4 0 0 1 4 -4h4" /></svg> Send Friendship';
    }
}

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