using System.Diagnostics;
using Windows.UI.Core;

namespace VESTIGENEWS;

public sealed partial class ArticleView : Page
{
    private Article Article;
    public ArticleView(Article Data)
    {
        InitializeComponent();
        Article = Data;

#if __ANDROID__
        var manager = SystemNavigationManager.GetForCurrentView();
        manager.BackRequested += OnBackRequested;
#endif
    }

    private void OnBackRequested(object? sender, BackRequestedEventArgs e)
    {
        if (Glob.NaviStack.Count > 1) { Glob.FuckGoBack(); }
    }

    /// <summary>
    /// Opens article in the users browser
    /// </summary>
    private void OpenBrowser(object sender, RoutedEventArgs e) => Process.Start(new ProcessStartInfo { FileName = Article.Url, UseShellExecute = true });


    /// <summary>
    /// Opens the reader mode UI, which should show the users 
    /// a no nonsense view of the articles text.
    /// </summary>
    private void OpenReader(object sender, RoutedEventArgs e)
    {
        Glob.NaviStack.Push(new ReaderMode(Article));
        Glob.DoNavi();
    }

    //Leaves this view, sends user back to the article view.
    private void GoBack(object sender, RoutedEventArgs e) { Glob.FuckGoBack(); }
}
