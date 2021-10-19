using JetBrains.Annotations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MrErsh.RadioRipper.Client.Dialogs;
using MrErsh.RadioRipper.Client.Dialogs.Content;
using MrErsh.RadioRipper.Client.Mvvm;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MrErsh.RadioRipper.Client.Services
{
    public class DialogService : IDialogService, IMessageService
    {
        private readonly IThreadingService _threadingService;
        private readonly ILogger _logger;

        private readonly Lazy<XamlRoot> _root;
        private XamlRoot Root => _root.Value;

        public DialogService(IThreadingService threadingService, ILogger logger, Func<XamlRoot> rootFactory)
        {
            _threadingService = threadingService;
            _logger = logger;
            _root = new Lazy<XamlRoot>(rootFactory);
        }

        #region Implementation of IDialogService

        public async Task<(bool, TViewModel)> ShowDialogAsync<TViewModel>(
            Type dialogType,
            Func<TViewModel, ICommand> primaryCommandFactory = null,
            Func<TViewModel, ICommand> secondaryCommandFactory = null,
            Action<TViewModel> vmInitializator = null)
            where TViewModel : class
        {
            var view = Activator.CreateInstance(dialogType);
            var dc = (view as Page).DataContext;
            if (dc is not TViewModel vm)
            {
                _logger.Error("Type mismatch. Expected: {ExpectedViewModel}, actual: unknown",
                              typeof(TViewModel).Name);
                //Application.Current.Exit();
                throw new ViewModelTypeMismatchException<TViewModel>(dc?.GetType());
            }

            vmInitializator?.Invoke(vm);

            var title = (vm as IHasLabel)?.Label ?? string.Empty;
            var dlg = CreateDialog(title, view);
            dlg.DefaultButton = ContentDialogButton.Primary;

            var pCommand = primaryCommandFactory?.Invoke(vm);
            dlg.PrimaryButtonCommand = pCommand;
            dlg.PrimaryButtonText = (pCommand as IHasLabel)?.Label ?? Localization.OK;

            var sCommand = secondaryCommandFactory?.Invoke(vm);
            if (sCommand == null)
            {
                dlg.IsSecondaryButtonEnabled = false;
            }
            else
            {
                dlg.SecondaryButtonCommand = sCommand;
                dlg.SecondaryButtonText = (sCommand as IHasLabel)?.Label ?? Localization.CANCEL;
            }

            var result = await WaitResponseAsync(dlg);
            return (result, vm);
        }

        #endregion

        #region Implementation of IMessageService

        public async Task<bool> ShowMessageAsync(string title, string message)
        {
            var content = new MessageView() { Message = message };
            var dlg = CreateDialog(title, content);

            dlg.DefaultButton = ContentDialogButton.Primary;
            dlg.PrimaryButtonText = Localization.OK;

            return await WaitResponseAsync(dlg);
        }

        public async Task ShowErrorAsync(string title, string text)
        {
            await _threadingService.OnUiThreadAsync(async () =>
            {
                var content = new ErrorView() { DataContext = text };
                var dlg = CreateDialog(title, content);

                dlg.DefaultButton = ContentDialogButton.Close;
                dlg.CloseButtonText = Localization.CLOSE;

                await dlg.ShowAsync();
            });
        }

        #endregion

        #region Private methods

        private ContentDialog CreateDialog(string title, object content)
        {
            var dlg = new ContentDialog
            {
                Title = title,
                Content = content
            };

            // WinUI wtf https://github.com/microsoft/microsoft-ui-xaml/issues/2504
            dlg.XamlRoot = Root; // Не вносить в инициализатор ContentDialog !!!  

            return dlg;
        }

        private static async Task<bool> WaitResponseAsync([NotNull] ContentDialog dialog)
        {
            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }

        #endregion
    }
}
