using Microsoft.UI.Xaml;
using System;

namespace MrErsh.RadioRipper.Client.UI.Converters
{
    public sealed class NavItemVisbilityConverter : ValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            var isAuthorized = (bool)value;
            var bParam = parameter as bool?;

            if (isAuthorized)
            {
                if (bParam == false)
                    return Visibility.Collapsed;
            }
            else
            {
                if (bParam == true)
                    return Visibility.Visible;
            }

            return Visibility.Visible;
        }
    }
}
