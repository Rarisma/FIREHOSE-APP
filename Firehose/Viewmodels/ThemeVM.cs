using Windows.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI;

namespace FirehoseApp.Viewmodels;

/// <summary>
/// Controls application color and theming
/// </summary>
public class ThemeVM : ObservableObject
{
    /// <summary>
    /// This is the main color used in the UI
    /// for things like the background
    /// (Black on Dark Theme and White on Light Theme)
    /// </summary>
    public SolidColorBrush MainBrush { get; set; } = null!;
    
    /// <summary>
    /// This is the secondary color used in the UI
    /// for things like selected buttons and text
    /// (white on dark theme and Black on light theme)
    /// </summary>
    public SolidColorBrush SecondaryBrush { get; set; } = null!;
    
    /// <summary>
    /// This is the accent color used in the UI
    /// for distinguishing backgrounds on the main brush
    /// (Lighter gray on light theme or Lighter black on dark theme)
    /// </summary>
    public SolidColorBrush TertiaryBrush { get; set; } = null!;
    
    /// <summary>
    /// Current app theme
    /// </summary>
    public StatusBarForegroundTheme StatusBar { get; set; }


    public ThemeVM() => UpdateColors();


    /// <summary>
    /// Updates all colors.
    /// </summary>
    void UpdateColors()
    {
        if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
        {
            MainBrush = new SolidColorBrush(Colors.Black);
            SecondaryBrush = new SolidColorBrush(Colors.White);
            TertiaryBrush = new SolidColorBrush(Color.FromArgb(50, 0, 0, 0));
            StatusBar = StatusBarForegroundTheme.Light;
        }
        else
        {
            MainBrush = new SolidColorBrush(Colors.White);
            SecondaryBrush = new SolidColorBrush(Colors.Black);
            TertiaryBrush = new SolidColorBrush(Color.FromArgb(50, 0, 0, 0));
            StatusBar = StatusBarForegroundTheme.Dark;
        }
    }
}
