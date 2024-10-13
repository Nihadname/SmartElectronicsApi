function addtoBasket(productId, variationId) {

    var data = {
        productId: productId
    };

    if (variationId) {
        data.variationId = variationId;
    }

    $.ajax({
        url: `/Basket/Add?productId=${data.productId}${data.variationId ? `&variationId=${data.variationId}` : ''}`,
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(data), // Use JSON.stringify instead of json.stringify
        success: function (response) {
            console.log(response);
            if (response.success) {
                Swal.fire(
                    'Added!',
                    response.message,
                    'success'
                );
            } else {
                alert(response.message);
            }
        },
        error: function (xhr, status, error) {
            alert('An error occurred: ' + error);
        }
    });
}
