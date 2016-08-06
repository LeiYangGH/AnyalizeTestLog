using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
namespace LogProcessorWPF.Converters
{
    [ValueConversion(typeof(string), typeof(Brush))]
    public class StatusOrderByConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? isChecked = value as bool?;
            if (isChecked ?? false)
                return Brushes.Blue;
            else
                return Brushes.LightBlue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}