
let list;
document.getElementById("search").oninput = async () => {
    let search = document.getElementById("search").value;
    list = await FetchApiPost('user/MainPage/FindUsers', search + ' ' + 10);
    if (list != 'false')
        ShowUsers(list);
}

function ShowUsers(list) {
    let l = document.getElementById("listUser");
    l.innerHTML = '';
    for (var i = 0; i < list.length; i++)
        l.innerHTML += `<li>${list[i].userName}</li>`;
}