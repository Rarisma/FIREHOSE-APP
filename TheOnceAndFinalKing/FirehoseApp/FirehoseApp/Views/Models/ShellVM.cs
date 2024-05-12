using FirehoseApp.Controls;
using FirehoseApp.Services;

namespace FirehoseApp.Views.Models;

class ShellVM : ObservableObject
{

    public LoadableCommand LoadPublicationDataCommand { get; }
    public LoadableCommand LoadArticleDataCommand { get; }
    public string LoadingText { get; set; }

    public ShellVM()
    {
        LoadPublicationDataCommand = new LoadableCommand(LoadPublicationData);
        LoadArticleDataCommand = new LoadableCommand(LoadArticleData);

    }

    async Task LoadArticleData()
    {
        LoadingText = "Loading Article data...";
        await new HYDRANT.API(AppState.APIKey).GetArticles(9999);
        LoadingText = "Loaded Article data";
    }

    async Task LoadPublicationData()
    {
        LoadingText = "Loading Publication data...";
        await new HYDRANT.API(AppState.APIKey).GetPublications();
        LoadingText = "Loaded Publication data";
    }
}
