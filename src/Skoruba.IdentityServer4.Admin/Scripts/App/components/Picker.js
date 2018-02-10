ko.components.register('picker', {
	viewModel: function (params) {

		var self = this;

		//Constants
		const minSearchSelecteConst = 2;
		const inputSearchDelay = 500;

		//Input
		this.textTerm = ko.observable("").extend({ rateLimit: inputSearchDelay });
		this.minSearchText = ko.observable(params.minSearchText || minSearchSelecteConst);
		this.multipleSelect = ko.observable(params.multipleSelect || false);

		//Labels
		this.searchInputPlaceholder = ko.observable(params.searchInputPlaceholder || `Enter ${this.minSearchText()} or more characters`);
		this.selectedItemsTitle = ko.observable(params.selectedItemsTitle || "Selected: ");

		//Collections
		this.searchResult = ko.observableArray([]);
		this.selectedResult = ko.observableArray(params.selectedItems || []);

		//Features
		this.loading = ko.observable(false);

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

		//Action methods
		this.add = function (item) {

			//Replace quotes
			item = item.replace(/'/g, "").replace(/"/g, "");

			//Check if selected item is exists in selected items array
			if (this.selectedResult.indexOf(item) > -1) {
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


		//Clear search input and search result
		this.clear = function () {
			this.textTerm("");
			self.searchResult([]);
		}

		//Remove selected item
		this.remove = function (item) {
			this.selectedResult.remove(item);
			this.sync();
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
	},
	template: '<input class="form-control" data-bind="textInput: textTerm, attr: {placeholder: searchInputPlaceholder}" />' +
	'<div class="block__buttons__add" data-bind="foreach: searchResult"><input type="button" class="btn button__add" data-bind="value: $data, click: function() { $parent.add($data); }"></div>' +
	'<img data-bind="visible: loading()" src="/images/loading.gif" alt="Loading.." />' +
	'<hr data-bind="visible: selectedResult().length > 0" />' +
	'<div data-bind="visible: selectedResult().length > 0, text: selectedItemsTitle" class="search-title"></div>' +
	'<div data-bind="foreach: selectedResult"><button class="btn button__delete" data-bind="click: function() { $parent.remove($data); }"><span data-bind="text: $data"></span> <span class="glyphicon glyphicon-remove"></span></button></div>'
});

ko.applyBindings();