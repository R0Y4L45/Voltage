let curUserName = curUserId = null,
    connection = new signalR.HubConnectionBuilder().withUrl("/signalRHub").withAutomaticReconnect().build()

connection.start().then(() => {
    connection.invoke("GetUserName").then(user => curUserName = user);
    connection.invoke("GetConnectionId").then(id => curUserId = id);
}).catch(err => console.error(err.toString()));

