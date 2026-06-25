using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace syc_pm_client.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string s && !string.IsNullOrWhiteSpace(s))
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
