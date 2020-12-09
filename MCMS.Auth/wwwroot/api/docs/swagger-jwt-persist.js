
$(function () {
    var body = $('body');
    body.on('click', '.auth-container form button.authorize', function () {
        var jwt = $(this).closest('form').find("input[type=text]").val();
        if (!jwt) return;
        localStorage.setItem('jwt-token', jwt);
    });
    body.on('click', '.auth-container form button.authorize + button', function () {
        localStorage.removeItem('jwt-token');
    });
    body.on('click', '.auth-container form button:not(.authorize):not(.btn-done)', function () {
        localStorage.removeItem('jwt-token');
    });
    if (localStorage.getItem('jwt-token')) {
        setToken(0, localStorage.getItem('jwt-token'));
    }
});

function setToken(c, token) {
    var btn = $(".scheme-container button.authorize");
    if (btn.length !== 0) {
        btn.click();
        setTimeout(function () {
            var inputJq = $('.auth-container form input[type=text]');

            var input = inputJq[0];
            // code stolen from https://github.com/facebook/react/issues/11488#issuecomment-347775628
            let lastValue = input.value;
            input.value = token;
            let event = new Event('input', {bubbles: true});
            event.simulated = true;
            let tracker = input._valueTracker;
            if (tracker) {
                tracker.setValue(lastValue);
            }
            input.dispatchEvent(event);

            var form = inputJq.closest('form');
            form.find('button.authorize').click();
            form.closest('.modal-ux').find('.close-modal').click();

        }, 250);
        return;
    }
    c++;
    if (c < 10) {
        setTimeout(function () {
            setToken(c, token);
        }, 500);
    }
}