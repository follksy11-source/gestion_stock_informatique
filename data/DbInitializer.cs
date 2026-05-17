// Data/DbInitializer.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecureApp.Models;

namespace SecureApp.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Gestionnaire", "Vendeur" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        public static async Task SeedProduitsAsync(AppDbContext context)
        {
            if (await context.Produits.AnyAsync()) return;

            var produits = new List<Produit>
            {
                new Produit { Nom = "Dell XPS 15", Categorie = "Laptops", Prix = 1499.99m, Quantite = 12, SeuilAlerte = 5 },
                new Produit { Nom = "MacBook Pro M3", Categorie = "Laptops", Prix = 2299.99m, Quantite = 8, SeuilAlerte = 5 },
                new Produit { Nom = "HP EliteBook 840", Categorie = "Laptops", Prix = 1199.99m, Quantite = 3, SeuilAlerte = 5 },
                new Produit { Nom = "iPhone 15 Pro", Categorie = "Smartphones", Prix = 1199.99m, Quantite = 25, SeuilAlerte = 10 },
                new Produit { Nom = "Samsung Galaxy S24", Categorie = "Smartphones", Prix = 999.99m, Quantite = 4, SeuilAlerte = 10 },
                new Produit { Nom = "Google Pixel 8", Categorie = "Smartphones", Prix = 799.99m, Quantite = 15, SeuilAlerte = 10 },
                new Produit { Nom = "Dell UltraSharp 27\"", Categorie = "Moniteurs", Prix = 599.99m, Quantite = 2, SeuilAlerte = 3 },
                new Produit { Nom = "LG 4K 32\"", Categorie = "Moniteurs", Prix = 449.99m, Quantite = 7, SeuilAlerte = 3 },
                new Produit { Nom = "Logitech MX Master 3", Categorie = "Accessoires", Prix = 99.99m, Quantite = 30, SeuilAlerte = 10 },
                new Produit { Nom = "Clavier Keychron K2", Categorie = "Accessoires", Prix = 89.99m, Quantite = 1, SeuilAlerte = 5 },
            };

            await context.Produits.AddRangeAsync(produits);
            await context.SaveChangesAsync();
        }
    }
}
