using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.Mvc.ViewModels;
using SmartElectronicsApi.Mvc.ViewModels.Auth;
using SmartElectronicsApi.Mvc.ViewModels.Brand;
using SmartElectronicsApi.Mvc.ViewModels.Category;
using SmartElectronicsApi.Mvc.ViewModels.Color;
using SmartElectronicsApi.Mvc.ViewModels.pagination;
using SmartElectronicsApi.Mvc.ViewModels.Product;
using SmartElectronicsApi.Mvc.ViewModels.SubCategory;
using System;
using System.Drawing.Printing;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace SmartElectronicsApi.Mvc.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]

    public class ProductController : Controller
    {
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 2, string searchQuery = null,
           int? categoryId = null)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            using HttpResponseMessage httpResponseMessage = await client.GetAsync($"http://localhost:5246/api/Product?pageNumber={pageNumber}&pageSize={pageSize}0&searchQuery={searchQuery}&categoryId={categoryId}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string ContentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<PaginatedResponseVM<ProdutListItemVM>>(ContentStream);
                return View(data);
            }
            else if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            else if (httpResponseMessage.StatusCode == HttpStatusCode.Forbidden)
            {
                return RedirectToAction("AccessDenied", "Account", new { area = "" });
            }
            else
            {
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            var jwtToken = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(jwtToken))
            {
                // Return a JSON response indicating that the user is not authenticated
                return Json(new { success = false, message = "User not authenticated." });
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await client.DeleteAsync($"http://localhost:5246/api/Product/{id}");
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Address successfully deleted." });
            }
            else
            {
                var errorResponseString = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ApiErrorResponse>(errorResponseString);

                if (errorResponse?.Errors != null && errorResponse?.Errors.Count() != 0)
                {
                    return Json(new { success = false, errors = errorResponse.Errors });
                }
                else
                {
                    return Json(new { success = false, message = errorResponse?.Message ?? "An unknown error occurred." });
                }
            }
        }
        public async Task<IActionResult> Create()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);

   
            using HttpResponseMessage categoryResponse = await client.GetAsync("http://localhost:5246/api/Category/GetAllForUserInterface?skip=0&take=35");
            if (categoryResponse.IsSuccessStatusCode)
            {
                string contentStream = await categoryResponse.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<List<CategoryAdminVM>>(contentStream);
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
            }
            using HttpResponseMessage ColorResponse = await client.GetAsync("http://localhost:5246/api/Color/GetAll");
            if (ColorResponse.IsSuccessStatusCode)
            {
                string contentStream = await ColorResponse.Content.ReadAsStringAsync();
                var Colors = JsonConvert.DeserializeObject<List<ColorListItemVM>>(contentStream);
                ViewBag.Colors = new SelectList(Colors, "Id", "Name");
            }
            ViewBag.Subcategories = new SelectList(new List<SubCategoryListItemVM>(), "Id", "Name");
            ViewBag.Brands = new SelectList(new List<BrandAdminReturnVM>(), "Id", "Name");

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateVM productCreateVM)
        {
            await LoadViewBagData();
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);

            // Validate category
            var categoryResponse = await client.GetAsync($"http://localhost:5246/api/Category/{productCreateVM.CategoryId}");
            if (!categoryResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError("CategoryId", "Invalid category selected.");
            }

            // Validate subcategory
            var subcategoryResponse = await client.GetAsync($"http://localhost:5246/api/Subcategory/{productCreateVM.SubcategoryId}");
            if (!subcategoryResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError("SubcategoryId", "Invalid subcategory selected.");
            }

            // Validate brand
            var brandResponse = await client.GetAsync($"http://localhost:5246/api/Brand/{productCreateVM.BrandId}");
            if (!brandResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError("BrandId", "The selected brand is not associated with the chosen subcategory.");
            }

            // Validate colors
            if (productCreateVM.ColorIds != null)
            {
                foreach (var colorId in productCreateVM.ColorIds)
                {
                    var colorResponse = await client.GetAsync($"http://localhost:5246/api/Color/{colorId}");
                    if (!colorResponse.IsSuccessStatusCode)
                    {
                        ModelState.AddModelError("ColorIds", $"Invalid color selected: {colorId}");
                    }
                }
            }

            // Check if ModelState is valid before proceeding
            if (!ModelState.IsValid)
            {
                // Load necessary data again for the view
                await LoadViewBagData();
                return View(productCreateVM);
            }

            // Create MultipartFormDataContent
            using var content = new MultipartFormDataContent();

            // Validate product properties before adding to content
            if (string.IsNullOrEmpty(productCreateVM.Name))
            {
                ModelState.AddModelError("Name", "Product name is required.");
            }
            if (string.IsNullOrEmpty(productCreateVM.Description))
            {
                ModelState.AddModelError("Description", "Product description is required.");
            }

            // Only add properties if they are not null
            if (ModelState.IsValid)
            {
                content.Add(new StringContent(productCreateVM.Name), "Name");
                content.Add(new StringContent(productCreateVM.Description), "Description");
                content.Add(new StringContent(productCreateVM.Price.ToString()), "Price");
                content.Add(new StringContent(productCreateVM.DiscountPercentage?.ToString() ?? "0"), "DiscountPercentage");
                content.Add(new StringContent(productCreateVM.isNew.ToString()), "isNew");
                content.Add(new StringContent(productCreateVM.IsDealOfTheWeek.ToString()), "IsDealOfTheWeek");
                content.Add(new StringContent(productCreateVM.IsFeatured.ToString()), "IsFeatured");
                content.Add(new StringContent(productCreateVM.StockQuantity.ToString()), "StockQuantity");
                content.Add(new StringContent(productCreateVM.ViewCount.ToString()), "ViewCount");
                content.Add(new StringContent(productCreateVM.CreatedTime?.ToString("o") ?? DateTime.UtcNow.ToString("o")), "CreatedTime");
                content.Add(new StringContent(productCreateVM.CategoryId.ToString()), "CategoryId");
                content.Add(new StringContent(productCreateVM.BrandId.ToString()), "BrandId");
                content.Add(new StringContent(productCreateVM.SubcategoryId.ToString()), "SubcategoryId");

                // Add color IDs if any
                if (productCreateVM.ColorIds != null)
                {
                    foreach (var colorId in productCreateVM.ColorIds)
                    {
                        content.Add(new StringContent(colorId.ToString()), "ColorIds");
                    }
                }

                // Add images to the form data
                if (productCreateVM.Images != null && productCreateVM.Images.Count > 0)
                {
                    foreach (var image in productCreateVM.Images)
                    {
                        if (image.Length > 0)
                        {
                            var stream = image.OpenReadStream();
                            var fileContent = new StreamContent(stream);
                            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(image.ContentType);
                            content.Add(fileContent, "Images", image.FileName);
                        }
                    }
                }

                // Send the request to create the product
                using HttpResponseMessage httpResponseMessage = await client.PostAsync("http://localhost:5246/api/Product", content);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    var errorResponseString = await httpResponseMessage.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<ApiErrorResponse>(errorResponseString);
                    if (errorResponse?.Errors != null)
                    {
                        foreach (var error in errorResponse.Errors)
                        {
                            ModelState.AddModelError(error.Key, error.Value);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, errorResponse?.Message ?? "An unknown error occurred.");
                    }
                    // Reload necessary data for the view again
                    await LoadViewBagData();
                    return View(productCreateVM);
                }
            }

            // Reload necessary data for the view if ModelState is not valid
            await LoadViewBagData();
            return View(productCreateVM);
        }


        // Method to load ViewBag data
        private async Task LoadViewBagData()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);

            // Fetch categories
            using HttpResponseMessage categoryResponse = await client.GetAsync("http://localhost:5246/api/Category/GetAllForUserInterface?skip=0&take=35");
            if (categoryResponse.IsSuccessStatusCode)
            {
                string contentStream = await categoryResponse.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<List<CategoryAdminVM>>(contentStream);
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
            }

            // Fetch colors
            using HttpResponseMessage colorResponse = await client.GetAsync("http://localhost:5246/api/Color/GetAll");
            if (colorResponse.IsSuccessStatusCode)
            {
                string colorContentStream = await colorResponse.Content.ReadAsStringAsync();
                var colors = JsonConvert.DeserializeObject<List<ColorListItemVM>>(colorContentStream);
                ViewBag.Colors = new SelectList(colors, "Id", "Name");
            }

            // Initialize subcategories and brands
            ViewBag.Subcategories = new SelectList(new List<SubCategoryListItemVM>(), "Id", "Name");
            ViewBag.Brands = new SelectList(new List<BrandAdminReturnVM>(), "Id", "Name");
        }


    }
}
