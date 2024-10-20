using SmartElectronicsApi.Mvc.ViewModels.Auth;

namespace SmartElectronicsApi.Mvc.ViewModels.Comment
{
    public class CommentListItemVM
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public List<string> Images { get; set; }
        public UserGetVm AppUser { get; set; }
    }
}
