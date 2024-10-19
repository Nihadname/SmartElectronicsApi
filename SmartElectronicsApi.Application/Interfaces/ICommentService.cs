using SmartElectronicsApi.Application.Dtos.Comment;
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
    }
}
