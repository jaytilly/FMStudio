using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FMStudio.Lib.Repositories
{
    public class AssemblyRepository : IAssemblyRepository
    {
        public Task<AssemblyName> GetReferenceByName(Assembly assembly, string referenceName)
        {
            return Task.Run(() =>
            {
                return assembly.GetReferencedAssemblies().FirstOrDefault(a => a.Name.Equals(referenceName, StringComparison.OrdinalIgnoreCase));
            });
        }

        public Task<Assembly> LoadFromArchive(string path)
        {
            return Task.Run(() =>
            {
                try
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();

                    using (var stream = File.OpenRead(path))
                    using (var zf = new ZipFile(stream))
                    {
                        foreach (ZipEntry entry in zf)
                        {
                            if (entry.IsFile && entry.Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                            {
                                Debug.WriteLine(string.Format("Found dll '{0}', trying to load...", Path.GetFileName(entry.Name)));

                                using (var entryStream = zf.GetInputStream(entry))
                                using (var ms = new MemoryStream())
                                {
                                    entryStream.CopyTo(ms);
                                    var bytes = ms.ToArray();

                                    var assembly = LoadFromBytes(bytes);

                                    if (assembly != null)
                                    {
                                        stopwatch.Stop();

                                        Debug.WriteLine(string.Format("Loaded assembly from an archive in {0}ms", stopwatch.ElapsedMilliseconds));

                                        return assembly;
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }

                return null;
            });
        }

        public Task<Assembly> LoadFromBytes(byte[] bytes)
        {
            return Task.Run(() =>
            {
                Assembly assembly = null;

                try
                {
                    assembly = Assembly.Load(bytes);

                    if (assembly.GetReferencedAssemblies().Any(r => r.Name.Equals("FluentMigrator", StringComparison.OrdinalIgnoreCase)))
                    {
                        return assembly;
                    }
                }
                catch { }

                return null;
            });
        }

        public Task<Assembly> LoadFromFile(string path)
        {
            return Task.Run(() =>
            {
                Debug.WriteLine(string.Format("Loading file '{0}'...", path));

                // Dll's can be loaded directly
                if (Path.GetExtension(path).Equals(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.WriteLine("File is a dll, attempting to load...");

                    var bytes = File.ReadAllBytes(path);

                    return LoadFromBytes(bytes);
                }

                // Try to unpack the file if it is not a dll
                Debug.WriteLine("Could not load the file directly, trying to unpack as an archive...");

                var assembly = LoadFromArchive(path);
                if (assembly != null)
                    return assembly;

                // We could add more types here
                return null;
            });
        }
    }
}