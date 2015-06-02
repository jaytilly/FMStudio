using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.SQLite;
using FluentMigrator.Runner.Processors.SqlServer;
using System;
using System.Linq;
using System.Reflection;

namespace FMStudio.Lib.Utility
{
    public static class MigrationHelper
    {
        public static bool CheckIfMigrationHasRun(ProjectInfo project, long version)
        {
            var announcer = new TextWriterAnnouncer(s => { });

            var migrationContext = new RunnerContext(announcer)
            {
                Tags = new string[0],
                PreviewOnly = true
            };

            var factory = CreateFactory(project.DatabaseType);
            using (var processor = factory.Create(project.ConnectionString, announcer, new MigrationProcessorOptions(migrationContext)))
            {
                var runner = new MigrationRunner(project.Assembly, migrationContext, processor);

                return runner.VersionLoader.VersionInfo.HasAppliedMigration(version);
            }
        }

        public static bool TestDatabaseConnectivity(string connectionString, DatabaseType databaseType, Assembly migrationsAssembly)
        {
            var announcer = new TextWriterAnnouncer(s => { });

            var migrationContext = new RunnerContext(announcer)
            {
                Tags = new string[0],
                PreviewOnly = true
            };

            var factory = CreateFactory(databaseType);
            using (var processor = factory.Create(connectionString, announcer, new MigrationProcessorOptions(migrationContext)))
            {
                try
                {
                    new MigrationRunner(migrationsAssembly, migrationContext, processor);

                    return true;
                }
                catch (Exception) { }
            }

            return false;
        }

        public static string GetMigrationSql(ProjectInfo project, string typeName)
        {
            var inst = project.Assembly.CreateInstance(typeName) as Migration;

            var context = new StubMigrationContext();

            inst.SetFieldValue("_context", context);
            inst.Up();

            return string.Join(Environment.NewLine, context.Expressions.Select(e =>
            {
                var sqlStatementExpression = e as ExecuteSqlStatementExpression;
                if (sqlStatementExpression != null)
                {
                    return sqlStatementExpression.SqlStatement;
                }

                return e.ToString();
            }).ToList());
        }

        public static void Run(ProjectInfo project, ProfileInfo profileInfo)
        {
            var announcer = new TextWriterAnnouncer(s => { });

            var migrationContext = new RunnerContext(announcer)
            {
                Tags = profileInfo.Tags.ToArray(),
                PreviewOnly = false
            };

            var factory = CreateFactory(project.DatabaseType);
            using (var processor = factory.Create(project.ConnectionString, announcer, new MigrationProcessorOptions(migrationContext)))
            {
                var runner = new MigrationRunner(project.Assembly, migrationContext, processor);

                runner.ApplyProfiles();

                Console.WriteLine("Running profiles");
            }
        }

        public static MigrationProcessorFactory CreateFactory(DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.SqlServer2008:
                    return new SqlServer2008ProcessorFactory();

                case DatabaseType.SqlServer2012:
                    return new SqlServer2012ProcessorFactory();

                case DatabaseType.Sqlite:
                    return new SQLiteProcessorFactory();

                default:
                    throw new InvalidOperationException("Unknown database type: " + databaseType.ToString());
            }
        }
    }
}