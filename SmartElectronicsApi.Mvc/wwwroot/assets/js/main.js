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
    cards.forEach(card => {
        card.addEventListener('click', () => {
            // Remove the active shadow class and reset the color of all cards
            cards.forEach(c => {
                c.classList.remove('shadow');
                // Reset text color and image to default
                const img = c.querySelector('img');
                const text = c.querySelector('span');
                img.style.filter = 'brightness(6.9) invert(1)'; // Assuming the default image is 'default.png'
                text.style.color = '#000'; // Reset text to black
            });
    
            // Add the active shadow class to the clicked card
            card.classList.add('shadow');
    
            // Change the image to blue and the text color to blue
            const clickedImg = card.querySelector('img');
            const clickedText = card.querySelector('span');
            clickedImg.style.filter = 'brightness(1) ';
                        clickedText.style.color = '#007bff'; // Change text to blue
        });
    });
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
        alert("User not authenticated.");
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
        alert("User not authenticated.");
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
                $('#editAddressModal').modal('hide');
                location.reload(); // Reload the page to show updated address
            } else {
                console.log(data)
                alert(response.message || 'An error occurred while updating the address.');
            }
        },
        error: function () {
            alert('Error updating address.');
        }
    });
});
