using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using LogProcessor;
using System.Windows.Media;
namespace LogProcessorW.Converters
{
    [ValueConversion(typeof(string), typeof(Brush))]
    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = value as string;
            Brush brush = Brushes.Black;
            switch (s)
            {
                case Constants.passCharString:
                    brush = Brushes.Green;
                    break;
                case Constants.failCharString:
                    brush = Brushes.Red;
                    break;
                case Constants.errorCharString:
                    brush = Brushes.Brown;
                    break;
            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
