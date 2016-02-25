using System.Data.SqlClient;
using System.Threading.Tasks;

namespace FMStudio.Lib.DatabaseHandlers
{
    public class SqlServerDatabaseHandler : IDatabaseHandler
    {
        private const string RecreateCommand = @"
ALTER DATABASE {0} SET OFFLINE WITH ROLLBACK IMMEDIATE
ALTER DATABASE {0} SET ONLINE

DROP DATABASE {0}

CREATE DATABASE {0}
";

        public async Task Recreate(string connectionString)
        {
            var connStr = new SqlConnectionStringBuilder(connectionString);

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var sql = string.Format(RecreateCommand, connStr.InitialCatalog);
                var command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}