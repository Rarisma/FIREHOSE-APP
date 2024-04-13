namespace VESTIGENEWS;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Preferences : Page
{
    private PreferencesModel Model = new();

    public Preferences()
    {
        InitializeComponent();
    }

    private void Back(object sender, RoutedEventArgs e)
    {
        Glob.GoBack();
    }
}
