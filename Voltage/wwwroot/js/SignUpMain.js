let monthSelect = document.getElementById("month");
let daySelect = document.getElementById("day");
let yearSelect = document.getElementById("year");
let hiddenDateOfBirthInput = document.getElementById("DateOfBirth");

function updateDays() {
    let selectedMonth = monthSelect.value;
    let selectedYear = yearSelect.value;

    daySelect.innerHTML = "<option value=''>Day</option>";

    if (selectedMonth && selectedYear) {
        let daysInMonth = new Date(selectedYear, selectedMonth, 0).getDate();

        for (let i = 1; i <= daysInMonth; i++) {
            let option = document.createElement("option");
            option.value = i;
            option.text = i;
            daySelect.appendChild(option);
        }
    }

    updateDateOfBirth();
}

function updateYears() {
    yearSelect.innerHTML = "<option value=''>Year</option>";

    let currentYear = new Date().getFullYear();
    let startYear = currentYear - 18;
    let endYear = currentYear - 80;

    for (let i = startYear; i >= endYear; i--) {
        let option = document.createElement("option");
        option.value = i;
        option.text = i;
        yearSelect.appendChild(option);
    }
    updateDateOfBirth();
}
yearSelect.addEventListener("change", updateDateOfBirth);

function updateDateOfBirth() {
    let selectedMonth = monthSelect.value;
    let selectedYear = yearSelect.value;
    let selectedDay = daySelect.value;

    if (selectedMonth && selectedDay && selectedYear) {
        let selectedDate = new Date(selectedYear, selectedMonth - 1, selectedDay);
        let formattedDate = selectedDate.toISOString().split('T')[0];
        hiddenDateOfBirthInput.value = formattedDate;
    } else {
        hiddenDateOfBirthInput.value = null;
    }
}

monthSelect.addEventListener("change", updateDays);
yearSelect.addEventListener("change", updateDays);
daySelect.addEventListener("change", updateDateOfBirth);

updateDays();
updateYears();