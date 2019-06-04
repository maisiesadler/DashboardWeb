// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', () => {
    function loadScript(script) {
        $.ajax({
            type: "GET",
            url: '/js/' + script,
            success: _ => { console.log('got script ' + script) },
            error: err => console.log('error getting script ' + err)
        })
    }

    loadScript('swpush.js')
    loadScript('reminders.js')
})