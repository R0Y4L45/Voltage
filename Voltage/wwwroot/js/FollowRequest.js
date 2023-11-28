async function followRequest(name) {
    await FetchApiPost('FollowRequest', name);
}