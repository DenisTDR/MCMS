(function ($) {
    const mcms = window.mcms = window.mcms || {};

    function main() {
        document.addEventListener('side-menu-toggled', function (e) {
            document.cookie = "side-menu-toggled=" + (e && typeof e.detail !== 'undefined' && e.detail ? 1 : 0) + ";path=/;max-age=86400;samesite=strict";
        });
        bindSideMenuCollapseSectionsPersistence();
    }

    window.formatDate = function (date, separator) {
        if (typeof separator === 'undefined') {
            separator = '/';
        }
        if (typeof date === 'string') {
            date = new Date(date);
        }
        let day = date.getDate();
        let month = date.getMonth() + 1;
        if (day < 10) day = "0" + day;
        if (month < 10) month = "0" + month;
        return day + separator + month + separator + date.getFullYear();
    }

    window.getFnRefByDottedName = function (fnName, rootObject) {
        if (typeof fnName !== 'string') {
            throw new Error('Invalid function name');
        }
        if (!rootObject) {
            rootObject = window;
        }
        let callbackFn;
        if (fnName.indexOf(".") > -1) {
            const callbackParts = fnName.split('.');
            let crtObj = rootObject;
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

        let states = {};
        const statesStr = sessionStorage.getItem('side-menu-section-states');
        const saveStates = function () {
            sessionStorage.setItem('side-menu-section-states', JSON.stringify(states));
        };
        if (statesStr) {
            let shouldSave = false;
            try {
                states = JSON.parse(statesStr);
                for (const sectionId in states) {
                    if (!states.hasOwnProperty(sectionId)) continue;
                    if (states[sectionId]) {
                        const section = $("#" + sectionId);
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

        const sectionLinks = $(document.body).find('#sidenavAccordion .collapse');
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

    mcms.adjustSafeScrollbarWidth = function () {
        if (window.mcms.skipSafeScrollBar) return;
        const hasScroll = document.body.scrollHeight > document.body.clientHeight && !document.body.classList.contains('forced-modal-open');
        if (mcms.hasScroll !== hasScroll) {
            mcms.hasScroll = hasScroll;
            document.getElementById("page-main").style['padding-right'] = 'calc(15px + ' + (mcms.hasScroll ? 0 : mcms.scrollbarWidth) + 'px)';
            document.getElementById("header-navbar").style['padding-right'] = 'calc(1rem + ' + (mcms.hasScroll ? 0 : mcms.scrollbarWidth) + 'px)';
        }
    }

    function detectScrollbarWidth() {
        const scrollDiv = document.createElement("div");
        scrollDiv.className = "scrollbar-measure";
        document.body.appendChild(scrollDiv);
        window.mcms.scrollbarWidth = scrollDiv.offsetWidth - scrollDiv.clientWidth;
        document.body.removeChild(scrollDiv);
    }

    if (window.hasOwnProperty('ResizeObserver')) {
        new window.ResizeObserver(_ => {
            window.mcms.adjustSafeScrollbarWidth();
        }).observe(document.getElementById("page-main"));
    }

    detectScrollbarWidth();
    main();
})(jQuery);

