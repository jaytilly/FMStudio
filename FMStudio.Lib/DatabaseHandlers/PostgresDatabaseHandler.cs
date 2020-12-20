using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMStudio.Lib.DatabaseHandlers
{
    public class PostgresDatabaseHandler : IDatabaseHandler
    {
        private const string RecreateCommand = @"
            ALTER DATABASE {0} SET OFFLINE WITH ROLLBACK IMMEDIATE
            ALTER DATABASE {0} SET ONLINE

            DROP DATABASE {0}

            CREATE DATABASE {0}
            ";

        public async Task Recreate(string connectionString)
        {
            var connStr = new NpgsqlConnectionStringBuilder(connectionString);
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var sql = string.Format(RecreateCommand, connStr.Database);
                
                var command = new NpgsqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
