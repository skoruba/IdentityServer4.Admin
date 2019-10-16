var FormMvc = {

	allowValidateHiddenField: function (form) {
		form.data("validator").settings.ignore = "";
	},

	disableEnter: function (form) {
		form.on('keyup keypress',
			function (e) {
				var keyCode = e.keyCode || e.which;
				if (keyCode === 13) {
					e.preventDefault();
					return false;
				}
			});
	}
};


$(function() {
	$(".single-select").removeAttr("multiple");
	$('[data-toggle="tooltip"]').tooltip();
});