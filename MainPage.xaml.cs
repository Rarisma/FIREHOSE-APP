using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;
using REMNANT.Typing;
using System.Threading.Tasks;
using Microsoft.Maui.Dispatching;

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
                    Glob.DB.Open();
                    MySqlCommand cmd = new MySqlCommand(Glob.query, Glob.DB);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var article = new Article
                        {
                            Url = reader["URL"].ToString(),
                            Title = reader["TITLE"].ToString(),
                            RssSummary = reader["RSS_SUMMARY"].ToString(),
                            PublishDate = Convert.ToDateTime(reader["PUBLISH_DATE"]),
                            LowQuality = Convert.ToBoolean(reader["LOW_QUALITY"]),
                            Paywall = Convert.ToBoolean(reader["PAYWALL"]),
                            Summary = reader["SUMMARY"].ToString()
                        };

                        Dispatcher.Dispatch(() => Articles.Add(article));
                    }
                    reader.Close();
                    Glob.DB.Close();


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