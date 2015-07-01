using FluentMigrator;
using FMStudio.Lib.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FMStudio.Lib
{
    public class ProfileInfo
    {
        public string Name { get; set; }

        public string Sql { get; set; }

        public List<string> Tags { get; set; }

        private ProjectInfo _project;

        private TypeInfo _typeInfo;

        private ProfileAttribute _profileAttribute;

        private List<TagsAttribute> _tagsAttributes;

        public ProfileInfo(ProjectInfo project, TypeInfo typeInfo)
        {
            _project = project;
            _typeInfo = typeInfo;

            Initialize();
        }

        public async Task Run()
        {
            _project.Output.Write(string.Format("Running profile '{0}'...", Name));

            await Task.Run(() => MigrationHelper.RunProfile(_project, this));

            _project.Output.Write("Done");
        }

        public override string ToString()
        {
            return Name;
        }

        public async Task InitializeAsync()
        {
            await Task.Run(() => Initialize());
        }

        public void Initialize()
        {
            _profileAttribute = _typeInfo.GetCustomAttribute<ProfileAttribute>();
            _tagsAttributes = _typeInfo.GetCustomAttributes<TagsAttribute>().ToList();

            if (_tagsAttributes.Any())
            {
                Tags = _tagsAttributes.SelectMany(t => t.TagNames).ToList();
            }
            else
            {
                Tags = Enumerable.Empty<string>().ToList();
            }

            Name = _profileAttribute.ProfileName;
            Sql = MigrationHelper.GetMigrationSql(_project, _typeInfo.FullName);
        }
    }
}