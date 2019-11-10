var errorLog = {

    eventHandlers: function () {
        $(".error-log-delete-button").click(function () {

            $(".error-log-form").validate();

            if ($(".error-log-form").validate().form()) {

                $("#deleteLogsModal").modal("show");
                return false;
            } else {
                $(this).submit();
                return false;
            }
        });

        $(".row-error-detail>td").each(function () {

            var json = $(this).data("error-json");
            var result;

            try {
                result = JSONTree.create(JSON.parse(json));
            } catch (e) {
                result = JSONTree.create(json);
            }

            $(this).html(result);
        });

        $(".btn-error-detail").click(function (e) {

            e.preventDefault();

            var errorId = $(this).data("error-id");

            if ($(".row-error-detail[data-error-id=" + errorId + "]").is(":visible")) {
                $(".row-error-detail[data-error-id=" + errorId + "]").addClass('d-none');
            } else {
                $(".row-error-detail[data-error-id=" + errorId + "]").removeClass('d-none');
            }

            return false;
        });
    },

    init: function () {
        $(function () {

            errorLog.eventHandlers();
        });
    }

};

errorLog.init();
