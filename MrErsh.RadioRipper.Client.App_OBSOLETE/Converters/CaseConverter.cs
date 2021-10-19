using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace MrErsh.RadioRipper.Client.Converters
{
    public sealed class VerySimpleEnumToObjectConverter : DependencyObject, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cas = Cases.FirstOrDefault(c => c.Key.ToString() == value.ToString());
            return cas == null ? Default : cas?.Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public List<Case> Cases { get; set; } = new List<Case>();

        public Object Default { get; set; }
    }

    public sealed class Case
    {
        public object Key { get; set; }

        public object Value { get; set; }
    }
}
