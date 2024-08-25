using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.Preferences;
using System.Collections.ObjectModel;
using HYDRANT.Definitions;
using Uno.Extensions;

//CLOUDSPOTTER
namespace FirehoseApp.UI.Dialogs;
public sealed partial class BlockedSources : Page
{
    /* WARNING:
     * ENABLED IS INVERSE HERE!
     * IF A SOURCE IS ENABLED IT MEANS IT'S BLOCKED !
     */

    public PreferencesModel Pref = Ioc.Default.GetRequiredService<PreferencesModel>();
    public ObservableCollection<Publication> publications { get; set; }

    public BlockedSources()
    {
        InitializeComponent();
        
        publications = new();
        foreach (var src in Glob.Publications) //Set checked items.
        {
            if (Pref.BlockedSources.Contains(src.ID))
            {
                src.Enabled = true;
            }
            else { src.Enabled = false; }
            publications.Add(src);
        }
        publications.RemoveAt(0); //Don't display example news network
    }
    
    /// <summary>
    /// Ran when checked,updates Pref.Blockedsources
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void UpdateBlockedSources(object sender, SelectionChangedEventArgs e)
    {
        //Select all Publication.IDs where Enabled is true
        Pref.BlockedSources = publications.Where(src => src.Enabled = true)
            .Select(src => src.ID).ToObservableCollection();
    }
}
