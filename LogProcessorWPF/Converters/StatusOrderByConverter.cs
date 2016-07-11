using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using LogProcessor;
using System.Windows.Media;
using System.Windows;
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