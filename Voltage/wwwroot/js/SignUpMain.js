let monthSelect = document.getElementById("month"),
    daySelect = document.getElementById("day"),
    yearSelect = document.getElementById("year"),
    hiddenDateOfBirthInput = document.getElementById("DateOfBirth");

function updateDays() {
    let selectedMonth = monthSelect.value,
        selectedYear = yearSelect.value;

    daySelect.innerHTML = "<option value=''>Day</option>";

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

function updateYears() {
    yearSelect.innerHTML = "<option value=''>Year</option>";

    let currentYear = new Date().getFullYear(),
        startYear = currentYear - 18,
        endYear = currentYear - 80,
        option;

    for (let i = startYear; i >= endYear; i--) {
        option = document.createElement("option");
        option.value = i;
        option.text = i;
        yearSelect.appendChild(option);
    }
    updateDateOfBirth();
}
yearSelect.addEventListener("change", updateDateOfBirth);

function updateDateOfBirth() {
    let selectedMonth = monthSelect.value,
        selectedYear = yearSelect.value,
        selectedDay = daySelect.value;

    if (selectedMonth && selectedDay && selectedYear) {
        let selectedDate = new Date(selectedYear, selectedMonth - 1, selectedDay),
            formattedDate = selectedDate.toISOString().split('T')[0];

        hiddenDateOfBirthInput.value = formattedDate;
    }
    else hiddenDateOfBirthInput.value = null;
}

monthSelect.addEventListener("change", updateDays);
yearSelect.addEventListener("change", updateDays);
daySelect.addEventListener("change", updateDateOfBirth);

updateDays();
updateYears();