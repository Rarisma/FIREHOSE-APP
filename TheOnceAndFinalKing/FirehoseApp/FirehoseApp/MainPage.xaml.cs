using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.Services;
using FirehoseApp.Views.Models;

namespace FirehoseApp;

public sealed partial class MainPage : Page
{
    private ShellVM ShellVM = Ioc.Default.GetService<ShellVM>();

    public MainPage()
    {
        InitializeComponent();
        
        //Load publication and article data
        ShellVM.LoadPublicationDataCommand.Execute(this);
        ShellVM.LoadArticleDataCommand.Execute(this);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        ShellVM.LoadArticleDataCommand.Execute(this);
    }
}
