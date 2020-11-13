// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//function ajaxSubmit(e) {
//    console.log("Cao");
//    $.ajax({
//        url: $(this).data("url"),
//        method: "POST", // use "GET" if server does not handle DELETE
//    }).done(function (msg) {
//        $("#log").html(msg);
//    }).fail(function (jqXHR, textStatus) {
//        alert("Request failed: " + textStatus);
//    }); 
//}
//$(document).on('click', '.btn-control-ajax-submit', ajaxSubmit);


//$(".jumbotron").css({ height: $(window).height() + "px" });

//$(window).on("resize", function () {
//    $(".jumbotron").css({ height: $(window).height() + "px" });
//});

//ocjene na dogadjajima:

$(".btnrating").on('click', (function (e) {

	var previous_value = $("#selected_rating").val();

	var selected_value = $(this).attr("data-attr");
	$("#selected_rating").val(selected_value);

	$(".selected-rating").empty();
	$(".selected-rating").html(selected_value);

	for (i = 1; i <= selected_value; ++i) {
		$("#rating-star-" + i).toggleClass('btn-warning');
		$("#rating-star-" + i).toggleClass('btn-default');
	}

	for (ix = 1; ix <= previous_value; ++ix) {
		$("#rating-star-" + ix).toggleClass('btn-warning');
		$("#rating-star-" + ix).toggleClass('btn-default');
	}

}));
