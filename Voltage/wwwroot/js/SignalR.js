let curUserName = curUserId = null,
    connection = new signalR.HubConnectionBuilder().withUrl("/signalRHub").withAutomaticReconnect().build(),
    notification = document.getElementById('notification');

connection.start().then(() => {
    connection.invoke("GetUserName").then(user => curUserName = user);
    connection.invoke("GetConnectionId").then(id => curUserId = id);
});

async function fetchApiPost(methodName, object) {
    return await fetch(methodName, {
        method: 'Post',
        headers: {
            'Content-type': 'application/json'
        },
        body: JSON.stringify(object)
    })
        .then(response => response.json())
        .then(data => data)
        .catch(error => console.error(error));
}

async function fetchApiGet(url) {
    return await fetch(url)
        .then(response => {
            if (response.ok) {
                const contentType = response.headers.get('Content-Type');

                if (contentType && contentType.includes('application/json')) return response.json();
                else return response.text();
            }
        });
}

async function getUserInfo(name) {
    return await fetchApiGet(`/UserInfo/GetId?name=${name}`);
}

//document.addEventListener('contextmenu', function (e) {
//    e.preventDefault();
//});

//document.onkeydown = (e) => {
//    if (e.key == 'F12') {
//        e.preventDefault();
//    }
//    if (e.ctrlKey && e.shiftKey && e.key == 'I') {
//        e.preventDefault();
//    }
//    if (e.ctrlKey && e.shiftKey && e.key == 'C') {
//        e.preventDefault();
//    }
//    if (e.ctrlKey && e.shiftKey && e.key == 'J') {
//        e.preventDefault();
//    }
//    if (e.ctrlKey && e.key == 'U') {
//        e.preventDefault();
//    }
//};