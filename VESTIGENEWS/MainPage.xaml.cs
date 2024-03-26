namespace VESTIGENEWS;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        //Set up shell-like frame navigation
        InitializeComponent();
        Content = Glob.Frame;

        Glob.NaviStack.Push(new ArticleList());
        Glob.DoNavi();
    }
}
