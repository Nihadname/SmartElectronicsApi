namespace SmartElectronicsApi.Mvc.ViewModels.Comment
{
    public class CommentCreateVM
    {
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}
