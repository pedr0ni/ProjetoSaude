function addSpinner(element) {
    element.html(" ").append("<div uk-spinner class='spinner-loading'></div>");
    
}

function removeSpinner(element) {
    element.remove('.spinner-loading');
}

function getToken() {
    return $("#RequestVerificationToken").val();
}

function validateInputs(inputs) {
    let result = true;
    inputs.forEach((element) => {
        if (element.val() == null || element.val() == "") {
            result = false;
            element.addClass("uk-form-danger");
        }
    });
    return result;
}

function resetInputs(inputs) {
    inputs.forEach((element) => {
        element.removeClass("uk-form-danger");
    });
}

$('#login-form').on('submit', (event) => {
    event.preventDefault();

    let login_cpf = $("#login-cpf");
    let login_password = $("#login-password");
    let login_submit = $("#login-submit");

    if (!validateInputs([login_cpf, login_password])) return;

    addSpinner(login_submit);

    resetInputs([login_cpf, login_password]);

    let url = new URL(window.location.href);
    let ReturnUrl = url.searchParams.get("ReturnUrl");

    setTimeout(() => {
        $.ajax({
            url: "/Account/LoginPost",
            method: "post",
            headers: {
                'RequestVerificationToken': getToken()
            },
            data: {
                Cpf: login_cpf.val(),
                Senha: login_password.val()
            },
            dataType: "json",
            success: (result) => {
                console.log(result);
                removeSpinner(login_submit);
                UIkit.modal.alert(result.message);
                if (result.authenticated) {

                    if (ReturnUrl != null)
                        setTimeout(() => window.location = window.location.origin + ReturnUrl, 1500);
                    else
                        setTimeout(() => location.reload(), 1500);

                } else {
                    login_submit.html("ENTRAR");
                }
            },
            error: (error) => {
                removeSpinner(login_submit);
                login_submit.html("ENTRAR");
                UIkit.modal.alert(error.message);
            }
        });
    }, 1500);
});

$('#register-form').on('submit', (event) => {
    event.preventDefault();

    let reg_submit = $('#register-submit');
    let reg_cpf = $('#register-cpf');
    let reg_nome = $('#register-nome');
    let reg_rg = $('#register-rg');
    let reg_email = $('#register-email');
    let reg_password = $('#register-password');

    let inputs = [reg_cpf, reg_nome, reg_rg, reg_email, reg_password];

    if (!validateInputs(inputs)) return;

    addSpinner(reg_submit);
    resetInputs(inputs);

    setTimeout(() => {
        $.ajax({
            url: "/Account/RegisterPost",
            method: "post",
            headers: {
                'RequestVerificationToken': getToken()
            },
            data: {
                Cpf: reg_cpf.val(),
                Nome: reg_nome.val(),
                Rg: reg_rg.val(),
                Email: reg_email.val(),
                Senha: reg_password.val()
            },
            dataType: "json",
            success: (result) => {
                removeSpinner(reg_submit);
                 UIkit.modal.alert(result.message);
                if (!result.success) {
                    reg_submit.html("REGISTRAR");
                }
            },
            error: (error) => {
                removeSpinner(reg_submit);
                reg_submit.html("REGISTRAR");
                UIkit.modal.alert(error.message);
            }
        });
    }, 1500);
});

$("#recover-form").on('submit', (event) => {
    event.preventDefault();

    let recover_submit = $("#recover-submit");
    let recover_cpf = $("#recover-cpf");
    let recover_email = $("#recover-email");

    if (!validateInputs([recover_cpf, recover_email])) return;
    addSpinner(recover_submit);
    resetInputs([recover_cpf, recover_email]);

    $.ajax({
        url: "/Account/RecoverPost",
        method: "post",
        headers: {
            'RequestVerificationToken': getToken()
        },
        data: {
            Cpf: recover_cpf.val(),
            Email: recover_email.val()
        },
        dataType: "json",
        success: (result) => {
            removeSpinner(recover_submit);
            UIkit.modal.alert(result.message);
            if (!result.success) {
                recover_submit.html("ENVIAR");
            }
        },
        error: (error) => {
            removeSpinner(recover_submit);
            recover_submit.html("ENVIAR");
            UIkit.modal.alert(error.message);
        }
    });
});