let tbody = document.querySelector('tbody');

connection.on("ReceiveRequests", (user, status) => {
    if (user != null) {
        if (status === 'request') {
            gotRequest(user);
        }
        else if (status === 'cancelled') {
            cancelledRequest(user);
        }
    }
})


window.onload = getRequestList();

async function getRequestList() {
    (await fetchApiGet('/UserInfo/GetRequestList')).forEach(i => gotRequest(i));
}

function gotRequest(user) {
    if (document.getElementById(`tr${user.userName}`) === null) {
        let tr = document.createElement('tr');
        tr.id = `tr${user.userName}`;
        tr.innerHTML = `
                <td class="text-sm-center">
                    <span class="avatar" style="background-image: url(${user.photo})"></span>
                </td>
                <td class="text-sm-center" data-label="Name">
                    <div class="d-flex py-1 align-items-center">
                        <div class="flex-fill">
                            <div class="font-weight-medium">${user.userName}</div>
                            <div class="text-secondary"><a href="#" class="text-reset">${user.email}</a></div>
                        </div>
                    </div>
                </td>
                <td class="text-sm-center">
                    <span class="me-2"></span>
                    ${user.country}
                </td>
                <td class="text-secondary text-sm-center" data-label="Role">
                    ${user.role}
                </td>
                <td>
                    <div class="align-items-center">
                        <div class="class="col-6 col-sm-4 col-md-2 col-xl-auto py-3">
                            <a onclick="acceptRequest('${user.userName}')" class="btn btn-icon btn-green w-100" style="display: inline-flex;">
                                <svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-check" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none" /><path d="M5 12l5 5l10 -10" /></svg>
                            </a>
                        </div>
                    </div>
                </td>
                <td>
                    <div class="align-items-center">
                        <div class="class="col-6 col-sm-4 col-md-2 col-xl-auto py-3">
                            <a onclick="declineRequest('${user.userName}')" class="btn btn-icon btn-red w-100" style="display: inline-flex;">
                                <svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-x" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none" /><path d="M18 6l-12 12" /><path d="M6 6l12 12" /></svg>
                            </a>
                        </div>
                    </div>
                </td>`;

        tbody.appendChild(tr);
    }
}


function cancelledRequest(user) {
    tbody.removeChild(document.getElementById(`tr${user}`));
}

async function acceptRequest(name) {
    await fetchApiPost('/RequestApi/AcceptRequest', name);
    createAlert(true, "Succsesfuly Accepted");
    var audio = new Audio('/staticMusic/frndnotfmusic.mp3');
    audio.play();
    tbody.removeChild(document.getElementById(`tr${name}`));

    let currentCount = parseInt(localStorage.getItem('requestCount') || '0');
    currentCount--;
    updateRequestCounter(currentCount);
}

async function declineRequest(name) {
    await fetchApiPost('/RequestApi/DeclineRequest', name);
    tbody.removeChild(document.getElementById(`tr${name}`));

    let currentCount = parseInt(localStorage.getItem('requestCount') || '0');
    currentCount--;
    updateRequestCounter(currentCount);
}

function updateRequestCounter(currentCount) {
    requestCounter.textContent = currentCount.toString();
    localStorage.setItem('requestCount', currentCount.toString());
    if (currentCount === 0) 
        requestCounter.style.display = 'none';
     else 
        requestCounter.style.display = 'inline-block';
    
}
