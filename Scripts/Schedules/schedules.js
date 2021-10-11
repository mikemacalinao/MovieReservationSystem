const Schedule = function () {

    let indexSchedule = {
        index: $("#schedule"),
        inputEvents() {
            const _this = this;
            const index = _this.index;

            index.find('#btnExcel').on('click', function () {
                _this.convertToExcel({ schedule_id: 0 });
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
                        showToastError('No records found.');
                    }
                    hideLoading(false);
                }
            );
        },
        init() {
            this.inputEvents();
        }
    };

    return {
        init() {
            indexSchedule.init();
        }
    }

};

$(document).ready(() => {
    Schedule().init();
});