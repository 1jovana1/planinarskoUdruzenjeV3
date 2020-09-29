// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function ajaxSubmit(e) {
    console.log("Cao Jole");
    $.ajax({
        url: $(this).data("url"),
        method: "POST", // use "GET" if server does not handle DELETE
    }).done(function (msg) {
        $("#log").html(msg);
    }).fail(function (jqXHR, textStatus) {
        alert("Request failed: " + textStatus);
    }); 
}
$(document).on('click', '.btn-control-ajax-submit', ajaxSubmit);
