using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.Viewmodels;
using HYDRANT.Definitions;
using Uno.Extensions;
//WE AIN'T MAKING OUTTA HOKKAIDO WITH THIS ONE
namespace FirehoseApp.UI.Controls;
public sealed partial class PublisherFilter : Page
{
    public ObservableCollection<Publication> publications = new();
    public ShellVM ShellVM = Ioc.Default.GetRequiredService<ShellVM>();
    public PublisherFilter()
    {
        InitializeComponent();
        publications.AddRange(Glob.Publications);
        publications.RemoveAt(0); //Don't display example news network.
    }
}
