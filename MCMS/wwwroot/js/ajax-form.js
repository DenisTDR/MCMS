function ajaxForm(form, asModal, callback, method) {
    form.submit(function (event) {
        event.preventDefault();
        displayLoadingModal();

        function close(result) {
            var modal = form.closest(".modal");
            modal.data('result', {params: result});
            modal.modal('hide');
        }

        $.ajax({
            type: this.method || 'POST',
            url: this.action,
            data: JSON.stringify(serializeFormAsJson(form)),
            // encode: true,
            contentType: "application/json; charset=utf-8",
            headers: {
                'X-Request-Modal': asModal
            }
        })
            .done(function () {
                hideLoadingModal();
                if (asModal) {
                    close(true);
                }
                if (typeof callback === 'function') {
                    callback(true);
                }
            })
            .fail(function (e) {
                hideLoadingModal();
                console.log('error')
                console.error(e);
                if (asModal) {
                    close(false);
                }
                if (typeof callback === 'function') {
                    callback(false);
                }
                if (e.responseJSON) {
                    alertModalText(e.responseText, "Json Error")
                } else {
                    alertModal(e.responseText);
                }
            });
    });
}

function serializeFormAsJson(form) {
    var jsonArray = form.serializeArray();
    var jsonObj = {};
    for (var i = 0; i < jsonArray.length; i++) {
        jsonObj[jsonArray[i].name] = jsonArray[i].value;
    }
    return jsonObj;
}