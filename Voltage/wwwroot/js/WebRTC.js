const conn = new signalR.HubConnectionBuilder().withUrl("/WebRTCHub").build(),
    configuration = {
        'iceServers': [{
            'urls': 'stun:stun.l.google.com:19302'
        }]
    },
    peerConn = new RTCPeerConnection(configuration),
    roomNameTxt = document.getElementById('roomNameTxt'),
    createRoomBtn = document.getElementById('createRoomBtn'),
    roomTable = document.getElementById('roomTable'),
    connStatusMessage = document.getElementById('connStatusMessage'),
    fileInput = document.getElementById('fileInput'),
    sendFileBtn = document.getElementById('sendFileBtn'),
    fileTable = document.getElementById('fileTable'),
    localVideo = document.getElementById('localVideo'),
    remoteVideo = document.getElementById('remoteVideo')

let myRoomId,
    localStream,
    remoteStream,
    fileReader,
    isInitiator = hasRoomJoined = false;

fileInput.disabled = true;
sendFileBtn.disabled = true;

$(roomTable).DataTable({
    columns: [
        { data: 'RoomId', "width": "30%" },
        { data: 'Name', "width": "50%" },
        { data: 'Button', "width": "15%" }
    ],
    "lengthChange": false,
    "searching": false,
    "language": {
        "emptyTable": "No room available"
    }
});

//setup my video here.
grabWebCamVideo();

/****************************************************************************
* Signaling server
****************************************************************************/

// Connect to the signaling server
conn.start().then(() => {
    conn.on('updateRoom', data => $(roomTable).DataTable().clear().rows.add(JSON.parse(data)).draw());

    conn.on('created', roomId => {
        console.log('Created room', roomId);
        roomNameTxt.disabled = true;
        createRoomBtn.disabled = true;
        hasRoomJoined = true;
        connStatusMessage.innerText = 'You created Room ' + roomId + '. Waiting for participants...';
        myRoomId = roomId;
        isInitiator = true;
    });

    conn.on('joined', roomId => {
        console.log('This peer has joined room', roomId);
        myRoomId = roomId;
        isInitiator = false;
    });

    conn.on('error', message => alert(message));

    conn.on('ready', () => {
        console.log('Socket is ready');
        roomNameTxt.disabled = true;
        createRoomBtn.disabled = true;
        hasRoomJoined = true;
        connStatusMessage.innerText = 'Connecting...';
        createPeerConnection(isInitiator, configuration);
    });

    conn.on('message', message => {
        console.log('Client received message:', message);
        signalingMessageCallback(message);
    });

    conn.on('bye', function () {
        console.log(`Peer leaving room.`);
        // If peer did not create the room, re-enter to be creator.
        connStatusMessage.innerText = `Other peer left room ${myRoomId}.`;
    });

    window.addEventListener('unload', () => {
        if (hasRoomJoined) {
            console.log(`Unloading window. Notifying peers in ${myRoomId}.`);
            conn.invoke("LeaveRoom", myRoomId).catch(err => console.error(err.toString()));
        }
    });

    //Get room list.
    conn.invoke("GetRoomInfo").catch(err => console.error(err.toString()));

}).catch(err => console.error(err.toString()));

/**
* Send message to signaling server
*/
function sendMessage(message) {
    console.log('Client sending message: ', message);
    conn.invoke("SendMessage", myRoomId, message).catch(err => console.error(err.toString()));
}

/****************************************************************************
* Room management
****************************************************************************/

$(createRoomBtn).click(() => {
    let name = roomNameTxt.value;
    conn.invoke("CreateRoom", name).catch(err => console.error(err.toString()));
});

$('#roomTable tbody').on('click', 'button', () => {
    if (hasRoomJoined) alert('You already joined the room. Please use a new tab or window.');
    else {
        let data = $(roomTable).DataTable().row($(this).parents('tr')).data();
        conn.invoke("Join", data.RoomId).catch(err => console.error(err.toString()));
    }
});

$(fileInput).change(() => {
    let file = fileInput.files[0];
    sendFileBtn.disabled = file ? false : true;
});

$(sendFileBtn).click(() => {
    sendFileBtn.disabled = true;
    sendFile();
});

/****************************************************************************
* User media (webcam)
****************************************************************************/

function grabWebCamVideo() {
    console.log('Getting user media (video) ...');
    navigator.mediaDevices.getUserMedia({
        audio: true,
        video: true
    })
        .then(gotStream())
        .catch(e => alert('getUserMedia() error: ' + e.name));
}

function gotStream(stream) {
    console.log('getUserMedia video stream URL:', stream);
    localStream = stream;
    peerConn.addStream(localStream);
    localVideo.srcObject = stream;
}

/****************************************************************************
* WebRTC peer conn and data channel
****************************************************************************/

var dataChannel;

