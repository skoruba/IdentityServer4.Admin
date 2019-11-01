var auditLog = {

    createJsonTree: function (json) {
        var result;

        try {
            result = JSONTree.create(JSON.parse(json));
        } catch (e) {
            result = JSONTree.create(json);
        }

        return result;
    },

    initJsonTrees: function () {
        $(".json-tree").each(function () {

            var json = $(this).data("json-tree");
            var result = auditLog.createJsonTree(json);

            $(this).html(result);
        });
    },

    eventHandlers: function () {
        $(".audit-subject-button").click(function () {
            var subjectId = $(this).data("subject-identifier");
            var subjectName = $(this).data("subject-name");
            var subjectType = $(this).data("subject-type");
            var json = $(this).data("subject-additional-data");

            $(".modal-title").html(subjectName + " - " + subjectId + " - " + "(" + subjectType + ")");
            $(".audit-modal-value").html(auditLog.createJsonTree(json));
            $(".audit-modal").modal("show");
        });

        $(".audit-action-button").click(function () {
            var json = $(this).data("action");
            $(".modal-title").html("");
            $(".audit-modal-value").html(auditLog.createJsonTree(json));
            $(".audit-modal").modal("show");
        });
    },

    init: function () {

        $(function () {
            auditLog.eventHandlers();
            auditLog.initJsonTrees();
        });

    }
};

auditLog.init();