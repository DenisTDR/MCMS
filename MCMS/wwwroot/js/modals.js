var mModals = {
    waitModal: $("#processing-modal").find('.modal'),
    _alertModal: $("#alert-modal").find('.modal'),
    closeWaitModal: false,
    visibleModals: 0,
    body: $('body'),
    init: function () {
        this.waitModal.on("shown.bs.modal", function (e) {
            if (mModals.closeWaitModal) {
                mModals.waitModal.modal('hide');
                mModals.closeWaitModal = false;
            }
        });

        mModals.body.on('click', '[data-toggle="ajax-modal"]', function (event) {
            if (event) {
                event.preventDefault();
            }
            var button = $(this);
            var url = button.data('url') || button.attr('href');
            if (!url) {
                console.error('Modal triggered by', this);
                throw new Error('But url for modal content not found!');
            }
            mModals.loadingUpModal.show();
            var headers = {'X-Request-Modal': 'true'};
            var requestOptions = {
                url: url,
                headers: headers,
                type: 'GET'
            }
            $.ajax(requestOptions)
                .done(function (data) {
                    // alert(1);
                    mModals.loadingUpModal.hide();
                    mModals.displayModalLinkResponse(data, button);
                })
                .fail(function (e) {
                    mModals.loadingUpModal.hide();
                    mModals.alertModal(e.responseText);
                });
        });
        this.initAlertModal();
    },
    loadingUpModal: {
        show: function () {
            mModals.closeWaitModal = false;
            mModals.customShowModal(mModals.waitModal);
        },
        hide: function () {
            mModals.closeWaitModal = true;
            mModals.waitModal.modal('hide');
        }
    },
    initialScrollPosition: {x: 0, y: 0},
    customShowModal: function (modal) {
        if (!modal.data('custom-modal-patched')) {
            modal.data('custom-modal-patched', true);
            modal.on("show.bs.modal", function (e) {
                if (mModals.visibleModals === 0) {
                    mModals.body.addClass('forced-modal-open');

                    mModals.initialScrollPosition = {x: window.scrollX, y: window.scrollY};
                    window.scrollTo(0, 0);
                }
                mModals.visibleModals++;
            });

            modal.on("hidden.bs.modal", function (e) {
                mModals.visibleModals--;
                if (mModals.visibleModals === 0) {
                    mModals.body.removeClass('forced-modal-open');

                    window.scrollTo(mModals.initialScrollPosition.x, mModals.initialScrollPosition.y);
                }
            });
        }
        modal.modal('show');
    },
    initAlertModal: function () {
        mModals._alertModal.on('hidden.bs.modal', function () {
            mModals._alertModal.find(".title").html("");
            mModals._alertModal.find(".modal-body").html("");
        });
    },
    alertModalText: function (text, title) {
        mModals._alertModal.find(".title").html(title);
        mModals._alertModal.find(".modal-body").html(text);
        this.customShowModal(mModals._alertModal);
    },
    alertModal: function (modalHtml) {
        var newElement = $("<div></div>");
        newElement.append($(modalHtml));
        mModals.body.append(newElement);
        var modal = newElement.find('.modal');
        modal.on("hidden.bs.modal", function () {
            setTimeout(function () {
                newElement.detach();
            }, 1000);
        });
        modal.modal({backdrop: 'static'});
        this.customShowModal(modal);
    },
    displayModalLinkResponse: function (data, button) {
        mModals.closeWaitModal = true;
        var newElement = $("<div></div>");
        newElement.append($(data));
        this.body.append(newElement);
        var modal = newElement.find('.modal');
        modal.on("hidden.bs.modal", function (a, b, c) {
            var result = modal.data('result');
            if (result && result.reload) {
                button.click();
                return;
            }
            var callback = button.data('modal-callback');
            var callbackFn = window[callback];
            if (typeof callbackFn === 'function') {
                callbackFn(button, result);
            }
            setTimeout(function () {
                newElement.detach();
            }, 1000);
        });
        modal.on("shown.bs.modal", function (e) {
            if (!modal.hasClass('stacked-modal')) {
                return;
            }
            var backDrop = newElement.nextAll(".modal-backdrop");
            backDrop.detach();
            newElement.before(backDrop);
            modal.addClass('shown-modal');
        });
        modal.modal({show: false, backdrop: button.data('modal-backdrop')});
        this.customShowModal(modal);
    }
}

mModals.init();