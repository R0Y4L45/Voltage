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
    result = await FetchApiPost('SearchUsers', searchObj);

    count = result.count;
    document.getElementById('countOfUsers').innerHTML = count == undefined ? '' :
        count > 1 ? count + ' Users' : count + ' User';

    cardDiv.innerHTML = '';
    if (count > 0) {
        showUsers(result.users);

        if (result.next) await addShowMoreButton();
        else showMore.innerHTML = '';
    }
    else showMore.innerHTML = '';
}

function showUsers(list) {
    let methodName, btnContent, modalAttribute;

    list.forEach(i => {
        let div = document.createElement('div');

        div.classList = "col-md-6 col-lg-3";

        if (i.requestStatus == 2) {
            methodName = 'removeFriend';
            btnContent = 'Friend...';
            modalAttribute = `data-bs-toggle="modal" data-bs-target="#modal-danger"`;
        }
        else if (i.requestStatus == null) {
            methodName = 'friendshipRequest';
            btnContent = 'Send Friendship';
        }
        else {
            if (i.requestStatus == 1 && i.senderName == i.userName) {
                methodName = 'answerRequest';
                btnContent = 'Accept or Decline';
            }
            else {
                methodName = 'pendingRequest';
                btnContent = 'Pending...';
                modalAttribute = `data-bs-toggle="modal" data-bs-target="#modal-danger"`;
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
                    <div class="d-flex">
                         <a id="btnId${i.userName}" onclick="${methodName}('${i.userName}')" class="card-btn" ${modalAttribute}>
                            <svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-user-plus" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none" /><path d="M8 7a4 4 0 1 0 8 0a4 4 0 0 0 -8 0" /><path d="M16 19h6" /><path d="M19 16v6" /><path d="M6 21v-2a4 4 0 0 1 4 -4h4" /></svg>
                            ${btnContent}
                        </a>
                    </div>
                </div>`;

        cardDiv.appendChild(div);
    });
}

async function addShowMoreButton() {
    let btn = document.createElement('button');
    btn.innerHTML = "Show More";
    btn.classList = 'btn btn-outline-warning btn-lg';
    btn.addEventListener('click', clickShowMore);

    showMore.innerHTML = '';
    showMore.appendChild(btn);
};

async function clickShowMore() {
    searchObj.content = document.getElementById('searchUsers').value;
    searchObj.skip += 4;
    sessionStorage.setItem('skip', searchObj.skip)

    result = await FetchApiPost('SearchUsers', searchObj);

    console.log(result);

    if (count > 0) {
        showUsers(result.users);
        if (!result.next) {
            showMore.innerHTML = '';
            searchObj.skip = 0;
        }
    }
    else showMore.innerHTML = '';
}