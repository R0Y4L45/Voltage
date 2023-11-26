document.addEventListener("DOMContentLoaded", _ => {
    const selectCountries = document.getElementById('select-countries'),
        countries = [
            { code: 'other', flagClass: '', name: 'Other' },

            //A
            { code: 'dz', flagClass: 'flag-country-dz', name: 'Algeria' },
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

            //C
            { code: 'ca', flagClass: 'flag-country-ca', name: 'Canada' },
            { code: 'cc', flagClass: 'flag-country-cc', name: 'Cocos Islands' },
            { code: 'cd', flagClass: 'flag-country-cd', name: 'DR Congo' },
            { code: 'cf', flagClass: 'flag-country-cf', name: 'Central Africa Respublic' },
            { code: 'cg', flagClass: 'flag-country-cg', name: 'Republic of the Congo' },
            { code: 'ch', flagClass: 'flag-country-ch', name: 'Switzerland' },
            { code: 'ci', flagClass: 'flag-country-ci', name: 'Cote dIvoire' },
            { code: 'ck', flagClass: 'flag-country-ck', name: 'Cook Islands' },
            { code: 'cl', flagClass: 'flag-country-cl', name: 'Chile' },
            { code: 'cm', flagClass: 'flag-country-cm', name: 'Cameroon' },
            { code: 'cn', flagClass: 'flag-country-cn', name: 'China' },
            { code: 'co', flagClass: 'flag-country-co', name: 'Colombia' },
            { code: 'cr', flagClass: 'flag-country-cr', name: 'Costa Rica' },
            { code: 'cu', flagClass: 'flag-country-cu', name: 'Cuba' },
            { code: 'cy', flagClass: 'flag-country-cy', name: 'Cyprus' },
            { code: 'cz', flagClass: 'flag-country-cz', name: 'Czechia' },

            //D
            { code: 'de', flagClass: 'flag-country-de', name: 'Germany' },
            { code: 'dj', flagClass: 'flag-country-dj', name: 'Djibouti' },
            { code: 'dk', flagClass: 'flag-country-dk', name: 'Denmark' },
            { code: 'dm', flagClass: 'flag-country-dm', name: 'Dominica' },
            { code: 'do', flagClass: 'flag-country-do', name: 'Dominican Republic' },
        ];

    countries.forEach(country => {
        const option = document.createElement('option');
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
                if (data.customProperties)
                    return '<div><span class="dropdown-item-indicator">' + data.customProperties + '</span>' + escape(data.text) + '</div>';
                return '<div>' + escape(data.text) + '</div>';
            },
            option: function (data, escape) {
                if (data.customProperties)
                    return '<div><span class="dropdown-item-indicator">' + data.customProperties + '</span>' + escape(data.text) + '</div>';
                return '<div>' + escape(data.text) + '</div>';
            },
        },
    });
});

