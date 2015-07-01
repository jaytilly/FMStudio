using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace FMStudio.Configuration
{
    public class FMConfiguration
    {
        public const string DefaultPath = "config.json";

        public int Version { get; set; }

        public List<CategoryConfiguration> Categories { get; set; }

        public Preferences Preferences { get; set; }

        public FMConfiguration()
        {
            Version = 1;
            Categories = new List<CategoryConfiguration>();
            Preferences = new Preferences();
        }

        /// <summary>
        /// Saves an FMConfiguration object to the specified path
        /// </summary>
        public void Save(string path)
        {
            var file = JsonConvert.SerializeObject(this, Formatting.Indented);

            File.WriteAllText(path, file);
        }

        /// <summary>
        /// Saves an FMConfiguration object to the default path
        /// </summary>
        public void Save()
        {
            Save(DefaultPath);
        }

        /// <summary>
        /// Loads an FMConfiguration object from the specified path
        /// </summary>
        public static FMConfiguration Load(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException("path");

            if (!File.Exists(path)) throw new FileNotFoundException(path);

            try
            {
                var file = File.ReadAllText(path);

                var config = JsonConvert.DeserializeObject<FMConfiguration>(file);

                config.Link();

                return config;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// Loads an FMConfiguration object from the default path
        /// </summary>
        public static FMConfiguration Load()
        {
            if (File.Exists(DefaultPath))
            {
                return Load(DefaultPath);
            }

            return new FMConfiguration();
        }

        public void Link()
        {
            foreach(var category in Categories)
            {
                Link(category);
            }
        }

        private void Link(CategoryConfiguration categoryConfiguration)
        {
            categoryConfiguration.RootConfiguration = this;

            foreach (var subCategoryConfiguration in categoryConfiguration.Categories)
            {
                Link(subCategoryConfiguration);
            }

            foreach (var projectConfiguration in categoryConfiguration.Projects)
            {
                projectConfiguration.RootConfiguration = this;

                projectConfiguration.ParentCategory = categoryConfiguration;
            }
        }
    }
}