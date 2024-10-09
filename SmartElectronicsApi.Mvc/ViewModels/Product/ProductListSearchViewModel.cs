namespace SmartElectronicsApi.Mvc.ViewModels.Product
{
    public class ProductListSearchViewModel
    {
        public List<ProdutListItemVM> Products { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchQuery { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
    }
}
