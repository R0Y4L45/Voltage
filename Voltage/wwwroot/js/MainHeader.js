document.addEventListener('DOMContentLoaded', function () {
    var currentUrl = window.location.pathname;
    var menuItems = document.querySelectorAll('.navbar-nav .nav-item');

    menuItems.forEach(function (menuItem) {
        var menuItemUrl = menuItem.getAttribute('data-url');
        var subMenuItems = menuItem.querySelectorAll('.dropdown-item');

        if (subMenuItems.length > 0) {
            subMenuItems.forEach(function (subMenuItem) {
                var subMenuItemUrl = subMenuItem.getAttribute('data-url');

                if (currentUrl.includes(subMenuItemUrl)) {
                    menuItem.classList.add('active');
                    subMenuItem.classList.add('active');
                }
            });
        } else {
            if (currentUrl === menuItemUrl) {
                menuItem.classList.add('active');
            }
        }
    });
});



function ToggleTheme(theme) {
    var url = '/Home/ToggleTheme?theme=' + theme;
    window.location.href = url;
}
