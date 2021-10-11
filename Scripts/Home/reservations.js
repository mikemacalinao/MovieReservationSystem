const Reservations = function () {

    const dropdowns = {
        schedule: function ($dropdown, defaultText = "Select", defaultValue = "", cinema_id = "0", schedule_id = "") {
            return new Promise(resolve => {
                $.getJSON(
                    "Reservations/GetCinemaSchedules",
                    {
                        cinema_id: cinema_id
                    },
                    function (response) {
                        $dropdown.empty();

                        $dropdown.append($('<option>', {
                            value: defaultValue,
                            text: defaultText,
                            "data-date_time": defaultValue,
                            "data-title": defaultValue,
                            "data-price": defaultValue,
                            "data-imgurl": defaultValue,
                        }));

                        if (response.length != 0) {

                            for (var i = 0; i < response.length; i++) {
                                var date_time = new Date(response[i].date_time).toLocaleDateString() + " " + new Date(response[i].date_time).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

                                $dropdown.append($('<option>', {
                                    value: response[i].schedule_id,
                                    text: response[i].title + " - " + date_time,
                                    "data-date_time": response[i].date_time,
                                    "data-title": response[i].title,
                                    "data-price": response[i].price,
                                    "data-imgurl": response[i].imgurl,
                                    "data-cinema_id": cinema_id,
                                }));
                            }

                            $dropdown.val(schedule_id).change();
                        }
                        resolve();
                    }
                );
            })
        },
        cinema: function ($dropdown, defaultText = "Select", defaultValue = "", cinema_id = "") {
            return new Promise(resolve => {
                $.getJSON(
                    "Cinemas/GetCinema",
                    {},
                    function (response) {
                        if (response.length != 0) {
                            $dropdown.empty();
                            $dropdown.append($('<option>', { value: defaultValue, text: defaultText }));

                            for (var i = 0; i < response.length; i++) {
                                if (response[i].schedule_count > 0) {
                                    $dropdown.append($('<option>', {
                                        value: response[i].cinema_id,
                                        text: response[i].description,
                                        "data-seat_count": response[i].seat_count,
                                    }));
                                }
                            }

                            $dropdown.val(cinema_id).change();
                        }
                        resolve();
                    }
                );
            })
        },
    }

    let indexReservations = {
        index: $("#reservations"),
        form: "#formReservation",
        vars: {
            dataset: null,
            validator: null,
            seatCount: 0,
            seatsAvailable: 0,
            cinemaSeats: [],
            selectedSeats: [],
            cinemaChanged: false,
            schedule_id: ""
        },
        indexEvents() {
            const _this = this;
            const index = _this.index;

            let url = new URL(window.location.href);
            let cinema_id = url.searchParams.get("cinema_id");
            let schedule_id = url.searchParams.get("schedule_id");

            if (schedule_id) {
                _this.vars.schedule_id = schedule_id;
            }

            dropdowns.cinema(index.find("#selectCinema"), "Select Cinema", "", cinema_id);
        },
        inputEvents() {
            const _this = this;
            const index = _this.index;
            const form = index.find(_this.form);

            index.find('#selectCinema').on('change', function () {
                dropdowns.schedule(index.find("#selectSchedule"), "Select Schedule", "", $(this).val(), _this.vars.schedule_id);

                index.find('#selectSchedule').val("");
                index.find('#inputDateTime').val("");
                index.find('#inputTitle').val("");
                index.find('#inputPrice').val("0.00");
                index.find('#imgMovie').hide();

                _this.vars.seatCount = parseInt($(this).find('option:selected').attr('data-seat_count'));
                _this.vars.selectedSeats = [];
                _this.countAndCompute();

                _this.vars.cinemaChanged = true;
            });

            index.find('#selectSchedule').on('change', async function () {
                if (!_this.vars.cinemaChanged) {
                    if ($(this).val() != "") {
                        location.href = "/Reservations?schedule_id=" + $(this).val() + "&cinema_id=" + $(this).find('option:selected').attr('data-cinema_id');
                    } else {
                        let cinema_id = index.find('#selectCinema').val();

                        if (cinema_id == "") {
                            location.href = "/Reservations";
                        } else {
                            location.href = "/Reservations?cinema_id=" + cinema_id;
                        }
                    }
                }

                if ($(this).val() != "") {
                    let APIBaseURL = $('#APIBaseURL').val();
                    let UploadFolder = $('#UploadFolder').val();
                    let _imgurl = APIBaseURL + UploadFolder + $(this).find('option:selected').attr('data-imgurl');

                    index.find('#inputDateTime').val($(this).find('option:selected').attr('data-date_time'));
                    index.find('#inputTitle').val($(this).find('option:selected').attr('data-title'));
                    index.find('#inputPrice').val(parseFloat($(this).find('option:selected').attr('data-price')).toFixed(2));
                    index.find('#imgMovie').attr('alt', $(this).find('option:selected').attr('data-title'));
                    index.find('#imgMovie').attr('src', _imgurl);
                    index.find('#imgMovie').show();

                    _this.vars.cinemaSeats = await _this.getCinemaSeats({ schedule_id: $(this).val() });
                    _this.vars.selectedSeats = [];
                    _this.countAndCompute();
                }

                _this.vars.cinemaChanged = false;
                _this.vars.schedule_id = "";
            });

            index.find('.btn-secondary').on('click', async function () {
                let selectedSeats = _this.vars.selectedSeats;
                let seat_id = parseInt($(this).attr('data-seat_id'));

                if ($(this).hasClass('btn-secondary')) {
                    $(this).removeClass('btn-secondary');
                    $(this).addClass('btn-success selected');

                    selectedSeats.push({
                        seat_id: seat_id
                    });
                } else {
                    $(this).removeClass('btn-success selected');
                    $(this).addClass('btn-secondary');

                    let index = selectedSeats.indexOf(seat_id);
                    selectedSeats.splice(index, 1);
                }

                _this.countAndCompute();
            });

            index.find('#btnSave').on('click', function (e) {
                e.preventDefault();
                e.stopImmediatePropagation();

                if (_this.vars.selectedSeats.length == 0) {
                    showToastError("Please select seats to be reserved.");
                } else {
                    let _seatCount = _this.vars.selectedSeats.length;
                    let _total = index.find('#inputTotal').val();
                    let _text = _seatCount + " seat(s) selected with a total of: ₱" + _total + ". Are you sure you want to proceed reservation?";
                    confirmation(_text);
                }
            });

            $('#confirmationYes').on('click', function () {
                form.submit();
            });

            index.find('#btnClear').on('click', function (e) {
                index.find('.selected').addClass('btn-secondary');
                index.find('.selected').removeClass('btn-success selected');
                _this.vars.selectedSeats = [];

                _this.countAndCompute();
            });
        },
        countAndCompute() {
            const _this = this;
            const index = _this.index;

            let _seatCount = _this.vars.seatCount;
            let _selectedSeats = _this.vars.selectedSeats.length;
            let _reservedSeats = _this.vars.cinemaSeats.filter(x => x.status == 'RESERVED').length;

            // Count
            _this.vars.seatsAvailable = _seatCount - _selectedSeats - _reservedSeats;

            index.find('#inputAvailable').val(_this.vars.seatsAvailable || 0);
            index.find('#inputSelected').val(_selectedSeats || 0);

            // Compute
            index.find('#inputTotal').val((_selectedSeats * parseFloat(index.find('#inputPrice').val())).toFixed(2));
        },
        getCinemaSeats(getData) {
            return new Promise(resolve => {
                $.getJSON(
                    "Reservations/GetCinemaSeats",
                    getData,
                    function (response) {
                        resolve(response);
                    }
                );
            });
        },
        validation() {
            const _this = this;
            const index = _this.index;
            const form = index.find(_this.form);

            _this.vars.validator = form.validate({
                debug: false,
                errorElement: "small",
                errorClass: "border-danger text-danger",
                rules: {
                    schedule_id: "required",
                    customer_name: "required",
                },
                messages: {
                    schedule_id: "Please select a Schedule",
                    customer_name: "Please specify Customer's Name",
                },
                submitHandler: function (form) {
                    let postData = {
                        schedule_id: index.find('#selectSchedule').val(),
                        customer_name: index.find('#inputName').val(),
                        reservation_details: JSON.stringify(_this.vars.selectedSeats),
                        total: index.find('#inputTotal').val(),
                    }

                    runAjax('/Reservations/SaveReservation', 'POST', postData, index);
                }
            });
        },
        init() {
            this.indexEvents();
            this.inputEvents();
            this.validation();
        }
    };

    return {
        init() {
            indexReservations.init();
        }
    }

};

$(document).ready(() => {
    Reservations().init();
});