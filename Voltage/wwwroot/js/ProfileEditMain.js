const monthSelect = document.getElementById("month"),
    daySelect = document.getElementById("day"),
    yearSelect = document.getElementById("year"),
    hiddenDateOfBirthInput = document.getElementById("hiddenDateOfBirth");


//Show Friend List

document.addEventListener("DOMContentLoaded", async function () {
    var friendListBtn = document.getElementById("friendListBtn");
    if (friendListBtn) {
        friendListBtn.addEventListener("click", async function (event) {
            event.preventDefault();
            document.getElementById("EditFromSection").classList.add("displaynone");
            document.getElementById("FriendListSection").classList.remove("displaynone");
            console.log("Friend list pressed");

            var rawUrl = window.location.href;
            var idIndex = rawUrl.lastIndexOf("/") + 1;
            var id = rawUrl.substring(idIndex);
            console.log(id);

            try {
                var response = await fetch(`/UserInfo/GetMyFriend?id=${id}`);
                if (response.ok) {
                    var data = await response.json();
                    console.log(data);
                    displayFriendList(data);
                } else {
                    console.error("Failed to get friend list.");
                }
            } catch (error) {
                console.error("Error:", error);
            }
        });
    }

    var filterBtn = document.querySelector("#modal-report .modal-footer .btn-primary");
    if (filterBtn) {
        filterBtn.addEventListener("click", async function (event) {
            event.preventDefault();
            var selectedStatus = document.querySelector("#modal-report select").value;
            var rawUrl = window.location.href;
            var idIndex = rawUrl.lastIndexOf("/") + 1;
            var id = rawUrl.substring(idIndex);
            console.log(selectedStatus);
            if (selectedStatus === "1") selectedStatus = "All"; 
            if (selectedStatus === "2") selectedStatus = "Pending"; 
            if (selectedStatus === "3") selectedStatus = "Accepted"; 
            console.log(selectedStatus);
            try {
                var response = await fetch(`/UserInfo/GetMyFriend?id=${id}&requestStatus=${selectedStatus}`);
                if (response.ok) {
                    var data = await response.json();
                    console.log(data);
                    displayFriendList(data); 
                } else 
                    console.error("Failed to get filtered friend list.");
            } catch (error) {
                console.error("Error:", error);
            }
        });
    }

    function displayFriendList(friends) {
        var tbody = document.querySelector("table tbody");
        tbody.innerHTML = ""; 

        friends.forEach(function (friend) {
            addFriendToTable(friend);
        });
    }

    function addFriendToTable(friend) {

        let methodName, modalAttribute, icon;
        var tbody = document.querySelector("table tbody");
        var row = document.createElement("tr");

        if (friend.requestStatus === "Accepted") {
            methodName = `removeRequest('${friend.userName}')`;
            modalAttribute = `data-bs-toggle="modal" data-bs-target="#modal-danger"`;
            icon = '<svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-user-minus" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M8 7a4 4 0 1 0 8 0a4 4 0 0 0 -8 0" /><path d="M6 21v-2a4 4 0 0 1 4 -4h4c.348 0 .686 .045 1.009 .128" /><path d="M16 19h6" /></svg>Remove';
        }
        else if (friend.requestStatus === "Pending") {
            methodName = `pendingRequest('${friend.userName}')`;
            modalAttribute = `data-bs-toggle="modal" data-bs-target="#modal-danger"`;
            icon = '<img id = "changingGif" width="16"  src="/staticPhotos/output-onlinegiftools.gif" alt="hourglass-sand-top"/> Pending...';
        }
        
        
        row.innerHTML = `
            <td class="text-sm-center">
                <span class="avatar" style="background-image: url(${friend.photo})"></span>
            </td>
            <td class="text-sm-center" data-label="Name">
                <div class="d-flex py-1 align-items-center">
                    <div class="flex-fill">
                        <div class="font-weight-medium">${friend.userName}</div>
                        <div class="text-secondary"><a class="text-reset">${friend.email}</a></div>
                    </div>
                </div>
            </td>
            <td class="text-sm-center">
                <span class="flag flag-xs flag-country-az me-2"></span>
                ${friend.country === null ? 'Other' : friend.country}
            </td>
            <td class="text-end">
                <div class="col-auto">
                    <a id="btnId${friend.userName}" onclick="${methodName}" class="btn" ${modalAttribute}>
                    ${icon}
                    </a>
                </div>
            </td>
        `;
        tbody.appendChild(row);
    }
});

function backProfile() {
    document.getElementById("EditFromSection").classList.remove("displaynone");
    document.getElementById("FriendListSection").classList.add("displaynone");
}




//Birthday select
function updateDays() {
    const selectedMonth = monthSelect.value,
        selectedYear = yearSelect.value;

    daySelect.innerHTML;

    if (selectedMonth && selectedYear) {
        let daysInMonth = new Date(selectedYear, selectedMonth, 0).getDate(),
            option;

        for (let i = 1; i <= daysInMonth; i++) {
            option = document.createElement("option");
            option.value = i;
            option.text = i;
            daySelect.appendChild(option);
        }
    }
    updateDateOfBirth();
}

function updateDateOfBirth() {
    const selectedMonth = monthSelect.value,
        selectedYear = yearSelect.value,
        selectedDay = daySelect.value;

    if (selectedMonth && selectedDay && selectedYear) {
        const selectedDate = new Date(selectedYear, selectedMonth - 1, selectedDay),
            formattedDate = selectedDate.toISOString().split('T')[0];

        hiddenDateOfBirthInput.value = formattedDate;
    }
    else hiddenDateOfBirthInput.value = null;
}

$(document).ready(_ => $("#changeAvatarBtn").click(_ => $("#avatarInput").click()));

monthSelect.addEventListener("change", updateDays);
yearSelect.addEventListener("change", updateDays);
daySelect.addEventListener("change", updateDateOfBirth);
updateDays();