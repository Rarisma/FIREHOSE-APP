using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseNews.Viewmodels;
using Microsoft.UI;

namespace FirehoseNews.UI.Controls;
public sealed partial class Filters : AppBarButton
{
    ThemeVM Themer = Ioc.Default.GetRequiredService<ThemeVM>();

    /// <summary>
    /// Visible display name
    /// </summary>
    public string DisplayName { get; set; }
    /// <summary>
    /// MySQL filter that will be used to filter articles
    /// </summary>
    public string SQLFilter { get; set; }
    /// <summary>
    /// Order by
    /// </summary>
    public string FilterOrder { get; set; }
    
    /// <summary>
    /// UI button for filters
    /// </summary>
    /// <param name="name"> Visible name to the user</param>
    /// <param name="filter">MySQL filtering rule</param>
    /// <param name="order">Order by, defaults to descending</param>
    public Filters(string name, string filter, string order = "ORDER BY PUBLISH_DATE DESC")
    {
        DisplayName = name;
        SQLFilter = filter;
        FilterOrder = order;
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
        if (ShellVM.FilterBy == f.SQLFilter)
        {
            //If the same button has been clicked on load older articles
            ShellVM.Offset += Glob.Model.ArticleFetchLimit;
        }
        else
        {
            ShellVM.FilterBy = f.SQLFilter;
            ShellVM.FilterOrder = f.FilterOrder;
            ShellVM.Offset = 0;
        }

        ShellVM.UpdateButtonsDelegate.Invoke(this);
        ShellVM.LoadArticleDataCommand.Execute(null);
    }
}
