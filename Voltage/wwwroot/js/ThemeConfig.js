function toggleTheme() {
    const currentTheme = document.body.getAttribute('data-bs-theme') || 'dark';

    if (currentTheme === 'dark') {
        console.log("light theme");
        document.body.setAttribute('data-bs-theme', 'light');
        localStorage.setItem('theme', 'light');
    }
    else {
        console.log("dark theme");
        document.body.setAttribute('data-bs-theme', 'dark');
        localStorage.setItem('theme', 'dark');
    }
}

document.addEventListener('DOMContentLoaded', _ => {
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme)
        document.body.setAttribute('data-bs-theme', savedTheme);
});
