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
                if (mModals.closeWaitModal) {
                    mModals._waitModal.modal('hide');
                    mModals.closeWaitModal = false;
                }
            });

            mModals.body.on('click', '[data-toggle="ajax-modal"]', function (event) {
                mModals.ajaxModalItemAction.apply(this, [event]);
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

            // show initial modal (with the spinner)
            modal.modal(opt);

            const headers = {'X-Request-Modal': 'true'};
            const requestOptions = {
                url: url,
                headers: headers,
                type: 'GET'
            };
            // make backend request to get the content
            $.ajax(requestOptions)
                .done(function (data) {
                    // display received content in the previous shown modal
                    mModals.displayModalLinkResponse(modal, data, button);
                })
                .fail(function (e) {
                    modal.modal('hide');
                    mModals.alertModalText(e.responseText || 'A fatal error occurred when tried to get modal content from backend. ' +
                        'Please make sure you are connected to the internet. Try refreshing this page.', 'Failed');
                });
        },
        loadingUpModal: {
            show: function () {
                mModals.closeWaitModal = false;
                mModals.bindStackedModalsBehaviour(mModals._waitModal);
                mModals._waitModal.modal('show');
            },
            hide: function () {
                mModals.closeWaitModal = true;
                mModals._waitModal.modal('hide');
            }
        },
        initialScrollPosition: {x: 0, y: 0},
        customShowModal: function (modal) {
            mModals.bindStackedModalsBehaviour(modal);
            return modal.modal('show');
        },
        bindStackedModalsBehaviour: function (modal, removeOnHidden) {
            if (!modal.data('custom-modal-patched')) {
                modal.data('custom-modal-patched', true);
                modal.on("show.bs.modal", function () {
                    if (mModals.visibleModals === 0) {
                        mModals.body.addClass('forced-modal-open');

                        mModals.initialScrollPosition = {x: window.scrollX, y: window.scrollY};
                        window.scrollTo(0, 0);
                    }
                    mModals.visibleModals++;
                });
                modal.on("hidden.bs.modal", function () {
                    mModals.visibleModals--;
                    if (mModals.visibleModals === 0) {
                        mModals.body.removeClass('forced-modal-open');

                        window.scrollTo(mModals.initialScrollPosition.x, mModals.initialScrollPosition.y);
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
        },
        alertModalText: function (text, title, options) {
            const crtAlertModal = mModals._alertModal.clone();
            crtAlertModal.find(".title").html(title);
            crtAlertModal.find(".modal-body").html(text);
            if (options?.size) {
                crtAlertModal.find(".modal-dialog").addClass(options.size);
            }
            mModals.bindStackedModalsBehaviour(crtAlertModal);
            return crtAlertModal.modal('show');
        },
        alertModal: function (modalHtml) {
            const newElement = $("<div></div>");
            if (!(modalHtml instanceof $)) {
                modalHtml = $(modalHtml);
            }
            newElement.append(modalHtml);
            mModals.body.append(newElement);
            const modal = newElement.find('.modal');
            modal.on("hidden.bs.modal", function () {
                setTimeout(function () {
                    newElement.remove();
                }, 1000);
            });
            if (modal.data('backdrop') === undefined) {
                modal.data('backdrop', 'static');
            }

            mModals.bindStackedModalsBehaviour(modal);
            return modal.modal('show');
        },
        displayModalLinkResponse: function (activeModal, backendData, button) {
            if (!backendData) {
                mModals.alertModalText('No content received from the server to display in a modal.', 'Something weird occurred');
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

                if (opt.backdrop !== undefined && initialOpt.backdrop === undefined) {
                    activeModal.data('bs.modal')._config.backdrop = opt.backdrop;
                }
                if (opt.keyboard !== undefined && initialOpt.keyboard === undefined) {
                    activeModal.data('bs.modal')._config.keyboard = opt.keyboard;
                }

                activeModal.find('>.modal-dialog')
                    .attr('class', dialog.attr('class'))
                    .append(dialog.find('>*'));

                const scriptTags = vElem.find('script, style');
                activeModal.append(scriptTags);
            }, 500)


            // do specific actions when modal was hidden
            activeModal.on("hidden.bs.modal", function () {
                const result = activeModal.data('result');
                if (result && result.reloadModal) {
                    button.click();
                    return;
                }
                const callback = button.data('modal-callback');
                if (typeof callback === 'string') {
                    const callbackFn = getFnRefByDottedName(callback);
                    if (typeof callbackFn === 'function') {
                        callbackFn(button, result);
                    }
                } else if (typeof callback === 'function') {
                    callback(button, result);
                }
            });

            // adjust backdrop if this is a stacked modal
            // the backdrop of this modal should be right before it
            if (activeModal.hasClass('stacked-modal')) {
                activeModal.on("shown.bs.modal", function () {
                    const backDrop = activeModal.nextAll(".modal-backdrop");
                    backDrop.remove();
                    activeModal.before(backDrop);
                    activeModal.addClass('shown-modal');
                });
            }
        },
        parseOptions: function (backdrop, keyboard) {
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

    mModals.init();
})(jQuery);
