using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.ViewModels;
using SmartElectronicsApi.Mvc.ViewModels.Brand;
using SmartElectronicsApi.Mvc.ViewModels.Product;
using SmartElectronicsApi.Mvc.ViewModels.Slider;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Mvc.Controllers
{
    public class BrandController : Controller
    {
        public async Task<IActionResult> Index(int? id)
        {
            BrandDetailVM brandDetailVM = new BrandDetailVM();
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");

            // Fetch Brand Details
            using HttpResponseMessage httpResponseMessage = await client.GetAsync($"http://localhost:5246/api/Brand/Ui/{id}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string brandContentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var brandData = JsonConvert.DeserializeObject<BrandListItemVM>(brandContentStream);
                brandDetailVM.BrandListItem = brandData;
            }

            // Fetch All Products for the Brand
            using HttpResponseMessage httpResponseMessage2 = await client.GetAsync($"http://localhost:5246/api/Product/GetAllProductsWithBrandId/{id}");
            if (httpResponseMessage2.IsSuccessStatusCode)
            {
                string productContentStream = await httpResponseMessage2.Content.ReadAsStringAsync();
                var productData = JsonConvert.DeserializeObject<List<ProdutListItemVM>>(productContentStream);
                brandDetailVM.produtListItemVMs = productData;
            }

            // Fetch Deal of the Week Products for the Brand
            using HttpResponseMessage httpResponseMessage3 = await client.GetAsync($"http://localhost:5246/api/Product/GetDealOfTheWeekInBrand/{id}");
            if (httpResponseMessage3.IsSuccessStatusCode)
            {
                string dealContentStream = await httpResponseMessage3.Content.ReadAsStringAsync();
                var dealData = JsonConvert.DeserializeObject<List<ProdutListItemVM>>(dealContentStream);
                brandDetailVM.produtListItemVMsWithDealOfTheWeek = dealData;
            }

            return View(brandDetailVM);
        }
    }
}
