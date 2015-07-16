using FluentMigrator;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using FMStudio.Lib.Exceptions;
using FMStudio.Lib.Repositories;
using FMStudio.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FMStudio.Lib
{
    public class MigrationInfo
    {
        private IMigrationsRepository _migrationsRepository;

        private ILog _log;

        private ProjectInfo _project;

        private TypeInfo _typeInfo;

        private string _sql;

        public MigrationInfo(
            IMigrationsRepository migrationsRepository,
            ILog log,
            ProjectInfo project,
            TypeInfo typeInfo)
        {
            _migrationsRepository = migrationsRepository;
            _log = log;

            _project = project;
            _typeInfo = typeInfo;

            Tags = new List<string>();
        }

        public event EventHandler MigrationUpdated = delegate { };

        public DateTime? AppliedOn { get; private set; }

        public string Description { get; private set; }

        public bool HasRun { get; private set; }

        public bool IsIncluded
        {
            get { return !DefaultMigrationConventions.TypeHasTags(_typeInfo) || DefaultMigrationConventions.TypeHasMatchingTags(_typeInfo, _project.Tags); }
        }

        public bool IsToBeRun
        {
            get { return !HasRun && IsIncluded; }
        }

        public List<string> Tags { get; private set; }

        public long Version { get; private set; }

        public async Task AddToVersionInfoTableAsync()
        {
            if (!_project.IsDatabaseInitialized)
                throw new InvalidOperationException("Cannot add version to version info table: the database connection has not been initialized");

            await Task.Run(() =>
            {
                try
                {
                    var context = _migrationsRepository.GetRunnerContext(_project.Profile, Tags, false);
                    using (var processor = _migrationsRepository.GetMigrationProcessor(_project.DatabaseType.Value, _project.ConnectionString, context))
                    {
                        var runner = new MigrationRunner(_project.MigrationsAssembly, context, processor);

                        runner.VersionLoader.UpdateVersionInfo(Version, Description);

                        _log.Info("Added version {0}: '{1}' to version info table", Version, Description);
                    }
                }
                catch (Exception e)
                {
                    throw new MigrationException("Could not add record to version info table", e, this);
                }
            });

            await InitializeAsync();
        }

        public async Task DeleteFromVersionInfoTableAsync()
        {
            if (!_project.IsDatabaseInitialized)
                throw new InvalidOperationException("Cannot delete migration from version info table: the database connection has not been initialized");

            await Task.Run(() =>
            {
                try
                {
                    var context = _migrationsRepository.GetRunnerContext(_project.Profile, _project.Tags, false);
                    using (var processor = _migrationsRepository.GetMigrationProcessor(_project.DatabaseType.Value, _project.ConnectionString, context))
                    {
                        var runner = new MigrationRunner(_project.MigrationsAssembly, context, processor);

                        runner.VersionLoader.DeleteVersion(Version);

                        _log.Info("Deleted version {0}: '{1}' from version info table", Version, Description);
                    }
                }
                catch (Exception e)
                {
                    throw new MigrationException("Could not delete version from version info table", e, this);
                }
            });

            await InitializeAsync();
        }

        public async Task DownAsync()
        {
            if (!_project.IsDatabaseInitialized)
                throw new InvalidOperationException("Cannot undo migration: database connection has not been initialized");

            await Task.Run(() =>
            {
                try
                {
                    var context = _migrationsRepository.GetRunnerContext(_project.Profile, Tags, false);
                    using (var processor = _migrationsRepository.GetMigrationProcessor(_project.DatabaseType.Value, _project.ConnectionString, context))
                    {
                        var runner = new MigrationRunner(_project.MigrationsAssembly, context, processor);

                        var migrations = runner.MigrationLoader.LoadMigrations();
                        var info = migrations.Single(m => m.Key == Version);

                        runner.ApplyMigrationDown(info.Value, true);

                        _log.Info("Successfully undone migration {0}: '{1}'", Version, Description);
                    }
                }
                catch (Exception e)
                {
                    throw new MigrationException("Could not undo migration", e, this);
                }
            });
        }

        public async Task<string> GetSqlAsync()
        {
            if (_sql == null)
                _sql = await _migrationsRepository.GetMigrationSql(_project.MigrationsAssembly, _typeInfo.FullName);

            return _sql;
        }

        public async Task InitializeAsync()
        {
            var migrationInfo = DefaultMigrationConventions.GetMigrationInfoFor(_typeInfo.AsType());

            Version = migrationInfo.Version;
            Description = migrationInfo.Description;

            var tagsAttributes = _typeInfo.GetCustomAttributes<TagsAttribute>().ToList();

            if (tagsAttributes.Any())
                Tags = tagsAttributes.SelectMany(t => t.TagNames).ToList();
            else
                Tags = Enumerable.Empty<string>().ToList();

            if (_project.IsDatabaseInitialized)
            {
                HasRun = await _migrationsRepository.IsVersionApplied(_project.MigrationsAssembly, Version, _project.DatabaseType.Value, _project.ConnectionString);

                if (HasRun)
                    AppliedOn = await _migrationsRepository.GetAppliedOnDate(_project.MigrationsAssembly, Version, _project.DatabaseType.Value, _project.ConnectionString);
            }

            MigrationUpdated(this, EventArgs.Empty);
        }

        public async Task MigrateToThisVersionAsync()
        {
            await _project.MigrateToVersion(Version);
        }

        public async Task UpAsync(bool clearFromVersionInfoTable)
        {
            if (!_project.IsDatabaseInitialized)
                throw new InvalidOperationException("Cannot run migration: database connection has not been initialized");

            await Task.Run(() =>
            {
                var context = _migrationsRepository.GetRunnerContext(_project.Profile, _project.Tags, false);
                using (var processor = _migrationsRepository.GetMigrationProcessor(_project.DatabaseType.Value, _project.ConnectionString, context))
                {
                    processor.BeginTransaction();

                    try
                    {
                        var runner = new MigrationRunner(_project.MigrationsAssembly, context, processor);

                        if (clearFromVersionInfoTable)
                            runner.VersionLoader.DeleteVersion(Version);

                        runner = new MigrationRunner(_project.MigrationsAssembly, context, processor);

                        var migrations = runner.MigrationLoader.LoadMigrations();
                        var info = migrations.Single(m => m.Key == Version);

                        runner.ApplyMigrationUp(info.Value, true);

                        processor.CommitTransaction();

                        _log.Info("Successfully applied migration {0}: '{1}'", Version, Description);
                    }
                    catch (Exception e)
                    {
                        processor.RollbackTransaction();

                        throw new MigrationException("Could not apply migration", e, this);
                    }
                }
            });

            await InitializeAsync();
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} ({2})", Version, Description, HasRun ? "Run" : "Not run");
        }
    }
}