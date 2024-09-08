using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace SigningTool.Converters
{
    public class KeyEventArgsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is KeyEventArgs keyEventArgs)
            {
                return keyEventArgs; // Pass the whole KeyEventArgs object.
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
