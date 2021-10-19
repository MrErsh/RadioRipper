using Microsoft.UI.Xaml.Controls;
using PropertyChanged;
using System;
using System.Windows.Input;

namespace MrErsh.RadioRipper.Client.Model
{
    [AddINotifyPropertyChangedInterface]
    public abstract class NavItemBase
    {
        public string Label { get; init; }
    }

    public sealed class PageNavItem : NavItemBase
    {
        public Symbol Symbol { get; init; }

        public Type Page { get; init; }
    }

    public sealed class CommandNavItem : NavItemBase
    {
        public ICommand Command { get; init; }
    }
}
