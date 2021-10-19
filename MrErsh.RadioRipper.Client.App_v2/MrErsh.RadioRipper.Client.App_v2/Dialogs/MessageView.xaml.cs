using Microsoft.UI.Xaml.Controls;

namespace MrErsh.RadioRipper.Client.Dialogs.Content
{
    public sealed partial class MessageView : Page
    {
        public MessageView()
        {
            this.InitializeComponent();
        }

        public string Message { get; set; }
    }
}
