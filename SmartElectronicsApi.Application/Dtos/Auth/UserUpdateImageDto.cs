using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Auth
{
    public class UserUpdateImageDto
    {
        public IFormFile formFile {  get; set; }
    }
}
