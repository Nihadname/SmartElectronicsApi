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
    if (confirm("Are you sure you want to delete this item?")) {
        $.ajax({
            url: '/Profile/Delete/' + id, 
            type: 'DELETE',
            success: function (response) {
                alert(response.message);
            },
            error: function (error) {
                alert('Error: ' + error.responseText);
            }
        });
    }
}