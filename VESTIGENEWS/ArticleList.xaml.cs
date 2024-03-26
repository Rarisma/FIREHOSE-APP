using System.Collections.ObjectModel;

namespace VESTIGENEWS;

public sealed partial class ArticleList : Page
{
    public ObservableCollection<Article> Articles { get; set; }
    public ArticleList()
    {
        Articles = new();
        LoadDataAsync();
        InitializeComponent();
    }

    private void LoadDataAsync()
    {
        try
        {
            Articles.Clear();
            foreach (Article article in Article.GetArticles(100).OrderBy(item => new Random().Next()))
            {
                if (article.ImageURL == null)
                {
                    article.ImageURL = "Assets/Placeholder.png";
                }
                Articles.Add(article);

            }
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.ToString());
        }
    }

    private void InvokeArticle(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        Article Data = (Article)((Grid)sender).DataContext;
        ArticleView V = new ArticleView(Data);
        Glob.NaviStack.Push(V);
        Glob.DoNavi();
    }
}
