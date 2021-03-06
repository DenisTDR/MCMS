/*!
    * Start Bootstrap - SB Admin v6.0.0 (https://startbootstrap.com/templates/sb-admin)
    * Copyright 2013-2020 Start Bootstrap
    * Licensed under MIT (https://github.com/BlackrockDigital/startbootstrap-sb-admin/blob/master/LICENSE)
    */
(function ($) {
    "use strict";

    // Add active state to sidbar nav links
    const path = window.location.href; // because the 'href' property of the DOM element is the absolute path
    $(".sb-layout-sidenav-nav .sb-sidenav a.nav-link").each(function () {
        if (path.indexOf(this.href) >= 0) {
            $(this).addClass("active");
        }
    });

    // Toggle the side navigation
    $("#sidebarToggle").on("click", function (e) {
        e.preventDefault();
        $("body").toggleClass("sb-sidenav-toggled");
        setTimeout(function () {
            document.dispatchEvent(new CustomEvent('side-menu-toggled', {detail: $("body").hasClass("sb-sidenav-toggled")}));
        }, 200);
    });

    $(".sb-layout-sidenav .sb-layout-sidenav-content-overlay")
        .click(function () {
            $("#sidebarToggle").click();
        });
})(jQuery);
