document.addEventListener('DOMContentLoaded', function() {
    const menuItems = document.querySelectorAll('.vertical-menu > li');

    menuItems.forEach(item => {
        item.addEventListener('click', function() {
            const details = this.querySelector('.category-details');

            // Toggle visibility of the details
            if (details.style.display === 'block') {
                details.style.display = 'none';
            } else {
                // Hide all other category details
                document.querySelectorAll('.category-details').forEach(detail => {
                    detail.style.display = 'none';
                });

                details.style.display = 'block';
            }
        });
    });
});
const swiper = new Swiper('.swiperMain', {
    loop: true,

    dots:true,
    // Disable pagination
    pagination: {
        el: '.swiper-pagination', // Unique Pagination Element
        clickable: true,
    },
    // Disable scrollbar
    scrollbar: {
        el: null, // Disable the scrollbar
    },
    // Disable navigation buttons
    navigation: {
        nextEl: null,
        prevEl: null,
    }
});
var swiperSa = new Swiper(".NewBrandProducts", {
    slidesPerView: 3, // Adjust the number of visible slides
    spaceBetween: 30,
    loop: true, // Allows continuous scrolling
    freeMode: true, // Enables free scrolling without snapping to slides
    autoplay: {
        delay: 3000, // Optional: Auto-scroll every 3 seconds
        disableOnInteraction: false,
    },
    breakpoints: {
        // when window width is >= 0px (for mobile devices)
        0: {
            slidesPerView: 1, // Show 1 slide at a time on small screens
            spaceBetween: 20,
        },
        // when window width is >= 768px (for tablets and larger devices)
        768: {
            slidesPerView: 2, // Show 2 slides at a time on medium screens
            spaceBetween: 30,
        },
        // when window width is >= 1024px (for desktops)
        1024: {
            slidesPerView: 3, // Show 3 slides at a time on larger screens
            spaceBetween: 30,
        }
    }
});
var swiperSa2 = new Swiper(".NewBrandProducts2", {
    slidesPerView: 3, // Adjust the number of visible slides
    spaceBetween: 30,
    loop: true, // Allows continuous scrolling
    freeMode: true, // Enables free scrolling without snapping to slides
    autoplay: {
        delay: 3000, // Optional: Auto-scroll every 3 seconds
        disableOnInteraction: false,
    },
    breakpoints: {
        // when window width is >= 0px (for mobile devices)
        0: {
            slidesPerView: 1, // Show 1 slide at a time on small screens
            spaceBetween: 20,
        },
        // when window width is >= 768px (for tablets and larger devices)
        768: {
            slidesPerView: 2, // Show 2 slides at a time on medium screens
            spaceBetween: 30,
        },
        // when window width is >= 1024px (for desktops)
        1024: {
            slidesPerView: 3, // Show 3 slides at a time on larger screens
            spaceBetween: 30,
        }
    }
});

var swiperSa3 = new Swiper(".NewBrandProducts3", {
    slidesPerView: 4, // Adjust the number of visible slides
    spaceBetween: 30,
    freeMode: true, // Enables free scrolling without snapping to slides
   
    breakpoints: {
        // when window width is >= 0px (for mobile devices)
        0: {
            slidesPerView: 1, // Show 1 slide at a time on small screens
            spaceBetween: 20,
        },
        // when window width is >= 768px (for tablets and larger devices)
        768: {
            slidesPerView: 2, // Show 2 slides at a time on medium screens
            spaceBetween: 30,
        },
        // when window width is >= 1024px (for desktops)
        1024: {
            slidesPerView: 3, // Show 3 slides at a time on larger screens
            spaceBetween: 30,
        }
    }
});
var SwiperDeal = new Swiper('.swiperDeal', {
    loop: true,

   // Disable pagination
    pagination: {
        el: null, // Unique Pagination Element
        clickable: false,
    },
    // Disable scrollbar
    scrollbar: {
        el: null, // Disable the scrollbar
    },
    // Disable navigation buttons
    navigation: {
        nextEl: null,
        prevEl: null,
    }
})
var swiperBrand = new Swiper('.swiperBrand', {
    loop: true,             // Enables infinite loop
    autoplay: {
      delay: 2000,          // Adjust delay (2 seconds)
      disableOnInteraction: false,
    },
    slidesPerView: 3,       // Show one image at a time
    spaceBetween: 5,       // Adjust spacing between slides if needed
    centeredSlides: true,   // Center the slides
    height: 400,            // Ensure swiper stays within the container's height
    breakpoints: {          // Optional: Handle responsiveness with breakpoints
      768: {                
        height: 500         
      }
    }
  });
