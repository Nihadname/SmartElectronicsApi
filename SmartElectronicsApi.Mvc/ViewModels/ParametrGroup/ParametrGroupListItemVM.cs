namespace SmartElectronicsApi.Mvc.ViewModels.ParametrGroup
{
    public class ParametrGroupListItemVM
    {
        public string Name { get; set; }
        public List<ParametrValueListItemVM> parametrValues { get; set; }
    }
}
