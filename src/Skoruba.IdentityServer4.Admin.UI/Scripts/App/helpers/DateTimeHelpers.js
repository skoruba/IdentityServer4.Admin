//Based on this article
//https://sebnilsson.com/blog/display-local-datetime-with-moment-js-in-asp-net/

$(function () {

	$('.local-datetime').each(function () {
		var $this = $(this), utcDate = parseInt($this.attr('data-utc'), 10) || 0;

		if (!utcDate) {
			return;
		}

		var local = moment.utc(utcDate).local();
		var formattedDate = local.format('DD MMM YYYY HH:mm');
		$this.text(formattedDate);
	});
	
	$('[data-toggle="tooltip"]').tooltip();

});