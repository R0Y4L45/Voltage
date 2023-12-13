let curUserName = curUserId = null,
    connection = new signalR.HubConnectionBuilder().withUrl("/signalRHub").withAutomaticReconnect().build(),
    notification = document.getElementById('notification');

connection.start().then(() => {
    connection.invoke("GetUserName").then(user => curUserName = user);
    connection.invoke("GetConnectionId").then(id => curUserId = id);
});

async function FetchApiPost(methodName, object) {
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

