using FMStudio.Configuration;
using System.Collections.Generic;

namespace FMStudio.App.ViewModels
{
    public class DatabaseTypeViewModel
    {
        public DatabaseType Value { get; set; }

        public string Name { get; set; }

        public DatabaseTypeViewModel(DatabaseType value, string name)
        {
            Value = value;
            Name = name;
        }

        public static List<DatabaseTypeViewModel> GetDatabaseTypes()
        {
            return new List<DatabaseTypeViewModel>()
            {
                new DatabaseTypeViewModel(DatabaseType.Postgres, "Postgres"),
                new DatabaseTypeViewModel(DatabaseType.SQLite, "SQLite"),
                new DatabaseTypeViewModel(DatabaseType.SqlServer2000, "SQL Server 2000"),
                new DatabaseTypeViewModel(DatabaseType.SqlServer2005, "SQL Server 2005"),
                new DatabaseTypeViewModel(DatabaseType.SqlServer2008, "SQL Server 2008"),
                new DatabaseTypeViewModel(DatabaseType.SqlServer2012, "SQL Server 2012"),
                new DatabaseTypeViewModel(DatabaseType.SqlServer2014, "SQL Server 2014")
            };
        }
    }
}