using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System.Threading.Tasks;

namespace FMStudio.App.Services
{
    public class DialogService : IDialogService
    {
        private MetroWindow _window;

        public DialogService(MetroWindow window)
        {
            _window = window;
        }

        public Task<bool> ConfirmAsync(string message)
        {
            return ConfirmAsync("Confirmation", message);
        }

        public async Task<bool> ConfirmAsync(string title, string message)
        {
            var settings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Yes",
                AnimateHide = false,
                AnimateShow = false,
                NegativeButtonText = "No"
            };

            var result = await _window.ShowMessageAsync(title, message, MessageDialogStyle.AffirmativeAndNegative, settings);

            return result == MessageDialogResult.Affirmative;
        }

        public async Task<string> PromptAsync(string title, string message)
        {
            var settings = new MetroDialogSettings()
            {
                AnimateHide = false,
                AnimateShow = false
            };

            var result = await _window.ShowInputAsync(title, message, settings);

            return result;
        }

        public Task<string> SaveDialogAsync(string defaultExtension, string filter, string fileName)
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = fileName;
            dialog.DefaultExt = defaultExtension;
            dialog.Filter = filter;

            var result = dialog.ShowDialog();

            if (result == true)
            {
                return Task.FromResult(dialog.FileName);
            }

            return Task.FromResult<string>(null);
        }

        public Task<string> OpenDialogAsync(string defaultExtension, string filter)
        {
            var dialog = new OpenFileDialog();
            dialog.FileName = "";
            dialog.DefaultExt = defaultExtension;
            dialog.Filter = filter;

            var result = dialog.ShowDialog();

            if (result == true)
            {
                return Task.FromResult(dialog.FileName);
            }

            return Task.FromResult<string>(null);
        }

        public async Task<object> ProgressAsync(string title, string message)
        {
            var settings = new MetroDialogSettings()
            {
                AnimateHide = false,
                AnimateShow = false
            };

            return await _window.ShowProgressAsync(title, message, false, settings);
        }

        public async Task CloseProgressAsync(object progress)
        {
            await ((ProgressDialogController)progress).CloseAsync();
        }
    }
}