var BarMenuOpen=document.querySelector(".BarMenuOpen");
var thisPart=document.querySelector(".thisPart");
BarMenuOpen.addEventListener("click",function(){
    if (window.innerWidth <= 768) { // Adjust the breakpoint as needed
        if (thisPart.style.display === 'block') {
            thisPart.style.display = 'none';
        } else {
            thisPart.style.display = 'block';
        }
    }})

    const cards = document.querySelectorAll('.SmalCard');

    // Add click event listener to each card
    //cards.forEach(card => {
    //    card.addEventListener('click', () => {
    //        // Remove the active shadow class and reset the color of all cards
    //        cards.forEach(c => {
    //            c.classList.remove('shadow');
    //            // Reset text color and image to default
    //            const img = c.querySelector('img');
    //            const text = c.querySelector('span');
    //            img.style.filter = 'brightness(6.9) invert(1)'; // Assuming the default image is 'default.png'
    //            text.style.color = '#000'; // Reset text to black
    //        });
    
    //        // Add the active shadow class to the clicked card
    //        card.classList.add('shadow');
    
    //        // Change the image to blue and the text color to blue
    //        const clickedImg = card.querySelector('img');
    //        const clickedText = card.querySelector('span');
    //        clickedImg.style.filter = 'brightness(1) ';
    //                    clickedText.style.color = '#007bff'; // Change text to blue
    //    });
    //});
    document.querySelectorAll('.color-circle').forEach(circle => {
        circle.addEventListener('click', function() {
            // Remove 'active' class from all circles
            document.querySelectorAll('.color-circle').forEach(c => c.classList.remove('active'));
    
            // Add 'active' class to the clicked circle
            this.classList.add('active');
        });
    });
