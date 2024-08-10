using System.Drawing;
using Microsoft.UI.Xaml.Data;
namespace FirehoseApp.UI.Controls;
public class FHNColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is Color drawingColor)
        {
            // Convert System.Drawing.Color to Windows.UI.Color
            var winUIColor = Windows.UI.Color.FromArgb(drawingColor.A,
                drawingColor.R, drawingColor.G, drawingColor.B);
            
            // Create and return a SolidColorBrush
            return new SolidColorBrush(winUIColor);
        }
        
        // If value is not a Color, return a default or transparent brush
        return new SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return null;
    }
}
