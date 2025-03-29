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
        data: JSON.stringify(data), 
        success: function (response) {
            console.log(response);
            if (response.success) {
                Swal.fire(
                    'Added!',
                    response.message,
                    'success'
                );
                console.log(response);
                let badge = document.querySelector('.basket-badge');
                console.log(badge);
                console.log(response)
                badge.innerText = response.basketCount;
                let cartcount = document.querySelector('.cart-count');
                cartcount.innerText = response.addedCount;
            } else {
                Swal.fire(
                    'Error!',
                    'Error: ' + "User has not logined",
                    'error'
                );
            }
        },
        error: function (xhr, status, error) {
            Swal.fire(
                'Error!',
                'Error: ' + error.responseText,
                'error'
            );
        }
    });
}
function changeQuantity(productId, quantityChange, variationId ) {
    var data = {
        productId: productId,
        quantityChange: quantityChange
    };

    // If variationId is provided, add it to the data object
    if (variationId !== null && variationId !== undefined) {
        data.variationId = variationId;
    } else {
        variationId = "0"; // Use '0' when variationId is null to ensure unique IDs
    }

    $.ajax({
        url: `/Basket/ChangeQuantity?productId=${data.productId}&quantityChange=${data.quantityChange}${data.variationId ? `&variationId=${data.variationId}` : ''}`,
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(data),
        success: function (response) {
            if (response.success) {
                console.log(response);

                // Update the specific basket item quantity dynamically
                let basketItem = $(`#basket-item-${productId}-${variationId}`);
                let quantityInput = basketItem.find('input[type="number"]');
                let newQuantity = parseInt(quantityInput.val()) + quantityChange;

                if (newQuantity <= 0) {

                    basketItem.remove();
                    if (response.basketCount == 0) {
                        showEmptyBasketUI();
                    }
                    // Remove the basket item from the DOM
                } else {
                    // Update the quantity input and subtotal for the item
                    quantityInput.val(newQuantity);

                    // Get the price and discounted price from the data attributes
                    let price = parseFloat(basketItem.attr('data-price')) || 0;
                    let discountedPrice = parseFloat(basketItem.attr('data-discounted-price')) || 0;

                    // Update the subtotal for this item
                    let newSubtotal = discountedPrice > 0 ? discountedPrice * newQuantity : price * newQuantity;
                    let badge = document.querySelector('.basket-badge');
                    if (badge) {
                        badge.innerText = response.basketCount; 
                    }
                    if (response.basketCount == 0) {
                        showEmptyBasketUI();
                    }

                }

                // Recalculate and update total price, discount, and final sale price dynamically
                updateTotals();
            } else {
                Swal.fire(
                    'Error!',
                    response.message,
                    'error'
                );
            }
        },
        error: function (xhr, status, error) {
            Swal.fire(
                'Error!',
                'Error: ' + xhr.responseText,
                'error'
            );
        }
    });
}
function deleteFromBasket(productId, variationId) {
    var data = {
        productId: productId
    };

    if (variationId !== null && variationId !== undefined && variationId !== "null") {
        data.variationId = variationId;
    }

    $.ajax({
        url: `/Basket/Delete?productId=${data.productId}${data.variationId ? `&variationId=${data.variationId}` : ''}`,
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
            if (response.success) {
               

                let variationIdText = variationId !== null && variationId !== undefined && variationId !== "null" ? variationId : "0";

                // Dynamically construct the selector based on productId and variationId
                let basketItemSelector = `#basket-item-${productId}-${variationIdText}`;

                // Find and remove the basket item
                let basketItem = $(basketItemSelector);

                if (basketItem.length) {
                    basketItem.remove();
                } else {
                    console.error('Basket item not found:', basketItemSelector);
                }

                // Update the basket badge count
                let badge = document.querySelector('.basket-badge');
                if (badge) {
                    badge.innerText = response.basketCount;
                }
                
                if (response.basketCount == 0) {
                    showEmptyBasketUI(); 
                }
                // Recalculate and update total price, discount, and final sale price dynamically
                updateTotals();

                // Check if the basket is empty after removing an item
               

            } else {
                Swal.fire(
                    'Error!',
                    'An error occurred while removing the product.',
                    'error'
                );
            }
        },
        error: function (xhr, status, error) {
            Swal.fire(
                'Error!',
                'An error occurred while removing the product.',
                'error'
            );
        }
    });
}

function updateTotals() {
    let totalPrice = 0;
    let totalSalePrice = 0;

    // Loop through all basket items and calculate the totals
    $('.basket-item').each(function () {
        let quantity = parseInt($(this).find('input[type="number"]').val()) || 0;
        let price = parseFloat($(this).attr('data-price')) || 0;
        let discountedPrice = parseFloat($(this).attr('data-discounted-price')) || 0;

        // Use discounted price if available, otherwise use the original price
        totalPrice += price * quantity;
        totalSalePrice += (discountedPrice > 0 ? discountedPrice : price) * quantity;
    });

    // Calculate the discount
    let discount = totalPrice - totalSalePrice;

    // Update the totals in the HTML
    $('#total-price').text(`${totalPrice.toFixed(2)} AZN`);
    $('#total-sale-price').text(`${totalSalePrice.toFixed(2)} AZN`);
    $('#discount').text(`${discount.toFixed(2)} AZN`);
}
function getJwtTokenFromCookie() {
    const name = "JwtToken=";
    const decodedCookie = decodeURIComponent(document.cookie);
    const cookies = decodedCookie.split(';');
    for (let i = 0; i < cookies.length; i++) {
        let cookie = cookies[i];
        while (cookie.charAt(0) === ' ') {
            cookie = cookie.substring(1);
        }
        if (cookie.indexOf(name) === 0) {
            return cookie.substring(name.length, cookie.length);
        }
    }
    return "";
}

function DeleteAll() {
    const token = getJwtTokenFromCookie();
    $.ajax({
        url: `http://localhost:5246/api/Basket/DeleteAll`,
        type: 'DELETE',
        headers: {
            'Authorization': `Bearer ${token}`,
        },
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
           
            // Remove all basket items from the DOM
            $('.basket-item').each(function () {
                $(this).remove();
            });

            // Update the basket badge count to 0
            let badge = document.querySelector('.basket-badge');
            if (badge) {
                badge.innerText = 0;
            }
           
                showEmptyBasketUI();
        
            // Recalculate and update total price, discount, and final sale price dynamically
            updateTotals();

            
        },
        error: function (xhr, status, error) {
            Swal.fire(
                'Error!',
                `${xhr.message}`,
                'error'
            );
        }
    });
}


function showEmptyBasketUI() {
    // Remove all remaining basket items if any are left
    $('.basket-item').remove();

    // Append the empty basket UI
    const emptyBasketHtml = `
        <div class="empty-basket" id="empty-basket">
            <i class="fas fa-box-open fa-3x text-muted mb-3"></i>
            <h2>Səbətiniz boşdur</h2>
            <p>Məhsulları səbətə əlavə edin və alış-verişə başlayın!</p>
            <a href="/" class="btn btn-primary">Alış-verişə davam edin</a>
        </div>`;

    $('.basket-header').after(emptyBasketHtml); // Add it after the header
}