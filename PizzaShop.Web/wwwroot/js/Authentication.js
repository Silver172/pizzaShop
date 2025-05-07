$(function () {
    var message = '@Html.Raw(TempData["ToastrMessage"])';
    var type = '@Html.Raw(TempData["ToastrType"])';
    if (message.trim() !== '') {
        switch (type) {
            case 'success':
                toastr.success(message)
                break;
            case 'error':
                toastr.error(message)
                break;
            case 'warning':
                toastr.warning(message)
                break;
            case 'info':
                toastr.info(message)
        }
    }
});

toastr.options = {
    "closeButton": true,      // Show close button
    "debug": false,
    "newestOnTop": true,
    "progressBar": true,      // Show progress bar
    "positionClass": "toast-top-right", // Change position
    "preventDuplicates": true,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",        // Auto-close after 5 seconds
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};

$(document).ready(function () {
    let email = new URLSearchParams(window.location.search).get('email');
    if (email) {
        $("#email").val(email);
    }
});

$("#ForgotPassword").click(function () {
    let email = $("#email").val();
    if (email) {
        window.location.href = "/Authentication/ForgotPassword?email=" + email;
    }
    else {
        toastr.warning("Please fill your email")
    }
})
$(function () {
    var message = '@Html.Raw(TempData["ToastrMessage"])';
    var type = '@Html.Raw(TempData["ToastrType"])';
    if (message.trim() !== '') {
        switch (type) {
            case 'success':
                toastr.success(message)
                break;
            case 'error':
                toastr.error(message)
                break;
            case 'warning':
                toastr.warning(message)
                break;
            case 'info':
                toastr.info(message)
        }
    }
});