using FMStudio.Lib.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace FMStudio.Lib.Test.Utility
{
    [TestClass]
    public class MigrationsAssemblyLoaderTest
    {
        [TestMethod]
        public void GetPathMatchingGlobTest()
        {
            var root = Path.GetFullPath(".");

            var pattern = Path.Combine(root, @"*Lib*.dll");

            var files = MigrationsAssemblyLoader.GetPathMatchingGlob(pattern);

            Assert.IsNotNull(files);
            Assert.IsTrue(files.Any(f => f.FullName.Equals(Path.Combine(root, "FluentMigrator.dll"))));
        }
    }
}