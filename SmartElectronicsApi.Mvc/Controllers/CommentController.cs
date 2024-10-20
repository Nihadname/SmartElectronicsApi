using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.ViewModels;
using SmartElectronicsApi.Mvc.ViewModels.Brand;
using SmartElectronicsApi.Mvc.ViewModels.Comment;
using System.Net.Http.Headers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SmartElectronicsApi.Mvc.Controllers
{
    public class CommentController : Controller
    {
        public async Task<IActionResult> Index(int productId, int pageNumber = 1,
                int pageSize = 10)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            using HttpResponseMessage httpResponseMessage = await client.GetAsync($"http://localhost:5246/api/Comment?productId={ productId}&pageNumber={pageNumber}&pageSize=10");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string ContentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var Data = JsonConvert.DeserializeObject<CommentDetailVM>(ContentStream);
                using HttpResponseMessage httpResponseMessage2 = await client.GetAsync($"http://localhost:5246/api/Comment/getAllImages?productId={productId}");
                string ContentStream2 = await httpResponseMessage2.Content.ReadAsStringAsync();
                var Data2 = JsonConvert.DeserializeObject<List<string>>(ContentStream2);
                ViewBag.Images = Data2;
                return View(Data);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            var jwtToken = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(jwtToken))
            {
                return Json(new { success = false, message = "User not authenticated." });
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await client.DeleteAsync($"http://localhost:5246/api/Comment/{id}");
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Comment successfully deleted." });
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
        public IActionResult Create(int productId)
        {
            var model = new CommentCreateVM
            {
                ProductId = productId
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CommentCreateVM commentCreateVM)
        {
            if (!ModelState.IsValid)
            {
                return View(commentCreateVM); 
            }
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");

            // Retrieve the JWT token
            var jwtToken = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(jwtToken)) 
            {
                 ModelState.AddModelError("","User not authenticated.");
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            // Prepare the form data to send to the API
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(commentCreateVM.Message), "Message");
            formData.Add(new StringContent(commentCreateVM.ProductId.ToString()), "ProductId");
            formData.Add(new StringContent(commentCreateVM.Rating.ToString()), "Rating");
            
           
            if (commentCreateVM.Images != null && commentCreateVM.Images.Count > 0)
            {
                foreach (var file in commentCreateVM.Images)
                {
                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    formData.Add(fileContent, "Images", file.FileName);
                }
            }

            
            var response = await client.PostAsync("http://localhost:5246/api/Comment", formData);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Detail", "Product", new { id = commentCreateVM.ProductId });
            }
            else
            {
                var errorResponseString = await response.Content.ReadAsStringAsync();
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
                return View(commentCreateVM);
            }
        }

    }
}
