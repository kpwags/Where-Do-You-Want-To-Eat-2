﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $('#selectall').click(function () {
        $('input:checkbox').attr('checked', 'checked');
    });

    $('#selectnone').click(function () {
        $('input:checkbox').removeAttr('checked');
    });

    $('#dismiss-cookie-popup').click(function () {
        setCookie('has_seen_cookie_popup', 'Y', 365);
        $('#cookie-popup-alert').alert('close');
    });

    if (getCookie('has_seen_cookie_popup') !== 'Y') {
        $('#cookie-popup-alert').removeClass('d-none');
    }
});

function setCookie(name, value, days) {
    var expires = '';
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + days * 24 * 60 * 60 * 1000);
        expires = '; expires=' + date.toUTCString();
    }
    document.cookie = name + '=' + (value || '') + expires + '; path=/';
}

function getCookie(name) {
    var nameEQ = name + '=';
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function eraseCookie(name) {
    document.cookie = `${name}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;`;
}
