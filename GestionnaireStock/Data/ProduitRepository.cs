using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using GestionnaireStock.Models;

namespace GestionnaireStock.Data
{
    // Équivalent C# des requêtes préparées PDO : SqlCommand + Parameters.Add
    // au lieu de concaténer les valeurs dans la requête SQL.
    public class ProduitRepository
    {
        public List<Produit> GetAll()
        {
            var produits = new List<Produit>();

            using var connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand(
                "SELECT Id, Reference, Nom, Prix, Quantite, SeuilAlerte FROM Produits ORDER BY Nom",
                connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                produits.Add(new Produit
                {
                    Id = reader.GetInt32(0),
                    Reference = reader.GetString(1),
                    Nom = reader.GetString(2),
                    Prix = reader.GetDecimal(3),
                    Quantite = reader.GetInt32(4),
                    SeuilAlerte = reader.GetInt32(5),
                });
            }

            return produits;
        }

        public void Add(Produit produit)
        {
            using var connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand(
                @"INSERT INTO Produits (Reference, Nom, Prix, Quantite, SeuilAlerte)
                  VALUES (@Reference, @Nom, @Prix, @Quantite, @SeuilAlerte)",
                connection);

            command.Parameters.AddWithValue("@Reference", produit.Reference);
            command.Parameters.AddWithValue("@Nom", produit.Nom);
            command.Parameters.AddWithValue("@Prix", produit.Prix);
            command.Parameters.AddWithValue("@Quantite", produit.Quantite);
            command.Parameters.AddWithValue("@SeuilAlerte", produit.SeuilAlerte);

            command.ExecuteNonQuery();
        }

        public void Update(Produit produit)
        {
            using var connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand(
                @"UPDATE Produits
                  SET Reference = @Reference, Nom = @Nom, Prix = @Prix, SeuilAlerte = @SeuilAlerte
                  WHERE Id = @Id",
                connection);

            command.Parameters.AddWithValue("@Reference", produit.Reference);
            command.Parameters.AddWithValue("@Nom", produit.Nom);
            command.Parameters.AddWithValue("@Prix", produit.Prix);
            command.Parameters.AddWithValue("@SeuilAlerte", produit.SeuilAlerte);
            command.Parameters.AddWithValue("@Id", produit.Id);

            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand("DELETE FROM Produits WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }
    }
}