function deleteItem(id) {
    var addressElement = document.querySelector(`.AddressText[data-id="${id}"]`);

    // SweetAlert confirmation
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
                url: '/Profile/Delete/' + id,
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
                    addressElement.remove();
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
function getJwtToken() {
    // Retrieve the JWT token from the cookies
    return document.cookie.split('; ').find(row => row.startsWith('JwtToken=')).split('=')[1];
}

function editAddress(id) {
    const jwtToken = getJwtToken();
    console.log(jwtToken)
    if (!jwtToken) {
        Swal.fire(
            'Error!',
            'Error: ' + "user not authenticated",
            'error'
        );
        return;
    } 

    // Make an API call to get the address details by id
    $.ajax({
        url: `/Profile/GetById/${id}`,
        type: 'GET',
        headers: {
            'Authorization': `Bearer ${jwtToken}`
        },
        success: function (data) {
            // Populate the modal fields with the address data
            $('#addressId').val(data.id);
            $('#country').val(data.country);
            $('#state').val(data.state);
            $('#city').val(data.city);
            $('#street').val(data.street);
            $('#zipcode').val(data.zipCode);
            $('#AddressType').val(data.AddressType);
            console.log("Address ID:", $('#addressId').val());

            // Show the modal
            $('#editAddressModal').modal('show');
        },
        error: function () {
            alert('Error retrieving address data.');
        }
    });
}

$('#updateAddressForm').submit(function (e) {
    e.preventDefault();

    const jwtToken = getJwtToken();
    if (!jwtToken) {
        Swal.fire(
            'Error!',
            'Error: ' + "user not authenticated",
            'error'
        );
        return;
    } 
    var Id = $("#addressId").val();
    var addressData = {
        Country: $('#country').val(),
        State: $('#state').val(),
        City: $('#city').val(),
        Street: $('#street').val(),
        ZipCode: $('#zipcode').val(),
            AddressType: $('#AddressType').val()
    };
    $.ajax({
        url: `/Profile/UpdateAddress/${Id}`,  // The endpoint in the controller to update address
        type: 'POST',
        contentType: 'application/json',
        headers: {
            'Authorization': `Bearer ${jwtToken}`
        },
        data: JSON.stringify(addressData),
    
        success: function (response) {
            console.log(JSON.stringify(addressData))
            if (response.success) {
                Swal.fire({
                    title: 'Updated!',
                    text: response.message,
                    icon: 'success',
                    timer: 2000,  // Delay in milliseconds (2000ms = 2 seconds)
                    showConfirmButton: false  // Hide the confirmation button
                }).then(() => {
                    
                    $('#editAddressModal').modal('hide');  
                    location.reload();
                });
            } else {
                displayErrors([response.message]);
            }
        },
        error: function (xhr) {
            if (xhr.status === 400) {
                var response = xhr.responseJSON;
                if (response.errors && response.errors.length > 0) {
                    displayErrors(response.errors);  
                } else {
                    displayErrors([response.message || 'An error occurred.']);
                }
            } else {
                displayErrors(['An unexpected error occurred.']);
            }
        }
    });
});

function displayErrors(errors) {
    var errorDiv = $('#errorMessages');
    errorDiv.html('');  // Clear previous errors

    if (errors && errors.length > 0) {
        errors.forEach(function (error) {
            errorDiv.append('<p>' + error + '</p>');  // Add each error
        });
        errorDiv.removeClass('d-none');  // Show error div
    }
}  

$("#SubForm").on("submit", function (e) {
    e.preventDefault(); // Prevent default form submission

    var formData = {
        Email: $("#email").val()
    };
    console.log(formData);
    $.ajax({
        url: '/Home/SubscriberCreate', 
        type: 'POST',
        data: JSON.stringify(formData),
        contentType: 'application/json',
        success: function (response) {
            console.log(response)
            if (response.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Subscribed successfully!',
                    text: 'You have subscribed successfully.',
                    showConfirmButton: true
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Subscription failed!',
                    text: response.message,
                    showConfirmButton: true
                });
            }
        },
        error: function (xhr) {
            console.log(xhr);
            Swal.fire({
                icon: 'error',
                title: 'An error occurred',
                text: 'Please try again later.',
                showConfirmButton: true
            });
        }
    });
});
function generateProductHTML(product) {
    // Use the first image from imageUrls array, or a default image if not available
    var productImageUrl = product.imageUrls.length > 0 ? product.imageUrls[0] : 'https://via.placeholder.com/320';
    var detailPageUrl = `/Product/Detail/${product.id}`; 
    return `
        <div class="col-12 col-md-6 col-lg-4 mb-3">
            <div class="product-card p-3 shadow-sm">
                <!-- Discount Badge -->
                <div class="discount-badge-circle">
                    Nagd alislarda<br>Endirim<br><strong>${(product.price - product.discountedPrice).toFixed()} AZN</strong>
                </div>

                <!-- Wishlist Icon -->
                 <div class="wishlist-heart" data-id="${product.id}">
                    <i class="fas fa-heart"></i>
                </div>

                <!-- Product Image -->
                <div class="product-image text-center mb-3">
                <a href="${detailPageUrl}" target="_blank"> <img  src="${productImageUrl}" class="card-img-top" alt="Product Image"></a>
                    
                </div>

                <!-- Product Name -->
                <h3 class="product-name text-center mb-1">
                    ${product.name}
                </h3>

                <!-- Reviews and Rating -->
                <div class="review-section text-center mb-2">
                    <span class="rating-stars">
                        <i class="fa fa-star text-warning"></i>
                        <i class="fa fa-star text-warning"></i>
                        <i class="fa fa-star text-warning"></i>
                        <i class="fa fa-star text-warning"></i>
                        <i class="fa fa-star-half-alt text-warning"></i>
                    </span>
                    <span class="review-count">(57 Reviews)</span>
                </div>

                <!-- Price -->
                <div class="price-section text-center mb-2">
                    <span class="price-old text-muted">${product.price.toFixed()} AZN</span><br>
                    <span class="price-new fw-bold">${product.discountedPrice.toFixed()} AZN</span>
                </div>

                <!-- Color Choices Section -->
                <div class="color-choices text-center my-2">
                    ${product.colorListItemDtos.map(color => `<span class="color-circle" style="background-color: ${color.code};"></span>`).join('')}
                </div>

                <!-- Monthly Payment Section -->
               

                <!-- Action Buttons -->
                <div class="action-buttons text-center">
<!-- Add to Basket Button -->
<a class="btn btn-add-to-basket w-100 mb-2">
    <i class="fa fa-shopping-cart"></i> 1 Kliklə Al
</a>
                </div>
            </div>
        </div>

    `;
}


function GetTheMostViewedOnes() {
    $.ajax({
        url: 'http://localhost:5246/api/Product/TheMostViewed',
        method: "GET",
        success: function (response) {
            console.log(response);
            var FilterPartinHome = document.querySelector(".FilterPartinHome");
            FilterPartinHome.innerHTML = ''; // Clear previous content

            // Only one forEach loop is needed
            response.forEach(product => {
                FilterPartinHome.innerHTML += generateProductHTML(product);
            });
        },
        error: function (xhr) {
            console.log(xhr);
        }
    });
}
function GetTheOnesWithDiscount() {
    $.ajax({
        url: 'http://localhost:5246/api/Product/WithDiscount',
        method: "GET",
        success: function (response) {
            console.log(response);
            var FilterPartinHome = document.querySelector(".FilterPartinHome");
            FilterPartinHome.innerHTML = ''; // Clear previous content

            // Only one forEach loop is needed
            response.forEach(product => {
                FilterPartinHome.innerHTML += generateProductHTML(product);
            });
        },
        error: function (xhr) {
            console.log(xhr);
        }
    });

}

//document.getElementById('seeMoreButton').addEventListener('click', function () {
//        window.location.href = '/Product'; // Redirect to the product page
//});
