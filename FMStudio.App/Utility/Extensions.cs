using System;

namespace FMStudio.App.Utility
{
    public static class Extensions
    {
        public static Configuration.DatabaseType ToConfiguration(this Lib.DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case Lib.DatabaseType.Sqlite:
                    return Configuration.DatabaseType.SQLite;

                case Lib.DatabaseType.SqlServer2000:
                    return Configuration.DatabaseType.SqlServer2000;

                case Lib.DatabaseType.SqlServer2005:
                    return Configuration.DatabaseType.SqlServer2005;

                case Lib.DatabaseType.SqlServer2008:
                    return Configuration.DatabaseType.SqlServer2008;

                case Lib.DatabaseType.SqlServer2012:
                    return Configuration.DatabaseType.SqlServer2012;

                case Lib.DatabaseType.SqlServer2014:
                    return Configuration.DatabaseType.SqlServer2014;

                default:
                    throw new InvalidOperationException("Unknown database type " + databaseType.ToString());
            }
        }

        public static Lib.DatabaseType? ToLib(this Configuration.DatabaseType? configDatabaseType)
        {
            if (configDatabaseType.HasValue)
                configDatabaseType.Value.ToLib();

            return null;
        }

        public static Lib.DatabaseType ToLib(this Configuration.DatabaseType configDatabaseType)
        {
            switch (configDatabaseType)
            {
                case Configuration.DatabaseType.SqlServer2000:
                    return Lib.DatabaseType.SqlServer2000;

                case Configuration.DatabaseType.SqlServer2005:
                    return Lib.DatabaseType.SqlServer2005;

                case Configuration.DatabaseType.SqlServer2008:
                    return Lib.DatabaseType.SqlServer2008;

                case Configuration.DatabaseType.SqlServer2012:
                    return Lib.DatabaseType.SqlServer2012;

                case Configuration.DatabaseType.SqlServer2014:
                    return Lib.DatabaseType.SqlServer2014;

                case Configuration.DatabaseType.SQLite:
                    return Lib.DatabaseType.Sqlite;

                default:
                    throw new InvalidOperationException("Unknown database type " + configDatabaseType.ToString());
            }
        }
    }
}