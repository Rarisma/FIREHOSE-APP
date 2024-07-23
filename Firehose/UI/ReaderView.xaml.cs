using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.Viewmodels;
using HYDRANT;
using HYDRANT.Definitions;

namespace FirehoseApp.UI;

public sealed partial class ReaderMode : Page
{
    private Article Article;

    public ReaderMode(Article Data)
    {
        Article = Data;
        InitializeComponent();
    }
}
