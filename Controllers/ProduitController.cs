// Controllers/ProduitController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureApp.Data;
using SecureApp.Models;

namespace SecureApp.Controllers
{
    [Authorize]
    public class ProduitController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProduitController> _logger;

        public ProduitController(AppDbContext context, ILogger<ProduitController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Liste des produits
        public async Task<IActionResult> Index()
        {
            var produits = await _context.Produits.ToListAsync();
            _logger.LogInformation("Liste des produits consultée par {User}", User.Identity!.Name);
            return View(produits);
        }

        // Formulaire d'ajout
        [Authorize(Roles = "Admin,Gestionnaire")]
        [HttpGet]
        public IActionResult Create() => View();

        // Ajout d'un produit
        [Authorize(Roles = "Admin,Gestionnaire")]
        [HttpPost]
        public async Task<IActionResult> Create(Produit produit)
        {
            if (!ModelState.IsValid) return View(produit);

            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Produit {Nom} ajouté par {User}", produit.Nom, User.Identity!.Name);
            return RedirectToAction(nameof(Index));
        }

        // Formulaire de modification
        [Authorize(Roles = "Admin,Gestionnaire")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var produit = await _context.Produits.FindAsync(id);
            if (produit == null) return NotFound();
            return View(produit);
        }

        // Modification d'un produit
        [Authorize(Roles = "Admin,Gestionnaire")]
        [HttpPost]
        public async Task<IActionResult> Edit(Produit produit)
        {
            if (!ModelState.IsValid) return View(produit);

            _context.Produits.Update(produit);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Produit {Nom} modifié par {User}", produit.Nom, User.Identity!.Name);
            return RedirectToAction(nameof(Index));
        }

        // Suppression d'un produit
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var produit = await _context.Produits.FindAsync(id);
            if (produit == null) return NotFound();

            _context.Produits.Remove(produit);
            await _context.SaveChangesAsync();
            _logger.LogWarning("Produit {Nom} supprimé par {User}", produit.Nom, User.Identity!.Name);
            return RedirectToAction(nameof(Index));
        }
    }
}
