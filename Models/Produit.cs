// Models/Produit.cs
using System.ComponentModel.DataAnnotations;

namespace SecureApp.Models
{
    public class Produit
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nom du produit")]
        public string Nom { get; set; } = "";

        [Required]
        [Display(Name = "Catégorie")]
        public string Categorie { get; set; } = "";

        [Required]
        [Display(Name = "Prix (€)")]
        public decimal Prix { get; set; }

        [Required]
        [Display(Name = "Quantité en stock")]
        public int Quantite { get; set; }

        [Display(Name = "Seuil d'alerte")]
        public int SeuilAlerte { get; set; } = 5;

        [Display(Name = "Date d'ajout")]
        public DateTime DateAjout { get; set; } = DateTime.UtcNow;
    }
}
