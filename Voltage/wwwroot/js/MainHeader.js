﻿document.addEventListener('DOMContentLoaded', function () {
    const currentUrl = window.location.pathname;
    const menuItems = document.querySelectorAll('.navbar-nav .nav-item');

    menuItems.forEach(function (menuItem) {
        const menuItemUrl = menuItem.getAttribute('data-url');
        const subMenuItems = menuItem.querySelectorAll('.dropdown-item');

        if (subMenuItems.length > 0) {
            subMenuItems.forEach(function (subMenuItem) {
                let subMenuItemUrl = subMenuItem.getAttribute('data-url');

                if (currentUrl.includes(subMenuItemUrl)) {
                    menuItem.classList.add('active');
                    subMenuItem.classList.add('active');
                }
            });
        } else {
            if (currentUrl === menuItemUrl) 
                menuItem.classList.add('active');
        }
    });
});



function ToggleTheme(theme) {
    const url = '/Home/ToggleTheme?theme=' + theme;
    window.location.href = url;
}
