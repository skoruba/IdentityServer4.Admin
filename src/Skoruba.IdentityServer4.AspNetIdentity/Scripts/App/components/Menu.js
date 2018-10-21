var Menu = {
	
	init: function() {

		$(function () {
			Menu.itemClick();
		});

	},

	itemClick: function() {

		$(".menu-button").click(function (e) {
			e.preventDefault();

			var isMenuVisible = $(".menu-item").is(":visible");
			isMenuVisible ? $(".menu-item").css("display", "") : $(".menu-item").show();
		});
	}


};

Menu.init();