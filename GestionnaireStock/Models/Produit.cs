namespace GestionnaireStock.Models
{
    public class Produit
    {
        public int Id { get; set; }
        public string Reference { get; set; } = "";
        public string Nom { get; set; } = "";
        public decimal Prix { get; set; }
        public int Quantite { get; set; }
        public int SeuilAlerte { get; set; }

        public bool EnAlerte => Quantite <= SeuilAlerte;
    }
}
