connection.on("ReceiveRequests", (user, message, createTime) => {
    console.log(user);
    console.log(message);
    console.log("hello notify");
})