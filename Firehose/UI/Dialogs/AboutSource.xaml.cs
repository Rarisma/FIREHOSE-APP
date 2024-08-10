using HYDRANT.Definitions;

namespace FirehoseApp.UI.Dialogs;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AboutSource : Page
{
    public Publication Publication { get; set; }
    public AboutSource(int PublicationID)
    {
        Publication = Glob.Publications[PublicationID];
        InitializeComponent();
    }
}
