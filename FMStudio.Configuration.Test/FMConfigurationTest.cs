using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace FMStudio.Configuration.Test
{
    [TestClass]
    public class FMConfigurationTest
    {
        [TestMethod]
        public void SerializationTest()
        {
            //var project1 = new ProjectConfiguration()
            //{
            //    ConnectionString = "connection-string-1",
            //    DatabaseType = DatabaseType.SqlServer2012,
            //    DllPath = "dll-path-1",
            //    Name = "name-1",
            //    Profile = "profile-1",
            //    Tags = new List<string>()
            //    {
            //        "tag-1",
            //        "tag-2"
            //    }
            //};

            //var project2 = new ProjectConfiguration()
            //{
            //    ConnectionString = "connection-string-2",
            //    DatabaseType = DatabaseType.SqlServer2008,
            //    DllPath = "dll-path-2",
            //    Name = "name-2",
            //    Profile = "profile-2",
            //    Tags = new List<string>()
            //    {
            //        "tag-3",
            //        "tag-4"
            //    }
            //};

            //var before = new FMConfiguration()
            //{
            //    Version = 12,
            //    Projects = new List<ProjectConfiguration>()
            //    {
            //        project1,
            //        project2
            //    }
            //};

            //before.Save();

            //var after = FMConfiguration.Load();

            //Assert.AreEqual(before.Version, after.Version);
            //Assert.AreEqual(before.Projects.Count, after.Projects.Count);

            //var p1 = after.Projects.FirstOrDefault(p => p.Name == project1.Name);
            //Assert.IsNotNull(p1);
            //Assert.AreEqual(project1.ConnectionString, p1.ConnectionString);
            //Assert.AreEqual(project1.DatabaseType, p1.DatabaseType);
            //Assert.AreEqual(project1.DllPath, p1.DllPath);
            //Assert.AreEqual(project1.Profile, p1.Profile);
            //Assert.AreEqual(project1.Tags.Count, p1.Tags.Count);
            //Assert.IsTrue(p1.Tags.Any(t => t == project1.Tags[0]));
            //Assert.IsTrue(p1.Tags.Any(t => t == project1.Tags[1]));

            //var p2 = after.Projects.FirstOrDefault(p => p.Name == project2.Name);
            //Assert.IsNotNull(p2);
            //Assert.AreEqual(project2.ConnectionString, p2.ConnectionString);
            //Assert.AreEqual(project2.DatabaseType, p2.DatabaseType);
            //Assert.AreEqual(project2.DllPath, p2.DllPath);
            //Assert.AreEqual(project2.Profile, p2.Profile);
            //Assert.AreEqual(project2.Tags.Count, p2.Tags.Count);
            //Assert.IsTrue(p2.Tags.Any(t => t == project2.Tags[0]));
            //Assert.IsTrue(p2.Tags.Any(t => t == project2.Tags[1]));
        }
    }
}