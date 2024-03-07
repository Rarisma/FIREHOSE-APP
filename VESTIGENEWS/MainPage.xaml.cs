using System.Collections.ObjectModel;

namespace VESTIGENEWS;

public sealed partial class MainPage : Page
{
    public ObservableCollection<Article> Articles { get; set; }

    public MainPage()
    {
        LoadDataAsync();
        InitializeComponent();

    }

    private void LoadDataAsync()
    {
        try
        {
            Articles = new();
            foreach (Article article in Article.GetArticles(100).OrderBy(item => new Random().Next()))
            {
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
        this.Content = new ArticleView(((Article)((Grid)sender).DataContext));
    }
}
