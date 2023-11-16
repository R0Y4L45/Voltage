document.addEventListener('DOMContentLoaded', function () {
    var menuItems = document.querySelectorAll('.navbar-nav .nav-item');
    menuItems.forEach(function (menuItem) {
        menuItem.addEventListener('click', function () {
            menuItems.forEach(function (item) {
                item.classList.remove('active');
            });

            menuItem.classList.add('active');
        });
    });

    var lastActive = sessionStorage.getItem('lastActiveMenuItem');
    if (lastActive) {
        var activeElement = document.getElementById(lastActive);
        if (activeElement) {
            activeElement.classList.add('active');
        }
    }

    document.addEventListener('click', function (event) {
        var clickedElement = event.target.closest('.nav-item');
        if (clickedElement) {
            var itemId = clickedElement.id;
            sessionStorage.setItem('lastActiveMenuItem', itemId);
        }
    });
});

function ToggleTheme(theme) {
    var url = '@Url.Action("ToggleTheme", "Home")' + '?theme=' + theme;
    window.location.href = url;
}