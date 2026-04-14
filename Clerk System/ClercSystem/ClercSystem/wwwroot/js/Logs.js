function ShowMore(event, documentLogId) {

    event.preventDefault(); // stop default redirect


    // Clear old content and show spinner
    $('#modalBody').html('<div class="text-center"><div class="spinner-border"></div></div>');

    var myModal = new bootstrap.Modal(document.getElementById('logModal'));
    myModal.show();

    $.ajax({
        url: '/Logs/GetLogDetails/' + id,
        type: 'GET',
        success: function (data) {
            $('#modalBody').html(data);
        },
        error: function () {
            $('#modalBody').html('<p class="text-danger">Could not retrieve log history.</p>');
        }
    });
}


