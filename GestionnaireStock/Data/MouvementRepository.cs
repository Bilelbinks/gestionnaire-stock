using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using GestionnaireStock.Models;

namespace GestionnaireStock.Data
{
    public class StockInsuffisantException : Exception
    {
        public StockInsuffisantException(string message) : base(message) { }
    }

    public class MouvementRepository
    {
        public List<Mouvement> GetAll()
        {
            var mouvements = new List<Mouvement>();

            using var connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand(
                @"SELECT m.Id, m.ProduitId, p.Nom, m.TypeMouvement, m.Quantite, m.DateMouvement, m.Commentaire
                  FROM Mouvements m
                  JOIN Produits p ON p.Id = m.ProduitId
                  ORDER BY m.DateMouvement DESC",
                connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                mouvements.Add(new Mouvement
                {
                    Id = reader.GetInt32(0),
                    ProduitId = reader.GetInt32(1),
                    ProduitNom = reader.GetString(2),
                    Type = reader.GetString(3) == "Entree" ? TypeMouvement.Entree : TypeMouvement.Sortie,
                    Quantite = reader.GetInt32(4),
                    Date = reader.GetDateTime(5),
                    Commentaire = reader.IsDBNull(6) ? null : reader.GetString(6),
                });
            }

            return mouvements;
        }

        /// <summary>
        /// Enregistre un mouvement et met à jour la quantité du produit dans
        /// la même transaction : soit les deux opérations réussissent, soit
        /// aucune n'est appliquée (pas de mouvement enregistré sans que le
        /// stock ne soit réellement à jour, et inversement).
        /// </summary>
        public void Ajouter(int produitId, TypeMouvement type, int quantite, string? commentaire)
        {
            using var connection = DatabaseHelper.GetConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                using (var commandStock = new SqlCommand(
                    "SELECT Quantite FROM Produits WHERE Id = @Id", connection, transaction))
                {
                    commandStock.Parameters.AddWithValue("@Id", produitId);
                    var quantiteActuelle = (int)commandStock.ExecuteScalar();

                    if (type == TypeMouvement.Sortie && quantiteActuelle < quantite)
                    {
                        throw new StockInsuffisantException(
                            $"Stock insuffisant : {quantiteActuelle} unité(s) disponible(s), {quantite} demandée(s).");
                    }
                }

                var delta = type == TypeMouvement.Entree ? quantite : -quantite;

                using (var commandMaj = new SqlCommand(
                    "UPDATE Produits SET Quantite = Quantite + @Delta WHERE Id = @Id", connection, transaction))
                {
                    commandMaj.Parameters.AddWithValue("@Delta", delta);
                    commandMaj.Parameters.AddWithValue("@Id", produitId);
                    commandMaj.ExecuteNonQuery();
                }

                using (var commandInsert = new SqlCommand(
                    @"INSERT INTO Mouvements (ProduitId, TypeMouvement, Quantite, Commentaire)
                      VALUES (@ProduitId, @Type, @Quantite, @Commentaire)", connection, transaction))
                {
                    commandInsert.Parameters.AddWithValue("@ProduitId", produitId);
                    commandInsert.Parameters.AddWithValue("@Type", type.ToString());
                    commandInsert.Parameters.AddWithValue("@Quantite", quantite);
                    commandInsert.Parameters.AddWithValue("@Commentaire", (object?)commentaire ?? DBNull.Value);
                    commandInsert.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
