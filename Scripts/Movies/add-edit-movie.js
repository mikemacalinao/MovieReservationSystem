const AddEditMovie = function () {

    let modalAddEditMovie = {
        modal: $("#aeMovie"),
        form: "#formMovie",
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
                        modal.find('#inputMovieID').val("0");

                        modal.find('#inputTitle').rules('add', 'required');
                        modal.find('#taDescription').rules('add', 'required');
                        modal.find('#inputImage').rules('add', 'required');

                        modal.find('#image').show();
                        modal.find('#edit').show();

                        modal.find('.modal-title').html('Add Movie');
                        break;
                    case "edit":
                        modal.find('#inputMovieID').val(data.movie_id);
                        modal.find('#inputImgType').val(data.imgtype);
                        modal.find('#inputTitle').val(data.title);
                        modal.find('#taDescription').val(data.description);
                        modal.find('#inputDefaultPrice').val(parseFloat(data.default_price).toFixed(2));

                        modal.find('#inputTitle').rules('add', 'required');
                        modal.find('#taDescription').rules('add', 'required');
                        modal.find('#inputImage').rules('remove');

                        modal.find('#image').hide();
                        modal.find('#edit').show();

                        modal.find('.modal-title').html('Edit Movie');
                        break;
                    case "image":
                        modal.find('#inputMovieID').val(data.movie_id);
                        modal.find('#inputTitle').val(data.title);
                        modal.find('#taDescription').val(data.description);
                        modal.find('#inputDefaultPrice').val(parseFloat(data.default_price).toFixed(2));

                        modal.find('#inputTitle').rules('remove');
                        modal.find('#taDescription').rules('remove');
                        modal.find('#inputImage').rules('add', 'required');

                        _this.showImage(data.imgurl, data.title);

                        modal.find('#image').show();
                        modal.find('#edit').hide();



                        modal.find('.modal-title').html(data.title);
                        break;
                    default:
                        break;
                }
            });

            modal.on('shown.bs.modal', function () {
                modal.find('#inputTitle').focus();
            });

            modal.on('hidden.bs.modal', function () {
                formReset(form, _this.vars.validator);
                _this.resetImage();
            });
        },
        inputEvents() {
            const _this = this;
            const modal = _this.modal;
            const form = modal.find(_this.form);

            modal.find('#inputImage').on('change', function (e) {
                let data = _this.vars.dataset;
                let file = e.target.files[0];

                if (file) {
                    _this.showImage(URL.createObjectURL(file), data.title);
                } else {
                    _this.resetImage(data.imgurl, data.title);
                }
            });

            modal.find('#btnSave').on('click', async function (e) {
                e.preventDefault();

                let data = _this.vars.dataset;

                let getData = {
                    movie_id: data.movie_id || 0,
                    title: modal.find('#inputTitle').val(),
                    description: modal.find('#taDescription').val(),
                }

                let _continue = await _this.checkMovie(getData);

                if (!_continue) {
                    showToastError("Movie conflicts with other movies. Try a different description.");
                    $(this).val('');
                    return false;
                }

                form.submit();
            });
        },
        showImage(src = "", alt = "") {
            const _this = this;
            const modal = _this.modal;

            modal.find('#imageView').attr('src', src);
            modal.find('#imageView').attr('alt', alt);
            modal.find('#col1').show();
            modal.find('#col2').addClass("col-md-6");
            modal.find('div:first-child').addClass("modal-lg");
        },
        resetImage(src = "", alt = "") {
            const _this = this;
            const modal = _this.modal;

            modal.find('#imageView').attr('src', src);
            modal.find('#imageView').attr('alt', alt);

            if (src == "") {
                modal.find('#col1').hide();
                modal.find('#col2').removeClass("col-md-6");
                modal.find('div:first-child').removeClass("modal-lg");
            }
        },
        checkMovie(getData) {
            return new Promise(resolve => {
                $.getJSON(
                    "Movies/CheckMovie",
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
                    title: 'required',
                    description: 'required',
                    imgfile: 'required',
                },
                messages: {
                    title: "Please specify Title",
                    description: "Please specify Description",
                    imgfile: "Please choose an Image",
                },
                submitHandler: function (form) {
                    form.submit();
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
            modalAddEditMovie.init();
        }
    }

};

$(document).ready(() => {
    AddEditMovie().init();
});