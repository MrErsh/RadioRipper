using System.Windows;

namespace MrErsh.RadioRipper.Client
{
    public partial class AddStationWindow : Window
    {
        public AddStationWindow() => InitializeComponent();

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
