using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.Controls;
using FirehoseApp.Services;
using FirehoseApp.Views.Models;

namespace FirehoseApp;

public sealed partial class MainPage : Page
{
    private ShellVM ShellVM = Ioc.Default.GetRequiredService<ShellVM>();

    public async Task LoadPublicationData()
    {
        await new HYDRANT.API(AppState.APIKey).GetPublications();
        Thread.Sleep(5000);
    }

    public MainPage()
    {
        LoadPublicationDataCommand = new LoadableCommand(LoadPublicationData);
        this.InitializeComponent();
        //LoadContent1Command = new AsyncCommand(LoadContent1);
    }
}
