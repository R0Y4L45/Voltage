let tbody = document.querySelector('tbody');

connection.on("ReceiveRequests", (user, status) => {
    console.log(user);
    console.log(status);
    if (user.id != null) {
        console.log('is not null..!');
        if (status === 'request')
            gotRequest(user);
        else if (status === 'cancelled')
            cancelledRequest(user);
    }
})

window.onload = getRequestList();

async function getRequestList() {
    (await FetchApiPost('GetRequestList')).forEach(i => gotRequest(i));
}


function gotRequest(user) {
    let tr = document.createElement('tr');
    tr.id = `tr${user.userName}`;
    tr.innerHTML = `
            <tr>
                <td data-label="Name">
                    <div class="d-flex py-1 align-items-center">
                        <span class="avatar me-2" style="background-image: url(${user.photo})"></span>
                        <div class="flex-fill">
                            <div class="font-weight-medium">${user.userName}</div>
                            <div class="text-secondary"><a href="#" class="text-reset">${user.email}</a></div>
                        </div>
                    </div>
                </td>
                <td>
                    <span class="flag flag-xs flag-country-az me-2"></span>
                    ${user.country}
                </td>
                <td class="text-secondary" data-label="Role">
                    ${user.role}
                </td>
                <td>
                    <div class="btn-list d-flex flex-nowrap">
                        <div class="col py-3">
                            <a href="#" class="btn btn-icon btn-green w-100">
                                <svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-check" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none" /><path d="M5 12l5 5l10 -10" /></svg>
                            </a>
                        </div>
                        <div class="col py-3">
                            <a href="#" class="btn btn-icon btn-red w-100">
                                <svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-x" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none" /><path d="M18 6l-12 12" /><path d="M6 6l12 12" /></svg>
                            </a>
                        </div>
                    </div>
                </td>
            </tr>`;

    tbody.appendChild(tr);
}

function cancelledRequest(user) {
    tbody.removeChild(document.getElementById(`tr${user.userName}`));
}