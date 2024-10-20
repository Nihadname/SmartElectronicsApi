using Microsoft.AspNetCore.Http;
using SmartElectronicsApi.Application.Dtos.Auth;
using SmartElectronicsApi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Comment
{
    public class CommentListItemDto
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public List<string> Images { get; set; }
        public UserGetDto AppUser {  get; set; } 
    }
}
