using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.Services;
using FirehoseApp.Views.Models;

namespace FirehoseApp;

public sealed partial class MainPage : Page
{
    private ShellVM ShellVM = Ioc.Default.GetService<ShellVM>();
    private ThemeVM ThemeVM = Ioc.Default.GetService<ThemeVM>();

    public MainPage()
    {
        InitializeComponent();
        ShellVM.LoadPublicationDataCommand.Execute(this);
        ShellVM.LoadArticleDataCommand.Execute(this);
    }

    private void AppBarButton_Click(object sender, RoutedEventArgs e)
    {
        //Deselect all buttons visually.
        foreach (AppBarButton button in TopBar.PrimaryCommands)
        {
            button.Foreground = ThemeVM.MainBrush;
            button.Background = ThemeVM.SecondaryBrush;
        }

        //Color Selected button inversely
        AppBarButton Button = sender as AppBarButton;
        Button.Foreground = ThemeVM.SecondaryBrush;
        Button.Background = ThemeVM.MainBrush;


    }
}
