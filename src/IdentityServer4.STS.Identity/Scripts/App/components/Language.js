window.onload = function () {
	var cultureSelect = document.getElementById('cultureSelect');
	var cultureForm = document.getElementById('selectLanguageForm');

	cultureSelect.onchange = function() {
		cultureForm.submit();
	};
};