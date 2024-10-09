function GetAllNewOnes() {
    $.ajax({
        url: 'http://localhost:5246/api/Product/GetTheNewOnes',
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

GetAllNewOnes();