using Npgsql;

namespace ReManage.Core
{
    public static class DatabaseConnection
    {
        private static string connectionString = "Server=localhost;Port=5432;Database=ReManage;User Id=postgres;Password=35x45x;";

        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(connectionString);
        }
    }
}