namespace SmartElectronicsApi.Mvc.ViewModels.ParametrGroup
{
    public class ParametrGroupListItemVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ParametrValueListItemVM> parametrValues { get; set; }
    }
}
