(function (factory) {
	typeof define === 'function' && define.amd ? define(factory) :
		factory();
})((function () {
	'use strict';

	var themeStorageKey = "tablerTheme";
	var defaultTheme = "dark";
	var selectedTheme;
	var params = new Proxy(new URLSearchParams(window.location.search), {
		get: function get(searchParams, prop) {
			return searchParams.get(prop);
		}
	});
	if (!!params.theme) {
		localStorage.setItem(themeStorageKey, params.theme);
		selectedTheme = params.theme;
	} else {
		var storedTheme = localStorage.getItem(themeStorageKey);
		selectedTheme = storedTheme ? storedTheme : defaultTheme;
	}
	if (selectedTheme === 'light') {
		document.body.setAttribute("data-bs-theme", selectedTheme);
	} else {
		document.body.removeAttribute("data-bs-theme");
	}

}));


//const btn = document.getElementById('themeBtn');

//btn.addEventListener('click', _ => {
//    const h = document.querySelector('header');
//    if (h.getAttribute('data-bs-theme') != null) {
//        h.removeAttribute('data-bs-theme')
//    }
//    else {
//        h.setAttribute('data-bs-theme', 'light');
//    }

//});