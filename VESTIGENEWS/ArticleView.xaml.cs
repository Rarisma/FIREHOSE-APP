using System.Diagnostics;
using Microsoft.UI;
using Windows.UI.Core;

namespace VESTIGENEWS;

public sealed partial class ArticleView : Page
{

    private Article Article;
    public ArticleView(Article Data)
    {
        Data.Url = Glob.Model.Proxy + Data.Url;
        InitializeComponent();
        Article = Data;
        SetBookmarkIcon();

        if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
        {
            Background = new SolidColorBrush(Colors.Black);
        }
        else { Background = new SolidColorBrush(Colors.LightGray); }

#if __ANDROID__
        var manager = SystemNavigationManager.GetForCurrentView();
        manager.BackRequested += OnBackRequested;
#endif
    }

    public void SetBookmarkIcon()
    {
        //Handle bookmarked status
        if (Glob.Model.bookmarkedArticles.Contains(Article))
        {
            Glyphy.Glyph = "\xE735";
        }
        else { Glyphy.Glyph = "\xE734"; }
    }

    private void OnBackRequested(object? sender, BackRequestedEventArgs e)
    {
        if (Glob.NaviStack.Count > 1) { Glob.GoBack(); }
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
    private void GoBack(object sender, RoutedEventArgs e) { Glob.GoBack(); }

    private void BookmarkClick(object sender, RoutedEventArgs e)
    {
        // Add or Remove article from bookmarked articles.
        if (Glob.Model.bookmarkedArticles.Contains(Article))
        {
            Glob.Model.bookmarkedArticles.Remove(Article);
        }
        else
        {
            Glob.Model.bookmarkedArticles.Add(Article);
        }

        // update icon state.
        SetBookmarkIcon();
    }
}
