var monthSelect = document.getElementById("month");
var daySelect = document.getElementById("day");
var yearSelect = document.getElementById("year");
var hiddenDateOfBirthInput = document.getElementById("hiddenDateOfBirth");

function updateDays() {
    var selectedMonth = monthSelect.value;
    var selectedYear = yearSelect.value;

    daySelect.innerHTML;

    if (selectedMonth && selectedYear) {
        var daysInMonth = new Date(selectedYear, selectedMonth, 0).getDate();

        for (var i = 1; i <= daysInMonth; i++) {
            var option = document.createElement("option");
            option.value = i;
            option.text = i;
            daySelect.appendChild(option);
        }
    }
    updateDateOfBirth();
}

function updateDateOfBirth() {
    var selectedMonth = monthSelect.value;
    var selectedYear = yearSelect.value;
    var selectedDay = daySelect.value;

    if (selectedMonth && selectedDay && selectedYear) {
        var selectedDate = new Date(selectedYear, selectedMonth - 1, selectedDay);
        var formattedDate = selectedDate.toISOString().split('T')[0];
        hiddenDateOfBirthInput.value = formattedDate;
    } else {
        hiddenDateOfBirthInput.value = null;
    }
}

$(document).ready(function () {
    $("#changeAvatarBtn").click(function () {
        $("#avatarInput").click();
    });
});

monthSelect.addEventListener("change", updateDays);
yearSelect.addEventListener("change", updateDays);
daySelect.addEventListener("change", updateDateOfBirth);
updateDays();