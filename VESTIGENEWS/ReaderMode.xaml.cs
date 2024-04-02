using Microsoft.UI.Xaml.Media.Animation;

namespace VESTIGENEWS;

public sealed partial class ReaderMode : Page
{
    private Article Article;

    public ReaderMode(Article Data)
    {
        Article = Data;
        InitializeComponent();
    }

    private void GoBack(object sender, RoutedEventArgs e) => Glob.Frame.GoBack(new SlideNavigationTransitionInfo
        { Effect = SlideNavigationTransitionEffect.FromLeft });
}
