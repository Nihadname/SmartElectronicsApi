using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Comment
{
    public class CommentDetailItemDto
    {
        public string ProductName { get; set; }
        public int ProductId { get; set; }

        public PaginatedResponse<CommentListItemDto> paginatedResponse {  get; set; }
        public int commentsCount { get; set; }
        public int AverageRating { get; set; }
    }
}
