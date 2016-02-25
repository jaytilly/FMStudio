using System.Threading.Tasks;

namespace FMStudio.Lib.DatabaseHandlers
{
    public interface IDatabaseHandler
    {
        Task Recreate(string connectionString);
    }
}
