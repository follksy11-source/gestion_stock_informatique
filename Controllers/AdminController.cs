// Controllers/AdminController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace SecureApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<AdminController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // Liste des utilisateurs
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userRoles = new Dictionary<string, IList<string>>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles;
            }

            ViewBag.UserRoles = userRoles;
            _logger.LogInformation("Page Admin consultée par {User}", User.Identity!.Name);
            return View(users);
        }

        // Promouvoir un utilisateur
        [HttpPost]
        public async Task<IActionResult> PromouvoirRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (!await _userManager.IsInRoleAsync(user, role))
            {
                await _userManager.AddToRoleAsync(user, role);
                _logger.LogWarning("Utilisateur {Email} promu au rôle {Role} par {Admin}", user.Email, role, User.Identity!.Name);
            }

            return RedirectToAction(nameof(Index));
        }

        // Rétrograder un utilisateur
        [HttpPost]
        public async Task<IActionResult> RetirerRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (await _userManager.IsInRoleAsync(user, role))
            {
                await _userManager.RemoveFromRoleAsync(user, role);
                _logger.LogWarning("Rôle {Role} retiré de {Email} par {Admin}", role, user.Email, User.Identity!.Name);
            }

            return RedirectToAction(nameof(Index));
        }

        // Supprimer un utilisateur
        [HttpPost]
        public async Task<IActionResult> Supprimer(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Empêcher l'admin de se supprimer lui-même
            if (user.Email == User.Identity!.Name)
            {
                TempData["Erreur"] = "Vous ne pouvez pas supprimer votre propre compte.";
                return RedirectToAction(nameof(Index));
            }

            await _userManager.DeleteAsync(user);
            _logger.LogWarning("Utilisateur {Email} supprimé par {Admin}", user.Email, User.Identity!.Name);
            return RedirectToAction(nameof(Index));
        }
    }
}
