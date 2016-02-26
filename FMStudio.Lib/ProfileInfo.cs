using FluentMigrator;
using FluentMigrator.Runner;
using FMStudio.Lib.Exceptions;
using FMStudio.Lib.Repositories;
using FMStudio.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FMStudio.Lib
{
    public class ProfileInfo
    {
        private IMigrationsRepository _migrationsRepository;

        private ILog _log;

        private ProjectInfo _project;

        private TypeInfo _typeInfo;

        private ProfileAttribute _profileAttribute;

        private List<TagsAttribute> _tagsAttributes;

        private string _sql;

        public string Name { get; private set; }

        public List<string> Tags { get; private set; }

        public bool IsToBeRun
        {
            get { return !string.IsNullOrEmpty(_project.Profile) && _project.Profile.Equals(Name, StringComparison.OrdinalIgnoreCase); }
        }

        public ProfileInfo(
            IMigrationsRepository migrationsRepository,
            ILog log,
            ProjectInfo project,
            TypeInfo typeInfo)
        {
            _migrationsRepository = migrationsRepository;
            _log = log;
            _project = project;
            _typeInfo = typeInfo;
        }

        public Task RunAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    var context = _migrationsRepository.GetRunnerContext(Name, Tags, false);
                    using (var processor = _migrationsRepository.GetMigrationProcessor(_project.DatabaseType.Value, _project.ConnectionString, context))
                    {
                        var runner = new MigrationRunner(_project.MigrationsAssembly, context, processor);

                        var sw = new Stopwatch();
                        sw.Start();

                        runner.ApplyProfiles();

                        sw.Stop();

                        _log.Info("Successfully ran profile '{0}', took {1}", Name, sw.Elapsed);
                    }
                }
                catch (Exception e)
                {
                    throw new ProfileException("Could not run profile", e, this);
                }
            });
        }

        public async Task InitializeAsync()
        {
            await Task.Run(() =>
            {
                _profileAttribute = _typeInfo.GetCustomAttribute<ProfileAttribute>();
                _tagsAttributes = _typeInfo.GetCustomAttributes<TagsAttribute>().ToList();

                if (_tagsAttributes.Any())
                    Tags = _tagsAttributes.SelectMany(t => t.TagNames).ToList();
                else
                    Tags = Enumerable.Empty<string>().ToList();

                Name = _profileAttribute.ProfileName;
            });
        }

        public async Task<string> GetSqlAsync()
        {
            if (_sql == null)
                _sql = await _migrationsRepository.GetMigrationSql(_project.MigrationsAssembly, _typeInfo.FullName);

            return _sql;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}