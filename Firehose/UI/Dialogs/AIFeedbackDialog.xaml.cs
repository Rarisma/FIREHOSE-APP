using CommunityToolkit.Mvvm.DependencyInjection;
using Firehose.Viewmodels;
using HYDRANT.Definitions;

//Let it ride
namespace Firehose.UI.Dialogs;

public sealed partial class AIFeedbackDialog : Page
{
    private ThemeVM Themer = Ioc.Default.GetRequiredService<ThemeVM>();
    public Article ItemSource;
    public AIFeedbackDialog(Article Article)
    {
        this.InitializeComponent();
        ItemSource = Article;
    }


}
