using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MrErsh.RadioRipper.Client.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UIDemo
{
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();

            ShowDialogCommand = new DependentCommand(this, ShowDialog, () => true);

            var today = DateTime.Today;
            Dates = new[]
            {
                today.AddDays(-1),
                today.AddDays(1)
            };

            Toggle.AsyncCommand = new ExecutionCommand<bool>(DelayBool);
        }

        public ICollection<DateTime> Dates { get; set; }

        public ICommand ShowDialogCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnMyButtonClick(object sender, RoutedEventArgs e) => ShowDialog();

        private async void ShowDialog()
        {
            var dialog = new ContentDialog
            {
                Content = "The font size must be a number between 8 and 100.",
                CloseButtonText = "Close",
                DefaultButton = ContentDialogButton.Close
            };

            dialog.XamlRoot = Content.XamlRoot;

            await dialog.ShowAsync();
        }

        private bool _state;
        private async Task<bool> DelayBool()
        {
            await Task.Delay(1000);
            _state = IsSuccessCheckBox.IsChecked == true ? !_state : _state;
            return _state;
        }


        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            Debug.WriteLine(args.SelectedItem);
        }
    }
}
