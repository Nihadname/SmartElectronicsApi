using System.ComponentModel.DataAnnotations;

namespace SmartElectronicsApi.Mvc.ViewModels
{
    public class ContactVM
    {
        public string Name { get; set; }
        [EmailAddress, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
