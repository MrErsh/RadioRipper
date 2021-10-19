using Microsoft.UI.Xaml;
using System;

namespace MrErsh.RadioRipper.Client.UI.Converters
{
    public sealed class DefaultToVisibilityConverter : ValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            return value == default
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
    }
}
