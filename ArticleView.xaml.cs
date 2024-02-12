namespace REMNANT;
//‘cause like that dude who married your mama, my skills go a step farther
public partial class ArticleView : ContentPage
{
	public ArticleView()
	{
		InitializeComponent();
        BrowserSession.Source = "https://www.thesun.co.uk"; //Glob.SelectedArticle.Url;
    }

    private void GetAISummary(object? sender, EventArgs e)
    {

    }
}