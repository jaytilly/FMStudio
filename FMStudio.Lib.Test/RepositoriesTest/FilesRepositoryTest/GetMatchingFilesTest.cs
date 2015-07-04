using FMStudio.Lib.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace FMStudio.Lib.Test.RepositoriesTest.FilesRepositoryTest
{
    [TestClass]
    public class GetMatchingFilesTest
    {
        [TestMethod]
        public void FilesRepository_GetMatchingFiles()
        {
            var repository = new FilesRepository();

            var root = Path.GetFullPath(".");

            var pattern = Path.Combine(root, @"*Migrator.dll");

            var files = repository.GetMatchingFiles(pattern).Result;

            Assert.IsNotNull(files);
            Assert.IsTrue(files.Any(f => f.FullName.Equals(Path.Combine(root, "FluentMigrator.dll"))));
        }
    }
}