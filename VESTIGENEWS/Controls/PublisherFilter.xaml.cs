using System.Collections.ObjectModel;
using Uno.Extensions;

namespace VESTIGENEWS.Controls;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PublisherFilter : Page
{
    public ObservableCollection<Publication> publications = new();

    public PublisherFilter()
    {
        InitializeComponent();
        publications.AddRange(Glob.Publications);
        publications.RemoveAt(0); //Don't display example news network.
    }


}
