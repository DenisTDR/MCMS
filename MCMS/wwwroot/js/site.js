// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function formatDate(date, separator) {
    if (typeof separator === 'undefined') {
        separator = '/';
    }
    if (typeof date === 'string') {
        date = new Date(date);
    }
    var day = date.getDate();
    var month = date.getMonth() + 1;
    if (day < 10) day = "0" + day;
    if (month < 10) month = "0" + month;
    return day + separator + month + separator + date.getFullYear();
}