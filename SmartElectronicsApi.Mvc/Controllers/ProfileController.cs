using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartElectronicsApi.Mvc.ViewModels;
using SmartElectronicsApi.Mvc.ViewModels.Address;
using SmartElectronicsApi.Mvc.ViewModels.Auth;
using SmartElectronicsApi.Mvc.ViewModels.Category;
using SmartElectronicsApi.Mvc.ViewModels.Color;
using SmartElectronicsApi.Mvc.ViewModels.Order;
using SmartElectronicsApi.Mvc.ViewModels.pagination;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SmartElectronicsApi.Mvc.Controllers
{
    public class ProfileController : Controller
    {
        public async Task<IActionResult> Index()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization =
          new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            using HttpResponseMessage httpResponseMessage = await client.GetAsync("http://localhost:5246/api/Auth/Profile");
            using HttpResponseMessage httpResponseMessage2 = await client.GetAsync("http://localhost:5246/api/Address");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var User = await httpResponseMessage.Content.ReadAsStringAsync();
                var FinalResult = JsonConvert.DeserializeObject<UserGetVm>(User);
                ProfileViewModel profileViewModel = new ProfileViewModel();
                profileViewModel.UserGetVm = FinalResult;
                if (httpResponseMessage2.IsSuccessStatusCode)
                {
                    var addresses=await httpResponseMessage2.Content.ReadAsStringAsync();
                    var FinalOne=JsonConvert.DeserializeObject<List<AddressListItemVM>>(addresses);
                    profileViewModel.AddressList = FinalOne;
                }
                return View(profileViewModel);
            } else if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Account");
            }
            else if (httpResponseMessage.StatusCode == HttpStatusCode.Forbidden)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            else
            {
                return RedirectToAction("Error404", "Home");
            }
        }
        
        public async Task<IActionResult> UpdateImage()
        {
           
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateImage(UserUpdateImageVM userUpdateImageVM)
        {
            if (!ModelState.IsValid)
            {
                return View(userUpdateImageVM);
            }
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            var jwtToken = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(jwtToken))
            {
                ModelState.AddModelError("", "User not authenticated.");
                return View(userUpdateImageVM);
            }
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken);

            var content = new MultipartFormDataContent();

            if (userUpdateImageVM.formFile != null)
            {
                var fileContent = new StreamContent(userUpdateImageVM.formFile.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(userUpdateImageVM.formFile.ContentType);
                content.Add(fileContent, nameof(userUpdateImageVM.formFile), userUpdateImageVM.formFile.FileName);
            }


            var response = await client.PatchAsync("Auth/UpdateImage", content);

            if (response.IsSuccessStatusCode)
            {
                
                TempData["AddingSuccess"] = "ProfileImage deyisdirildi";
                return RedirectToAction("Index", "Profile");
            }
            else
            {
                var errorResponseString = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ApiErrorResponse>(errorResponseString);
                if (errorResponse?.Errors != null && errorResponse?.Errors.Count() != 0)
                {
                    foreach (var error in errorResponse.Errors)
                    {
                        ModelState.AddModelError(error.Key, error.Value);
                        TempData["AddingError"] = (error.Key, error.Value);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorResponse?.Message ?? "An unknown error occurred.");
                }
                return View(userUpdateImageVM);
            }
        }
        public async Task<IActionResult> UpdateInformation()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateInformation(UpdateUserInformationVM updateUserInformationVM)
        {
           
          
            var handler = new HttpClientHandler();
            using var client = new HttpClient(handler);
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            var jwtToken = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(jwtToken))
            {
                ModelState.AddModelError("", "User not authenticated.");
                return View(updateUserInformationVM);
            }
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken);
            var stringData = JsonConvert.SerializeObject(updateUserInformationVM);
            var content = new StringContent(stringData, Encoding.UTF8, "application/json");
            var response = await client.PutAsync("Auth/UpdateUserInformation", content);
            if (response.IsSuccessStatusCode)
            {

                TempData["AddingSuccessInformation"] = "ProfileImage deyisdirildi";
                return RedirectToAction("Index", "Profile");
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
                        TempData["AddingError"] = (error.Key, error.Value);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorResponse?.Message ?? "An unknown error occurred.");
                }
                return View(updateUserInformationVM);
            }
        }
        public  IActionResult CreateAddress()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateAddress(AddressCreateVm addressCreateVm)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            var jwtToken = Request.Cookies["JwtToken"];

            if (string.IsNullOrEmpty(jwtToken))
            {
                ModelState.AddModelError("", "User not authenticated.");
                return View(addressCreateVm);
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var stringData = JsonConvert.SerializeObject(addressCreateVm);
            var content = new StringContent(stringData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:5246/api/Address", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Profile");
            }
            else
            {
                var errorResponseString = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ApiErrorResponse>(errorResponseString);

                if (errorResponse?.Errors != null&&errorResponse?.Errors.Count()!=0)
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

                return View(addressCreateVm);
            }
        }
        public async Task<IActionResult> Delete(int? id)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");

            // Retrieve the JWT token from cookies
            var jwtToken = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(jwtToken))
            {
                // Return a JSON response indicating that the user is not authenticated
                return Json(new { success = false, message = "User not authenticated." });
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await client.DeleteAsync($"http://localhost:5246/api/Address/{id}");

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
       public async Task<IActionResult> GetById(int? id)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            var jwtToken = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(jwtToken))
            {
                return Json(new { success = false, message = "User not authenticated." });
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await client.GetAsync($"Address/{id}");

            if (response.IsSuccessStatusCode)
            {
                var address = await response.Content.ReadAsStringAsync();
                var addressObj = JsonConvert.DeserializeObject<AddressListItemVM>(address);
                return Json(addressObj);
            }
            else
            {
                return Json(new { success = false, message = "Failed to retrieve address." });
            }

        }
        [HttpPost]
        public async Task<IActionResult> UpdateAddress(int? id,[FromBody]AddressUpdateVm addressUpdateVm)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            var jwtToken = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(jwtToken))
            {
                return Json(new { success = false, message = "User not authenticated." });
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var content = new StringContent(JsonConvert.SerializeObject(addressUpdateVm), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"Address/{id}", content);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Address updated successfully." });
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, message = errorResponse ?? "Failed to update address." });
            }
        }
        public async Task<IActionResult> Orders(int pageNumber = 1, int pageSize = 2)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization =
          new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            using HttpResponseMessage httpResponseMessage = await client.GetAsync($"http://localhost:5246/api/Order/GetAllForUser?pageNumber={pageNumber}&pageSize={pageSize}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string ContentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<PaginatedResponseVM<OrderListItemVm>>(ContentStream);
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

        public async Task<IActionResult> GetOrderById(int orderId)
        {
            if(orderId ==0) return RedirectToAction("Index", "Home", new { area = "" }); 
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            return Json(new { success = true, message = "Order not found." });
        }
    }
}
