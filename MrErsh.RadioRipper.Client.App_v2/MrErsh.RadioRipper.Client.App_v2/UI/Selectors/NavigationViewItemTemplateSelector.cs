using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MrErsh.RadioRipper.Client.Model;

namespace MrErsh.RadioRipper.Client.UI.Selectors
{
    public sealed class NavigationViewItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PageItemDataTemplate { get; set; }

        public DataTemplate LogInItemDataTemplate { get; set; }

        public DataTemplate LogOutItemDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            switch (item)
            {
                case PageNavItem cItem when cItem.Label == Constants.NavItemLabels.LOGIN:
                    return LogInItemDataTemplate;
                case CommandNavItem cItem when cItem.Label == Constants.NavItemLabels.LOGOUT:
                    return LogOutItemDataTemplate;
                case PageNavItem:
                    return PageItemDataTemplate;
                default:
                    return null;
            }
        }
    }
}
