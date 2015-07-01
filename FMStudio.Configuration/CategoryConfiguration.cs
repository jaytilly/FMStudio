using System.Collections.Generic;

namespace FMStudio.Configuration
{
    public class CategoryConfiguration
    {
        public string Name { get; set; }

        public List<CategoryConfiguration> Categories { get; set; }

        public List<ProjectConfiguration> Projects { get; set; }

        public CategoryConfiguration()
        {
            Categories = new List<CategoryConfiguration>();
            Projects = new List<ProjectConfiguration>();
        }
    }
}