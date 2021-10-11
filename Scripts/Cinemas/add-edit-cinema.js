const AddEditCinema = function () {

    let modalAddEditCinema = {
        modal: $("#aeCinema"),
        form: "#formCinema",
        vars: {
            validator: null,
            dataset: null,
        },
        modalEvents() {
            const _this = this;
            const modal = _this.modal;
            const form = modal.find(_this.form);

            modal.on('show.bs.modal', function (e) {
                _this.vars.dataset = e.relatedTarget.dataset;

                let data = _this.vars.dataset;

                switch (data.action) {
                    case "add":
                        modal.find('.modal-title').html('Add Cinema');
                        break;
                    case "edit":
                        modal.find('#inputDescription').val(data.description);
                        modal.find('#inputSeatCount').val(data.seat_count);
                        modal.find('#inputDefaultPrice').val(parseFloat(data.default_price).toFixed(2));

                        modal.find('.modal-title').html('Edit Cinema');
                        break;
                    default:
                        break;
                }
            });

            modal.on('shown.bs.modal', function () {
                modal.find('#inputDescription').focus();
            });

            modal.on('hidden.bs.modal', function () {
                formReset(form, _this.vars.validator)
            });
        },
        inputEvents() {
            const _this = this;
            const modal = _this.modal;
            const form = modal.find(_this.form);

            modal.find('#btnSave').on('click', async function (e) {
                e.preventDefault();

                let data = _this.vars.dataset;

                let getData = {
                    cinema_id: data.cinema_id || 0,
                    description: modal.find('#inputDescription').val(),
                }

                let _continue = await _this.checkCinema(getData);

                if (!_continue) {
                    showToastError("Cinema conflicts with other cinemas. Try a different description.");
                    $(this).val('');
                    return false;
                }

                form.submit();
            });
        },
        checkCinema(getData) {
            return new Promise(resolve => {
                $.getJSON(
                    "Cinemas/CheckCinema",
                    getData,
                    function (response) {
                        resolve(response);
                    }
                );
            });
        },
        validation() {
            const _this = this;
            const modal = _this.modal;
            const form = modal.find(_this.form);

            _this.vars.validator = form.validate({
                errorElement: "small",
                errorClass: "border-danger text-danger",
                rules: {
                    description: "required",
                    seats: {
                        required: true,
                        min: 18,
                        max: 130,
                    }
                },
                messages: {
                    description: "Please specify Description",
                    seats: {
                        required: "Please specify Seat Count",
                        min: "Please specify Seat Count from 18 to 130",
                        max: "Please specify Seat Count from 18 to 130",
                    }
                },
                submitHandler: function (form) {
                    let postData = {
                        cinema_id: _this.vars.dataset.cinema_id,
                        description: modal.find('#inputDescription').val(),
                        seats: modal.find('#inputSeatCount').val(),
                        default_price: modal.find('#inputDefaultPrice').val(),
                    }

                    runAjax('/Cinemas/SaveCinema', 'POST', postData, modal);
                }
            });
        },
        init() {
            this.modalEvents();
            this.inputEvents();
            this.validation();
        }
    };

    return {
        init() {
            modalAddEditCinema.init();
        }
    }

};

$(document).ready(() => {
    AddEditCinema().init();
});