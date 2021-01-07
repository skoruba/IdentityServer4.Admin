ko.bindingHandlers.modal = {
    init: function (element, valueAccessor) {
        $(element).modal({
            show: false
        });

        var value = valueAccessor();
        if (ko.isObservable(value)) {
            $(element).on('hidden.bs.modal', function () {
                value(false);
            });
        }

    },
    update: function (element, valueAccessor) {
        var value = valueAccessor();
        if (ko.utils.unwrapObservable(value)) {
            $(element).modal('show');
        } else {
            $(element).modal('hide');
        }
    }
}

ko.components.register('picker', {
    viewModel: function (params) {

        var self = this;

        //Constants
        const minSearchSelectConst = 2;
        const inputSearchDelay = 500;
        const topSuggestedItemsConst = 5;

        //Input
        this.textTerm = ko.observable("").extend({ rateLimit: inputSearchDelay });
        this.minSearchText = ko.observable(params.minSearchText || minSearchSelectConst);
        this.multipleSelect = ko.observable(params.multipleSelect || false);

        //Labels
        this.searchInputPlaceholder = ko.observable(params.searchInputPlaceholder || `Enter ${this.minSearchText()} or more characters`);
        this.selectedItemsTitle = ko.observable(params.selectedItemsTitle || "Selected: ");
        this.searchResultTitle = ko.observable(params.searchResultTitle || "Search result: ");
        this.suggestedItemsTitle = ko.observable(params.suggestedItemsTitle || "Suggested items: ");

        this.noItemSelectedTitle = ko.observable(params.noItemSelectedTitle || "No item/s selected");
        this.showAllItemsTitle = ko.observable(params.showAllItemsTitle || "more");
        this.allowSuggestedItems = ko.observable((params.allowSuggestedItems && params.url) || false);
        this.topSuggestedItems = ko.observable(params.topSuggestedItems || topSuggestedItemsConst);
        this.allowItemAlreadySelectedNotification = ko.observable(params.allowItemAlreadySelectedNotification || true);
        this.itemAlreadySelectedTitle = ko.observable(params.itemAlreadySelectedTitle || "item already selected");

        //Collections
        this.searchResult = ko.observableArray([]);
        this.selectedResult = ko.observableArray(params.selectedItems || []);
        this.suggestedResult = ko.observableArray([]);

        //Features
        this.loading = ko.observable(false);

        //Setup values for editing mode
        //Value for dialog
        this.isVisibleEditDialog = ko.observable(false);

        //In this value will be stored new value during editing mode
        this.editedItem = ko.observable("");

        //In this value will be stored original value during editing mode
        this.editedItemOriginal = ko.observable("");

        //Sync selected items to hiddenField for server-side
        const selectedItems = ko.toJSON(this.selectedResult);
        if (this.multipleSelect() === true) {
            if (this.selectedResult().length === 0) {
                $(`#${params.hiddenId}`).val("");
            } else {
                $(`#${params.hiddenId}`).val(selectedItems);
            }
        } else {
            if (this.selectedResult().length === 0) {
                $(`#${params.hiddenId}`).val("");
            } else {
                $(`#${params.hiddenId}`).val(this.selectedResult()[0]);
            }
        }

        //Track changes on search input
        this.textTerm.subscribe(function (searchTerm) {

            //If is search input clear -> clear search result
            if (searchTerm.trim() === "") {
                self.searchResult([]);
            }

            //If search term isn't empty and has min length characters
            if (searchTerm.trim() !== "" && searchTerm.trim().length >= self.minSearchText()) {

                if (params.url) {
                    //start loading
                    self.loading(true);

                    //make ajax request and result add to search result
                    $.get(`${params.url}=${searchTerm}`,
                        function (data) {

                            if (data.indexOf(searchTerm) === -1) {
                                data.push(searchTerm);
                            }

                            self.searchResult(data);

                            //stop loading
                            self.loading(false);
                        });
                } else {
                    self.searchResult([searchTerm]);
                }
            }
        });

        this.notify = function (item) {
            toastr.options.closeButton = true;
            toastr.options.preventDuplicates = true;
            toastr.info(`${item} ${this.itemAlreadySelectedTitle()}`);
        }

        this.notifyError = function (error) {
            toastr.options.closeButton = true;
            toastr.options.preventDuplicates = true;
            toastr.error(error);
        }

        //Action methods
        this.add = function (item) {

            //Replace quotes
            item = item.replace(/'/g, "").replace(/"/g, "");

            //Check if selected item is exists in selected items array
            if (this.selectedResult.indexOf(item) > -1) {

                if (this.allowItemAlreadySelectedNotification() === true) {
                    this.notify(item);
                }

                return;
            }

            //Single select -> the original value replace with new
            if (this.multipleSelect() === false) {
                this.selectedResult([]);
                this.selectedResult.push(item);
                this.clear();
                this.sync();
            }
            //Multiple select
            else if (this.multipleSelect() === true) {
                this.selectedResult.push(item);
                this.clear();
                this.sync();
            }
        }

        //Get suggested items - by default 5 items
        this.getSuggestedItems = function () {

            if (self.allowSuggestedItems() === false) return;

            if (params.url) {
                //start loading
                self.loading(true);

                //make ajax request and result add to suggested result
                $.get(params.url,
                    {
                        limit: self.topSuggestedItems()
                    },
                    function (data) {

                        self.suggestedResult(data);

                        //stop loading
                        self.loading(false);
                    });
            }
        }

        //Clear search input and search result
        this.clear = function () {
            this.textTerm("");
            self.searchResult([]);
        }

        //Show dialog for editing items
        this.showEditDialog = function (item) {
            //Setup value for showing the dialog
            self.isVisibleEditDialog(true);

            //Setup values for editing
            self.editedItem(item);
            self.editedItemOriginal(item);
        }

        //Save item after editing
        this.submitEditDialog = function () {

            //If is editing item empty
            if (self.editedItem().trim() === "") {
                return;
            }

            //If item already exists show error message
            if (self.checkIfItemExists(self.editedItemOriginal().trim(), self.editedItem().trim())) {
                self.notifyError(`${self.editedItem().trim()} ${this.itemAlreadySelectedTitle()}`);
                return;
            }

            //Update old value with new one
            self.update(self.editedItemOriginal().trim(), self.editedItem().trim());

            //Hide the dialog for editing
            self.isVisibleEditDialog(false);
        }

        //Check if item which is editing is already selected
        this.checkIfItemExists = function (item, newValue) {

            //The original item is same like new item
            if (item.trim() === newValue.trim()) {
                return false;
            }

            //Item is already selected
            if (this.selectedResult.indexOf(newValue) > -1) {
                return true;
            }

            return false;
        }

        //Update selected result
        this.update = function (item, newValue) {

            for (var i = 0; i < self.selectedResult().length; i++) {
                if (self.selectedResult()[i] === item) {
                    self.selectedResult()[i] = newValue;
                    self.selectedResult.valueHasMutated();
                    break;
                }
            }

            self.sync();
        }

        //Remove selected item
        this.remove = function (item) {
            this.selectedResult.remove(item);
            this.sync();
        }

        //Show all items into suggested items
        this.showAll = function () {

            if (params.url) {
                //start loading
                self.loading(true);

                //make ajax request and result add to search result
                $.get(`${params.url}`,
                    function (data) {

                        self.suggestedResult(data);

                        //stop loading
                        self.loading(false);
                    });
            }
        }

        //Synchronize the selected items to hidden field for server-side part
        this.sync = function () {
            const selectedItems = ko.toJSON(this.selectedResult);
            if (this.multipleSelect() === true) {
                if (this.selectedResult().length === 0) {
                    $(`#${params.hiddenId}`).val("");
                } else {
                    $(`#${params.hiddenId}`).val(selectedItems);
                }
            } else {
                if (this.selectedResult().length === 0) {
                    $(`#${params.hiddenId}`).val("");
                } else {
                    $(`#${params.hiddenId}`).val(this.selectedResult()[0]);
                }
            }
        }

        //Load suggested items
        self.getSuggestedItems();
    },
    template: '<div class="card">' +
        '<div class="card-body">' +
        '<input class="form-control" data-bind="textInput: textTerm, attr: {placeholder: searchInputPlaceholder}" />' +
        '<hr data-bind="visible: searchResult().length > 0" />' +
        '<div data-bind="visible: searchResult().length > 0, text: searchResultTitle" class="search-title"></div>' +
        '<div class="block__buttons__add" data-bind="foreach: searchResult"><input type="button" class="btn btn-primary button__add" data-bind="value: $data, click: function() { $parent.add($data); }"></div>' +
        '<img data-bind="visible: loading()" src="/images/loading.gif" alt="Loading.." />' +
        '<hr />' +
        '<div data-bind="visible: selectedResult().length == 0, text: noItemSelectedTitle" class="search-title"></div>' +
        '<div data-bind="visible: selectedResult().length > 0, text: selectedItemsTitle" class="search-title"></div>' +
        '<div class="row" data-bind="foreach: selectedResult">' +
        '<div class="col-12">' +
        '<span class="button__text text-primary border-primary" data-bind="text: $data"></span>' +
        '<button class="btn btn-outline-primary button__update" data-bind="click: function() { $parent.showEditDialog($data) }"><span class="oi oi-pencil"></span></button>' +
        '<button class="btn btn-outline-primary button__delete" data-bind="click: function() { $parent.remove($data); }"><span class="oi oi-x"></span></button>' +
        '</div>' +
        '</div>' +
        '<hr data-bind="visible: suggestedResult().length > 0" />' +
        '<div data-bind="visible: suggestedResult().length > 0, text: suggestedItemsTitle" class="search-title"></div>' +
        '<!-- ko foreach: suggestedResult -->' +
        '<input type="button" class="btn btn-light button__add" data-bind="value: $data, click: function() { $parent.add($data); }">' +
        '<!-- /ko -->' +
        '<button class="btn btn-secondary button__show-all" data-bind="visible:allowSuggestedItems(), click: function() { $component.showAll(); }"><span data-bind="text: showAllItemsTitle"></span> <span class="oi oi-plus"></span></button>' +
        '</div>' +
        '</div>' +
        '<div class="modal fade" tabindex="-1" role="dialog" data-bind="modal:isVisibleEditDialog">' +
        '<div class="modal-dialog">' +
        '<div class="modal-content"> ' +
        '<div class="modal-header">' +
        '<h5 data-bind="text: editedItemOriginal" class="modal-title"></h5>' +
        '<button type="button" class="close" data-dismiss="modal">&times;</button>' +
        '</div>' +
        '<div class="modal-body">' +
        '<input class="form-control" type="hidden" data-bind="value: editedItemOriginal" />' +
        '<input class="form-control" type="text" data-bind="value: editedItem" />' +
        '</div>' +
        '<div class="modal-footer">' +
        '<button type="button" class="btn btn-primary btn-xs" data-bind="click: submitEditDialog">OK</button>' +
        '</div>' +
        '</div>' +
        '</div>' +
        '</div>'
});

ko.applyBindings();