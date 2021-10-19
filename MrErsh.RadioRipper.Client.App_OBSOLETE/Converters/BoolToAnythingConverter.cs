
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MrErsh.RadioRipper.Client.Converters
{
    public class BoolToAnythingConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty TrueValueProperty = DependencyProperty.Register(
            nameof(TrueValue), typeof(object), typeof(BoolToAnythingConverter), null);

        public static readonly DependencyProperty FalseValueProperty = DependencyProperty.Register(
            nameof(FalseValue), typeof(object), typeof(BoolToAnythingConverter), null);

        public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register(
            nameof(DefaultValue), typeof(object), typeof(BoolToAnythingConverter), null);

        public object TrueValue
        {
            get { return GetValue(TrueValueProperty); }
            set { SetValue(TrueValueProperty, value); }
        }

        public object FalseValue
        {
            get { return GetValue(FalseValueProperty); }
            set { SetValue(FalseValueProperty, value); }
        }

        public object DefaultValue
        {
            get { return GetValue(DefaultValueProperty); }
            set { SetValue(DefaultValueProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return boolValue ? TrueValue : FalseValue;

            return DefaultValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return boolValue;

            throw new NotImplementedException();
        }
    }
}
