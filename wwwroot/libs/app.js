function addSpinner(element) {
    element.append("<div uk-spinner class='uk-margin-small-left spinner-loading'></div>");
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

    login_submit.html("ENTRANDO...");
    addSpinner(login_submit);

    resetInputs([login_cpf, login_password]);

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

    if (!validateInputs([reg_cpf, reg_nome, reg_rg, reg_email, reg_password])) return;

    reg_submit.html("REGISTRANDO...");
    addSpinner(reg_submit);

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
                    reg_submit.html("ENTRAR");
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
