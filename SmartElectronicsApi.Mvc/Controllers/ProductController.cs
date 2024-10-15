using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.ViewModels.pagination;
using SmartElectronicsApi.Mvc.ViewModels.Product;
using SmartElectronicsApi.Mvc.ViewModels.Slider;

namespace SmartElectronicsApi.Mvc.Controllers
{
    public class ProductController : Controller
    {
        public async Task<IActionResult> Index(
    string sortBy = "name_asc",
        int pageNumber = 1,
        int pageSize = 10,
        int? categoryId = null,
        int? subCategoryId = null,  // Added subCategoryId
        int? brandId = null,
        int? minPrice = null,
        int? maxPrice = null,
        List<int> colorIds = null
    )
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");

            // Construct the query parameters dynamically based on the filters
            var queryParams = $"pageNumber={pageNumber}&pageSize={pageSize}&sortOrder={sortBy}";

            if (categoryId.HasValue)
            {
                queryParams += $"&categoryId={categoryId}";
            }

            if (subCategoryId.HasValue)  // Added subCategoryId to the query string
            {
                queryParams += $"&subCategoryId={subCategoryId}";
            }

            if (brandId.HasValue)
            {
                queryParams += $"&brandId={brandId}";
            }

            if (minPrice.HasValue)
            {
                queryParams += $"&minPrice={minPrice}";
            }

            if (maxPrice.HasValue)
            {
                queryParams += $"&maxPrice={maxPrice}";
            }

            if (colorIds != null && colorIds.Any())
            {
                queryParams += $"&colorIds={string.Join(",", colorIds)}";
            }

            // Send the request to the API
            using HttpResponseMessage httpResponseMessage = await client.GetAsync($"Product/Filter?{queryParams}");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string contentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<PaginatedResponseVM<ProdutListItemVM>>(contentStream);

                return View(data); // Pass the data to the view
            }

            return BadRequest();
        }

        public async Task<IActionResult> Detail(int? id)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            using HttpResponseMessage httpResponseMessage = await client.GetAsync($"Product/{id}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string ContentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<ProductReturnVM>(ContentStream);
                DateTime startDate = DateTime.Now.AddDays(-3);
                using HttpResponseMessage httpResponseMessage2 = await client.GetAsync($"http://localhost:5246/api/Basket/GetUsersWhoAddedProduct?productId={id}&startDate={startDate}");
                if (httpResponseMessage2.IsSuccessStatusCode)
                {
                    string ContentStream2 = await httpResponseMessage2.Content.ReadAsStringAsync();
                    ViewBag.BaskCount= ContentStream2;
                }

                return View(data);

            }
            return BadRequest();
        }
        public async Task<IActionResult> Search(string sortBy = "Name",
      string sortOrder = "asc", int pageNumber = 1, string searchQuery = "")
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");

            // Construct the query parameters
            var queryParams = $"pageNumber={pageNumber}&pageSize=10&sortBy={sortBy}&sortOrder={sortOrder}";

            // Only add searchQuery if it's not null or empty
            if (!string.IsNullOrEmpty(searchQuery)&&searchQuery!=null)
            {
                queryParams += $"&searchQuery={searchQuery}";
            }

            // Use the constructed query parameters to make the API call
            using HttpResponseMessage httpResponseMessage = await client.GetAsync($"Product/Search?{queryParams}");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string contentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<PaginatedResponseVM<ProdutListItemVM>>(contentStream);
                return View(data);
            }

            return BadRequest();
        }

    }
}
