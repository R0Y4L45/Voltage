const approval_div = document.getElementById("approval_div"),
    denial_div = document.getElementById("denial_div");

async function friendshipRequest(name) {
    if (await FetchApiPost('FriendshipRequest', name)) {
        let btn = document.getElementById(`btnId${name}`);
        btn.onclick = _ => pendingRequest(name);
        btn.innerText = 'Pending...';
    }

    console.log('Follow');
}

async function cancelRequest(name) {
    //const followBtn = document.getElementById('followBtn' + name);
    console.log("cancelRequest");
    //await FetchApiPost('FollowRequest', name);
}

async function pendingRequest(name) {
    const approvalBtn = document.createElement('a'),
        denialBtn = document.createElement('a');

    approvalBtn.classList = "btn-danger btn w-100";
    denialBtn.classList = "btn w-100";
    approvalBtn.setAttribute("data-bs-dismiss", "modal");
    denialBtn.setAttribute("data-bs-dismiss", "modal");
    denialBtn.id = `denialBtn${name}`;
    approvalBtn.id = `denialBtn${name}`;
    denialBtn.innerText = 'Return Back';
    approvalBtn.innerText = 'Cancel Request';
    approvalBtn.onclick = _ => cancelRequest(name);

    approval_div.innerHTML = '';
    denial_div.innerHTML = '';

    approval_div.appendChild(approvalBtn);
    denial_div.appendChild(denialBtn);
    console.log("Pending");
}

async function cancelRequest(name) {
    console.log(name);

    if (await FetchApiPost('CancelRequest', name)) {
        let btn = document.getElementById(`btnId${name}`);
        btn.onclick = _ => friendshipRequest(name);
        btn.innerText = 'Follow';
    }
}

async function removeFriend(name) {
    console.log("removeFriend");

    const approvalBtn = document.createElement('a'),
        denialBtn = document.createElement('a');

    approvalBtn.classList = "btn-danger btn w-100";
    denialBtn.classList = "btn w-100";
    approvalBtn.setAttribute("data-bs-dismiss", "modal");
    denialBtn.setAttribute("data-bs-dismiss", "modal");
    denialBtn.id = `denialBtn${name}`;
    approvalBtn.id = `denialBtn${name}`;
    denialBtn.innerText = 'Return Back';
    approvalBtn.innerText = 'Remove Friend';
    approvalBtn.onclick = _ => removeFriend(name);

    approval_div.innerHTML = '';
    denial_div.innerHTML = '';

    approval_div.appendChild(approvalBtn);
    denial_div.appendChild(denialBtn);
}

async function removeFriend(name) {
    console.log(name);

    if (await FetchApiPost('RemoveFriend', name))
        document.getElementById(`btnId${name}`).innerText = 'Send Friendship';
}