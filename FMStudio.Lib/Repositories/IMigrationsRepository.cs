using FluentMigrator;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace FMStudio.Lib.Repositories
{
    public interface IMigrationsRepository
    {
        Task<bool> CanConnectToDatabase(DatabaseType databaseType, string connectionString);

        Task<DateTime?> GetAppliedOnDate(Assembly migrationsAssembly, long version, DatabaseType databaseType, string connectionString);

        IEnumerable<TypeInfo> GetMigrationEntities(Assembly assembly);

        IMigrationProcessor GetMigrationProcessor(DatabaseType databaseType, string connectionString, RunnerContext context);

        Task<string> GetMigrationSql(DatabaseType databaseType, Assembly migrationsAssembly, string typeName);

        Task<IEnumerable<TypeInfo>> GetMigrationTypes(Assembly assembly);

        MigrationProcessorFactory GetProcessorFactory(DatabaseType databaseType);

        Task<IEnumerable<TypeInfo>> GetProfileTypes(Assembly assembly);

        RunnerContext GetRunnerContext(string profile, IEnumerable<string> tags, bool previewOnly);

        Task<bool> IsVersionApplied(Assembly migrationsAssembly, long version, DatabaseType databaseType, string connectionString);
    }
}