const AddEditSchedule = function () {

    const dropdowns = {
        movie: function ($dropdown, defaultText = "Select", defaultValue = "") {
            return new Promise(resolve => {
                $.getJSON(
                    "Movies/GetMovie",
                    { },
                    function (response) {
                        if (response.length != 0) {
                            $dropdown.empty();
                            $dropdown.append($('<option>', {
                                value: defaultValue,
                                text: defaultText,
                                "data-default_price": "0.00"
                            }));

                            for (var i = 0; i < response.length; i++) {
                                $dropdown.append($('<option>', {
                                    value: response[i].movie_id,
                                    text: response[i].title,
                                    "data-default_price": response[i].default_price,
                                }));
                            }
                        }
                        resolve();
                    }
                );
            })
        },
        cinema: function ($dropdown, defaultText = "Select", defaultValue = "") {
            return new Promise(resolve => {
                $.getJSON(
                    "Cinemas/GetCinema",
                    { },
                    function (response) {
                        if (response.length != 0) {
                            $dropdown.empty();
                            $dropdown.append($('<option>', {
                                value: defaultValue,
                                text: defaultText,
                                "data-default_price": "0.00"
                            }));

                            for (var i = 0; i < response.length; i++) {
                                $dropdown.append($('<option>', {
                                    value: response[i].cinema_id,
                                    text: response[i].description,
                                    "data-default_price": response[i].default_price,
                                }));
                            }
                        }
                        resolve();
                    }
                );
            })
        },
    }

    let modalAddEditSchedule = {
        modal: $("#aeSchedule"),
        form: "#formSchedule",
        vars: {
            dataset: null,
            validator: null,
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
                        modal.find('.modal-title').html('Add Schedule');
                        break;
                    case "edit":
                        modal.find('#selectMovie').val(data.movie_id);
                        modal.find('#selectCinema').val(data.cinema_id);
                        modal.find('#inputDateTime').val(data.date_time);
                        modal.find('#inputPrice').val(data.price);

                        modal.find('.modal-title').html('Edit Schedule');
                        break;
                    default:
                        break;
                }
            });

            modal.on('shown.bs.modal', function () {
                modal.find('#selectMovie').focus();
            });

            modal.on('hidden.bs.modal', function () {
                formReset(form, _this.vars.validator);
            });
        },
        inputEvents() {
            const _this = this;
            const modal = _this.modal;
            const form = modal.find(_this.form);

            dropdowns.movie(modal.find("#selectMovie"), "Select Movie");
            dropdowns.cinema(modal.find("#selectCinema"), "Select Cinema");

            modal.find('#selectMovie').on('change', function (e) {
                modal.find('#inputPrice').val(parseFloat($(this).find('option:selected').attr('data-default_price')).toFixed(2));
            });

            modal.find('#selectCinema').on('change', function (e) {
                modal.find('#inputPrice').val(parseFloat($(this).find('option:selected').attr('data-default_price')).toFixed(2));
            });

            modal.find('#inputDateTime').on('change', function (e) {
                let data = _this.vars.dataset;
                let val = data.action == "edit" ? data.date_time : "";

                if (new Date() > new Date($(this).val())) {
                    showToastError("Date and Time must not have passed already.");
                    $(this).val(val);
                }
            });

            modal.find('#btnSave').on('click', async function (e) {
                e.preventDefault();

                let data = _this.vars.dataset;

                // Check maximum 5 time slots per day
                let getData = {
                    schedule_id: data.schedule_id || 0,
                    cinema_id: modal.find('#selectCinema').val(),
                    date_time: modal.find('#inputDateTime').val(),
                }

                let _continue = await _this.checkScheduleCount(getData);

                if (!_continue) {
                    showToastError("Specified Cinema reached maximum of 5 schedules for the specified Date.");
                    $(this).val('');
                    return false;
                }

                // Check schedule conflict
                _continue = await _this.checkSchedule(getData);

                if (!_continue) {
                    showToastError("Schedule conflicts with other schedule. Try a different time.");
                    $(this).val('');
                    return false;
                }

                form.submit();
            });
        },
        checkScheduleCount(getData) {
            return new Promise(resolve => {
                $.getJSON(
                    "Schedules/CheckScheduleCount",
                    getData,
                    function (response) {
                        resolve(response);
                    }
                );
            });
        },
        checkSchedule(getData) {
            return new Promise(resolve => {
                $.getJSON(
                    "Schedules/CheckSchedule",
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
                debug: false,
                errorElement: "small",
                errorClass: "border-danger text-danger",
                rules: {
                    movie_id: "required",
                    cinema_id: "required",
                    date_time: "required",
                    price: {
                        required: true,
                        min: 0.01,
                    }
                },
                messages: {
                    movie_id: "Please select a Movie",
                    cinema_id: "Please select a Cinema",
                    date_time: "Please specify Date Time",
                    price: "Please specify Price",
                },
                submitHandler: function (form) {
                    let data = _this.vars.dataset;

                    let postData = {
                        schedule_id: data.schedule_id || 0,
                        movie_id: modal.find('#selectMovie').val(),
                        cinema_id: modal.find('#selectCinema').val(),
                        date_time: modal.find('#inputDateTime').val(),
                        price: modal.find('#inputPrice').val(),
                    }

                    runAjax('/Schedules/SaveSchedule', 'POST', postData, modal);
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
            modalAddEditSchedule.init();
        }
    }

};

$(document).ready(() => {
    AddEditSchedule().init();
});