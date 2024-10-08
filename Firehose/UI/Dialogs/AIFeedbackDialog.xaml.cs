using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.Viewmodels;
using HYDRANT.Definitions;

//Whatever keeps you warm at night.
namespace FirehoseApp.UI.Dialogs;

public sealed partial class AIFeedbackDialog : Page
{
    private ThemeVM Themer = Ioc.Default.GetRequiredService<ThemeVM>();
    public Article ItemSource;
    public AIFeedbackDialog(Article Article)
    {
        this.InitializeComponent();
        ItemSource = Article;
    }
    
    /// <summary>
    /// Spawns a dialog showing this page
    /// </summary>
    public static async void ReportSummary(Article Item)
    {
        ContentDialog d = new()
        {
            Title = "Report issue with summary for " + Item.Title,
            Content = new AIFeedbackDialog(Item),
            PrimaryButtonText = "Send Feedback",
            SecondaryButtonText = "Close"
        };
        var Res = await Glob.OpenContentDialog(d);
        
        if (Res == ContentDialogResult.Primary) //Send feedback clicked
        {
            await Ioc.Default.GetRequiredService<ShellVM>().Hallon.ReportArticle(Item.URL);
            
        }
    }

}
