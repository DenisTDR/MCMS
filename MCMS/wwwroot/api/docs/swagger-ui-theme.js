var c = 0;

function setHeaderTitle() {
    console.log("setHeaderTitle");
    if (document.querySelector("h2.title")) {
        var topbar = document.querySelector(".topbar .link span");
        if (topbar) {
            topbar.innerHTML = document.querySelector("h2.title").innerText;
        }
    } else {
        c++;
        if (c > 20) {
            console.log("Can't set title! :(");
        } else {
            setTimeout(function () {
                setHeaderTitle();
            }, 250);
        }
    }
}

function doDarkIfNeeded() {
    console.log("doDarkIfNeeded");
    var now = new Date();
    var body = document.getElementsByTagName("body")[0];
    if (!body) {
        console.log("no body! :(");
    }
    if (now.getHours() < 9 || now.getHours() >= 18 || true) {
        setClassName(body, "dark");
    } else {
        setClassName(body, "");
    }

    function setClassName(element, className) {
        element.className = className;
    }

    setTimeout(function () {
        doDarkIfNeeded();
    }, 10 * 60 * 1000);
}

window.addEventListener('load', doDarkIfNeeded);
window.addEventListener('load', setHeaderTitle);
