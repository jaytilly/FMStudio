using FluentMigrator;
using FluentMigrator.Runner;
using FMStudio.Lib.Exceptions;
using FMStudio.Lib.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FMStudio.Lib
{
    public class MigrationInfo
    {
        private MigrationAttribute _migrationAttribute;
        private IMigrationsRepository _migrationsRepository;

        private ProjectInfo _project;

        private string _sql;
        private List<TagsAttribute> _tagsAttribute;
        private TypeInfo _typeInfo;

        public MigrationInfo(
            IMigrationsRepository migrationsRepository,
            ProjectInfo project,
            TypeInfo typeInfo)
        {
            _migrationsRepository = migrationsRepository;

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
            get { return !Tags.Any() || _project.Tags.Any(pt => Tags.Contains(pt)); }
        }

        public bool IsToBeRun
        {
            get { return !HasRun && IsIncluded; }
        }

        public bool IsValid { get; private set; }

        public List<string> Tags { get; private set; }

        public long Version { get; private set; }

        public async Task AddToVersionInfoTableAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    var context = _migrationsRepository.GetRunnerContext(_project.Profile, Tags, false);
                    using (var processor = _migrationsRepository.GetMigrationProcessor(_project.DatabaseType.Value, _project.ConnectionString, context))
                    {
                        var runner = new MigrationRunner(_project.MigrationsAssembly, context, processor);

                        runner.VersionLoader.UpdateVersionInfo(Version, Description);
                    }
                }
                catch (Exception e)
                {
                    throw new MigrateUpFailedException("Could not add record to version info table", e, this);
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
                var context = _migrationsRepository.GetRunnerContext(_project.Profile, _project.Tags, false);
                using (var processor = _migrationsRepository.GetMigrationProcessor(_project.DatabaseType.Value, _project.ConnectionString, context))
                {
                    var runner = new MigrationRunner(_project.MigrationsAssembly, context, processor);

                    runner.VersionLoader.DeleteVersion(Version);
                }
            });

            await InitializeAsync();
        }

        public async Task DownAsync()
        {
            // TODO: Implement
        }

        public async Task<string> GetSqlAsync()
        {
            if (_sql == null)
                _sql = await _migrationsRepository.GetMigrationSql(_project.MigrationsAssembly, _typeInfo.FullName);

            return _sql;
        }

        public async Task InitializeAsync()
        {
            _migrationAttribute = _typeInfo.GetCustomAttribute<MigrationAttribute>();

            Version = _migrationAttribute.Version;
            Description = _migrationAttribute.Description;

            _tagsAttribute = _typeInfo.GetCustomAttributes<TagsAttribute>().ToList();

            if (_tagsAttribute.Any())
                Tags = _tagsAttribute.SelectMany(t => t.TagNames).ToList();
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

        public override string ToString()
        {
            return string.Format("{0}: {1} ({2})", Version, Description, HasRun ? "Run" : "Not run");
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
                    }
                    catch (Exception e)
                    {
                        processor.RollbackTransaction();
                    }
                }
            });

            await InitializeAsync();
        }
    }
}