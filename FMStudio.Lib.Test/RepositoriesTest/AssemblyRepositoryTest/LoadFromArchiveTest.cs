using FMStudio.Lib.Repositories;
using Ionic.Zip;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace FMStudio.Lib.Test.RepositoriesTest.AssemblyRepositoryTest
{
    [TestClass]
    public class LoadFromArchiveTest
    {
        private AssemblyRepository _sut;

        [TestInitialize]
        public void Initialize()
        {
            _sut = new AssemblyRepository();
        }

        [TestMethod]
        public void AssemblyRepository_LoadFromArchive_SingleFileInRoot()
        {
            // Arrange
            using (var zipFile = new ZipFile())
            using (var stream = new MemoryStream())
            {
                zipFile.AddFile(Constants.FMTestMigrationsPath);
                zipFile.Save(stream);

                var bytes = stream.ToArray();

                // Act
                var result = _sut.LoadFromArchive(bytes).Result;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(Constants.FMTestMigrationsFullName, result.FullName);
            }
        }

        [TestMethod]
        public void AssemblyRepository_LoadFromArchive_SingleFileInSubdirectory()
        {
            // Arrange
            var subdirectoryName = "Subdirectory";

            using (var zipFile = new ZipFile())
            using (var stream = new MemoryStream())
            {
                zipFile.AddDirectoryByName(subdirectoryName);
                zipFile.AddFile(Constants.FMTestMigrationsPath, subdirectoryName);
                zipFile.Save(stream);

                var bytes = stream.ToArray();

                // Act
                var result = _sut.LoadFromArchive(bytes).Result;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(Constants.FMTestMigrationsFullName, result.FullName);
            }
        }

        [TestMethod]
        public void AssemblyRepository_LoadFromArchive_IncludingFluentMigratorDll()
        {
            // Arrange
            using (var zipFile = new ZipFile())
            using (var stream = new MemoryStream())
            {
                zipFile.AddFile(Constants.FMDllPath);
                zipFile.AddFile(Constants.FMTestMigrationsPath);
                zipFile.Save(stream);

                var bytes = stream.ToArray();

                // Act
                var result = _sut.LoadFromArchive(bytes).Result;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(Constants.FMTestMigrationsFullName, result.FullName);
            }
        }

        [TestMethod]
        public void AssemblyRepository_LoadFromArchive_OnlyIncludingFluentMigratorDll()
        {
            // Arrange
            using (var zipFile = new ZipFile())
            using (var stream = new MemoryStream())
            {
                zipFile.AddFile(Constants.FMDllPath);
                zipFile.Save(stream);

                var bytes = stream.ToArray();

                // Act
                var result = _sut.LoadFromArchive(bytes).Result;

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void AssemblyRepository_LoadFromArchive_IncludingFluentMigratorRunnerDll()
        {
            // Arrange
            using (var zipFile = new ZipFile())
            using (var stream = new MemoryStream())
            {
                zipFile.AddFile(Constants.FMRunnerDllPath);
                zipFile.AddFile(Constants.FMTestMigrationsPath);
                zipFile.Save(stream);

                var bytes = stream.ToArray();

                // Act
                var result = _sut.LoadFromArchive(bytes).Result;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(Constants.FMTestMigrationsFullName, result.FullName);
            }
        }

        [TestMethod]
        public void AssemblyRepository_LoadFromArchive_OnlyIncludingFluentMigratorRunnerDll()
        {
            // Arrange
            using (var zipFile = new ZipFile())
            using (var stream = new MemoryStream())
            {
                zipFile.AddFile(Constants.FMRunnerDllPath);
                zipFile.Save(stream);

                var bytes = stream.ToArray();

                // Act
                var result = _sut.LoadFromArchive(bytes).Result;

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void AssemblyRepository_LoadFromArchive_NotReferencingFluentMigratorDll()
        {
            // Arrange
            using (var zipFile = new ZipFile())
            using (var stream = new MemoryStream())
            {
                zipFile.AddFile(Constants.FMUtilityDllPath);
                zipFile.Save(stream);

                var bytes = stream.ToArray();

                // Act
                var result = _sut.LoadFromArchive(bytes).Result;

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void AssemblyRepository_LoadFromArchive_NotAZipFile()
        {
            // Arrange
            var bytes = File.ReadAllBytes(Constants.FMUtilityDllPath);

            // Act
            var result = _sut.LoadFromArchive(bytes).Result;

            // Assert
            Assert.IsNull(result);
        }
    }
}