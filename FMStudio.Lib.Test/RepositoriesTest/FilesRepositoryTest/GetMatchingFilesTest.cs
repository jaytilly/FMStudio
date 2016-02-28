using FMStudio.Lib.Repositories;
using System.IO;
using System.Linq;
using Xunit;

namespace FMStudio.Lib.Test.RepositoriesTest.FilesRepositoryTest
{
    public class GetMatchingFilesTest
    {
        [Fact]
        public void GetMatchingFiles()
        {
            var repository = new FilesRepository();

            var root = Path.GetFullPath(".");

            var pattern = Path.Combine(root, @"*Migrator.dll");

            var files = repository.GetMatchingFiles(pattern).Result;

            Assert.NotNull(files);
            Assert.True(files.Any(f => f.FullName.Equals(Path.Combine(root, "FluentMigrator.dll"))));
        }
    }
}