using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace MrErsh.RadioRipper.Client.UI.Converters
{
    public class BoolToAnythingConverter : DependencyObject, IValueConverter
    {
        #region Dependency properties

        public static readonly DependencyProperty TrueValueProperty = DependencyProperty.Register(
            nameof(TrueValue),
            typeof(object),
            typeof(BoolToAnythingConverter),
            null);

        public static readonly DependencyProperty FalseValueProperty = DependencyProperty.Register(
            nameof(FalseValue),
            typeof(object),
            typeof(BoolToAnythingConverter),
            null);

        public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register(
            nameof(DefaultValue),
            typeof(object),
            typeof(BoolToAnythingConverter),
            null);

        #endregion

        #region Properties

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

        #endregion

        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue)
                return boolValue ? TrueValue : FalseValue;

            return DefaultValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue)
                return boolValue;

            throw new ArgumentException("Value should be boolean", nameof(value));
        }

        #endregion
    }
}
