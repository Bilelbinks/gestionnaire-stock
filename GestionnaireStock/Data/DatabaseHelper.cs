using Microsoft.Data.SqlClient;

namespace GestionnaireStock.Data
{
    public static class DatabaseHelper
    {
        // "Trusted_Connection=True" = authentification Windows intégrée,
        // pas de mot de passe stocké dans le code (équivalent SQL Server
        // de se connecter avec le compte Windows courant).
        private const string ConnectionString =
            @"Server=localhost\MSSQLSERVER2022;Database=GestionnaireStockDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }
    }
}
