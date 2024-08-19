using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.Viewmodels;
using HYDRANT.Definitions;
//I hope your next dream is a more pleasant one
namespace FirehoseApp.UI.Controls;
public partial class FilterButton : Button
{
    private ThemeVM Themer = Ioc.Default.GetRequiredService<ThemeVM>();
    public FilterButton(string name)
    {
        Content = name;
        init();
    }
    public FilterButton(Filter filter)
    {
        Content = filter.Name;
        init();
    }
    
    public FilterButton()
    {
        init();
    }

    /// <summary>
    /// Initializes control.
    /// </summary>
    private void init()
    {
        Padding = new(10);
        Width = 100;
        Height = 50;
        CornerRadius = new(4);
        Margin = new(5, 0, 5, 0);
        BorderThickness = new Thickness(0);
        Unset();
        Ioc.Default.GetRequiredService<ShellVM>().UIFilters.Add(this);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Set()
    {
        Background = Themer.SecondaryBrush;
        Foreground = Themer.MainBrush;
    }

    /// <summary>
    /// Unsets the button
    /// </summary>
    public void Unset()
    {
        Foreground = Themer.SecondaryBrush;
        Background = Themer.MainBrush;
    }
}
