//Let it ride
namespace VESTIGENEWS.Controls;

public sealed partial class AIFeedbackDialog : Page
{
    public Article ItemSource;
    public AIFeedbackDialog(Article Article)
    {
        InitializeComponent();
        ItemSource = Article;
    }


}
