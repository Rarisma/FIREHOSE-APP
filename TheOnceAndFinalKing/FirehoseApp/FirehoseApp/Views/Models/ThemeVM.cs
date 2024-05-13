using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI;

namespace FirehoseApp.Views.Models;

public class ThemeVM : ObservableObject
{
    /// <summary>
    /// This is the main color used in the UI
    /// for things like the background
    /// (Black on Dark Theme and White on Light Theme)
    /// </summary>
    public SolidColorBrush MainBrush { get; set; }

    /// <summary>
    /// This is the secondary color used in the UI
    /// for things like selected buttons and text
    /// (white on dark theme and Black on light theme
    /// </summary>
    public SolidColorBrush SecondaryBrush { get; set; }

    public ThemeVM() => UpdateColors();

    void UpdateColors()
    {
        if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
        {
            MainBrush = new SolidColorBrush(Colors.Black);
            SecondaryBrush = new SolidColorBrush(Colors.White);
        }
        else
        {
            MainBrush = new SolidColorBrush(Colors.White);
            SecondaryBrush = new SolidColorBrush(Colors.Black);
        }
    }
}
