(function ($) {
    window.mcmsModals = window.mModals = {
        _waitModal: $("#processing-modal").find('.modal'),
        _alertModal: $("#alert-modal").find('.modal'),
        _backendModalTemplate: $("#backend-modal-template>.modal"),
        closeWaitModal: false,
        visibleModals: 0,
        body: $('body'),
        currentStack: [],
        init: function () {
            this._waitModal.on("shown.bs.modal", function () {
                if (mcmsModals.closeWaitModal) {
                    mcmsModals._waitModal.modal('hide');
                    mcmsModals.closeWaitModal = false;
                }
            });

            mcmsModals.body.on('click', '[data-toggle="ajax-modal"]', function (event) {
                mcmsModals.processDataToggleModalElemClick.apply(this, [event]);
            });
        },
        processDataToggleModalElemClick: function (event, postData) {
            if (event) {
                event.preventDefault();
                event.stopPropagation();
            }
            const elem = $(this);
            const url = elem.data('url') || elem.attr('href');
            if (!url) {
                console.error('Modal triggered by', this);
                throw new Error('But url for modal content not found!');
            }
            const options = {
                url,
                postData,
                modalOptions: {
                    ...mcmsModals.parseOptions(elem.data('modal-backdrop'), elem.data('modal-keyboard')),
                },
                // triggerElem: elem,
                onHideCallback: elem.data('modal-callback'),
                method: elem.data('modal-method') || 'GET',
            };
            // console.log(opt);
            const modal = mcmsModals.requestNewBackendModal(options);
            // console.log(modal);
            return modal;
        },
        requestNewBackendModal: options => {
            const modal = mcmsModals._backendModalTemplate.clone();
            mcmsModals.bindCustomModalsBehaviour(modal, true);
            if (options.modalOptions) {
                modal.data('initialOpt', options.modalOptions);
            }
            // show initial modal (with the loading spinner)
            modal.modal({});

            modal.data('requestBackendModalOptions', options);

            return mcmsModals.requestBackendModal(options, modal);
        },
        reloadBackendModal: modal => {
            // console.log('reload backend modal', modal);
            const opt = modal.data('requestBackendModalOptions');
            if(!opt) {
                console.log('no options from modal data requestBackendModalOptions');
            }
            // modal.data("reloaded", true);
            modal.data("result", { reloaded: true });
            return mcmsModals.requestBackendModal(opt, modal);
        },
        requestBackendModal: (options, modal) => {
            const reqOptions = {
                url: options.url,
                headers: {'X-Request-Modal': 'true'},
                method: options.method || 'GET',
            };

            if (reqOptions.method === "POST" && options.postData) {
                reqOptions.data = JSON.stringify(options.postData);
                reqOptions.contentType = "application/json; charset=utf-8";
            }

            // do backend request to get the content
            $.ajax({
                ...reqOptions,
                success: data => {
                    if (!data) {
                        modal.data('shouldHide', true);
                        modal.modal('hide');
                        mcmsModals.alertModalText('No content received from the server to display in a modal.', 'Something weird occurred');
                        return;
                    }
                    mcmsModals.displayDataInShownModal(data, modal, options);
                },
                error: e => {
                    modal.data('shouldHide', true);
                    modal.modal('hide');
                    mcmsModals.alertModalText(e.responseText || 'A fatal error occurred when tried to get modal content from backend. ' +
                        'Please make sure you are connected to the internet. Try refreshing this page.', 'Failed');
                }
            });
            return modal;
        },
        displayDataInShownModal: (data, currentModal, options) => {
            const vElem = $("<div></div>");
            vElem.append(data);

            $(".tooltip").tooltip("hide");
            // console.log('clearing current modal dialog content');
            const currentDialog = currentModal.find('>.modal-dialog');
            currentDialog.html('');

            const newModal = vElem.find('>.modal');
            const newDialog = newModal.find('>.modal-dialog');

            // const opt = mcmsModals.parseOptions(newModal.data('backdrop'), newModal.data('keyboard'));
            // const initialOpt = currentModal.data('initialOpt');

            currentModal.attr("tabindex", newModal.attr("tabindex"));

            currentDialog
                .attr('class', newDialog.attr('class'))
                .append(newDialog.find('>*'));

            const hasForms = currentDialog.find('form :input:not([type=hidden]):not(button), mcms-form-params-wrapper, .formly-debug').length;
            currentModal.data('bs.modal')._config.keyboard = !hasForms;
            currentModal.data('bs.modal')._config.backdrop = hasForms ? 'static' : true;

            setTimeout(() => {
                // clear pre existing script, style and link tags
                currentModal.find('>script, >style, >link').detach();

                //append existing script, style and link tags;
                const scriptTags = vElem.find('script, style, link');
                currentModal.append(scriptTags);
            }, 250);

            if (!currentModal.data("added-hidden-result")) {
                currentModal.data("added-hidden-result", true);
                currentModal.one("hidden.bs.modal", function () {
                    const result = currentModal.data('result');
                    // console.log('modal closed, processing result data', result);

                    const callback = options.onHideCallback;
                    if (typeof callback === 'string') {
                        const callbackFn = getFnRefByDottedName(callback);
                        if (typeof callbackFn === 'function') {
                            // console.log('calling callbackFn');
                            callbackFn(currentModal, result);
                        }
                    } else if (typeof callback === 'function') {
                        callback(currentModal, result);
                    }
                });
            }

            mcmsModals.fixStackedModalBehaviour(currentModal);
            return currentModal;

        },
        loadingUpModal: {
            show: function () {
                mcmsModals.closeWaitModal = false;
                mcmsModals.bindCustomModalsBehaviour(mcmsModals._waitModal);
                mcmsModals._waitModal.modal('show');
                mcmsModals.body.append(mcmsModals._waitModal.parent());
            },
            hide: function () {
                mcmsModals.closeWaitModal = true;
                mcmsModals._waitModal.modal('hide');
            }
        },
        initialScrollPosition: {x: 0, y: 0},
        customShowModal: function (modal) {
            mcmsModals.bindCustomModalsBehaviour(modal);
            return modal.modal('show');
        },
        bindCustomModalsBehaviour: (modal, removeOnHidden) => {
            // console.log('bindCustomModalsBehaviour');
            if (!modal.data('custom-modal-patched')) {
                modal.data('custom-modal-patched', true);
                modal.one("show.bs.modal", () => {
                    if (mcmsModals.visibleModals === 0) {
                        mcmsModals.body.addClass('forced-modal-open');

                        // mcmsModals.initialScrollPosition = {x: window.scrollX, y: window.scrollY};
                        // window.scrollTo(0, 0);
                        // window.mcms.adjustSafeScrollbarWidth();
                    }
                    mcmsModals.visibleModals++;
                });
                modal.on("hidden.bs.modal", () => {
                    mcmsModals.visibleModals--;
                    if (mcmsModals.visibleModals === 0) {
                        mcmsModals.body.removeClass('forced-modal-open');

                        // window.scrollTo(mcmsModals.initialScrollPosition.x, mcmsModals.initialScrollPosition.y);
                        // window.mcms.adjustSafeScrollbarWidth();
                    }
                });

                if (removeOnHidden) {
                    // console.log('bindCustomModalsBehaviour.removeOnHidden');
                    modal.on("hidden.bs.modal", () => {
                        setTimeout(() => {
                            modal.remove();
                        }, 1000);
                    });
                }
                modal.one("shown.bs.modal", () => {
                    if (modal.data('shouldHide')) {
                        modal.modal('hide');
                    }
                });
            }
            mcmsModals.fixStackedModalBehaviour(modal);
        },
        alertModalText: function (text, title, options) {
            const crtAlertModal = mcmsModals._alertModal.clone();
            crtAlertModal.find(".title").html(title);
            crtAlertModal.find(".modal-body").html(text);
            if (options?.size) {
                crtAlertModal.find(".modal-dialog").addClass(options.size);
            }
            mcmsModals.bindCustomModalsBehaviour(crtAlertModal);
            return crtAlertModal.modal('show');
        },
        alertModal: function (modalHtml) {
            const newElement = $("<div></div>");
            if (!(modalHtml instanceof $)) {
                modalHtml = $(modalHtml);
            }
            newElement.append(modalHtml);
            mcmsModals.body.append(newElement);
            const modal = newElement.find('.modal');
            modal.on("hidden.bs.modal", function () {
                setTimeout(function () {
                    newElement.remove();
                }, 1000);
            });
            if (modal.data('backdrop') === undefined) {
                modal.data('backdrop', 'static');
            }

            mcmsModals.bindCustomModalsBehaviour(modal);
            return modal.modal('show');
        },
        fixStackedModalBehaviour: function (modal) {
            // 1. Adjust backdrop if this is a stacked modal. The backdrop of this modal should be right before it.
            // 2. Keep track of opened modals in a stack and when a modal is hidden focus the modal opened right before it.
            if (modal.hasClass('stacked-modal') && !modal.data('stacked-modal-patched')) {
                modal.data('stacked-modal-patched', true);

                modal.one("shown.bs.modal", () => {
                    const backdropEl = $(modal.data('bs.modal')._backdrop);
                    if (!backdropEl.length) {
                        console.error('couldn\'t find modal backdrop for', modal);
                        return;
                    }
                    backdropEl.remove();
                    modal.before(backdropEl);
                    modal.addClass('shown-modal');
                    mcmsModals.currentStack.push(modal);
                });
                modal.one("hidden.bs.modal", () => {
                    if (mcmsModals.currentStack.indexOf(modal) === mcmsModals.currentStack.length - 1) {
                        mcmsModals.currentStack.pop();
                    } else {
                        mcmsModals.currentStack.splice(mcmsModals.currentStack.indexOf(modal), 1);
                    }
                    if (mcmsModals.currentStack.length) {
                        mcmsModals.currentStack[mcmsModals.currentStack.length - 1].focus();
                    }
                });
            }
        },
        parseOptions: function (backdrop, keyboard, tabindex) {
            const opt = {};
            if (backdrop !== undefined) {
                if (backdrop === 'static') {
                    opt.backdrop = 'static';
                }
                if (backdrop === 'true' || backdrop === true) {
                    opt.backdrop = true;
                }
                if (backdrop === 'false' || backdrop === false) {
                    opt.backdrop = false;
                }
            }
            if (keyboard !== undefined) {
                if (keyboard === 'true' || keyboard === true) {
                    opt.keyboard = true;
                }
                if (keyboard === 'false' || keyboard === false) {
                    opt.keyboard = false;
                }
            }
            return opt;
        }
    };

    mcmsModals.init();
})(jQuery);
