using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.Preferences;
using System.Collections.ObjectModel;
using HYDRANT.Definitions;
using Uno.Extensions;

namespace FirehoseApp.UI.Dialogs;
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
