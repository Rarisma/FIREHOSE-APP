using HYDRANT.Definitions;

namespace Firehose.UI;

public sealed partial class ReaderMode : Page
{
    private Article Article;

    public ReaderMode(Article Data)
    {
        Article = Data;
        InitializeComponent();
    }

    private void GoBack(object sender, RoutedEventArgs e) => Glob.GoBack();
}
