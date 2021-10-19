using System;
using System.Windows.Input;

namespace MrErsh.RadioRipper.Client.Mvvm
{
    public sealed class EmptyCommand : ICommand, IHasLabel
    {
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) { }

        public string Label { get; init; }

        public static EmptyCommand Create(string label) => new EmptyCommand() { Label = label };
    }
}
