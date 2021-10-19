using System;
using System.Windows;
using System.Windows.Markup;

namespace MrErsh.RadioRipper.Client.Infrastructure.Markup
{
    // generic?
    public class VisibleOnDebug : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
#if DEBUG
            => Visibility.Visible;
#else
            => Visibility.Collapsed;
#endif
    }
}
