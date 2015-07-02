using Newtonsoft.Json;
using System.Collections.Generic;

namespace FMStudio.Configuration
{
    public class CategoryConfiguration
    {
        [JsonIgnore]
        public FMConfiguration RootConfiguration { get; set; }

        [JsonIgnore]
        public CategoryConfiguration ParentCategory { get; set; }

        public string Name { get; set; }

        public bool IsExpanded { get; set; }

        public List<CategoryConfiguration> Categories { get; set; }

        public List<ProjectConfiguration> Projects { get; set; }

        public CategoryConfiguration()
        {
            Categories = new List<CategoryConfiguration>();
            Projects = new List<ProjectConfiguration>();
        }

        public void Add(CategoryConfiguration category)
        {
            category.ParentCategory = this;

            if (!Categories.Contains(category))
                Categories.Add(category);
        }

        public void Add(ProjectConfiguration project)
        {
            project.ParentCategory = this;

            if (!Projects.Contains(project))
                Projects.Add(project);
        }

        public void MoveTo(CategoryConfiguration category)
        {
            ParentCategory.Remove(this);
            category.Add(this);
        }

        public void Remove(CategoryConfiguration category)
        {
            category.ParentCategory = null;
            Categories.Remove(category);
        }

        public void Remove(ProjectConfiguration project)
        {
            project.ParentCategory = null;
            Projects.Remove(project);
        }
    }
}