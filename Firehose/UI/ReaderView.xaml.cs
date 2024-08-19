using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.Viewmodels;

namespace FirehoseApp.UI;

/// <summary>
/// Simple Reader mode
/// </summary>
public sealed partial class ReaderMode : Page
{
    private ShellVM ShellVM = Ioc.Default.GetRequiredService<ShellVM>();
    public ReaderMode()
    {
        InitializeComponent();
    }
}
