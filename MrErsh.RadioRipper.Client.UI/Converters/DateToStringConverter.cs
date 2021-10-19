using System;

namespace MrErsh.RadioRipper.Client.UI.Converters
{
    public class DateFormatConverter : ValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(parameter is string format))
                throw new ArgumentNullException(nameof(parameter));

            if (!(value is DateTime datetime))
                return null;

            return datetime.ToString(format);
        }
    }
}
