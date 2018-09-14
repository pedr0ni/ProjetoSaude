function addSpinner(element) {
    element.append("<div uk-spinner class='uk-margin-small-left spinner-loading'></div>");
}

function removeSpinner(element) {
    element.remove('.spinner-loading');
}

function getToken() {
    return $("#RequestVerificationToken").val();
}

$('#login-form').on('submit', (event) => {
    event.preventDefault();
    $('#login-submit').html("ENTRANDO...");
    addSpinner($('#login-submit'));

    setTimeout(() => {
        $.ajax({
            url: "/Account/LoginPost",
            method: "post",
            headers: {
                'RequestVerificationToken': getToken()
            },
            data: {
                Cpf: $("#login-cpf").val(),
                Senha: $("#login-password").val()
            },
            dataType: "json",
            success: (result) => {
                console.log(result);
                removeSpinner($("#login-submit"));
                if (result.authenticated) {
                    location.reload();
                } else {
                    $("#login-submit").html("ENTRAR");
                }
            },
            error: (error) => {
                removeSpinner($("#login-submit"));
                $("#login-submit").html("ENTRAR");
            }
        });
    }, 1500);
});