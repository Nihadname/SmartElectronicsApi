﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.ViewModels.Brand;
using SmartElectronicsApi.Mvc.ViewModels.Category;
using SmartElectronicsApi.Mvc.ViewModels.Color;
using SmartElectronicsApi.Mvc.ViewModels.pagination;
using SmartElectronicsApi.Mvc.ViewModels.Product;
using SmartElectronicsApi.Mvc.ViewModels.Slider;
using SmartElectronicsApi.Mvc.ViewModels.SubCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SmartElectronicsApi.Mvc.Controllers
{
    public class ProductController : Controller
    {
        public async Task<IActionResult> Index(
    string sortBy = "name_asc",
    int pageNumber = 1,
    int pageSize = 10,
    int? categoryId = null,
    int? brandId = null,
    int? subCategoryId = null,
    int? minPrice = null,
    int? maxPrice = null,
    List<int> colorIds = null
    )
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            using HttpResponseMessage categoryResponse = await client.GetAsync("http://localhost:5246/api/Category/GetAllForUserInterface?skip=0&take=35");
            if (categoryResponse.IsSuccessStatusCode)
            {
                string contentStream = await categoryResponse.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<List<CategoryAdminVM>>(contentStream);
                ViewBag.Categories = categories;
            }
            using HttpResponseMessage BrandResponse = await client.GetAsync("http://localhost:5246/api/Brand/GetForUi?skip=0&take=0");
            if (BrandResponse.IsSuccessStatusCode)
            {
                string contentStream2 = await BrandResponse.Content.ReadAsStringAsync();
                var brands =  JsonConvert.DeserializeObject<List<BrandListItemVM>>(contentStream2);
                ViewBag.Brands=brands;
            }

            using HttpResponseMessage ColorResponse = await client.GetAsync("http://localhost:5246/api/Color/GetAll");
            if (ColorResponse.IsSuccessStatusCode)
            {
                string contentStream3 = await ColorResponse.Content.ReadAsStringAsync();
                var Colors = JsonConvert.DeserializeObject<List<ColorListItemVM>>(contentStream3);
                ViewBag.Colors = Colors;
            }
            using HttpResponseMessage SubCategoryResponse = await client.GetAsync("http://localhost:5246/api/SubCategory/GetAll");
            if (SubCategoryResponse.IsSuccessStatusCode)
            {
                string contentStream4 = await SubCategoryResponse.Content.ReadAsStringAsync();
                var SubCategories = JsonConvert.DeserializeObject<List<SubCategoryListItemVM>>(contentStream4);
                ViewBag.SubCategories = SubCategories;
            }
            // Construct the query parameters dynamically based on the filters
            var queryParams = $"pageNumber={pageNumber}&pageSize={pageSize}&sortOrder={sortBy}";

            if (categoryId.HasValue)
            {
                queryParams += $"&categoryId={categoryId}";
            }

            if (subCategoryId.HasValue)
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

            // Make API call to fetch filtered products
            using HttpResponseMessage httpResponseMessage = await client.GetAsync($"Product/Filter?{queryParams}");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string contentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<PaginatedResponseVM<ProdutListItemVM>>(contentStream);

                return View(data); // Pass data to view
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
                    var categoryId = data.Category.Id;
                    var BrandId = data.BrandId;
                    using HttpResponseMessage httpResponseMessage3 = await client.GetAsync($"http://localhost:5246/api/Product/GetProductsByCategoryIdAndBrandId?categoryId={categoryId}&BrandId={BrandId}&excludeProductId={id}");
                    if (httpResponseMessage3.IsSuccessStatusCode)
                    {
                        string ContentStream3 = await httpResponseMessage3.Content.ReadAsStringAsync();
                        var data3 = JsonConvert.DeserializeObject<List<ProdutListItemVM>>(ContentStream3);
                        ViewBag.Products=data3;
                        Console.WriteLine(ViewBag.Products);
                    }
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
        public async Task<IActionResult> Test()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            var categoryId =10;
            var BrandId = 3;
            using HttpResponseMessage httpResponseMessage3 = await client.GetAsync($"http://localhost:5246/api/Product/GetProductsByCategoryIdAndBrandId?categoryId={categoryId}&BrandId={BrandId}&excludeProductId={21}");
            if (httpResponseMessage3.IsSuccessStatusCode)
            {
                string ContentStream3 = await httpResponseMessage3.Content.ReadAsStringAsync();
                var data3 = JsonConvert.DeserializeObject<List<ProdutListItemVM>>(ContentStream3);
                ViewBag.Products = data3;
           return View(data3);
            }
            return RedirectToAction("Index");
        }
    }
}
