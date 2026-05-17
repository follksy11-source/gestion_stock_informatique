// Services/UserService.cs
using Microsoft.AspNetCore.Identity;

namespace SecureApp.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<UserService> _logger;

        public UserService(UserManager<IdentityUser> userManager, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<bool> IsAdminAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("Utilisateur {Email} non trouvé", email);
                return false;
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            _logger.LogInformation("Vérification admin pour {Email} : {IsAdmin}", email, isAdmin);
            return isAdmin;
        }

        public async Task<int> GetUserCountAsync()
        {
            var count = _userManager.Users.Count();
            _logger.LogInformation("Nombre d'utilisateurs : {Count}", count);
            return await Task.FromResult(count);
        }
    }
}
