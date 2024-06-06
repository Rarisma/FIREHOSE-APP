using Firehose.Preferences;
using Firehose.UI.Dialogs;
using HYDRANT;
using HYDRANT.Definitions;

namespace Firehose.UI.Controls;
public sealed partial class CommonNavigationBar : Grid
{

    /// <summary>
    /// Set this to the Article Object you want to display publisher information for
    /// </summary>
    public static readonly DependencyProperty ItemSourceProperty =
        DependencyProperty.Register(
            "ItemSource", // Name of the property
            typeof(Article), // Type of the property
            typeof(CommonNavigationBar), // Owner of the property (this control)
            new PropertyMetadata(default(string)));
    
    /// <summary>
    /// Publication information for given article.
    /// </summary>
    public Article ItemSource
    {
        get { return (Article)GetValue(ItemSourceProperty); }
        set { SetValue(ItemSourceProperty, value); }
    }

    public CommonNavigationBar()
    {
        InitializeComponent();
    }

    private async void ReportSummary(object sender, RoutedEventArgs e)
    {
        ContentDialog d = new()
        {
            Title = "Report issue with summary for " + ItemSource.Title,
            XamlRoot = XamlRoot,
            Content = new AIFeedbackDialog(ItemSource),
            PrimaryButtonText = "Send Feedback",
            SecondaryButtonText = "Close"
        };
        var Res = await Glob.OpenContentDialog(d);
        
        if (Res == ContentDialogResult.Primary) //Send feedback clicked
        {
            int reason = (((d.Content as AIFeedbackDialog)!.Content as Grid)!.Children[3] as ComboBox)!.SelectedIndex;
            new API().ReportArticle(ItemSource, reason);
            
        }
    }
    
    /// <summary>
    /// Opens article in the users browser
    /// </summary>
    private async void OpenBrowser(object sender, RoutedEventArgs e) => await Windows.System.Launcher.LaunchUriAsync(new Uri(ItemSource.Url));
    
    
    /// <summary>
    /// Opens the reader mode UI, which should show the users 
    /// a no nonsense view of the articles text.
    /// </summary>
    private void OpenReader(object sender, RoutedEventArgs e) => Glob.DoNavi(new ReaderMode(ItemSource));
    
    /// <summary>
    /// Return to article view.
    /// </summary>
    private void GoBack(object sender, RoutedEventArgs e) { Glob.GoBack(); }
    
    public void SetBookmarkIcon()
    {
        //Handle bookmarked status
        if (Glob.Model.BookmarkedArticles.Count(article => article.Url == ItemSource.Url) != 0)
        {
            Glyphy.Glyph = "\xE735";
        }
        else { Glyphy.Glyph = "\xE734"; }
    }
    
    private void BookmarkClick(object sender, RoutedEventArgs e)
    {
        // Add or Remove article from bookmarked articles.
        if (Glob.Model.BookmarkedArticles.Contains(ItemSource))
        {
            Glob.Model.BookmarkedArticles.Remove(ItemSource);
        }
        else
        {
            Glob.Model.BookmarkedArticles.Add(ItemSource);
        }
        
        // update icon state and save bookmarks
        SetBookmarkIcon();
        PreferencesModel.Save();
    }
    
    private void ReportClickbait(object sender, RoutedEventArgs e)
    {
        new API().VoteClickbait(ItemSource);

        //Show visual feedback
        ClickbaitButton.Text = "Already reported";
        ClickbaitButton.IsEnabled = false; //Prevent multiple reports.
    }
    
    private void CommonNavigationBar_OnLoaded(object sender, RoutedEventArgs e)
    {
        SetBookmarkIcon();
        
    }
}
