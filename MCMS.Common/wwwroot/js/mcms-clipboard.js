(function ($) {
    window.mcmsClipboard = {
        enable(targetSelector, container) {
            const options = {};
            if (typeof container === "string") {
                options.container = $(container)[0];
            } else if (typeof container === 'object') {
                options.container = container;
            }

            const clipboard = new ClipboardJS(targetSelector, options);

            clipboard.on('success', function (e) {
                mcmsClipboard.showTooltip($(e.trigger), "Copied!");
            });

            clipboard.on('error', function (e) {
                mcmsClipboard.showTooltip($(e.trigger), "Couldn't copy. Please use the standard Ctrl-C/⌘+C way.");
            });

        },
        showTooltip(target, text) {
            if (!target.data("leave-bound")) {
                target.data("leave-bound", true)
                target.on("mouseleave", function () {
                    setTimeout(function () {
                        target.tooltip("dispose");
                    });
                });
            }
            target.attr("data-placement", "bottom");
            target.attr("title", text);
            target.tooltip("show", {container: 'body'});
        }
    }
})(jQuery);
