$(function () {
    $('input[name="Policy"]').change(function () {
        var selectedLoginPolicy = $('[name=Policy]:checked').val();
        $('.resetPasswordBy').hide();
        $('#resetPasswordBy' + selectedLoginPolicy).show();
    });

    $('.resetPasswordBy').hide();
});