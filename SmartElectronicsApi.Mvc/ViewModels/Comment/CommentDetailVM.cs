using SmartElectronicsApi.Mvc.ViewModels.pagination;

namespace SmartElectronicsApi.Mvc.ViewModels.Comment
{
    public class CommentDetailVM
    {
        public string? ProductName { get; set; }
        public int? ProductId { get; set; }
        public PaginatedResponseVM<CommentListItemVM>? paginatedResponse { get; set; }
        public int? commentsCount { get; set; }
        public int? AverageRating { get; set; }
    }
}
