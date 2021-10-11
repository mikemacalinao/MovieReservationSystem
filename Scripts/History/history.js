const History = function () {

    let indexHistory = {
        index: $("#history"),
        vars: {
            selected: [],
            schedule_id: "",
        },
        indexEvents() {
            const _this = this;
            const index = _this.index;

            let url = new URL(window.location.href);
            _this.vars.schedule_id = url.searchParams.get("schedule_id");

            if (_this.vars.schedule_id) {
                let init = async function () {
                    let schedule = await _this.getSchedule({ schedule_id: _this.vars.schedule_id });

                    let date_time = schedule[0].date_time;
                    let reserved_count = parseInt(schedule[0].reserved_count);

                     // disable cancellation if date & time already passed or if there's none to be cancelled
                    index.find('#btnCancel').attr('disabled', new Date(date_time) < new Date() || reserved_count == 0);

                    let date_time_formatted = new Date(date_time).toLocaleDateString() + " " + new Date(date_time).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
                    index.find('.card-title').html(schedule[0].title + " (" + date_time_formatted + ")");
                }();
            }
        },
        inputEvents() {
            const _this = this;
            const index = _this.index;

            index.find('#btnCancel').on('click', async function () {
                if (_this.vars.selected.length == 0) {
                    confirmation("No reservations selected, do you wish to cancel all reservations?");

                    $('#confirmationYes').on('click', function () {
                        let postData = {
                            schedule_id: _this.vars.schedule_id,
                        }

                        runAjax('/Reservations/CancelAllReservations', 'POST', postData, index);
                    });
                } else {
                    let postData = {
                        cancel_reservation: JSON.stringify(_this.vars.selected),
                    }

                    runAjax('/Reservations/CancelReservations', 'POST', postData, index);
                }
            });

            index.find('.btn-secondary').on('click', async function () {
                let selected = _this.vars.selected;
                let reservation_id = parseInt($(this).attr('data-reservation_id'));

                if ($(this).hasClass('btn-secondary')) {
                    $(this).removeClass('btn-secondary');
                    $(this).addClass('btn-danger');

                    selected.push({
                        reservation_id: reservation_id
                    });
                } else {
                    $(this).removeClass('btn-danger');
                    $(this).addClass('btn-secondary');

                    let index = selected.indexOf(reservation_id);
                    selected.splice(index, 1);
                }
            });

            index.find('#btnExcel').on('click', function () {
                _this.convertToExcel({ schedule_id: _this.vars.schedule_id });
            });
        },
        convertToExcel(getData) {
            showLoading();
            $.getJSON(
                "History/ExportHistory",
                getData,
                function (response) {
                    if (response.data === 'success') {
                        window.location = "History/DownloadHistory";
                        showToastSuccess("Successfully exported History to Excel");
                    } else {
                        showToastError();
                    }
                    hideLoading(false);
                }
            );
        },
        getSchedule(getData) {
            showLoading();
            return new Promise(resolve => {
                $.getJSON(
                    "Schedules/GetSchedule",
                    getData,
                    function (response) {
                        resolve(response);
                        hideLoading(false);
                    }
                );
            });
        },
        init() {
            this.indexEvents();
            this.inputEvents();
        }
    };

    return {
        init() {
            indexHistory.init();
        }
    }

};

$(document).ready(() => {
    History().init();
});