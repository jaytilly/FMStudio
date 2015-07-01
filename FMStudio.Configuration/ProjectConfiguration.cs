using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace FMStudio.Configuration
{
    public class ProjectConfiguration
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        
        public string DllPath { get; set; }

        public string ConnectionString { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DatabaseType DatabaseType { get; set; }

        public List<string> Tags { get; set; }

        public string Profile { get; set; }

        public ProjectConfiguration()
        {
            Id = Guid.NewGuid();
            Tags = new List<string>();
        }
    }
}