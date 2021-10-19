using Microsoft.UI.Xaml.Data;
using System;

namespace MrErsh.RadioRipper.Client.UI.Converters
{
    public abstract class ValueConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
        public virtual object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}
