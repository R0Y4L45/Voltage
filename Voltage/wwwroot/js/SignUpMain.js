document.addEventListener("DOMContentLoaded", function () {
    var selectCountries = document.getElementById('select-countries');

    var countries = [
        { code: 'other', flagClass: '', name: 'Other' },

        //A
        { code: 'ad', flagClass: 'flag-country-ad', name: 'Andorra' },
        { code: 'ae', flagClass: 'flag-country-ae', name: 'United Arab Emirates' },
        { code: 'af', flagClass: 'flag-country-af', name: 'Afghanistan' },
        { code: 'ag', flagClass: 'flag-country-ag', name: 'Antigua and barbuda' },
        { code: 'ai', flagClass: 'flag-country-ai', name: 'Anguilla' },
        { code: 'al', flagClass: 'flag-country-al', name: 'Albania' },
        { code: 'ao', flagClass: 'flag-country-ao', name: 'Angola' },
        { code: 'aq', flagClass: 'flag-country-aq', name: 'Antarctica' },
        { code: 'as', flagClass: 'flag-country-as', name: 'Samoa' },
        { code: 'at', flagClass: 'flag-country-at', name: 'Austria' },
        { code: 'au', flagClass: 'flag-country-au', name: 'Australia' },
        { code: 'aw', flagClass: 'flag-country-aw', name: 'Aruba' },
        { code: 'ax', flagClass: 'flag-country-ax', name: 'Åland Islands' },
        { code: 'az', flagClass: 'flag-country-az', name: 'Azerbaijan' },

        //B
        { code: 'ba', flagClass: 'flag-country-ba', name: 'Bosnia and Herzegovina' },
        { code: 'bb', flagClass: 'flag-country-bb', name: 'Barbados' },
        { code: 'bd', flagClass: 'flag-country-bd', name: 'Bangladesh' },
        { code: 'be', flagClass: 'flag-country-be', name: 'Belgium' },
        { code: 'bf', flagClass: 'flag-country-bf', name: 'Burkina Faso' },
        { code: 'bg', flagClass: 'flag-country-bg', name: 'Bulgaria' },
        { code: 'bh', flagClass: 'flag-country-bh', name: 'Bahrain' },
        { code: 'bj', flagClass: 'flag-country-bj', name: 'Benin' },
        { code: 'bm', flagClass: 'flag-country-bm', name: 'Bermuda' },
        { code: 'bn', flagClass: 'flag-country-bn', name: 'Brunei' },
        { code: 'br', flagClass: 'flag-country-br', name: 'Brazil' },
        { code: 'bs', flagClass: 'flag-country-bs', name: 'Bahamas' },
        { code: 'bt', flagClass: 'flag-country-bt', name: 'Bhutan' },
        { code: 'bv', flagClass: 'flag-country-bv', name: 'Bouvet Island' },
        { code: 'bw', flagClass: 'flag-country-bw', name: 'Botswana' },
        { code: 'by', flagClass: 'flag-country-by', name: 'Belarus' },
    ];

    countries.forEach(function (country) {
        var option = document.createElement('option');
        option.value = country.name;
        option.setAttribute('data-custom-properties', '<span class="flag flag-xs ' + country.flagClass + '"></span>');
        option.text = country.name;
        selectCountries.appendChild(option);
    });

    window.TomSelect && new TomSelect(selectCountries, {
        copyClassesToDropdown: false,
        dropdownParent: 'body',
        controlInput: '<input>',
        render: {
            item: function (data, escape) {
                if (data.customProperties) {
                    return '<div><span class="dropdown-item-indicator">' + data.customProperties + '</span>' + escape(data.text) + '</div>';
                }
                return '<div>' + escape(data.text) + '</div>';
            },
            option: function (data, escape) {
                if (data.customProperties) {
                    return '<div><span class="dropdown-item-indicator">' + data.customProperties + '</span>' + escape(data.text) + '</div>';
                }
                return '<div>' + escape(data.text) + '</div>';
            },
        },
    });
});






var monthSelect = document.getElementById("month");
var daySelect = document.getElementById("day");
var yearSelect = document.getElementById("year");
var hiddenDateOfBirthInput = document.getElementById("DateOfBirth");

function updateDays() {
    var selectedMonth = monthSelect.value;
    var selectedYear = yearSelect.value;

    daySelect.innerHTML = "<option value=''>Day</option>";

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

function updateYears() {
    yearSelect.innerHTML = "<option value=''>Year</option>";

    var currentYear = new Date().getFullYear();
    var startYear = currentYear - 18;
    var endYear = currentYear - 80;

    for (var i = startYear; i >= endYear; i--) {
        var option = document.createElement("option");
        option.value = i;
        option.text = i;
        yearSelect.appendChild(option);
    }
    updateDateOfBirth();
}
yearSelect.addEventListener("change", updateDateOfBirth);

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

monthSelect.addEventListener("change", updateDays);
yearSelect.addEventListener("change", updateDays);
daySelect.addEventListener("change", updateDateOfBirth);

updateDays();
updateYears();