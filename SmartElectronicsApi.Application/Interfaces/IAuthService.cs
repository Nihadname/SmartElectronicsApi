namespace SmartElectronicsApi.Application.Interfaces
{
    public interface IAuthService
    {
        public string SignInWithGoogle();
        public string Login();
        public void Register();
    }
}
