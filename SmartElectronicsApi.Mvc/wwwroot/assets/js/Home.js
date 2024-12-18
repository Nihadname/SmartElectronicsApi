// JavaScript
function GetAllNewOnes() {
    // Show the loader
    var loader = document.querySelector(".loader");
    loader.style.display = "block";

    $.ajax({
        url: 'http://localhost:5246/api/Product/GetTheNewOnes',
        method: "GET",
        success: function (response) {
            console.log(response);
            var FilterPartinHome = document.querySelector(".FilterPartinHome");
            FilterPartinHome.innerHTML = ''; 

            response.forEach(product => {
                FilterPartinHome.innerHTML += generateProductHTML(product);
            });
        },
        error: function (xhr) {
            console.log(xhr);
        },
        complete: function () {
            loader.style.display = "none";
        }
    });
}

GetAllNewOnes();
