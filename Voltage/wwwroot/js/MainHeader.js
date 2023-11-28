document.addEventListener('DOMContentLoaded', _ => {
    const currentUrl = window.location.pathname,
        menuItems = document.querySelectorAll('.navbar-nav .nav-item');

    menuItems.forEach(menuItem => {
        const menuItemUrl = menuItem.getAttribute('data-url'),
            subMenuItems = menuItem.querySelectorAll('.dropdown-item');

        if (subMenuItems.length > 0) {
            subMenuItems.forEach(subMenuItem => {
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
