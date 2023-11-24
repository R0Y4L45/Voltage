const monthSelect = document.getElementById("month");
const daySelect = document.getElementById("day");
const yearSelect = document.getElementById("year");
const hiddenDateOfBirthInput = document.getElementById("hiddenDateOfBirth");

function updateDays() {
    const selectedMonth = monthSelect.value;
    const selectedYear = yearSelect.value;

    daySelect.innerHTML;

    if (selectedMonth && selectedYear) {
        const daysInMonth = new Date(selectedYear, selectedMonth, 0).getDate();

        for (let i = 1; i <= daysInMonth; i++) {
            const option = document.createElement("option");
            option.value = i;
            option.text = i;
            daySelect.appendChild(option);
        }
    }
    updateDateOfBirth();
}

function updateDateOfBirth() {
    const selectedMonth = monthSelect.value;
    const selectedYear = yearSelect.value;
    const selectedDay = daySelect.value;

    if (selectedMonth && selectedDay && selectedYear) {
        const selectedDate = new Date(selectedYear, selectedMonth - 1, selectedDay);
        const formattedDate = selectedDate.toISOString().split('T')[0];
        hiddenDateOfBirthInput.value = formattedDate;
    } else hiddenDateOfBirthInput.value = null;
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