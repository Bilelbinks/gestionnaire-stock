using System;

namespace GestionnaireStock.Models
{
    public enum TypeMouvement
    {
        Entree,
        Sortie
    }

    public class Mouvement
    {
        public int Id { get; set; }
        public int ProduitId { get; set; }
        public string ProduitNom { get; set; } = "";
        public TypeMouvement Type { get; set; }
        public int Quantite { get; set; }
        public DateTime Date { get; set; }
        public string? Commentaire { get; set; }
    }
}
