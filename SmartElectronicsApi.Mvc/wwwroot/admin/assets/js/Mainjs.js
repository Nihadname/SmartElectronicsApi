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

function DeleteProduct(id) {
    var ProductTable = document.querySelector(`.ProductTable[data-id="${id}"]`);
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
            // Proceed with AJAX request if user confirms
            $.ajax({
                url: '/AdminArea/Product/Delete/' + id,
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                success: function (response) {
                    Swal.fire(
                        'Deleted!',
                        response.message,
                        'success'
                    );
                    // Remove the corresponding address element from the DOM
                    ProductTable.remove();
                },
                error: function (error) {
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

function DeleteBrand(id) {
    var BrandTable = document.querySelector(`.BrandTable[data-id="${id}"]`);
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
            // Proceed with AJAX request if user confirms
            $.ajax({
                url: '/AdminArea/Brand/Delete/' + id,
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                success: function (response) {
                    Swal.fire(
                        'Deleted!',
                        response.message,
                        'success'
                    );
                    // Remove the corresponding address element from the DOM
                    BrandTable.remove();
                },
                error: function (error) {
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
function DeleteCategory(id) {
    var BrandTable = document.querySelector(`.CategoryTable[data-id="${id}"]`);
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
            // Proceed with AJAX request if user confirms
            $.ajax({
                url: '/AdminArea/Category/Delete/' + id,
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                success: function (response) {
                    Swal.fire(
                        'Deleted!',
                        response.message,
                        'success'
                    );
                    // Remove the corresponding address element from the DOM
                    BrandTable.remove();
                },
                error: function (error) {
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
function DeleteColor(id) {
    var BrandTable = document.querySelector(`.ColorTable[data-id="${id}"]`);
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
            // Proceed with AJAX request if user confirms
            $.ajax({
                url: '/AdminArea/Color/Delete/' + id,
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                success: function (response) {
                    Swal.fire(
                        'Deleted!',
                        response.message,
                        'success'
                    );
                    // Remove the corresponding address element from the DOM
                    BrandTable.remove();
                },
                error: function (error) {
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
function DeleteSetting(id) {
    var BrandTable = document.querySelector(`.SettingTable[data-id="${id}"]`);
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
            // Proceed with AJAX request if user confirms
            $.ajax({
                url: '/AdminArea/Setting/Delete/' + id,
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                success: function (response) {
                    Swal.fire(
                        'Deleted!',
                        response.message,
                        'success'
                    );
                    // Remove the corresponding address element from the DOM
                    BrandTable.remove();
                },
                error: function (error) {
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
function DeleteProductVariation(id) {
    var BrandTable = document.querySelector(`.ProductVariationTable[data-id="${id}"]`);
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
            // Proceed with AJAX request if user confirms
            $.ajax({
                url: '/AdminArea/ProductVariation/Delete/' + id,
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                success: function (response) {
                    Swal.fire(
                        'Deleted!',
                        response.message,
                        'success'
                    );
                    // Remove the corresponding address element from the DOM
                    BrandTable.remove();
                },
                error: function (error) {
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
function DeleteParametrGroupTable(id) {
    var BrandTable = document.querySelector(`.ParametrGroupTable[data-id="${id}"]`);
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
            // Proceed with AJAX request if user confirms
            $.ajax({
                url: '/AdminArea/ParametrGroup/Delete/' + id,
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                success: function (response) {
                    Swal.fire(
                        'Deleted!',
                        response.message,
                        'success'
                    );
                    // Remove the corresponding address element from the DOM
                    BrandTable.remove();
                },
                error: function (error) {
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

function DeleteSubCategory(id) {
    var BrandTable = document.querySelector(`.SubCategoryTable[data-id="${id}"]`);
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
            // Proceed with AJAX request if user confirms
            $.ajax({
                url: '/AdminArea/SubCategory/Delete/' + id,
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                success: function (response) {
                    Swal.fire(
                        'Deleted!',
                        response.message,
                        'success'
                    );
                    // Remove the corresponding address element from the DOM
                    BrandTable.remove();
                },
                error: function (error) {
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


function DeleteSubscriber(id) {
    var BrandTable = document.querySelector(`.Subscriber[data-id="${id}"]`);
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
            // Proceed with AJAX request if user confirms
            $.ajax({
                url: '/AdminArea/Subscriber/Delete/' + id,
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                success: function (response) {
                    Swal.fire(
                        'Deleted!',
                        response.message,
                        'success'
                    );
                    // Remove the corresponding address element from the DOM
                    BrandTable.remove();
                },
                error: function (error) {
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
function DeleteOrder(id) {
    var OrderTable = document.querySelector(`.OrderTable[data-id="${id}"]`);
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
                url: '/AdminArea/Order/Delete/' + id,
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                success: function (response) {
                    Swal.fire(
                        'Deleted!',
                        response.message,
                        'success'
                    );
                    OrderTable.remove();

                    
                },
                error: function (error) {
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