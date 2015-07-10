function onSaveBegin() {
    var btnSave = $("#save");
    var loader = $("div[data-role='loader']");
    loader.height(btnSave.height() + 20);

    btnSave.hide("fast");
    loader.show("fast");
}

function onSaveComplete(data) {
    $("div[data-role='loader']").hide("fast");
    $("#save").show("fast");
}

function onSuccess(data, status) {
    if (window.frameElement) {
        $("#saveResult").removeClass("message-error").addClass("message-success").html("Сохранено успешно");
        $("#saveResult").show("fast");
        setTimeout(function() {
            $("#saveResult").hide("fast");
        }, 5000);
    } else {
        location.href = window.card.successUrl;
    }
}

function onFailure(data) {
    $("#saveResult").removeClass("message-success").addClass("message-error").html("Сохранить не удалось");
    $("#saveResult").show("fast");
    setTimeout(function () {
        $("#saveResult").hide("fast");
    }, 5000);
}