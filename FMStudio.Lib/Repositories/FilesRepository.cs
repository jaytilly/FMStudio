using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FMStudio.Lib.Repositories
{
    public class FilesRepository : IFilesRepository
    {
        public Task<IEnumerable<FileInfo>> GetMatchingFiles(string path)
        {
            return Task.Run(() =>
            {
                var directory = Path.GetDirectoryName(path);
                var glob = Path.GetFileName(path);

                var files = Directory.GetFiles(directory);

                var result = files
                    .Where(f => Path.GetFileName(f).Like(glob))
                    .Select(f => new FileInfo(f))
                    .OrderBy(f => f.LastWriteTime)
                    .ToList()
                ;

                return (IEnumerable<FileInfo>)result;
            });
        }

        public async Task<FileInfo> GetMostRecentMatchingFile(string fullPath)
        {
            var matchingFiles = await GetMatchingFiles(fullPath);

            return matchingFiles.OrderByDescending(f => f.LastWriteTime).FirstOrDefault();
        }
    }
}