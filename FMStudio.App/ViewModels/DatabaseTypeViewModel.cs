using FMStudio.Configuration;
using System.Collections.Generic;

namespace FMStudio.App.ViewModels
{
    public class DatabaseTypeViewModel
    {
        public FMStudio.Configuration.DatabaseType Value { get; set; }

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
                new DatabaseTypeViewModel(Configuration.DatabaseType.SQLite, "SQLite"),
                new DatabaseTypeViewModel(Configuration.DatabaseType.SqlServer2008, "SQL Server 2008"),
                new DatabaseTypeViewModel(Configuration.DatabaseType.SqlServer2012, "SQL Server 2012")
            };
        }
    }
}