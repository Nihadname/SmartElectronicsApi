namespace SmartElectronicsApi.Mvc.ViewModels.ParametrGroup
{
    public class ParametrGroupCreateVM
    {
        public string Name { get; set; }
        public int ProductId { get; set; }
        public List<ParametrValueListItemVM> parametrValues { get; set; }
    }
}
