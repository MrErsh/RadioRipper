using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MrErsh.RadioRipper.Client.UI.Converters
{
    public sealed class SimpleCaseToAnythingConverter : DependencyObject, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cas = Cases.FirstOrDefault(c => c.Key.ToString() == value.ToString());
            return cas == null ? Default : cas?.Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();

        public List<Case> Cases { get; set; } = new List<Case>();

        public object Default { get; set; }
    }

    public sealed class Case
    {
        public object Key { get; set; }

        public object Value { get; set; }
    }
}
