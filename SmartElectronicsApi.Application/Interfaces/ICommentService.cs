using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Comment;
using SmartElectronicsApi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface ICommentService 
    {
        Task<CommentCreateDto> Create(CommentCreateDto commentCreateDto);
        Task<int> Delete(int? Id);
        Task<CommentDetailItemDto> Get(int productId, int pageNumber = 1,
                int pageSize = 10);
        Task<List<string>> GetAllIMages(int? productId);
    }
}
