﻿using FluentMigrator;
using FluentMigrator.Runner;
using FMStudio.Lib.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FMStudio.Lib
{
    public class ProfileInfo
    {
        private IMigrationsRepository _migrationsRepository;

        private ProjectInfo _project;

        private TypeInfo _typeInfo;

        private ProfileAttribute _profileAttribute;

        private List<TagsAttribute> _tagsAttributes;

        private string _sql;

        public string Name { get; private set; }

        public List<string> Tags { get; private set; }

        public bool IsToBeRun
        {
            get { return _project.Profile.Equals(Name, System.StringComparison.OrdinalIgnoreCase); }
        }

        public ProfileInfo(
            IMigrationsRepository migrationsRepository,
            ProjectInfo project,
            TypeInfo typeInfo)
        {
            _migrationsRepository = migrationsRepository;
            _project = project;
            _typeInfo = typeInfo;
        }

        public Task RunAsync()
        {
            return Task.Run(() =>
            {
                var context = _migrationsRepository.GetRunnerContext(Name, Tags, false);
                using (var processor = _migrationsRepository.GetMigrationProcessor(_project.DatabaseType.Value, _project.ConnectionString, context))
                {
                    var runner = new MigrationRunner(_project.MigrationsAssembly, context, processor);

                    runner.ApplyProfiles();
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