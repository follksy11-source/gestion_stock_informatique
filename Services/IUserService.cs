// Services/IUserService.cs
namespace SecureApp.Services
{
    public interface IUserService
    {
        Task<bool> IsAdminAsync(string email);
        Task<int> GetUserCountAsync();
    }
}
