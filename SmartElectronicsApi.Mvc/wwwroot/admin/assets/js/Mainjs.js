function getJwtToken() {
    return document.cookie.split('; ').find(row => row.startsWith('JwtToken=')).split('=')[1];
}
function deleteItemUser(id) {
    console.log("Attempting to delete user with ID:", id); // Log the ID
    var addressElement = document.querySelector(`.UserTable[data-id="${id}"]`);
    if (!addressElement) {
        console.error("No element found with the specified ID: " + id);
        return;
    }
    console.log("Element found:", addressElement);

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
                url: '/AdminArea/User/Delete/' + id, 
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
                        addressElement.remove(); // Remove the element from the DOM
                    } else {
                        Swal.fire(
                            'Error!',
                            response.message || 'An unknown error occurred.',
                            'error'
                        );
                    }
                },
                error: function (error) {
                    console.log(error);
                    Swal.fire(
                        'Error!',
                        'Error: ' + error.responseText,
                        'error'
                    );
                }
            });
        }
    });
}

function deleteItemRole(id) {
    console.log("Attempting to delete user with ID:", id); // Log the ID
    var addressElement = document.querySelector(`.RoleTable[data-id="${id}"]`);
    if (!addressElement) {
        console.error("No element found with the specified ID: " + id);
        return;
    }
    console.log("Element found:", addressElement);
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
                url: '/AdminArea/Role/Delete/' + id,
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
                        addressElement.remove(); // Remove the element from the DOM
                    } else {
                        Swal.fire(
                            'Error!',
                            response.message || 'An unknown error occurred.',
                            'error'
                        );
                    }
                },
                error: function (error) {
                    console.log(error);
                    Swal.fire(
                        'Error!',
                        'Error: ' + error.responseText,
                        'error'
                    );
                }
            });
        }
    });
}