function signalingMessageCallback(message) {
    if (message.type === 'offer') {
        console.log('Got offer. Sending answer to peer.');
        peerConn.setRemoteDescription(new RTCSessionDescription(message), function () { },
            logError);
        peerConn.createAnswer(onLocalSessionCreated, logError);

    } else if (message.type === 'answer') {
        console.log('Got answer.');
        peerConn.setRemoteDescription(new RTCSessionDescription(message), function () { },
            logError);

    } else if (message.type === 'candidate') {
        peerConn.addIceCandidate(new RTCIceCandidate({
            candidate: message.candidate
        }));

    }
}

function createPeerConnection(isInitiator, config) {
    console.log('Creating Peer conn as initiator?', isInitiator, 'config:',
        config);

    // send any ice candidates to the other peer
    peerConn.onicecandidate = function (event) {
        console.log('icecandidate event:', event);
        if (event.candidate) {
            // Trickle ICE
            //sendMessage({
            //    type: 'candidate',
            //    label: event.candidate.sdpMLineIndex,
            //    id: event.candidate.sdpMid,
            //    candidate: event.candidate.candidate
            //});
        } else {
            console.log('End of candidates.');
            // Vanilla ICE
            sendMessage(peerConn.localDescription);
        }
    };

    peerConn.ontrack = function (event) {
        console.log('icecandidate ontrack event:', event);
        remoteVideo.srcObject = event.streams[0];
    };

    if (isInitiator) {
        console.log('Creating Data Channel');
        dataChannel = peerConn.createDataChannel('sendDataChannel');
        onDataChannelCreated(dataChannel);

        console.log('Creating an offer');
        peerConn.createOffer(onLocalSessionCreated, logError);
    } else {
        peerConn.ondatachannel = function (event) {
            console.log('ondatachannel:', event.channel);
            dataChannel = event.channel;
            onDataChannelCreated(dataChannel);
        };
    }
}

function onLocalSessionCreated(desc) {
    console.log('local session created:', desc);
    peerConn.setLocalDescription(desc, function () {
        // Trickle ICE
        //console.log('sending local desc:', peerConn.localDescription);
        //sendMessage(peerConn.localDescription);
    }, logError);
}

function onDataChannelCreated(channel) {
    console.log('onDataChannelCreated:', channel);

    channel.onopen = function () {
        console.log('Channel opened!!!');
        connStatusMessage.innerText = 'Channel opened!!';
        fileInput.disabled = false;
    };

    channel.onclose = function () {
        console.log('Channel closed.');
        connStatusMessage.innerText = 'Channel closed.';
    }

    channel.onmessage = onReceiveMessageCallback();
}

function onReceiveMessageCallback() {
    let count;
    let fileSize, fileName;
    let receiveBuffer = [];

    return function onmessage(event) {
        if (typeof event.data === 'string') {
            const fileMetaInfo = event.data.split(',');
            fileSize = parseInt(fileMetaInfo[0]);
            fileName = fileMetaInfo[1];
            count = 0;
            return;
        }

        receiveBuffer.push(event.data);
        count += event.data.byteLength;

        if (fileSize === count) {
            // all data chunks have been received
            const received = new Blob(receiveBuffer);
            receiveBuffer = [];

            $(fileTable).children('tbody').append('<tr><td><a></a></td></tr>');
            const downloadAnchor = $(fileTable).find('a:last');
            downloadAnchor.attr('href', URL.createObjectURL(received));
            downloadAnchor.attr('download', fileName);
            downloadAnchor.text(`${fileName} (${fileSize} bytes)`);
        }
    };
}

function sendFile() {
    const file = fileInput.files[0];
    console.log(`File is ${[file.name, file.size, file.type, file.lastModified].join(' ')}`);

    if (file.size === 0) {
        alert('File is empty, please select a non-empty file.');
        return;
    }

    //send file size and file name as comma separated value.
    dataChannel.send(file.size + ',' + file.name);

    const chunkSize = 16384;
    fileReader = new FileReader();
    let offset = 0;
    fileReader.addEventListener('error', error => console.error('Error reading file:', error));
    fileReader.addEventListener('abort', event => console.log('File reading aborted:', event));
    fileReader.addEventListener('load', e => {
        console.log('FileRead.onload ', e);
        dataChannel.send(e.target.result);
        offset += e.target.result.byteLength;
        if (offset < file.size) {
            readSlice(offset);
        } else {
            alert(`${file.name} has been sent successfully.`);
            sendFileBtn.disabled = false;
        }
    });
    const readSlice = o => {
        console.log('readSlice ', o);
        const slice = file.slice(offset, o + chunkSize);
        fileReader.readAsArrayBuffer(slice);
    };
    readSlice(0);
}

/****************************************************************************
* Auxiliary functions
****************************************************************************/

function logError(err) {
    if (!err) return;
    if (typeof err === 'string') {
        console.warn(err);
    } else {
        console.warn(err.toString(), err);
    }
}