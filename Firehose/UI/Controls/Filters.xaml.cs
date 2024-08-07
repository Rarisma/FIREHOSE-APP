using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.Preferences;
using FirehoseApp.Viewmodels;
using HYDRANT.Definitions;
using Microsoft.UI;

namespace FirehoseApp.UI.Controls;
public sealed partial class Filters : Button
{
    ThemeVM Themer = Ioc.Default.GetRequiredService<ThemeVM>();

    /// <summary>
    /// Visible display name
    /// </summary>
    public string DisplayName { get; set; }
    
    /// <summary>
    /// UI button for filters
    /// </summary>
    /// <param name="name"> Visible name to the user</param>
    /// <param name="filter">MySQL filtering rule</param>
    /// <param name="order">Order by, defaults to descending</param>
    public Filters(Filter filter)
    {
        DisplayName = filter.Name;
        //FilterOrder = order;
        this.InitializeComponent();
    }

    private void Clicked(object sender, RoutedEventArgs e)
    {
        ShellVM ShellVM = Ioc.Default.GetRequiredService<ShellVM>();

        //Clear filter buttons
        foreach (var FilterButton in ShellVM.Filters)
        {
            FilterButton.Background = new SolidColorBrush(Colors.Transparent);
            FilterButton.Foreground = Themer.SecondaryBrush;
        }

        
        Background = Themer.MainBrush;
        Foreground = Themer.SecondaryBrush;
        
        Filters f = ShellVM.Filters.First(f => f.DisplayName == DisplayName);
        if (ShellVM.CurrentFilter == f.DisplayName)
        {
            //If the same button has been clicked on load older articles
            ShellVM.Offset += Ioc.Default.GetRequiredService<PreferencesModel>().ArticleFetchLimit;
        }
        else
        {
            ShellVM.Offset = 0;
            ShellVM.CurrentFilter = DisplayName;
        }

        ShellVM.UpdateButtonsDelegate.Invoke(this);
        ShellVM.LoadArticleDataCommand.Execute(null);
    }
}
