var waitModal = $("#processing-modal").find('.modal');
var _alertModal = $("#alert-modal").find('.modal');

var closeWaitModal = false;
waitModal.on("shown.bs.modal", function (e) {
    if (closeWaitModal) {
        waitModal.modal('hide');
        closeWaitModal = false;
    }
});

function displayLoadingModal() {
    closeWaitModal = false;
    waitModal.modal('show');
}

function hideLoadingModal() {
    closeWaitModal = true;
    waitModal.modal('hide');
}

var body = $('body');
body.on('click', 'button[data-toggle="ajax-modal"], a[data-toggle="ajax-modal"]', function (event) {
    if (event) {
        event.preventDefault();
    }
    var button = $(this);
    var url = button.data('url') || button.attr('href');
    if (!url) {
        console.error('Modal triggered by', this);
        throw new Error('But url for modal content not found!');
    }
    displayLoadingModal()
    $.get(url).done(function (data) {
        hideLoadingModal();
        displayModal(data, button);
    }).fail(function (e) {
        hideLoadingModal();
        alertModal(e.responseText);
    }).always(function () {
    });
});

function displayModal(data, button) {
    closeWaitModal = true;
    var newElement = $("<div></div>");
    newElement.append($(data));
    body.append(newElement);
    var modal = newElement.find('.modal');
    modal.on("hidden.bs.modal", function () {
        var callback = button.data('modal-callback');
        // console.log('modal closed!');
        // console.log(modal.data("result"));
        if (callback && window.hasOwnProperty(callback) && typeof window[callback] === 'function') {
            window[callback](button, modal.data("result"));
        }
        setTimeout(function () {
            newElement.detach();
        }, 1000);
    });
    modal.on("shown.bs.modal", function (e) {
        if (!modal.hasClass('stacked-modal')) {
            return;
        }
        var backDrop = newElement.next();
        backDrop.detach();
        newElement.before(backDrop);
        modal.addClass('shown-modal');
    });
    modal.modal({backdrop: button.data('modal-backdrop')});
    modal.modal('show');
}


function initAlertModal() {
    _alertModal.on('hidden.bs.modal', function () {
        _alertModal.find(".title").html("");
        _alertModal.find(".modal-body").html("");
    });
}

function alertModal(modalHtml) {
    var newElement = $("<div></div>");
    newElement.append($(modalHtml));
    body.append(newElement);
    var modal = newElement.find('.modal');
    modal.on("hidden.bs.modal", function () {
        setTimeout(function () {
            newElement.detach();
        }, 1000);
    });
    modal.modal({backdrop: 'static'});
    // modal.modal('show');
}

function alertModalText(text, title) {
    _alertModal.find(".title").html(title);
    _alertModal.find(".modal-body").html(text);
    _alertModal.modal('show');
}