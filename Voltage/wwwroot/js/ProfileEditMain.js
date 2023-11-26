const monthSelect = document.getElementById("month"),
    daySelect = document.getElementById("day"),
    yearSelect = document.getElementById("year"),
    hiddenDateOfBirthInput = document.getElementById("hiddenDateOfBirth");

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