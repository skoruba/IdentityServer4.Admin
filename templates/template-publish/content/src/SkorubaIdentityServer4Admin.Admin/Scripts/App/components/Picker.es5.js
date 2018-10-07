"use strict";

ko.components.register('picker', {
	viewModel: function viewModel(params) {

		var self = this;

		//Constants
		var minSearchSelectConst = 2;
		var inputSearchDelay = 500;
		var topSuggestedItemsConst = 5;

		//Input
		this.textTerm = ko.observable("").extend({ rateLimit: inputSearchDelay });
		this.minSearchText = ko.observable(params.minSearchText || minSearchSelectConst);
		this.multipleSelect = ko.observable(params.multipleSelect || false);

		//Labels
		this.searchInputPlaceholder = ko.observable(params.searchInputPlaceholder || "Enter " + this.minSearchText() + " or more characters");
		this.selectedItemsTitle = ko.observable(params.selectedItemsTitle || "Selected: ");
		this.searchResultTitle = ko.observable(params.searchResultTitle || "Search result: ");
		this.suggestedItemsTitle = ko.observable(params.suggestedItemsTitle || "Suggested items: ");

		this.noItemSelectedTitle = ko.observable(params.noItemSelectedTitle || "No item/s selected");
		this.showAllItemsTitle = ko.observable(params.showAllItemsTitle || "more");
		this.allowSuggestedItems = ko.observable(params.allowSuggestedItems && params.url || false);
		this.topSuggestedItems = ko.observable(params.topSuggestedItems || topSuggestedItemsConst);
		this.allowItemAlreadySelectedNotification = ko.observable(params.allowItemAlreadySelectedNotification || true);
		this.itemAlreadySelectedTitle = ko.observable(params.itemAlreadySelectedTitle || "item already selected");

		//Collections
		this.searchResult = ko.observableArray([]);
		this.selectedResult = ko.observableArray(params.selectedItems || []);
		this.suggestedResult = ko.observableArray([]);

		//Features
		this.loading = ko.observable(false);

		//Sync selected items to hiddenField for server-side
		var selectedItems = ko.toJSON(this.selectedResult);
		if (this.multipleSelect() === true) {
			if (this.selectedResult().length === 0) {
				$("#" + params.hiddenId).val("");
			} else {
				$("#" + params.hiddenId).val(selectedItems);
			}
		} else {
			if (this.selectedResult().length === 0) {
				$("#" + params.hiddenId).val("");
			} else {
				$("#" + params.hiddenId).val(this.selectedResult()[0]);
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
					$.get(params.url + "=" + searchTerm, function (data) {

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
			toastr.info(item + " " + this.itemAlreadySelectedTitle());
		};

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
		};

		//Get suggested items - by default 5 items
		this.getSuggestedItems = function () {

			if (self.allowSuggestedItems() === false) return;

			if (params.url) {
				//start loading
				self.loading(true);

				//make ajax request and result add to suggested result
				$.get(params.url, {
					limit: self.topSuggestedItems()
				}, function (data) {

					self.suggestedResult(data);

					//stop loading
					self.loading(false);
				});
			}
		};

		//Clear search input and search result
		this.clear = function () {
			this.textTerm("");
			self.searchResult([]);
		};

		//Remove selected item
		this.remove = function (item) {
			this.selectedResult.remove(item);
			this.sync();
		};

		//Show all items into suggested items
		this.showAll = function () {

			if (params.url) {
				//start loading
				self.loading(true);

				//make ajax request and result add to search result
				$.get("" + params.url, function (data) {

					self.suggestedResult(data);

					//stop loading
					self.loading(false);
				});
			}
		};

		//Synchronize the selected items to hidden field for server-side part
		this.sync = function () {
			var selectedItems = ko.toJSON(this.selectedResult);
			if (this.multipleSelect() === true) {
				if (this.selectedResult().length === 0) {
					$("#" + params.hiddenId).val("");
				} else {
					$("#" + params.hiddenId).val(selectedItems);
				}
			} else {
				if (this.selectedResult().length === 0) {
					$("#" + params.hiddenId).val("");
				} else {
					$("#" + params.hiddenId).val(this.selectedResult()[0]);
				}
			}
		};

		//Load suggested items
		self.getSuggestedItems();
	},
	template: '<div class="card">' + '<div class="card-body">' + '<input class="form-control" data-bind="textInput: textTerm, attr: {placeholder: searchInputPlaceholder}" />' + '<hr data-bind="visible: searchResult().length > 0" />' + '<div data-bind="visible: searchResult().length > 0, text: searchResultTitle" class="search-title"></div>' + '<div class="block__buttons__add" data-bind="foreach: searchResult"><input type="button" class="btn btn-primary button__add" data-bind="value: $data, click: function() { $parent.add($data); }"></div>' + '<img data-bind="visible: loading()" src="/images/loading.gif" alt="Loading.." />' + '<hr />' + '<div data-bind="visible: selectedResult().length == 0, text: noItemSelectedTitle" class="search-title"></div>' + '<div data-bind="visible: selectedResult().length > 0, text: selectedItemsTitle" class="search-title"></div>' + '<div data-bind="foreach: selectedResult"><button class="btn btn-outline-primary button__delete" data-bind="click: function() { $parent.remove($data); }"><span data-bind="text: $data"></span> <span class="oi oi-x"></span></button></div>' + '<hr data-bind="visible: suggestedResult().length > 0" />' + '<div data-bind="visible: suggestedResult().length > 0, text: suggestedItemsTitle" class="search-title"></div>' + '<!-- ko foreach: suggestedResult -->' + '<input type="button" class="btn btn-light button__add" data-bind="value: $data, click: function() { $parent.add($data); }">' + '<!-- /ko -->' + '<button class="btn btn-default button__show-all" data-bind="visible:allowSuggestedItems(), click: function() { $component.showAll(); }"><span data-bind="text: showAllItemsTitle"></span> <span class="oi oi-plus"></span></button>' + '</div>' + '</div>'
});

ko.applyBindings();

