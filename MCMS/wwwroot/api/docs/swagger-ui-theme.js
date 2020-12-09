var alwaysDarkMode = true;

function doDarkIfNeeded() {
    var now = new Date();
    var body = document.body;
    if (alwaysDarkMode || now.getHours() < 7 || now.getHours() >= 18) {
        body.classList.add("dark")
    } else {
        body.classList.remove("dark")
    }

    if (!alwaysDarkMode) {
        setTimeout(function () {
            doDarkIfNeeded();
        }, 10 * 60 * 1000);
    }
}

window.addEventListener('load', doDarkIfNeeded);
