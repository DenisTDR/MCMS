(function ($) {
    window.ajaxForm = function (form, asModal, callback) {
        form.submit(function (event) {
            event.preventDefault();
            mModals.loadingUpModal.show();

            function close(result, failed) {
                const modal = form.closest(".modal");
                result = {params: result};
                if (failed) {
                    result.failed = failed;
                }
                modal.data('result', result);
                modal.modal('hide');
            }

            $.ajax({
                type: $(this).data("ajax-method") || this.method || 'POST',
                url: this.action,
                data: JSON.stringify(serializeFormAsObject(form)),
                contentType: "application/json; charset=utf-8",
                headers: {
                    'X-Request-Modal': asModal
                }
            })
                .done(function (e) {
                    mModals.loadingUpModal.hide();
                    if (asModal) {
                        close(true);
                    }
                    if (typeof callback === 'function') {
                        callback(true, e);
                    }
                })
                .fail(function (e) {
                    mModals.loadingUpModal.hide();
                    console.log('error')
                    console.error(e);
                    if (asModal) {
                        close(false, true);
                    }
                    if (typeof callback === 'function') {
                        callback(false);
                    }
                    if (e.responseJSON) {
                        const obj = e.responseJSON;
                        if (obj.error) {
                            mModals.alertModalText(obj.error, "Error");
                        } else {
                            mModals.alertModalText(e.responseText, "Json Error");
                        }
                    } else {
                        mModals.alertModal(e.responseText);
                    }
                });
        });
    }


    window.serializeFormAsObject = function (form) {
        const jsonArray = form.serializeArray();
        const jsonObj = {};
        for (let i = 0; i < jsonArray.length; i++) {
            jsonObj[jsonArray[i].name] = jsonArray[i].value;
        }
        return jsonObj;
    }
})(jQuery);

