using SmartElectronicsApi.DataAccess.Migrations;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface IAuthService
    {
        public string SignInWithGoogle();
        public string Login();
        public void Register();
        Task<AppUser> FindOrCreateUserAsync(string email, string userName, string googleId);

    }
}
