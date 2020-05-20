// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $('#selectall').click(function () {
        $('input:checkbox').attr('checked', 'checked');
    });

    $('#selectnone').click(function () {
        $('input:checkbox').removeAttr('checked');
    });
});
