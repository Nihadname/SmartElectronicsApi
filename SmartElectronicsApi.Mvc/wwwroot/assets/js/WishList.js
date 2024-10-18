function getJwtTokenFromCookieWish() {
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

$(document).ready(function () {
    // Fetch wishlist product IDs for the current user
    const token = getJwtTokenFromCookieWish();

    $.ajax({
        url: 'http://localhost:5246/api/WishList/GetUserWishListProducts', // Adjust based on your API endpoint
        type: 'GET',
        headers: {
            'Authorization': `Bearer ${token}`,
        },
        success: function (response) {
            var wishListProductIds = response.productIds;

            // Loop through all wishlist-heart elements
            $('.wishlist-heart').each(function () {
                var productId = $(this).data('id');

                // Check if the product is in the user's wishlist and highlight accordingly
                if (wishListProductIds.includes(productId)) {
                    $(this).find('i').addClass('added').css('color', '#ef4444');
                }
            });
        }
    });

    // Existing logic for handling heart icon click remains unchanged
    $('.wishlist-heart').on('click', function () {
        var heartIcon = $(this).find('i');
        var productId = $(this).data('id');
        var variationId = null;

        var isAdded = heartIcon.hasClass('added');

        if (!isAdded) {
            var data = { productId: productId };

            if (variationId) {
                data.variationId = variationId;
            }

            $.ajax({
                url: `/WishList/Add?productId=${data.productId}${data.variationId ? `&variationId=${data.variationId}` : ''}`,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(data),
                success: function (response) {
                    if (response.success) {
                        heartIcon.addClass('added').css('color', '#ef4444');
                        let badge = document.querySelector('.WishList-badge');
                        badge.innerText = response.wishProductCount;
                    } else {
                        Swal.fire(
                            'Error!',
                            'Error: ' + "User has not authozrized yet",
                            'error'
                        );
                    }
                }
            });
        } else {
            $.ajax({
                url: '/WishList/Delete',
                type: 'POST',
                data: {
                    productId: productId,
                    variationId: variationId
                },
                success: function (response) {
                    if (response.success) {
                        heartIcon.removeClass('added').css('color', '#6c757d');
                        let badge = document.querySelector('.WishList-badge');
                        badge.innerText = response.wishProductCount;
                        $('#wishlist-item-' + productId).fadeOut(300, function () {
                            $(this).remove();
                        });

                    } else {
                        alert(response.message);
                    }
                }
            });
        }
    });
});
