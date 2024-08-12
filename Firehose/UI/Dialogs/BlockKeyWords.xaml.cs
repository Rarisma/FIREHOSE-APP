using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.Preferences;
namespace FirehoseApp.UI.Dialogs;

/// <summary>
/// Dialog that allows a user to set words they don't want to see.
/// </summary>
public sealed partial class BlockKeyWords : Page
{
    public PreferencesModel Pref = Ioc.Default.GetRequiredService<PreferencesModel>();

    public BlockKeyWords()
    {
        InitializeComponent();
    }
    
    /// <summary>
    /// Adds a keyword to the blocked keywords list
    /// </summary>
    private void AddKeyword(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (!string.IsNullOrEmpty(sender.Text))
        {
            Pref.BlockedKeywords.Add(sender.Text);
            sender.Text = "";
        }
    }
    
    /// <summary>
    /// Deletes a word from the keyword list
    /// </summary>
    private void RemoveKeyword(object sender, RoutedEventArgs e)
    {
        //Get word via parent
        string Keyword = (((sender as Button).Parent as StackPanel)
            .Children[1] as TextBlock).Text;
        Pref.BlockedKeywords.Remove(Keyword);
    }
}
