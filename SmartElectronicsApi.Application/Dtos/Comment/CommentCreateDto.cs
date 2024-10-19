using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Comment
{
    public class CommentCreateDto
    {
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public List<IFormFile> Images { get; set; }
    }
}
