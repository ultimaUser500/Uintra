﻿'use strict';
var EditNewsController = (function() {
    var holder;
    var userSelect;
    var datePicker;
    var editor;

    var initUserSelect = function () {
        userSelect = holder.find('#js-user-select').select2({});
    }

    var initPublishDatePicker = function () {
        var dateElem = holder.find('#js-publish-date');
        var dateFormat = dateElem.data('dateFormat');
        var dateElemValue = holder.find('#js-publish-date-value');
        var defaultDate = new Date(dateElem.data('defaultDate'));

        datePicker = new Flatpickr(dateElem[0], {
            allowInput: true,
            weekNumbers: true,
            dateFormat: dateFormat,
            locale: FlatpickrLang.da,
            onChange: function (selectedDates) {
                if (selectedDates.length === 0) {
                    dateElemValue.val('');
                }

                var format = dateElemValue.data('dateFormat');
                var result = format.replace('d', selectedDates[0].getDate())
                                   .replace('M', selectedDates[0].getMonth() + 1)
                                   .replace('yyyy', selectedDates[0].getFullYear());
                dateElemValue.val(result);
            }
        });
        datePicker.setDate(defaultDate, true);
    }

    var initDescriptionControl = function () {
        var dataStorage = holder.find('#js-hidden-description-container');
        if (!dataStorage) {
            throw new Error("EditNews: Hiden input field missing");
        }
        var descriptionElem = holder.find('#description');
        var btn = holder.find('.form__btn._submit');

        editor = App.Helpers.initQuill(descriptionElem[0], dataStorage[0], { theme: 'snow' });

        editor.on('text-change', function () {
            dataStorage.val(editor.container.firstChild.innerHTML);
            if (editor.getLength() > 1 && descriptionElem.hasClass('input-validation-error')) {
                descriptionElem.removeClass('input-validation-error');
            }
        });

        btn.click(function () {
            editor.getLength() <= 1 ?
                descriptionElem.addClass('input-validation-error') :
                descriptionElem.removeClass('input-validation-error');
        });
    }

    var controller =   {
        init: function () {
            holder = $('#js-news-edit-page');
            if (!holder.length) {
                return;
            }
            
            initUserSelect();
            initPublishDatePicker();
            initDescriptionControl();
            App.FileUploadController.init(holder);
        }
    }
    App.AppInitializer.add(controller.init);
    return controller;
})();

window.App = window.App || {};
window.App.EditNewsController = EditNewsController;
