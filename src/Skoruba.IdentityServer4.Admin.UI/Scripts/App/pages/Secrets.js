$(function () {

    var adminSecrets = {

        guid: function () {
            return "ss-s-s-s-sss".replace(/s/g, adminSecrets.s4);
        },

        s4: function () {
            return Math.floor((1 + Math.random()) * 0x10000)
                .toString(16)
                .substring(1);
        },

        eventHandlers: function() {
            $("#generate-guid-button").click(function() {
                $("#secret-input").val(adminSecrets.guid());
            });
        },

        init: function() {

            adminSecrets.eventHandlers();

        }

    };


    adminSecrets.init();

})