// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.




$('#actionId').on('change', function () {
    
    if (this.value == '1') {
        $('#request').val($('#creationOrderTemplate').val());
    } else if (this.value == '2') {
        $('#request').val($('#xmlInvoiceTemplate').val());
    } else if (this.value == '3') {
        $('#request').val($('#invoicedMessageTemplate').val());
    } else if (this.value == '4') {
        $('#request').val($('#shippedMessageTemplate').val());
    } else if (this.value == '5') {
        $('#request').val($('#deliveredMessageTemplate').val());
    }
});

$('#btnClean').on('click', function () {
    $('#request').val('');
    $('#response').val('');
    $('#AnymarketOrder').val('');
    $("#environmentId").val($("#environmentId option:first").val());
    $("#actionId").val($("#actionId option:first").val());
});

$('#btnExecute').on('click', function () {

    $("#response").val('');

    var request = $.ajax({
        url: "Anymarket/Anymarket/PostOnAnymarket",
        method: "POST",
        data: {
            RequestPayload: $('#request').val(),
            Action: $('#actionId option:selected').val(),
            Environment: $('#environmentId option:selected').val(),
            AnymarketOrder: $('#AnymarketOrder').val()
        },
        dataType: "text"
    });

    request.done(function (msg) {
        $("#response").val(msg);
    });

    request.fail(function (jqXHR, textStatus) {
        $("#response").val("Request failed: " + textStatus);
    });
});

