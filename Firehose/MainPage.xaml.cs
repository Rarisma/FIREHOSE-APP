using CommunityToolkit.Mvvm.DependencyInjection;
using Firehose.UI;
using Firehose.Viewmodels;
using Microsoft.UI;

namespace Firehose;

public sealed partial class MainPage : Page
{
    private ShellVM ShellVM = Ioc.Default.GetRequiredService<ShellVM>();
    public MainPage()
    {
        InitializeComponent();
        //OnLayoutUpdated(null, null);

    }
    
    private void Loaded(object sender, RoutedEventArgs e)
    {
        if (Display.CurrentLayout >= Uno.Toolkit.UI.Layout.Wide)
        {
            Glob.NaviStack.Push(new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Children = { new TextBlock
                {
                    Text = "Click an article on the left to open it",
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                } }
            });
        }
        else
        {
            Glob.DoNavi(new ArticleList());
        }
    }
}
