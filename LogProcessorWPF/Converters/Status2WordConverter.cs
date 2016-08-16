using System;
using System.Globalization;
using System.Windows.Data;
using LogProcessor;
using System.Windows.Media;
namespace LogProcessorWPF.Converters
{
    [ValueConversion(typeof(string), typeof(Brush))]
    public class Status2WordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = value as string;
            switch (s)
            {
                case Constants.passCharString:
                    return "Pass";
                case Constants.failCharString:
                    return "Fail";

                case Constants.errorCharString:
                    return "Error";
            }
            return "Unknown Status";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
