namespace SmartElectronicsApi.Mvc.ViewModels.pagination
{
    public class PaginatedResponseVM<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
