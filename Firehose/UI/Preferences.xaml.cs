using Windows.UI.Text;
using FirehoseApp.Preferences;
using FirehoseApp.UI.Dialogs;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace FirehoseApp.UI;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Preferences : Page
{
    PreferencesModel Pref = Ioc.Default.GetRequiredService<PreferencesModel>();


    public string AppVer
    {
        get => $"{Package.Current.DisplayName} Version {Package.Current.Id.Version.Major}." +
               $"{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}";
    }
    public Preferences()
    {
        InitializeComponent();
    }

    private void Back(object sender, RoutedEventArgs e)
    {
        PreferencesModel.Save();
        App.UI.GoBack();
    }
    
    private void DevMenu(object sender, RoutedEventArgs e)
    {
        
        Button BetaLogin = new()
        {
            Content = "Show Beta Login Flow"
        };
        BetaLogin.Click += (_, _) => { App.UI.Navigate(typeof(LoginFlow)); };
        
        TextBox TokenBox = new()
        {
            Header = "Account Token",
            Text = Pref.AccountToken
            
        };
        TokenBox.TextChanged += (_, _) => { Pref.AccountToken = TokenBox.Text; };


        Glob.OpenContentDialog(new()
        {
            Title = "DEVELOPER MENU",
            Content = new StackPanel
            {
                Children =
                {
                    BetaLogin,
                    TokenBox,

                    new TextBlock
                    {
                        Text = "Are you supposed to be here?",
                        Margin = new Thickness(0,50,0,0),
                        FontStyle = FontStyle.Italic,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        HorizontalAlignment = HorizontalAlignment.Center
                    }

                }
            },
            PrimaryButtonText = "close"

        });
    }
    
    private async void BlockSources(object sender, RoutedEventArgs e)
    {
        await Glob.OpenContentDialog(new()
        {
            Title = "Editing blocked sources...",
            Content = new BlockedSources(),
            PrimaryButtonText = "Close"
        }, true);
        
        PreferencesModel.Save();
    }
    
    /// <summary>
    /// Brings up UI to block UI
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private async void BlockKeywords(object sender, RoutedEventArgs e)
    {
        await Glob.OpenContentDialog(new()
        {
            Title = "Editing blocked keywords...",
            Content = new BlockKeyWords(),
            PrimaryButtonText = "Close"
        },true);
        
        PreferencesModel.Save();
    }
}
