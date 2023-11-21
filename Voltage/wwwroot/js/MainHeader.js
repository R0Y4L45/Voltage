document.addEventListener('DOMContentLoaded', function () {
    var menuItems = document.querySelectorAll('.navbar-nav .nav-item');
    var currentUrl = window.location.pathname;

    menuItems.forEach(function (menuItem) {
        var menuItemUrl = menuItem.getAttribute('data-url');

        if (currentUrl === menuItemUrl) {
            menuItem.classList.add('active');
        }
    });
});

function ToggleTheme(theme) {
    var url = '/Home/ToggleTheme?theme=' + theme;
    window.location.href = url;
}
