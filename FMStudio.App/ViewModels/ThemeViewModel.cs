using System.Collections.Generic;

namespace FMStudio.App.ViewModels
{
    public class ThemeViewModel
    {
        public string Name { get; set; }

        public string ResourceName { get; set; }

        public string SqlViewerResourceName { get; set; }

        public static List<ThemeViewModel> GetThemesList()
        {
            return new List<ThemeViewModel>()
            {
                new ThemeViewModel()
                {
                    Name = "Light",
                    ResourceName = "Light",
                    SqlViewerResourceName = "Light_sql"
                },
                new ThemeViewModel()
                {
                    Name = "Dark",
                    ResourceName = "Dark",
                    SqlViewerResourceName = "Dark_sql"
                }
            };
        }
    }
}