using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Generators.Postgres;
using FluentMigrator.Runner.Generators.SqlServer;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.Postgres;
using FluentMigrator.Runner.Processors.SQLite;
using FluentMigrator.Runner.Processors.SqlServer;
using FMStudio.Lib.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FMStudio.Lib.Repositories
{
    public class MigrationsRepository : IMigrationsRepository
    {
        public Task<bool> CanConnectToDatabase(DatabaseType databaseType, string connectionString)
        {
            return Task.Run(() =>
            {
                try
                {
                    var context = GetRunnerContext(null, null, true);
                    using (var processor = GetMigrationProcessor(databaseType, connectionString, context))
                    {
                        // If the MigrationRunner class can be constructed, it indicates a working database connection
                        // We don't want to have a dependency on the migrations assembly in this method, so we'll use
                        // the calling assembly instead
                        new MigrationRunner(Assembly.GetCallingAssembly(), context, processor);

                        return true;
                    }
                }
                catch { }

                return false;
            });
        }

        public Task<DateTime?> GetAppliedOnDate(Assembly migrationsAssembly, long version, DatabaseType databaseType, string connectionString)
        {
            return Task.Run(() =>
            {
                try
                {
                    var context = GetRunnerContext(null, null, true);
                    using (var processor = GetMigrationProcessor(databaseType, connectionString, context))
                    {
                        var runner = new MigrationRunner(migrationsAssembly, context, processor);

                        var schemaName = runner.VersionLoader.VersionTableMetaData.SchemaName;
                        var tableName = runner.VersionLoader.VersionTableMetaData.TableName;
                        var versionColumnName = runner.VersionLoader.VersionTableMetaData.ColumnName;
                        var appliedOnColumnName = runner.VersionLoader.VersionTableMetaData.AppliedOnColumnName;

                        var dataSet = processor.ReadTableData(schemaName, tableName);

                        var table = dataSet.Tables[0];
                        foreach (DataRow row in table.Rows)
                        {
                            var versionColumn = (long)row[versionColumnName];

                            if (versionColumn == version)
                                return (DateTime)row[appliedOnColumnName];
                        }
                    }
                }
                catch { }

                return new DateTime?();
            });
        }

        public IEnumerable<TypeInfo> GetMigrationEntities(Assembly assembly)
        {
            var migrationEntities = assembly
                .DefinedTypes
                .Where(t => t.IsSubclassOf(typeof(Migration)))
                .ToList()
            ;

            return migrationEntities;
        }

        public IMigrationProcessor GetMigrationProcessor(DatabaseType databaseType, string connectionString, RunnerContext context)
        {
            var announcer = new NullAnnouncer();
            var options = new MigrationProcessorOptions(context);

            var factory = GetProcessorFactory(databaseType);
            var processor = factory.Create(connectionString, announcer, options);

            return processor;
        }

        public Task<string> GetMigrationSql(DatabaseType databaseType, Assembly migrationsAssembly, string typeName)
        {
            return Task.Run(() =>
            {
                using (var announcer = new InterceptingAnnouncer())
                {
                    var context = new StubMigrationContext();
                    var generator = GetGenerator(databaseType);

                    var runnerContext = new RunnerContext(announcer);
                    var processor = new ConnectionlessProcessor(generator, runnerContext, new MigrationProcessorOptions(runnerContext));

                    var inst = migrationsAssembly.CreateInstance(typeName) as Migration;
                    inst.GetUpExpressions(context);
                    context.Expressions.ToList().ForEach(e => e.ExecuteWith(processor));
                    announcer.Flush();

                    announcer.SqlStream.Position = 0;
                    var length = Math.Min(1024 * 1024, (int)announcer.SqlStream.Length);
                    var bytes = new byte[length];
                    announcer.SqlStream.Read(bytes, 0, length);

                    return Encoding.Default.GetString(bytes);
                }
            });
        }

        public Task<IEnumerable<TypeInfo>> GetMigrationTypes(Assembly assembly)
        {
            return Task.Run(() =>
            {
                var entities = GetMigrationEntities(assembly);

                // Find migrations
                var migrationTypes = entities
                    .Where(t => t.GetCustomAttribute<MigrationAttribute>() != null)
                    .ToList()
                ;

                return (IEnumerable<TypeInfo>)migrationTypes;
            });
        }

        public MigrationProcessorFactory GetProcessorFactory(DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.SqlServer2000:
                    return new SqlServer2000ProcessorFactory();

                case DatabaseType.SqlServer2005:
                    return new SqlServer2005ProcessorFactory();

                case DatabaseType.SqlServer2008:
                    return new SqlServer2008ProcessorFactory();

                case DatabaseType.SqlServer2012:
                    return new SqlServer2012ProcessorFactory();

                case DatabaseType.SqlServer2014:
                    return new SqlServer2014ProcessorFactory();

                case DatabaseType.Sqlite:
                    return new SQLiteProcessorFactory();

                case DatabaseType.Postgres:
                    return new PostgresProcessorFactory();

                default:
                    throw new InvalidOperationException("Unknown database type: " + databaseType.ToString());
            }
        }

        public IMigrationGenerator GetGenerator(DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.SqlServer2000:
                    return new SqlServer2000Generator();

                case DatabaseType.SqlServer2005:
                    return new SqlServer2005Generator();

                case DatabaseType.SqlServer2008:
                    return new SqlServer2008Generator();

                case DatabaseType.SqlServer2012:
                    return new SqlServer2012Generator();

                case DatabaseType.SqlServer2014:
                    return new SqlServer2014Generator();

                case DatabaseType.Postgres:
                    return new PostgresGenerator(new PostgresQuoter(new PostgresOptions(){ ForceQuote = false }));

                default:
                    throw new InvalidOperationException("Unknown database type: " + databaseType.ToString());
            }
        }

        public Task<IEnumerable<TypeInfo>> GetProfileTypes(Assembly assembly)
        {
            return Task.Run(() =>
            {
                var entities = GetMigrationEntities(assembly);

                // Find profiles
                var profileTypes = entities
                    .Where(t => t.GetCustomAttribute<ProfileAttribute>() != null)
                    .ToList()
                ;

                return (IEnumerable<TypeInfo>)profileTypes;
            });
        }

        public RunnerContext GetRunnerContext(string profile, IEnumerable<string> tags, bool previewOnly)
        {
            var announcer = new NullAnnouncer();
            var context = new RunnerContext(announcer)
            {
                PreviewOnly = previewOnly,
                Profile = profile,
                Tags = tags
            };

            return context;
        }

        public Task<bool> IsVersionApplied(Assembly migrationsAssembly, long version, DatabaseType databaseType, string connectionString)
        {
            return Task.Run(() =>
            {
                var context = GetRunnerContext(null, null, true);
                using (var processor = GetMigrationProcessor(databaseType, connectionString, context))
                {
                    var runner = new MigrationRunner(migrationsAssembly, context, processor);

                    return runner.VersionLoader.VersionInfo.HasAppliedMigration(version);
                }
            });
        }
    }
}