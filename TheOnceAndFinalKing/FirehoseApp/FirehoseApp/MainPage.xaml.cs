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
        ShellVM.LoadPublicationDataCommand.Execute(this);
        //LoadContent1Command = new AsyncCommand(LoadContent1);
    }
}
