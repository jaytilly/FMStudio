using System;
using System.Diagnostics;
using System.Reflection;

namespace FMStudio.Lib.Utility
{
    public static class References
    {
        public static void Load()
        {
            var sqlite = typeof(FluentMigrator.Runner.Processors.SQLite.SQLiteProcessorFactory);
        }

        public static void InitializeAssemblyBinding()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public static string GetFluentMigratorAssemblyVersion()
        {
            return typeof(FluentMigrator.Migration).Assembly.GetName().Version.ToString();
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name);

            if (name.Name.Equals("FluentMigrator", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(FluentMigrator.Migration).Assembly;
            }

            return null;
        }
    }
}