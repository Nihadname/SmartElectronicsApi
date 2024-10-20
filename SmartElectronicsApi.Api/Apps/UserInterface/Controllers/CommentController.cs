using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Dtos.Comment;
using SmartElectronicsApi.Application.Interfaces;

namespace SmartElectronicsApi.Api.Apps.UserInterface.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> Create(CommentCreateDto commentCreateDto)
        {
            return Ok(await _commentService.Create(commentCreateDto));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            return Ok(await _commentService.Delete(id));    
        }
        [HttpGet]
        public async Task<IActionResult> Get(int productId, int pageNumber = 1,
           int pageSize = 10)
        {
            return Ok(await _commentService.Get(productId,pageNumber,pageSize));
        }
        [HttpGet("getAllImages")]
        public async Task<IActionResult> GetAllImages(int?  productId)
        {
            return Ok(await _commentService.GetAllIMages(productId));
        }
    }
}
