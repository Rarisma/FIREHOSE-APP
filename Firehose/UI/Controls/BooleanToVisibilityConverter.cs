using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace FirehoseApp.UI.Controls;
public class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool booleanValue)
        {
            return booleanValue ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is Visibility visibilityValue)
        {
            return visibilityValue == Visibility.Visible;
        }
        return false;
    }
}
