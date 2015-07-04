using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FMStudio.Lib.Repositories
{
    public interface IFilesRepository
    {
        Task<IEnumerable<FileInfo>> GetMatchingFiles(string path);

        Task<FileInfo> GetMostRecentMatchingFile(string path);
    }
}