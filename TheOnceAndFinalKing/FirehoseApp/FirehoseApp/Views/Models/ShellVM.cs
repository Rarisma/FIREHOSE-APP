using FirehoseApp.Controls;
using FirehoseApp.Services;

namespace FirehoseApp.Views.Models;

class ShellVM : ObservableObject
{

    public LoadableCommand LoadPublicationDataCommand { get; }
    public LoadableCommand LoadArticleDataCommand { get; }

    public ShellVM()
    {
        LoadPublicationDataCommand = new LoadableCommand(LoadPublicationData);
        LoadArticleDataCommand = new LoadableCommand(LoadArticleData);

    }

    async Task LoadArticleData()
    {
        await new HYDRANT.API(AppState.APIKey).GetArticles(9999);

    }

    async Task LoadPublicationData()
    {
        await new HYDRANT.API(AppState.APIKey).GetPublications();
    }
}
