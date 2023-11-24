const cardDiv = document.getElementById("cardDiv"),
    showMore = document.getElementById('showMore');
let result, count, searchObj;

searchObj = {
    content: '',
    take: 4,
    skip: 0
};

document.getElementById('searchUsers').oninput = async _ => {
    searchObj.content = document.getElementById('searchUsers').value;

    result = await FetchApiPost('FindUsers', searchObj);
    count = result.count;
    console.log(searchObj.skip);

    document.getElementById('countOfUsers').innerHTML = count == undefined ? '' :
        count > 1 ? count + ' Users' : count + ' User';

    cardDiv.innerHTML = '';
    if (count > 0) {
        ShowUsers(result.users);

        if (result.next) await AddShowMoreButton();
        else showMore.innerHTML = '';
    }
}

function ShowUsers(list) {
    list.forEach(i => {
        let d = document.createElement('div');
        d.classList = "col-md-6 col-lg-3";
        d.innerHTML =
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
                    <a href="#" class="card-btn">
                        <svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-user-plus" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none" /><path d="M8 7a4 4 0 1 0 8 0a4 4 0 0 0 -8 0" /><path d="M16 19h6" /><path d="M19 16v6" /><path d="M6 21v-2a4 4 0 0 1 4 -4h4" /></svg>
                        
                        Follow
                    </a>
                </div>
            </div>`

        cardDiv.appendChild(d);
    })
}

async function AddShowMoreButton() {
    let btn = document.createElement('button');

    btn.innerHTML = "Show More";
    btn.classList = 'btn btn-outline-warning btn-lg';
    btn.addEventListener('click', ClickShowMore);

    showMore.innerHTML = '';
    showMore.appendChild(btn);
};

async function ClickShowMore() {
    searchObj.content = document.getElementById('searchUsers').value;
    searchObj.skip += 4;
    sessionStorage.setItem('skip', searchObj.skip)
    console.log(searchObj.skip);

    result = await FetchApiPost('FindUsers', searchObj);

    if (count > 0) {
        ShowUsers(result.users);
        if (!result.next) { 
            showMore.innerHTML = '';
            searchObj.skip = 0;
        }
    }
}