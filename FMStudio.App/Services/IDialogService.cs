using System.Threading.Tasks;

namespace FMStudio.App.Services
{
    public interface IDialogService
    {
        Task<bool> ConfirmAsync(string message);

        Task<bool> ConfirmAsync(string title, string message);

        Task<string> PromptAsync(string title, string message);

        Task<string> SaveDialogAsync(string defaultExtension, string filter, string fileName);

        Task<string> OpenDialogAsync(string defaultExtension, string filter);

        Task<object> ProgressAsync(string title, string message);

        Task CloseProgressAsync(object progress);
    }
}