using Firehose.Preferences;

namespace Firehose.UI;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Preferences : Page
{
    private PreferencesModel Model;

    public Preferences()
    {
        InitializeComponent();
        Model = Glob.Model;
    }

    private void Back(object sender, RoutedEventArgs e)
    {
        Glob.Model = Model;
        PreferencesModel.Save();
        Glob.GoBack();
    }
}
