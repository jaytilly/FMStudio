using FMStudio.Lib.Repositories;
using System;

namespace FMStudio.Lib.DatabaseHandlers
{
    public static class DatabaseHandlerFactory
    {
        public static IDatabaseHandler CreateDatabaseHandler(DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.SqlServer2000:
                case DatabaseType.SqlServer2005:
                case DatabaseType.SqlServer2008:
                case DatabaseType.SqlServer2012:
                case DatabaseType.SqlServer2014:
                    return new SqlServerDatabaseHandler();

                default:
                    throw new InvalidOperationException("There is no database handler available for database type '" + databaseType + "'.");
            }
        }
    }
}