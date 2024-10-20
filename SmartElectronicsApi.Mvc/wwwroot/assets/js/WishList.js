function getJwtTokenFromCookieWish() {
    const name = "JwtToken=";
    const decodedCookie = decodeURIComponent(document.cookie);
    const cookies = decodedCookie.split(';');
    for (let i = 0; i < cookies.length; i++) {
        let cookie = cookies[i].trim();
        if (cookie.indexOf(name) === 0) {
            return cookie.substring(name.length, cookie.length);
        }
    }
    return "";
}

$(document).ready(function () {
    const token = getJwtTokenFromCookieWish();

    if (!token) {
        console.error('JWT token not found. User might not be logged in.');
        return;
    }

    // Fetch wishlist product IDs for the current user
    $.ajax({
        url: 'http://localhost:5246/api/WishList/GetUserWishListProducts', // Adjust based on your API endpoint
        type: 'GET',
        headers: {
            'Authorization': `Bearer ${token}`,
        },
        success: function (response) {
            if (!response || !response.productIds) {
                console.warn('No products in the wishlist.');
                return;
            }

            var wishListProductIds = response.productIds;

            // Loop through all wishlist-heart elements and highlight if they are in the wishlist
            $('.wishlist-heart').each(function () {
                var productId = $(this).data('id');
                if (wishListProductIds.includes(productId)) {
                    $(this).find('i').addClass('added').css('color', '#ef4444');
                }
            });
        },
        error: function () {
            console.error('Failed to fetch wishlist products.');
        }
    });

    // Handle heart icon click using event delegation (better performance for dynamically generated elements)
    $(document).on('click', '.wishlist-heart', function () {
        var heartIcon = $(this).find('i');
        var productId = $(this).data('id');
        var variationId = null;

        var isAdded = heartIcon.hasClass('added');
        const token = getJwtTokenFromCookieWish();

        if (!token) {
            Swal.fire('Unauthorized!', 'You need to be logged in to manage your wishlist.', 'error');
            return;
        }

        if (!isAdded) {
            var data = { productId: productId };

            if (variationId) {
                data.variationId = variationId;
            }

            $.ajax({
                url: `/WishList/Add?productId=${data.productId}${data.variationId ? `&variationId=${data.variationId}` : ''}`,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                headers: {
                    'Authorization': `Bearer ${token}`,
                },
                data: JSON.stringify(data),
                success: function (response) {
                    if (response.success) {
                        heartIcon.addClass('added').css('color', '#ef4444');
                        let badge = document.querySelector('.WishList-badge');
                        badge.innerText = response.wishProductCount;
                    } else {
                        Swal.fire('Error!', 'Unable to add to wishlist.', 'error');
                    }
                },
                error: function () {
                    Swal.fire('Error!', 'Something went wrong while adding to wishlist.', 'error');
                }
            });
        } else {
            $.ajax({
                url: '/WishList/Delete',
                type: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                },
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
                        Swal.fire('Error!', 'Unable to remove from wishlist.', 'error');
                    }
                },
                error: function () {
                    Swal.fire('Error!', 'Something went wrong while removing from wishlist.', 'error');
                }
            });
        }
    });
});
