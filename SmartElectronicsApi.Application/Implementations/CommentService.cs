using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Category;
using SmartElectronicsApi.Application.Dtos.Comment;
using SmartElectronicsApi.Application.Dtos.Product;
using SmartElectronicsApi.Application.Dtos.ProductVariation;
using SmartElectronicsApi.Application.Dtos.Setting;
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

           
            await _unitOfWork.CommentRepository.Create(comment);
             _unitOfWork.Commit();
            if (commentCreateDto.Message.Any()&& commentCreateDto.Images != null)
            {
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
            }
            _unitOfWork.Commit();
            return commentCreateDto;
        }
        public async Task<int> Delete(int? Id)
        {
            if (Id is null) throw new CustomException(400, "Id", "id can't be null");

                // Get the current user ID
                var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    throw new CustomException(400, "User", "User ID cannot be null");
                }

                var comment = await _unitOfWork.CommentRepository.GetEntity(s => s.Id == Id && s.IsDeleted == false);
                if (comment is null) throw new CustomException(404, "NotFound", "Comment not found");

                if (comment.AppUserId != userId)
                {
                    throw new CustomException(403, "Unauthorized", "You are not allowed to delete this comment");
                }

                var commentImages = await _unitOfWork.CommentImageRepository.GetAll(s => s.CommentId == comment.Id);
                if (commentImages.Any())
                {
                    foreach (var image in commentImages)
                    {
                        await _unitOfWork.CommentImageRepository.Delete(image);  // Delete each image
                    }
                }

                await _unitOfWork.CommentRepository.Delete(comment);

                _unitOfWork.Commit();

                return comment.Id;
            
          
        }

        public async Task<CommentDetailItemDto> Get(int productId, int pageNumber = 1, int pageSize = 10)
        {
            var comments = await _unitOfWork.CommentRepository.GetAll(
                s => s.IsDeleted == false && s.ProductId == productId,
                (pageNumber - 1) * pageSize,
                pageSize,
                includes: new Func<IQueryable<Comment>, IQueryable<Comment>>[]
                {
                query => query.Include(c => c.commentImages)
                              .Include(s => s.Product)
                              .Include(s => s.AppUser)
                });

            var commentsAll = await _unitOfWork.CommentRepository.GetAll(s => s.ProductId == productId && s.IsDeleted == false);
            var commentsCount = commentsAll.Count();

            // Handle case when there are no comments (set average rating to 0 or handle appropriately)
            var averageRating = commentsAll.Any() ? commentsAll.Average(s => s.Rating) : 0;

            var paginatedResult = new PaginatedResponse<CommentListItemDto>
            {
                Data = mapper.Map<List<CommentListItemDto>>(comments),
                TotalRecords = commentsCount,  // Only count non-deleted comments
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var product = await _unitOfWork.productRepository.GetEntity(s => s.Id == productId);
            if (product == null)
            {
                throw new CustomException(404, "ProductNotFound", "Product not found");
            }

            return new CommentDetailItemDto
            {
                ProductName = product.Name?.Trim() ?? "Unknown Product",
                ProductId = productId,
                paginatedResponse = paginatedResult,
                commentsCount = commentsCount,
                AverageRating = (int)averageRating
            };
        }

        public async Task<List<string>> GetAllIMages(int? productId)
        {
            if (productId == null) throw new CustomException(400, "ProductId", "Product ID cannot be null");

            var uriBuilder = new UriBuilder(_contextAccessor.HttpContext.Request.Scheme,
                _contextAccessor.HttpContext.Request.Host.Host,
                _contextAccessor.HttpContext.Request.Host.Port ?? 80);

            var url = uriBuilder.Uri.AbsoluteUri;
            List<string> images = new List<string>();

            var allImages = await _unitOfWork.CommentImageRepository.GetAll(
                s => s.IsDeleted == false && s.Comment.ProductId == productId,
                includes: new Func<IQueryable<CommentImage>, IQueryable<CommentImage>>[]
                {
                query => query.Include(c => c.Comment)
                });

            foreach (var image in allImages)
            {
                images.Add($"{url}/img/{image.Name}");
            }

            return images;
        }

    }
}
