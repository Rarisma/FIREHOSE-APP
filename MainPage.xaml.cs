using System.Collections.ObjectModel;
using HYDRANT;

namespace REMNANT;

public partial class MainPage : ContentPage
{
    public ObservableCollection<Article> Articles { get; set; }

    public MainPage()
    {
        LoadDataAsync();
        InitializeComponent();
        BindingContext = this; }

    private async Task LoadDataAsync()
    {
        try
        {
            Articles = new();
            await Task.Run(() =>
                {
                    // Randomize order.
                    Dispatcher.Dispatch(() =>
                    {
                        var rnd = new Random();
                        var a = Articles.OrderBy(item => rnd.Next()).ToList();
                        Articles.Clear();
                        foreach (var article in a)
                        {
                            Articles.Add(article);
                        }
                    });
                });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    /// <summary>
    /// Open the article in WebView.
    /// </summary>
    private async void OpenArticle(object? sender, TappedEventArgs e)
    {
        //Glob.SelectedArticle = "https://www.thesun.co.uk/";
        await Shell.Current.GoToAsync("////ArticleView");
    }
}