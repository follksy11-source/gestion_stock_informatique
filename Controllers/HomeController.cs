using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureApp.Data;
using Microsoft.EntityFrameworkCore;
using SecureApp.Services;
namespace SecureApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger,IUserService userService, AppDbContext context)
        {
            _logger = logger;
            _userService = userService;
            _context = context;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Page d'accueil visitée");
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            _logger.LogInformation("Page secrète visitée par {User}", User.Identity!.Name);
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminOnly()
        {
            _logger.LogWarning("Page Admin visitée par {User}", User.Identity!.Name);
            return View();
        }

        [Authorize(Policy = "AdminOnly")]
        public IActionResult PolicyPage()
        {
            _logger.LogInformation("Page Policy visitée par {User}", User.Identity!.Name);
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult TestError()
        {
            _logger.LogError("Test d'erreur déclenché");
            throw new Exception("Ceci est une erreur de test");
        }
        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var produits = await _context.Produits.ToListAsync();

            ViewBag.TotalProduits = produits.Count;
            ViewBag.ValeurStock = produits.Sum(p => p.Prix * p.Quantite);
            ViewBag.AlertesRupture = produits.Count(p => p.Quantite <= p.SeuilAlerte);
            ViewBag.TotalCategories = produits.Select(p => p.Categorie).Distinct().Count();
            ViewBag.ProduitsParCategorie = produits
                .GroupBy(p => p.Categorie)
                .Select(g => new { Categorie = g.Key, Count = g.Count(), Valeur = g.Sum(p => p.Prix * p.Quantite) })
                .ToList();
            ViewBag.ProduitsEnAlerte = produits.Where(p => p.Quantite <= p.SeuilAlerte).ToList();

            _logger.LogInformation("Dashboard consulté par {User}", User.Identity!.Name);
            return View();
        }
    }
}
