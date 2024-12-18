using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.Mvc.Interfaces;
using SmartElectronicsApi.Mvc.ViewModels;
using SmartElectronicsApi.Mvc.ViewModels.Auth;
using SmartElectronicsApi.Mvc.ViewModels.Brand;
using SmartElectronicsApi.Mvc.ViewModels.Category;
using SmartElectronicsApi.Mvc.ViewModels.Color;
using SmartElectronicsApi.Mvc.ViewModels.pagination;
using SmartElectronicsApi.Mvc.ViewModels.Product;
using SmartElectronicsApi.Mvc.ViewModels.SubCategory;
using SmartElectronicsApi.Mvc.ViewModels.Subscriber;
using System;
using System.Drawing.Printing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SmartElectronicsApi.Mvc.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]

    public class ProductController : Controller
    {
        private readonly IEmailService _EmailService;

        public ProductController(IEmailService emailService)
        {
            _EmailService = emailService;
        }

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

            using HttpResponseMessage AuthCheck = await client.GetAsync("http://localhost:5246/api/Auth/CheckAdmin");
            if (AuthCheck.IsSuccessStatusCode)
            {

            }
            else if (AuthCheck.StatusCode == HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            else if (AuthCheck.StatusCode == HttpStatusCode.Forbidden)
            {
                return RedirectToAction("AccessDenied", "Account", new { area = "" });
            }
            else
            {
                return RedirectToAction("Error404", "Home", new { area = "" });
            }

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

          
            var categoryResponse = await client.GetAsync($"http://localhost:5246/api/Category/{productCreateVM.CategoryId}");
            if (!categoryResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError("CategoryId", "Invalid category selected.");
            }

       
            var subcategoryResponse = await client.GetAsync($"http://localhost:5246/api/Subcategory/{productCreateVM.SubcategoryId}");
            if (!subcategoryResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError("SubcategoryId", "Invalid subcategory selected.");
            }

           
            var brandResponse = await client.GetAsync($"http://localhost:5246/api/Brand/{productCreateVM.BrandId}");
            if (!brandResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError("BrandId", "The selected brand is not associated with the chosen subcategory.");
            }

       
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

         
            if (!ModelState.IsValid)
            {
            
                await LoadViewBagData();
                return View(productCreateVM);
            }

         
            using var content = new MultipartFormDataContent();

         
            if (string.IsNullOrEmpty(productCreateVM.Name))
            {
                ModelState.AddModelError("Name", "Product name is required.");
            }
            if (string.IsNullOrEmpty(productCreateVM.Description))
            {
                ModelState.AddModelError("Description", "Product description is required.");
            }

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
                content.Add(new StringContent(productCreateVM.CategoryId.ToString()), "CategoryId");
                content.Add(new StringContent(productCreateVM.BrandId.ToString()), "BrandId");
                content.Add(new StringContent(productCreateVM.SubcategoryId.ToString()), "SubcategoryId");

                
                if (productCreateVM.ColorIds != null)
                {
                    foreach (var colorId in productCreateVM.ColorIds)
                    {
                        content.Add(new StringContent(colorId.ToString()), "ColorIds");
                    }
                }

                
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

                using HttpResponseMessage httpResponseMessage = await client.PostAsync("http://localhost:5246/api/Product", content);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    using HttpResponseMessage GettingSubs = await client.GetAsync("http://localhost:5246/api/Subscriber/GetAll");
                    string ContentStream = await GettingSubs.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<List<SubscriberListItemVM>>(ContentStream);

                    if (data != null && data.Any())
                    {
                        var productResponseString = await httpResponseMessage.Content.ReadAsStringAsync();
                        var newProduct = JsonConvert.DeserializeObject<ProductReturnVM>(productResponseString); // Adjust ProductDetailVM based on your API response

                        string emailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <title>New Product Alert</title>
    <style type='text/css'>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}
        table {{
            width: 100%;
            max-width: 600px;
            margin: 20px auto;
            background: #fff;
            border-radius: 8px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            overflow: hidden;
            border-collapse: collapse;
        }}
        .header {{
            background-color: #FFA73B;
            text-align: center;
            padding: 20px;
            color: #fff;
        }}
        .header h1 {{
            margin: 0;
            font-size: 24px;
        }}
        .content {{
            padding: 20px;
            font-size: 16px;
            color: #333;
            line-height: 1.6;
        }}
        .content p {{
            margin: 10px 0;
        }}
        .button {{
            text-align: center;
            margin: 20px 0;
        }}
        .button a {{
            background: #FFA73B;
            color: #fff;
            text-decoration: none;
            padding: 12px 25px;
            font-size: 16px;
            font-weight: bold;
            border-radius: 5px;
            display: inline-block;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        }}
        .button a:hover {{
            background: #e67e22;
        }}
        .footer {{
            text-align: center;
            font-size: 14px;
            color: #888;
            padding: 15px;
            background: #f9f9f9;
            border-top: 1px solid #ddd;
        }}
        .footer a {{
            color: #FFA73B;
            text-decoration: none;
        }}
    </style>
</head>
<body>
    <table>
        <tr>
            <td class='header'>
                <h1>New Product Alert: {newProduct.Name}</h1>
            </td>
        </tr>
        <tr>
            <td class='content'>
                <p>Hello,</p>
                <p>We are excited to introduce our latest product, <strong>{newProduct.Name}</strong>, just for you!</p>
                <p>
                    <strong>Price:</strong> ${newProduct.Price}<br> 
                    <strong>Discount:</strong> {newProduct.DiscountPercentage}% off<br>
                    <strong>Category:</strong> {newProduct.Category.Name}
                </p>
                <p>Don't miss out on this fantastic addition to our collection!</p>
                <div class='button'>
                    <a href='https://localhost:7170/Product/Detail/{newProduct.Id}'>View Product</a>
                </div>
            </td>
        </tr>
        <tr>
            <td class='footer'>
                <p>Have questions? Reach out to us anytime!<br>
                <a href='mailto:support@example.com'>support@example.com</a></p>
            </td>
        </tr>
    </table>
</body>
</html>";


                        var emails = data.Select(s => s.Email).ToList();
                        await _EmailService.SendEmailAsyncToManyPeople(
                            from: "nihadmi@code.edu.az\r\n",
                            recipients: emails,
                            subject: $"New Product Alert: {newProduct.Name}",
                            body: emailBody,
                            smtpHost: "smtp.gmail.com",
                            smtpPort: 587,
                            enableSsl: true,
                            smtpUser: "nihadmi@code.edu.az\r\n",
                            smtpPass: "wmgd lwju ehhs aoaq\r\n"
                        );
                    }

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
        public async Task<IActionResult> Update(int? id)
        {
            await LoadViewBagData();
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            var jwtToken = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(jwtToken))
            {
                return RedirectToAction("Index");
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            var httpResponseMessage = await client.GetAsync($"http://localhost:5246/api/Product/{id}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string ContentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<ProductReturnVM>(ContentStream);
                return View(new ProductUpdateVm
                {
                    Name = data.Name,
                    Description = data.Description,
                    Price = data.Price,
                    DiscountedPrice = data.DiscountedPrice,
                    DiscountPercentage = data.DiscountPercentage,
                    isNew = data.isNew,
                    ProductCode = data.ProductCode,
                    IsDealOfTheWeek = data.IsDealOfTheWeek,
                    IsFeatured = data.IsFeatured,
                    StockQuantity = data.StockQuantity,
                    ViewCount = data.ViewCount,
                    ColorIds = data.colorListItemDtos.Select(x => x.Id).ToList()
                }
                );
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
        public async Task<IActionResult> Update(int id, ProductUpdateVm productUpdateVm)
        {
            await LoadViewBagData();
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);

            // Validate category
           

            // Validate brand
           

            if (productUpdateVm.ColorIds != null)
            {
                foreach (var colorId in productUpdateVm.ColorIds)
                {
                    var colorResponse = await client.GetAsync($"http://localhost:5246/api/Color/{colorId}");
                    if (!colorResponse.IsSuccessStatusCode)
                    {
                        ModelState.AddModelError("ColorIds", $"Invalid color selected: {colorId}");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                await LoadViewBagData();
                return View(productUpdateVm);
            }

            using var content = new MultipartFormDataContent();

            if (string.IsNullOrEmpty(productUpdateVm.Name))
            {
                ModelState.AddModelError("Name", "Product name is required.");
            }

            if (string.IsNullOrEmpty(productUpdateVm.Description))
            {
                ModelState.AddModelError("Description", "Product description is required.");
            }

            if (ModelState.IsValid)
            {
                content.Add(new StringContent(productUpdateVm.Name), "Name");
                content.Add(new StringContent(productUpdateVm.Description), "Description");
                content.Add(new StringContent(productUpdateVm.Price.ToString()), "Price");
                content.Add(new StringContent(productUpdateVm.DiscountPercentage?.ToString() ?? "0"), "DiscountPercentage");
                content.Add(new StringContent(productUpdateVm.isNew.ToString()), "isNew");
                content.Add(new StringContent(productUpdateVm.IsDealOfTheWeek.ToString()), "IsDealOfTheWeek");
                content.Add(new StringContent(productUpdateVm.IsFeatured.ToString()), "IsFeatured");
                content.Add(new StringContent(productUpdateVm.StockQuantity.ToString()), "StockQuantity");
                content.Add(new StringContent(productUpdateVm.ViewCount.ToString()), "ViewCount");
                

                if (productUpdateVm.ColorIds != null)
                {
                    foreach (var colorId in productUpdateVm.ColorIds)
                    {
                        content.Add(new StringContent(colorId.ToString()), "ColorIds");
                    }
                }

                if (productUpdateVm.Images != null && productUpdateVm.Images.Count > 0)
                {
                    foreach (var image in productUpdateVm.Images)
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
                else
                {
                    // Optional: Add a flag indicating no new images if your API requires it
                    content.Add(new StringContent("false"), "RetainExistingImages");
                }
                using HttpResponseMessage httpResponseMessage = await client.PutAsync($"http://localhost:5246/api/Product/{id}", content);
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

                    await LoadViewBagData();
                    return View(productUpdateVm);
                }
            }

            await LoadViewBagData();
            return View(productUpdateVm);
        }
    }
}
