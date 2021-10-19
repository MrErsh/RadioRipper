using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MrErsh.RadioRipper.Client.App_v2
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public Frame RootFrame => ContentFrame;
    }
}
