using FluentMigrator;
using FMStudio.Lib.Exceptions;
using FMStudio.Lib.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FMStudio.Lib
{
    public class ProjectInfo
    {
        public string PathToMigrationsDll { get; set; }

        public string ConnectionString { get; set; }

        public DatabaseType DatabaseType { get; set; }

        public List<string> Tags { get; set; }

        public string Profile { get; set; }

        public Assembly Assembly { get; private set; }

        public AssemblyName FMAssembly { get; private set; }

        public List<MigrationInfo> Migrations { get; private set; }

        public List<ProfileInfo> Profiles { get; private set; }

        public IOutputWriter Output { get; set; }

        public int ToBeRunMigrationsCount
        {
            get { return Migrations.Count(m => m.IsToBeRun); }
        }

        static ProjectInfo()
        {
            References.InitializeAssemblyBinding();
        }

        public ProjectInfo(string pathToMigrationsDll, string connectionString, DatabaseType databaseType)
        {
            PathToMigrationsDll = pathToMigrationsDll;
            ConnectionString = connectionString;
            DatabaseType = databaseType;
            Tags = new string[0].ToList();
            Profile = string.Empty;
            Output = new NotifyingOutputWriter();
        }

        public async Task InitializeAsync()
        {
            await Task.Run(() =>
            {
                if (!File.Exists(PathToMigrationsDll))
                    throw new InitializeProjectException(ExceptionType.CouldNotFindMigrationsDll, "Path to migrations dll '{0}' does not exist.", PathToMigrationsDll);

                try
                {
                    // Load migrations assembly
                    var bytes = File.ReadAllBytes(PathToMigrationsDll);

                    Assembly = Assembly.Load(bytes);
                    Output.Write("Loading migrations from assembly " + Assembly.FullName.ToString());
                }
                catch (Exception e)
                {
                    throw new InitializeProjectException(ExceptionMessages.InitializeProject_CouldNotLoadDll, e);
                }

                // Load references Fluent Migrator assembly
                FMAssembly = Assembly.GetReferencedAssemblies().FirstOrDefault(a => a.Name.Equals("FluentMigrator", StringComparison.OrdinalIgnoreCase));
                Output.Write("Found FluentMigrator assembly version " + FMAssembly.Version.ToString());

                if (FMAssembly == null)
                    throw new InitializeProjectException(ExceptionType.CouldNotFindFluentMigratorDllReference);

                if (!MigrationHelper.TestDatabaseConnectivity(ConnectionString, DatabaseType, Assembly))
                    throw new InitializeProjectException(ExceptionMessages.InitializeProject_CouldNotConnectToDatabase);

                // Find migration classes
                var migrationEntities = Assembly
                    .DefinedTypes
                    .Where(t => t.IsSubclassOf(typeof(Migration)))
                    .ToList()
                ;

                // Find migrations
                Migrations = migrationEntities
                    .Where(t => t.GetCustomAttribute<MigrationAttribute>() != null)
                    .Select(migration => new MigrationInfo(this, migration, true))
                    .ToList()
                ;

                // Find profiles
                Profiles = Assembly
                    .DefinedTypes
                    .Where(t => t.GetCustomAttribute<ProfileAttribute>() != null)
                    .Select(profile => new ProfileInfo(this, profile))
                    .ToList()
                ;
            });
        }

        /// <summary>
        /// Runs all applicable migrations and profiles
        /// </summary>
        public async Task FullUpdateAsync()
        {
            await RunApplicableMigrationsAsync();

            await RunApplicableProfilesAsync();
        }

        /// <summary>
        /// Run all migrations with a tag that has been specified on this project, or no tags at all
        /// </summary>
        public async Task RunApplicableMigrationsAsync()
        {
            var applicableMigrations = Migrations
                .Where(m =>
                    m.Tags.Any(t1 => Tags.Any(t2 => t1 == t2))
                    || !m.Tags.Any());

            foreach (var migration in applicableMigrations)
            {
                await migration.UpAsync(false);
            }
        }

        /// <summary>
        /// // Run all profiles with a name that has been specified on this project
        /// </summary>
        public async Task RunApplicableProfilesAsync()
        {
            var applicableProfiles = Profiles.Where(p => p.Name == Profile);

            foreach (var profile in applicableProfiles)
            {
                await profile.Run();
            }
        }
    }
}