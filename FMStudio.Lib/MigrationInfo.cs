using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using FMStudio.Lib.Exceptions;
using FMStudio.Lib.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FMStudio.Lib
{
    public class MigrationInfo
    {
        public event EventHandler OnUpdate = delegate { };

        public bool IsValid { get; set; }

        public long Version { get; set; }

        public string Description { get; set; }

        public List<string> Tags { get; set; }

        public bool HasRun { get; set; }

        public bool IsToBeRun
        {
            get { return !HasRun && IsIncluded; }
        }

        public bool IsIncluded
        {
            get { return !Tags.Any() || _project.Tags.Any(pt => Tags.Contains(pt)); }
        }

        public DateTime? AppliedOn { get; private set; }

        public string Sql { get; set; }

        private ProjectInfo _project;

        private TypeInfo _typeInfo;

        private MigrationAttribute _migrationAttribute;

        private List<TagsAttribute> _tagsAttribute;

        public MigrationInfo(ProjectInfo project, TypeInfo typeInfo, bool initialize)
        {
            _project = project;
            _typeInfo = typeInfo;

            if (initialize)
            {
                InitializeAsync().Wait();
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} ({2})", Version, Description, HasRun ? "Run" : "Not run");
        }

        public async Task DownAsync()
        {
            // TODO: Implement
        }

        public async Task UpAsync(bool clearFromVersionInfoTable)
        {
            await Task.Run(() =>
            {
                _project.Output.Write(string.Format("Running migration {0}: '{1}'", Version, Description));

                var announcer = new TextWriterAnnouncer(s => { });

                var migrationContext = new RunnerContext(announcer)
                {
                    Tags = Tags.ToArray(),
                    PreviewOnly = false
                };

                var factory = MigrationHelper.CreateFactory(_project.DatabaseType);

                if (clearFromVersionInfoTable)
                {
                    using (var processor = factory.Create(_project.ConnectionString, announcer, new MigrationProcessorOptions(migrationContext)))
                    {
                        processor.BeginTransaction();

                        try
                        {
                            var runner = new MigrationRunner(_project.Assembly, migrationContext, processor);

                            runner.VersionLoader.DeleteVersion(Version);

                            runner = new MigrationRunner(_project.Assembly, migrationContext, processor);

                            var m1 = runner.MigrationLoader.LoadMigrations();
                            var info = m1.Single(m => m.Key == Version);

                            runner.ApplyMigrationUp(info.Value, true);

                            processor.CommitTransaction();
                        }
                        catch (Exception e)
                        {
                            processor.RollbackTransaction();
                            throw new MigrateUpFailedException("Could not remove potential record from version info table", e, this);
                        }
                    }
                }
                else
                {
                    try
                    {
                        using (var processor = factory.Create(_project.ConnectionString, announcer, new MigrationProcessorOptions(migrationContext)))
                        {
                            var runner = new MigrationRunner(_project.Assembly, migrationContext, processor);

                            var m1 = runner.MigrationLoader.LoadMigrations();
                            var info = m1.Single(m => m.Key == Version);

                            runner.ApplyMigrationUp(info.Value, true);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new MigrateUpFailedException("Could not run the migration up-action", e, this);
                    }
                }
            });

            await InitializeAsync();
        }

        public async Task AddToVersionInfoTableAsync()
        {
            await Task.Run(() =>
            {
                var announcer = new TextWriterAnnouncer(s => _project.Output.Write(s));

                var migrationContext = new RunnerContext(announcer)
                {
                    Tags = Tags.ToArray(),
                    PreviewOnly = false
                };

                var factory = MigrationHelper.CreateFactory(_project.DatabaseType);

                using (var processor = factory.Create(_project.ConnectionString, announcer, new MigrationProcessorOptions(migrationContext)))
                {
                    try
                    {
                        var runner = new MigrationRunner(_project.Assembly, migrationContext, processor);

                        runner.VersionLoader.UpdateVersionInfo(_migrationAttribute.Version, _migrationAttribute.Description);
                    }
                    catch (Exception e)
                    {
                        processor.RollbackTransaction();
                        throw new MigrateUpFailedException("Could not add record to version info table", e, this);
                    }
                }
            });

            await InitializeAsync();
        }

        public async Task DeleteFromVersionInfoTableAsync()
        {
            await Task.Run(() =>
            {
                var announcer = new TextWriterAnnouncer(s => _project.Output.Write(s));

                var migrationContext = new RunnerContext(announcer)
                {
                    Tags = Tags.ToArray(),
                    PreviewOnly = false
                };

                var factory = MigrationHelper.CreateFactory(_project.DatabaseType);

                using (var processor = factory.Create(_project.ConnectionString, announcer, new MigrationProcessorOptions(migrationContext)))
                {
                    try
                    {
                        var runner = new MigrationRunner(_project.Assembly, migrationContext, processor);

                        runner.VersionLoader.DeleteVersion(Version);
                    }
                    catch (Exception e)
                    {
                        processor.RollbackTransaction();
                        throw new MigrateUpFailedException("Could not remove potential record from version info table", e, this);
                    }
                }
            });

            await InitializeAsync();
        }

        public async Task InitializeAsync()
        {
            await Task.Run(() =>
            {
                _migrationAttribute = _typeInfo.GetCustomAttribute<MigrationAttribute>();
                _tagsAttribute = _typeInfo.GetCustomAttributes<TagsAttribute>().ToList();

                Version = _migrationAttribute.Version;
                Description = _migrationAttribute.Description;

                if (_tagsAttribute.Any())
                {
                    Tags = _tagsAttribute.SelectMany(t => t.TagNames).ToList();
                }
                else
                {
                    Tags = Enumerable.Empty<string>().ToList();
                }

                HasRun = MigrationHelper.CheckIfMigrationHasRun(_project, Version);
                
                if (HasRun)
                    AppliedOn = MigrationHelper.GetAppliedOnDate(_project, Version);

                Sql = MigrationHelper.GetMigrationSql(_project, _typeInfo.FullName);
            });

            OnUpdate(this, EventArgs.Empty);
        }
    }
}