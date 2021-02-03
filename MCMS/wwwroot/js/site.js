// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function main() {
    document.addEventListener('side-menu-toggled', function (e) {
        document.cookie = "side-menu-toggled=" + (e && typeof e.detail !== 'undefined' && e.detail ? 1 : 0) + ";path=/;max-age=86400;samesite=strict";
    });
    bindSideMenuCollapseSectionsPersistence();
}

function formatDate(date, separator) {
    if (typeof separator === 'undefined') {
        separator = '/';
    }
    if (typeof date === 'string') {
        date = new Date(date);
    }
    var day = date.getDate();
    var month = date.getMonth() + 1;
    if (day < 10) day = "0" + day;
    if (month < 10) month = "0" + month;
    return day + separator + month + separator + date.getFullYear();
}

function getFnRefByDottedName(fnName, rootObject) {
    if (typeof fnName !== 'string') {
        throw new Error('Invalid function name');
    }
    if (!rootObject) {
        rootObject = window;
    }
    var callbackFn;
    if (fnName.indexOf(".") > -1) {
        var callbackParts = fnName.split('.');
        var crtObj = rootObject;
        while (callbackParts.length) {
            if (crtObj[callbackParts[0]]) {
                crtObj = crtObj[callbackParts[0]];
            } else {
                crtObj = null;
                break;
            }
            callbackParts.splice(0, 1);
        }
        callbackFn = crtObj;
    } else {
        callbackFn = rootObject[fnName];
    }
    return callbackFn;
}

function bindSideMenuCollapseSectionsPersistence() {
    if (!sessionStorage) return;

    var states = {};
    var statesStr = sessionStorage.getItem('side-menu-section-states');
    var saveStates = function () {
        sessionStorage.setItem('side-menu-section-states', JSON.stringify(states));
    }
    if (statesStr) {
        var shouldSave = false;
        try {
            states = JSON.parse(statesStr);
            for (var sectionId in states) {
                if (!states.hasOwnProperty(sectionId)) continue;
                if (states[sectionId]) {
                    var section = $("#" + sectionId);
                    section.addClass('show');
                    if (!section.length) {
                        delete states[sectionId];
                        shouldSave = true;
                    }
                }
            }
        } catch (e) {
        }
        if (shouldSave) {
            saveStates();
        }
    }

    var sectionLinks = $(document.body).find('#sidenavAccordion .collapse');
    sectionLinks.on('shown.bs.collapse', function (e) {
        if (this.id !== e.target.id) {
            return;
        }
        states[this.id] = true;
        saveStates();
    });
    sectionLinks.on('hidden.bs.collapse', function (e) {
        if (this.id !== e.target.id) {
            return;
        }
        delete states[this.id];
        saveStates();
    });
}

main();
