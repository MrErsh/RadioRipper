using MrErsh.RadioRipper.Client.Shared;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace MrErsh.RadioRipper.Client.Infrastructure
{
    public sealed class MessageService : IMessageService
    {
        public void ShowError(string caption, string text)
        {// TODO VE: uithread
            MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public Task<bool?> ShowDialogAsync(string title, string primaryButtonText = "Ok", string closeButtonText = "Cancel")
         => throw new NotImplementedException();
    }
}
