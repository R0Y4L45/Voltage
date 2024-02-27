const monthSelect = document.getElementById("month"),
    daySelect = document.getElementById("day"),
    yearSelect = document.getElementById("year"),
    hiddenDateOfBirthInput = document.getElementById("hiddenDateOfBirth");


//Show Friend List

document.addEventListener("DOMContentLoaded", function () {
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
                    data.forEach(function (friend) {
                        addFriendToTable(friend);
                    });
                } else {
                    console.error("Failed to get friend list.");
                }
            } catch (error) {
                console.error("Error:", error);
            }
        });
    }
});


function addFriendToTable(friend) {
    var tbody = document.querySelector("table tbody");

    var row = document.createElement("tr");

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
            ${friend.country === null ? 'Other': friend.country}
        </td>
        <td class="text-end">
            
        </td>
    `;

    tbody.appendChild(row);
}




function showUsersinTable() {

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