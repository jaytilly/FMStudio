using FMStudio.Lib.Repositories;
using Ionic.Zip;
using System.IO;
using Xunit;

namespace FMStudio.Lib.Test.RepositoriesTest.AssemblyRepositoryTest
{
    public class LoadFromArchiveTest
    {
        private AssemblyRepository _sut;

        public LoadFromArchiveTest()
        {
            _sut = new AssemblyRepository();
        }

        [Theory]
        [ClassData(typeof(ContextProvider))]
        public void SingleFileInRoot(Context context)
        {
            // Arrange
            using (var zipFile = new ZipFile())
            using (var stream = new MemoryStream())
            {
                zipFile.AddFile(context.FMTestMigrationsPath);
                zipFile.Save(stream);

                var bytes = stream.ToArray();

                // Act
                var result = _sut.LoadFromArchive(bytes).Result;

                // Assert
                Assert.NotNull(result);
                Assert.StartsWith(context.FMTestMigrationsFullName, result.FullName);
            }
        }

        [Theory]
        [ClassData(typeof(ContextProvider))]
        public void SingleFileInSubdirectory(Context context)
        {
            // Arrange
            var subdirectoryName = "Subdirectory";

            using (var zipFile = new ZipFile())
            using (var stream = new MemoryStream())
            {
                zipFile.AddDirectoryByName(subdirectoryName);
                zipFile.AddFile(context.FMTestMigrationsPath, subdirectoryName);
                zipFile.Save(stream);

                var bytes = stream.ToArray();

                // Act
                var result = _sut.LoadFromArchive(bytes).Result;

                // Assert
                Assert.NotNull(result);
                Assert.StartsWith(context.FMTestMigrationsFullName, result.FullName);
            }
        }

        [Theory]
        [ClassData(typeof(ContextProvider))]
        public void IncludingFluentMigratorDll(Context context)
        {
            // Arrange
            using (var zipFile = new ZipFile())
            using (var stream = new MemoryStream())
            {
                zipFile.AddFile(context.FMDllPath);
                zipFile.AddFile(context.FMTestMigrationsPath);
                zipFile.Save(stream);

                var bytes = stream.ToArray();

                // Act
                var result = _sut.LoadFromArchive(bytes).Result;

                // Assert
                Assert.NotNull(result);
                Assert.StartsWith(context.FMTestMigrationsFullName, result.FullName);
            }
        }

        [Theory]
        [ClassData(typeof(ContextProvider))]
        public void OnlyIncludingFluentMigratorDll(Context context)
        {
            // Arrange
            using (var zipFile = new ZipFile())
            using (var stream = new MemoryStream())
            {
                zipFile.AddFile(context.FMDllPath);
                zipFile.Save(stream);

                var bytes = stream.ToArray();

                // Act
                var result = _sut.LoadFromArchive(bytes).Result;

                // Assert
                Assert.Null(result);
            }
        }

        [Theory]
        [ClassData(typeof(ContextProvider))]
        public void IncludingFluentMigratorRunnerDll(Context context)
        {
            // Arrange
            using (var zipFile = new ZipFile())
            using (var stream = new MemoryStream())
            {
                zipFile.AddFile(context.FMRunnerDllPath);
                zipFile.AddFile(context.FMTestMigrationsPath);
                zipFile.Save(stream);

                var bytes = stream.ToArray();

                // Act
                var result = _sut.LoadFromArchive(bytes).Result;

                // Assert
                Assert.NotNull(result);
                Assert.StartsWith(context.FMTestMigrationsFullName, result.FullName);
            }
        }

        [Theory]
        [ClassData(typeof(ContextProvider))]
        public void OnlyIncludingFluentMigratorRunnerDll(Context context)
        {
            // Arrange
            using (var zipFile = new ZipFile())
            using (var stream = new MemoryStream())
            {
                zipFile.AddFile(context.FMRunnerDllPath);
                zipFile.Save(stream);

                var bytes = stream.ToArray();

                // Act
                var result = _sut.LoadFromArchive(bytes).Result;

                // Assert
                Assert.Null(result);
            }
        }

        [Theory]
        [ClassData(typeof(ContextProvider))]
        public void NotReferencingFluentMigratorDll(Context context)
        {
            // Arrange
            using (var zipFile = new ZipFile())
            using (var stream = new MemoryStream())
            {
                zipFile.AddFile(context.FMUtilityDllPath);
                zipFile.Save(stream);

                var bytes = stream.ToArray();

                // Act
                var result = _sut.LoadFromArchive(bytes).Result;

                // Assert
                Assert.Null(result);
            }
        }

        [Theory]
        [ClassData(typeof(ContextProvider))]
        public void NotAZipFile(Context context)
        {
            // Arrange
            var bytes = File.ReadAllBytes(context.FMUtilityDllPath);

            // Act
            var result = _sut.LoadFromArchive(bytes).Result;

            // Assert
            Assert.Null(result);
        }
    }
}