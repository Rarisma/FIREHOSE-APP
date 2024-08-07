using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.Preferences;
using System.Collections.ObjectModel;
using HYDRANT.Definitions;
using Uno.Extensions;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FirehoseApp.UI.Dialogs;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class BlockedSources : Page
{
    public PreferencesModel Pref = Ioc.Default.GetRequiredService<PreferencesModel>();
    public ObservableCollection<Publication> publications = new();

    public BlockedSources()
    {
        publications.AddRange(Glob.Publications);
        publications.RemoveAt(0); //Don't display example news network.
        this.InitializeComponent();
    }
}
