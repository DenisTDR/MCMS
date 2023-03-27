(function ($) {

    window.mcmsModals = window.mModals = {
        _waitModal: $("#processing-modal").find('.modal'),
        _alertModal: $("#alert-modal").find('.modal'),
        _backendModalTemplate: $("#backend-modal-template>.modal"),
        closeWaitModal: false,
        visibleModals: 0,
        body: $('body'),
        init: function () {
            this._waitModal.on("shown.bs.modal", function () {
                if (mcmsModals.closeWaitModal) {
                    mcmsModals._waitModal.modal('hide');
                    mcmsModals.closeWaitModal = false;
                }
            });

            mcmsModals.body.on('click', '[data-toggle="ajax-modal"]', function (event) {
                mcmsModals.ajaxModalItemAction.apply(this, [event]);
            });
        },
        ajaxModalItemAction: function (event) {
            if (event) {
                event.preventDefault();
            }
            const button = $(this);
            const url = button.data('url') || button.attr('href');
            if (!url) {
                console.error('Modal triggered by', this);
                throw new Error('But url for modal content not found!');
            }

            const modal = mcmsModals._backendModalTemplate.clone();

            mcmsModals.bindStackedModalsBehaviour(modal, true);

            // set options from button if set
            const opt = mcmsModals.parseOptions(button.data('modal-backdrop'), button.data('modal-keyboard'));

            modal.data('initialOpt', opt);

            modal.on("shown.bs.modal", function () {
                if (modal.data('shouldHide')) {
                    modal.modal('hide');
                }
            })

            // show initial modal (with the spinner)
            modal.modal(opt);

            const requestOptions = {
                url: url,
                headers: {'X-Request-Modal': 'true'},
                type: 'GET'
            };
            // make backend request to get the content
            $.ajax(requestOptions)
                .done(function (data) {
                    // display received content in the previous shown modal
                    mcmsModals.displayModalLinkResponse(modal, data, button);
                })
                .fail(function (e) {
                    modal.data('shouldHide', true);
                    modal.modal('hide');
                    mcmsModals.alertModalText(e.responseText || 'A fatal error occurred when tried to get modal content from backend. ' +
                        'Please make sure you are connected to the internet. Try refreshing this page.', 'Failed');
                });
        },
        requestToAjaxModal: function (url, data, callback, method) {
            method = method || 'GET';
            mcmsModals.postRequestAjaxModal(url, data, callback, method);
        },
        postRequestAjaxModal: function (url, data, callback, method) {
            method = method || 'POST';
            const modal = mcmsModals._backendModalTemplate.clone();

            mcmsModals.bindStackedModalsBehaviour(modal, true);

            modal.on("shown.bs.modal", function () {
                if (modal.data('shouldHide')) {
                    modal.modal('hide');
                }
            })

            // show initial modal (with the spinner)
            modal.modal({});

            const requestOptions = {
                url: url,
                headers: {'X-Request-Modal': 'true'},
                type: method,
                data: method === 'POST' ? JSON.stringify(data) : undefined,
                contentType: 'application/json'
            };
            // make backend request to get the content
            $.ajax(requestOptions)
                .done(function (data) {
                    const fakeTarget = $("<div></div>");
                    if (callback) {
                        fakeTarget.data('modal-callback', callback)
                    }

                    // display received content in the previous shown modal
                    mcmsModals.displayModalLinkResponse(modal, data, fakeTarget);
                })
                .fail(function (e) {
                    modal.data('shouldHide', true);
                    modal.modal('hide');
                    mcmsModals.alertModalText(e.responseText || 'A fatal error occurred when tried to get modal content from backend. ' +
                        'Please make sure you are connected to the internet. Try refreshing this page.', 'Failed');
                });
        },
        loadingUpModal: {
            show: function () {
                mcmsModals.closeWaitModal = false;
                mcmsModals.bindStackedModalsBehaviour(mcmsModals._waitModal);
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
            mcmsModals.bindStackedModalsBehaviour(modal);
            return modal.modal('show');
        },
        bindStackedModalsBehaviour: function (modal, removeOnHidden) {
            if (!modal.data('custom-modal-patched')) {
                modal.data('custom-modal-patched', true);
                modal.on("show.bs.modal", function () {
                    if (mcmsModals.visibleModals === 0) {
                        mcmsModals.body.addClass('forced-modal-open');

                        mcmsModals.initialScrollPosition = {x: window.scrollX, y: window.scrollY};
                        window.scrollTo(0, 0);
                        window.mcms.adjustSafeScrollbarWidth();
                    }
                    mcmsModals.visibleModals++;
                });
                modal.on("hidden.bs.modal", function () {
                    mcmsModals.visibleModals--;
                    if (mcmsModals.visibleModals === 0) {
                        mcmsModals.body.removeClass('forced-modal-open');

                        window.scrollTo(mcmsModals.initialScrollPosition.x, mcmsModals.initialScrollPosition.y);
                        window.mcms.adjustSafeScrollbarWidth();
                    }
                });

                if (removeOnHidden) {
                    modal.on("hidden.bs.modal", function () {
                        setTimeout(function () {
                            modal.remove();
                        }, 1000);
                    });
                }
            }
            mcmsModals.fixStackedModalBackDropBehaviour(modal);
        },
        alertModalText: function (text, title, options) {
            const crtAlertModal = mcmsModals._alertModal.clone();
            crtAlertModal.find(".title").html(title);
            crtAlertModal.find(".modal-body").html(text);
            if (options?.size) {
                crtAlertModal.find(".modal-dialog").addClass(options.size);
            }
            mcmsModals.bindStackedModalsBehaviour(crtAlertModal);
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

            mcmsModals.bindStackedModalsBehaviour(modal);
            return modal.modal('show');
        },
        displayModalLinkResponse: function (activeModal, backendData, target) {
            if (!backendData) {
                mcmsModals.alertModalText('No content received from the server to display in a modal.', 'Something weird occurred');
                return;
            }
            const vElem = $("<div></div>");
            vElem.append(backendData);

            setTimeout(() => {
                activeModal.find('>.modal-dialog').html('');
                const modal = vElem.find('>.modal');
                const dialog = modal.find('>.modal-dialog');

                const opt = mcmsModals.parseOptions(modal.data('backdrop'), modal.data('keyboard'));
                const initialOpt = activeModal.data('initialOpt');

                if (opt.backdrop !== undefined && initialOpt?.backdrop === undefined) {
                    activeModal.data('bs.modal')._config.backdrop = opt.backdrop;
                }
                if (opt.keyboard !== undefined && initialOpt?.keyboard === undefined) {
                    activeModal.data('bs.modal')._config.keyboard = opt.keyboard;
                }
                activeModal.attr("tabindex", modal.attr("tabindex"));

                activeModal.find('>.modal-dialog')
                    .attr('class', dialog.attr('class'))
                    .append(dialog.find('>*'));

                const scriptTags = vElem.find('script, style, link');
                activeModal.append(scriptTags);
            }, 500);


            // do specific actions when modal was hidden
            activeModal.on("hidden.bs.modal", function () {
                const result = activeModal.data('result');
                if (target && result?.reloadModal) {
                    target.click();
                    if (!result.alsoTriggerCallback) {
                        return;
                    }
                }
                const callback = target.data('modal-callback');
                if (typeof callback === 'string') {
                    const callbackFn = getFnRefByDottedName(callback);
                    if (typeof callbackFn === 'function') {
                        callbackFn(target, result);
                    }
                } else if (typeof callback === 'function') {
                    callback(target, result);
                }
            });

            mcmsModals.fixStackedModalBackDropBehaviour(activeModal);
        },
        fixStackedModalBackDropBehaviour: function (modal) {
            // adjust backdrop if this is a stacked modal
            // the backdrop of this modal should be right before it
            if (modal.hasClass('stacked-modal') && !modal.data('stacked-modal-fixed')) {
                modal.data('stacked-modal-fixed', true);

                modal.on("shown.bs.modal", function () {
                    const backdropEl = $(modal.data('bs.modal')._backdrop);
                    if (!backdropEl.length) {
                        console.error('couldn\'t find modal backdrop for', modal);
                        return;
                    }
                    backdropEl.remove();
                    modal.before(backdropEl);
                    modal.addClass('shown-modal');
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
