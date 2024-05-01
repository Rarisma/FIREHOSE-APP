using System.Diagnostics;
using System.Security.Permissions;
using Microsoft.UI;

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

    /// <summary>
    /// Opens article in the users browser
    /// </summary>
    private async void OpenBrowser(object sender, RoutedEventArgs e)
    {
        await Windows.System.Launcher.LaunchUriAsync(new Uri(Article.Url));
    }


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

        // update icon state and save bookmarks
        SetBookmarkIcon();
        PreferencesModel.Save();
    }

    private async void ReportSummary(object sender, RoutedEventArgs e)
    {
        ContentDialog CD = new()
        {
            Title = "Report issue with summary for " + Article.Title,
            XamlRoot = this.XamlRoot,
            Content = new Controls.AIFeedbackDialog(Article),
            PrimaryButtonText = "Send Feedback",
            SecondaryButtonText = "Close"
        };

        await CD.ShowAsync();
    }
}
