using MrErsh.RadioRipper.Client.ViewModels;
using System.Windows;

namespace MrErsh.RadioRipper.Client.Dialogs
{
    public partial class LoginDialog : Window
    {
        public LoginDialog()
        {
            InitializeComponent();
        }

        // pff
        public LoginViewModel VM => (LoginViewModel)DataContext;

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
