using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FMStudio.Lib.Utility
{
    public static class MigrationsAssemblyLoader
    {
        public static Assembly Load(string path)
        {
            Debug.WriteLine(string.Format("Loading file '{0}'...", path));

            // Dll's can be loaded directly
            if (Path.GetExtension(path).Equals(".dll", StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine("File is a dll, attempting to load...");

                var bytes = File.ReadAllBytes(path);

                return LoadAssembly(bytes);
            }

            // Try to unpack the file if it is not a dll
            Debug.WriteLine("Could not load the file directly, trying to unpack as an archive...");

            var assembly = LoadArchive(path);
            if (assembly != null)
                return assembly;

            // We could add more types here
            return null;
        }

        public static Assembly LoadArchive(string path)
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

                                var assembly = LoadAssembly(bytes);

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
            catch (Exception e)
            {
            }

            return null;
        }

        public static Assembly LoadAssembly(byte[] bytes)
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
            catch (Exception e)
            {
            }

            return null;
        }

        public static IEnumerable<FileInfo> GetPathMatchingGlob(string fullPath)
        {
            var path = Path.GetDirectoryName(fullPath);
            var glob = Path.GetFileName(fullPath);
            
            var files = Directory.GetFiles(path);

            var result = files
                .Where(f => Path.GetFileName(f).Like(glob))
                .Select(f => new FileInfo(f))
                .OrderBy(f => f.LastWriteTime)
                .ToList()
            ;

            return result;
        }
    }
}