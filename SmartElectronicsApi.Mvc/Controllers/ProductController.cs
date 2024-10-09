using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.ViewModels.pagination;
using SmartElectronicsApi.Mvc.ViewModels.Product;
using SmartElectronicsApi.Mvc.ViewModels.Slider;

namespace SmartElectronicsApi.Mvc.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
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
                return View(data);
            }
            return BadRequest();
        }
        public async Task<IActionResult> Search(string sortBy = "Name",
      string sortOrder = "asc", int pageNumber = 1, int pageSize = 10, string searchQuery = "")
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");

            // Construct the query parameters
            var queryParams = $"pageNumber={pageNumber}&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}";

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
