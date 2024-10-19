using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SmartElectronicsApi.Application.Dtos.Comment;
using SmartElectronicsApi.Application.Dtos.Product;
using SmartElectronicsApi.Application.Dtos.ProductVariation;
using SmartElectronicsApi.Application.Exceptions;
using SmartElectronicsApi.Application.Extensions;
using SmartElectronicsApi.Application.Interfaces;
using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.DataAccess.Data.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Implementations
{
    public class CommentService:ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper mapper;
        private UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        public CommentService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            this.mapper = mapper;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
        }
        public async Task<CommentCreateDto> Create(CommentCreateDto commentCreateDto)
        {
            var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new CustomException(400, "Id", "User ID cannot be null");
            }

            // You can still set it here
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) throw new CustomException(403, "this user doesn't exist");

            var comment = new Comment
            {
                AppUserId = userId,
                Message = commentCreateDto.Message,
                ProductId = commentCreateDto.ProductId,
                Rating = commentCreateDto.Rating,
                CreatedAt = DateTime.Now,
                commentImages = new List<CommentImage>()
            };

            foreach (var item in commentCreateDto.Images)
            {
                var imagePath = item.Save(Directory.GetCurrentDirectory(), "img");
                var commentImage = new CommentImage
                {
                    Name = Path.GetFileName(imagePath),
                    CommentId = comment.Id,
                };

                comment.commentImages.Add(commentImage);
            }

            await _unitOfWork.CommentRepository.Create(comment);
             _unitOfWork.Commit();

            return commentCreateDto;
        }
        public async Task<int> Delete(int? Id)
        {
            if (Id is null) throw new CustomException(400, "Id", "id cant be null");
            var comment = await _unitOfWork.CommentRepository.GetEntity(s => s.Id == Id && s.IsDeleted == false);
            if (comment is null) throw new CustomException(404, "Not found");
            await _unitOfWork.CommentRepository.Delete(comment);
            _unitOfWork.Commit();
            return comment.Id;
        }

    }
}
