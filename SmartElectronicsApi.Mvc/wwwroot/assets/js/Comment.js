function DeleteComment(id) {
    var commentCard = document.querySelector(`.comment-card-unique[data-id="${id}"]`);

    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Comment/Delete/' + id,
                type: "POST", 
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.success) {
                        Swal.fire(
                            'Deleted!',
                            response.message,
                            'success'
                        );
                        commentCard.remove();
                    } else if (response.errors && response.errors.length > 0) {
                      
                        Swal.fire(
                            'Error!',
                            'Errors: ' + response.errors.join(', '),
                            'error'
                        );
                    } else {
                        Swal.fire(
                            'Error!',
                            'Error: ' + response.message,
                            'error'
                        );
                    }
                },
                error: function (error) {
                    Swal.fire(
                        'Error!',
                        'Error: ' + (error.responseJSON?.message || error.responseText || 'An unknown error occurred.'),
                        'error'
                    );
                }
            });
        }
    });
}


$('#submitComment').submit(function () {
        var formData = new FormData();
        var commentText = $('#commentText').val();
        var rating = $('#star-rating .star.selected').last().data('value');
        var ProductId = $('.ProductId').val();
        var files = $('#imageUpload')[0].files;

        // Append form data
        formData.append('Message', commentText);
        formData.append('Rating', rating);
        formData.append('ProductId', ProductId); // Assuming the product ID is in the model

        // Append images
        for (var i = 0; i < files.length; i++) {
            formData.append('Images', files[i]);
        }

        // AJAX request to MVC controller
        $.ajax({
            url: '/Comment/Create',
            type: 'POST',
            headers: {
                'Authorization': 'Bearer ' + getCookie('JwtToken') // Fetch the token from the cookie
            },
            processData: false,  // Don't process the files
            contentType: false,  // Set content type to false for FormData
            data: formData,
            success: function (response) {
                if (response.success) {
                    Swal.fire('Success', response.message, 'success');
                    $('#commentModal').modal('hide');
                } else {
                    Swal.fire('Error', response.message, 'error');
                }
            },
            error: function (error) {
                Swal.fire('Error', 'Failed to submit the comment', 'error');
            }
        });
    });

    // Star rating click event
    $('.star').click(function () {
        $(this).siblings().removeClass('selected');
        $(this).addClass('selected');
    });

