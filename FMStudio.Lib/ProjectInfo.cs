using FMStudio.Lib.Exceptions;
using FMStudio.Lib.Repositories;
using FMStudio.Lib.Utility;
using FMStudio.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Validation;

namespace FMStudio.Lib
{
    public class ProjectInfo
    {
        private IAssemblyRepository _assemblyRepository;

        private IFilesRepository _filesRepository;

        private IMigrationsRepository _migrationsRepository;

        private ILog _log;

        public event EventHandler ProjectUpdated = delegate { };

        public string Name { get; set; }

        public string PathToMigrationsDll { get; private set; }

        public string ConnectionString { get; private set; }

        public DatabaseType? DatabaseType { get; private set; }

        public bool IsDatabaseInitialized { get; private set; }

        public string Profile { get; set; }

        public List<string> Tags { get; set; }

        public Assembly MigrationsAssembly { get; private set; }

        public AssemblyName FluentMigratorAssemblyName { get; private set; }

        public List<MigrationInfo> Migrations { get; private set; }

        public List<ProfileInfo> Profiles { get; private set; }

        public int ToBeRunMigrationsCount { get { return Migrations.Count(m => m.IsToBeRun); } }

        static ProjectInfo()
        {
            References.Initialize();
        }

        public ProjectInfo()
            : this(null, null, null, null)
        {
        }

        public ProjectInfo(
            IAssemblyRepository assemblyRepository,
            IFilesRepository filesRepository,
            IMigrationsRepository migrationsRepository,
            ILog log)
        {
            _assemblyRepository = assemblyRepository ?? new AssemblyRepository();
            _filesRepository = filesRepository ?? new FilesRepository();
            _migrationsRepository = migrationsRepository ?? new MigrationsRepository();
            _log = log ?? new Log();
        }

        /// <summary>
        /// Attempts to load a migrations assembly from the specified path and populate this object
        /// </summary>
        public async Task InitializeMigrationsAsync(string pathToMigrations)
        {
            Requires.NotNull(pathToMigrations, "pathToMigrations");

            // Find files based on the specified glob
            var file = await _filesRepository.GetMostRecentMatchingFile(pathToMigrations);
            if (file == null)
                throw new ProjectException("Path to migrations dll does not exist", null, this);

            // Load migrations assembly
            MigrationsAssembly = await _assemblyRepository.LoadFromFile(file.FullName);
            if (MigrationsAssembly == null)
                throw new ProjectException("Could not load any migrations from the specified file", null, this);

            // Load referenced Fluent Migrator assembly
            FluentMigratorAssemblyName = await _assemblyRepository.GetReferenceByName(MigrationsAssembly, "FluentMigrator");
            if (FluentMigratorAssemblyName == null)
                throw new ProjectException("Could not find a reference to the FluentMigrator assembly", null, this);

            // Find migration classes
            var migrationTypes = await _migrationsRepository.GetMigrationTypes(MigrationsAssembly);
            Migrations = migrationTypes.Select(m => new MigrationInfo(_migrationsRepository, _log, this, m)).ToList();

            // Find profile classes
            var profileTypes = await _migrationsRepository.GetProfileTypes(MigrationsAssembly);
            Profiles = profileTypes.Select(p => new ProfileInfo(_migrationsRepository, _log, this, p)).ToList();

            await Task.WhenAll(Migrations.Select(m => m.InitializeAsync()));
            await Task.WhenAll(Profiles.Select(p => p.InitializeAsync()));

            PathToMigrationsDll = pathToMigrations;

            _log.Info("Loaded {0} migrations and {1} profiles for project '{2}' from assembly {3}, which uses FluentMigrator {4}",
                Migrations.Count, Profiles.Count, Name, MigrationsAssembly.GetName().Name, FluentMigratorAssemblyName.Version.ToString());
        }

        /// <summary>
        /// Checks whether the specified connection parameters work
        /// </summary>
        public async Task InitializeDatabase(DatabaseType databaseType, string connectionString)
        {
            Requires.NotNull(connectionString, "connectionString");

            var canConnectToDatabase = await _migrationsRepository.CanConnectToDatabase(databaseType, connectionString);
            if (!canConnectToDatabase)
                throw new ProjectException("Could not connect to database", null, this);

            await Task.WhenAll(Migrations.Select(m => m.InitializeAsync()));
            await Task.WhenAll(Profiles.Select(p => p.InitializeAsync()));

            DatabaseType = databaseType;
            ConnectionString = connectionString;
            IsDatabaseInitialized = true;

            _log.Info("Initialized {0} database connection for project '{1}'", databaseType.ToString(), Name);
        }

        /// <summary>
        /// Runs all applicable migrations and profiles
        /// </summary>
        public async Task FullUpdateAsync()
        {
            await RunApplicableMigrationsAsync();

            await RunApplicableProfilesAsync();

            _log.Info("Ran full update for project '{0}'", Name);
        }

        /// <summary>
        /// Run all migrations with a tag that has been specified on this project, or no tags at all
        /// </summary>
        public async Task RunApplicableMigrationsAsync()
        {
            foreach (var migration in Migrations.Where(m => m.IsToBeRun).OrderBy(m => m.Version))
                await migration.UpAsync(false);

            _log.Info("Ran all applicable migrations for project '{0}'", Name);
        }

        /// <summary>
        /// // Run all profiles with a name that has been specified on this project
        /// </summary>
        public async Task RunApplicableProfilesAsync()
        {
            foreach (var profile in Profiles.Where(p => p.IsToBeRun))
                await profile.RunAsync();

            _log.Info("Ran all applicable profiles for project '{0}'", Name);
        }
    }
}