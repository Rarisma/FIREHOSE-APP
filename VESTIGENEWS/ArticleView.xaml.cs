using System.Diagnostics;

namespace VESTIGENEWS;

public sealed partial class ArticleView : Page
{
    private Article Article;
    public ArticleView(Article Data)
    {
        InitializeComponent();
        Article = Data;

        //Title has a XAML Parsing error so set it manually.
        //ArticleTitle.Text = Data.Title;
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
