namespace SmartElectronicsApi.Mvc.ViewModels
{
    public class ApiErrorResponse
    {
        public string Message { get; set; }
        public Dictionary<string, string> Errors { get; set; }
    }
}
