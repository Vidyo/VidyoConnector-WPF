using System;
using System.Globalization;
using System.Windows.Data;
using VidyoConnector.Model;

namespace VidyoConnector.Converters
{
    public class ConnectionStateToEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool retval = false;
            if (value is ConnectionState)
            {
                switch ((ConnectionState)value)
                {
                    case ConnectionState.Connected:
                        retval = true;
                        break;
                    case ConnectionState.NotConnected:
                    case ConnectionState.OperationInProgress:
                    case ConnectionState.Undefined:
                        retval = false;
                        break;
                }
            }

            return retval;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
