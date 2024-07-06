using System.Configuration;
using Npgsql;

namespace ReManage.Core
{
    public static class DatabaseConnection
    {
        private const string ConnectionStringName = "DefaultConnection";

        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings[ConnectionStringName]?.ConnectionString;
        }

        public static void SetConnectionString(string newConnectionString)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var connectionStringsSection = config.ConnectionStrings;

            if (connectionStringsSection.ConnectionStrings[ConnectionStringName] == null)
            {
                connectionStringsSection.ConnectionStrings.Add(new ConnectionStringSettings(ConnectionStringName, newConnectionString, "Npgsql"));
            }
            else
            {
                connectionStringsSection.ConnectionStrings[ConnectionStringName].ConnectionString = newConnectionString;
            }

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("connectionStrings");
        }

        public static bool IsConnectionStringSet()
        {
            return !string.IsNullOrEmpty(GetConnectionString());
        }
    }
}