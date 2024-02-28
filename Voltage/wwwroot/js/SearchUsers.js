const cardDiv = document.getElementById("cardDiv"),
    showMore = document.getElementById('showMore');

let result, count,
    searchObj = {
        content: '',
        take: 4,
        skip: 0
    };

document.getElementById('searchUsers').oninput = async _ => {
    searchObj.content = document.getElementById('searchUsers').value;
    result = await fetchApiPost('/SearchUsersApi/SearchUsers', searchObj);

    count = result.count;
    document.getElementById('countOfUsers').innerHTML = count == undefined ? '' :
        count > 1 ? count + ' Users' : count + ' User';

    cardDiv.innerHTML = '';
    if (count > 0) {
        showUsers(result.users);

        if (result.next) addShowMoreButton();
        else showMore.innerHTML = '';
    }
    else showMore.innerHTML = '';

}

function showUsers(list) {
    let methodName, btnContent, modalAttribute, icon, div;
    list.forEach(i => {
        methodName = btnContent = modalAttribute = icon = '', div = document.createElement('div');
        div.classList = "col-md-6 col-lg-3";

        if (i.requestStatus == 2) {
            methodName = `removeRequest('${i.userName}')`;
            btnContent = ' Remove Friend ';
            modalAttribute = `data-bs-toggle="modal" data-bs-target="#modal-danger"`;
            icon = '<svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-user-minus" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M8 7a4 4 0 1 0 8 0a4 4 0 0 0 -8 0" /><path d="M6 21v-2a4 4 0 0 1 4 -4h4c.348 0 .686 .045 1.009 .128" /><path d="M16 19h6" /></svg>';
            //<img id = "changingGif" width="16"  src="/staticPhotos/output-onlinegiftools.gif"
            //alt = "hourglass-sand-top" /> Pending...
            //Bunan  deysmey untma
        }
        else if (i.requestStatus == null) {
            methodName = `friendshipRequest('${i.userName}')`;
            btnContent = 'Send Friendship';
            icon = '<svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-user-plus" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none" /><path d="M8 7a4 4 0 1 0 8 0a4 4 0 0 0 -8 0" /><path d="M16 19h6" /><path d="M19 16v6" /><path d="M6 21v-2a4 4 0 0 1 4 -4h4" /></svg>';
        }
        else {
            if (i.requestStatus == 1 && i.senderName == i.userName) {
                methodName = `acceptRequest('${i.userName}')`;
                icon = '<svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-circle-check-filled" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M17 3.34a10 10 0 1 1 -14.995 8.984l-.005 -.324l.005 -.324a10 10 0 0 1 14.995 -8.336zm-1.293 5.953a1 1 0 0 0 -1.32 -.083l-.094 .083l-3.293 3.292l-1.293 -1.292l-.094 -.083a1 1 0 0 0 -1.403 1.403l.083 .094l2 2l.094 .083a1 1 0 0 0 1.226 0l.094 -.083l4 -4l.083 -.094a1 1 0 0 0 -.083 -1.32z" stroke-width="0" fill="currentColor"/></svg>'
                btnContent = 'Accept';
            }
            else {
                methodName = `pendingRequest('${i.userName}')`;
                btnContent = 'Pending...';
                modalAttribute = `data-bs-toggle="modal" data-bs-target="#modal-danger"`;
                icon = '<img width="16" height="16" src="https://img.icons8.com/office/16/hourglass-sand-top.png" alt="hourglass-sand-top"/>';
            }
        }

        div.innerHTML =
            `<div class="card">
                    <div class="card-body p-4 text-center">
                        <span class="avatar avatar-xl mb-3 rounded" style="background-image: url(${i.photo})"></span>
                        <h3 class="m-0 mb-1"><a href="#">${i.userName}</a></h3>
                        <div class="text-secondary">${i.country}</div>
                        <div class="mt-3">
                            <span class="badge bg-purple-lt">Owner</span>
                        </div>
                    </div>
                    <div id="DivId" class="d-flex">
                         <a id="btnId${i.userName}" onclick="${methodName}" class="card-btn" ${modalAttribute}>
                            ${icon}
                            <span style="margin-right: 5px;" ></span>
                            ${btnContent}
                        </a>
                        ${generateDeclineIcon(i)}
                    </div>
                </div>`;

        cardDiv.appendChild(div);
    });
}

function generateDeclineIcon(user) {
    let methodName, icon, btnContent;
    if (user.requestStatus === 1 && user.senderName === user.userName) {
        methodName = `onclick="declineRequest('${user.userName}')"`;
        icon = '<svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-circle-x-filled" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M17 3.34a10 10 0 1 1 -14.995 8.984l-.005 -.324l.005 -.324a10 10 0 0 1 14.995 -8.336zm-6.489 5.8a1 1 0 0 0 -1.218 1.567l1.292 1.293l-1.292 1.293l-.083 .094a1 1 0 0 0 1.497 1.32l1.293 -1.292l1.293 1.292l.094 .083a1 1 0 0 0 1.32 -1.497l-1.292 -1.293l1.292 -1.293l.083 -.094a1 1 0 0 0 -1.497 -1.32l-1.293 1.292l-1.293 -1.292l-.094 -.083z" stroke-width="0" fill="currentColor" /></svg>'
        btnContent = 'Decline';
    } else {
        methodName = '';
        icon = '';
        btnContent = '';
        return '';
    }

    return `
        <a id="btnIdDecline${user.userName}" ${methodName} class="card-btn">
            ${icon}
            <span style="margin-right: 5px;" ></span>
            ${btnContent}
        </a>`;
}

function addShowMoreButton() {
    let btn = document.createElement('button');
    btn.innerHTML = "Show More";
    btn.classList = 'btn btn-outline-info btn-lg';
    btn.addEventListener('click', clickShowMore);

    showMore.innerHTML = '';
    showMore.appendChild(btn);
};

async function clickShowMore() {
    searchObj.content = document.getElementById('searchUsers').value;
    searchObj.skip += 4;
    sessionStorage.setItem('skip', searchObj.skip)

    result = await fetchApiPost('/SearchUsersApi/SearchUsers', searchObj);

    if (count > 0) {
        showUsers(result.users);
        if (!result.next) {
            showMore.innerHTML = '';
            searchObj.skip = 0;
        }
    }
    else showMore.innerHTML = '';
}