const Layout = function () {

    const NavBar = function () {

        $(".nav-link").each(function () {
            if ($(this).attr("data-title") == $("title").html()) {
                $(this).addClass("active");
            }
        });

    };

    const Forms = function () {

        $("input").on('keyup keypress', function (e) {
            if (e.which === 13) {
                e.preventDefault();
            }
        });

        $("input[type=number].decimal").on('change', function (e) {
            $(this).val(parseFloat($(this).val()).toFixed(2));
        });

    };

    return {
        init() {
            NavBar();
            Forms();
        }
    }

};

function showLoading(text = "Loading...") {
    $("#loading").modal('show');
};

function hideLoading(reload = true) {
    setTimeout(function () {
        $("#loading").modal('hide');
    }, 1000);

    if (reload) {
        setTimeout(function () {
            location.reload();
        }, 1500);
    }
};

function showToastError(text = "Something went wrong.") {
    var toast = new bootstrap.Toast($('.toast-error'))
    $(".toast-error .toast-body").html(text);
    toast.show();
};

function showToastSuccess(text = "Great! It was successful.") {
    var toast = new bootstrap.Toast($('.toast-success'))
    $(".toast-success .toast-body").html(text);
    toast.show();
};

function confirmation(text = "Do you wish to continue?") {
    $("#confirmationText").text(text);
    $("#confirmation").modal('show');
};

async function runAjax(url, type, data, modal, reload = true) {
    showLoading();

    await $.ajax({
        url: url,
        type: type,
        data: data
    }).done(async function (data, textStatus, xhr) {
        modal.modal('hide');
        showToastSuccess();
        return true;
    }).fail(function (xhr, textStatus) {
        showToastError();
        return false;
    }).always(function (data, textStatus, xhr) {
        hideLoading(reload);
    });

    return true;
}

function formReset(form, validator) {
    form.trigger('reset');
    form.find('input[type=hidden]').val('0');
    form.find('input[type=text]').val('');
    form.find('textarea').val("");
    form.find('input[type=radio]:not([value="No"])').prop('checked', false);
    form.find('.radio-group').each(function (i, rbGroup) {
        $(rbGroup).find('input[type=radio]:first').prop('checked', true).change();
    });
    form.find('input[type=checkbox]').each(function (i, cb) {
        $(cb).attr('checked', false);
        $(cb).next('input[type=hidden]').val('false');
    });
    form.find('.form-select').each(function (i, select) {
        $(this).val($(select).find('option:first').val());
        if ($(this).hasClass('disabled')) {
            $(this).next('button').attr('disabled', true);
        }
    });

    if (validator) {
        validator.resetForm();
    }

    let errorElements = form.find('.border-danger');
    errorElements.map(function (e) {
        $(errorElements[e]).removeClass('border-danger');
    });

    errorElements = form.find('.text-danger:not(i)');
    errorElements.map(function (e) {
        $(errorElements[e]).removeClass('text-danger');
    });
}

function parseMDY(value) {
    var date = value.split("/");
    var m = parseInt(date[0], 10),
        d = parseInt(date[1], 10),
        y = parseInt(date[2], 10);
    return new Date(y, m - 1, d);
}

$(document).ready(() => {
    Layout().init();
});