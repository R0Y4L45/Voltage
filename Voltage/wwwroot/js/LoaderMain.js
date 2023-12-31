document.getElementById('MainPage').style.display = 'none';
window.onload = function () {
    setTimeout(function () {
        document.getElementById('loader').style.display = 'none';
        document.getElementById('MainPage').style.display = 'block';
    });
};