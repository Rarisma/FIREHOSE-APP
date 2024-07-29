using Windows.UI.Text;
using FirehoseApp.Preferences;
using FirehoseApp.UI.Dialogs;

namespace FirehoseApp.UI;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Preferences : Page
{
    private PreferencesModel Model;
    
    public string AppVer
    {
        get => $"{Package.Current.DisplayName} Version {Package.Current.Id.Version.Major}." +
               $"{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}";
    }
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
    
    private void DevMenu(object sender, RoutedEventArgs e)
    {
        
        Button BetaLogin = new()
        {
            Content = "Show Beta Login Flow"
        };
        BetaLogin.Click += (_, _) => { Glob.DoNavi(new LoginFlow()); };
        
        TextBox TokenBox = new()
        {
            Header = "Account Token",
            Text = Model.AccountToken
            
        };
        TokenBox.TextChanged += (_, _) => { Model.AccountToken = TokenBox.Text; };


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
}
