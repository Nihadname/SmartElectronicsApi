namespace SmartElectronicsApi.Api.Interfaces
{
    public interface IAuthService
    {
        public string SignInWithGoogle();
        public string Login();
        public void Register();
    }
}
