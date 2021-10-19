using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;

namespace MrErsh.RadioRipper.Client.UI.Markup
{
    public class VisibleOnDebug : MarkupExtension
    {
        protected override object ProvideValue()
#if DEBUG
                        => Visibility.Visible;
#else
                        => Visibility.Collapsed;
#endif

        protected override object ProvideValue(IXamlServiceProvider serviceProvider) => ProvideValue();
    }
}
