using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace FMStudio.Configuration
{
    public class ProjectConfiguration
    {
        [JsonIgnore]
        public FMConfiguration RootConfiguration { get; set; }

        [JsonIgnore]
        public CategoryConfiguration ParentCategory { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsReadOnly { get; set; }

        public string DllPath { get; set; }

        public string ConnectionString { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DatabaseType? DatabaseType { get; set; }

        public List<string> Tags { get; set; }

        public string Profile { get; set; }

        public bool IsExpanded { get; set; }

        public bool IsMigrationsExpanded { get; set; }

        public bool IsProfilesExpanded { get; set; }

        public ProjectConfiguration()
        {
            Id = Guid.NewGuid();
            Tags = new List<string>();
        }

        public void MoveTo(CategoryConfiguration category)
        {
            if (ParentCategory != null)
                ParentCategory.Remove(this);

            ParentCategory.Add(this);
        }
    }
}