$(document).ready(function () {
    $('#download-data').click(function () {
        var dataFormat = $('input[name="DataFormat"]:checked').val();

        if (typeof dataFormat === "undefined")
        {
            $('#data-format-error').text('Please select a format');
            return;
        }

        var url = '/Account/DownloadUserData?dataFormat=' + dataFormat;
        window.open(url, '_blank');
    });
});