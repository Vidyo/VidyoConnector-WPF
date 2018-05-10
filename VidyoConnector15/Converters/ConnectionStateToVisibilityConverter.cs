using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using VidyoConnector.Model;

namespace VidyoConnector.Converters
{
    /// <summary>
    /// Represents converting from ConnectionState to Visibility enumeration.
    /// May be used as an example.
    /// </summary>
    public class ConnectionStateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var retVal = default(Visibility);
            if (value is ConnectionState)
            {
                switch ((ConnectionState)value)
                {
                    case ConnectionState.Connected:
                    case ConnectionState.NotConnected:
                    case ConnectionState.Undefined:
                        retVal = Visibility.Visible;
                        break;
                    case ConnectionState.OperationInProgress:
                        retVal = Visibility.Collapsed;
                        break;
                }
            }

            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}