document.addEventListener("DOMContentLoaded", _ => {
    const passwordInput = document.getElementById("password-input"),
        showPasswordToggle = document.getElementById("show-password-toggle"),
        currentTheme = localStorage.getItem("tablerTheme");

    if (currentTheme === "dark") setIconColor("white");
    else setIconColor("black");

    showPasswordToggle.addEventListener("click", event => {
        event.preventDefault();
        togglePasswordVisibility(passwordInput, showPasswordToggle);
    });

    function setIconColor(color) {
        document.querySelectorAll(".icon-tabler-eye").forEach(icon =>
            icon.setAttribute("fill", color)
        );
    }

    function togglePasswordVisibility(input, toggleIcon) {
        if (input.type === "password") {
            input.type = "text";
            toggleIcon.innerHTML =
                `<svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-eye-closed" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">
                    <path stroke="none" d="M0 0h24v24H0z" fill="none"></path>
                    <path d="M21 9c-2.4 2.667 -5.4 4 -9 4c-3.6 0 -6.6 -1.333 -9 -4"></path>
                    <path d="M3 15l2.5 -3.8"></path>
                    <path d="M21 14.976l-2.492 -3.776"></path>
                    <path d="M9 17l.5 -4"></path>
                    <path d="M15 17l-.5 -4"></path>
                </svg>`;
        }
        else {
            input.type = "password";
            toggleIcon.innerHTML =
                `<svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-eye" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">
                    <path stroke="none" d="M0 0h24v24H0z" fill="none"></path>
                    <path d="M10 12a2 2 0 1 0 4 0a2 2 0 0 0 -4 0"></path>
                    <path d="M21 12c-2.4 4 -5.4 6 -9 6c-3.6 0 -6.6 -2 -9 -6c2.4 -4 5.4 -6 9 -6c3.6 0 6.6 2 9 6"></path>
                </svg>`;
        }
    }
});