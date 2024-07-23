using Windows.UI.Core;
using FirehoseApp.Preferences;
using HYDRANT.Definitions;

//I'VE GOT A RECKLESS TONGUE
namespace FirehoseApp;
public static class Glob
{
    /// <summary>
    /// Is the user part of the test group
    /// </summary>
    public static bool TestGroup = true;

    /// <summary>
    /// Does the user have a FHN+ subscription?
    /// (Not implemented, does nothing if set to true)
    /// </summary>
    public static bool FNHPlus = false;
    
    public static Stack<object> NaviStack = new();
    public static PreferencesModel? Model;
    public static XamlRoot? XamlRoot;

    /// <summary>
    /// List of publications from firehose
    /// </summary>
    public static List<Publication> Publications = new();


    /// <summary>
    /// The built-in navigation within Frames doesn't work
    /// like I want it to work, so we persist the page items properly.
    /// </summary>
    private static void DoNavi()
    {
        App.MainWindow.Content = (UIElement)NaviStack.First();
    }
    
    public static void DoNavi(object PageSource)
    {
        NaviStack.Push(PageSource);
        App.MainWindow.Content = (UIElement)NaviStack.First();
    }

    /// <summary>
    /// Get rid of the top item, then go back.
    /// </summary>
    public static void GoBack()
    {
        if (NaviStack.Count > 1)
        {
            NaviStack.Pop();
            DoNavi();
        }

    }

    public static void OnBackRequested(object? sender, BackRequestedEventArgs e)
    {
        if (NaviStack.Count > 1) { GoBack(); }
    }

    public static void Log(LogLevel Level, String Message)
    {
        Console.WriteLine(Level.ToString(),Message);
    }

    public enum LogLevel
    {
        Inf,
        Wrn,
        Err,
        Sev
    }
    

    public static bool IsDialogOpen { get; private set; }
    public static async Task<ContentDialogResult> OpenContentDialog(ContentDialog dialog)
    {
        if (!IsDialogOpen)
        {
            IsDialogOpen = true;    //Stop any other dialogs from opening
            dialog.XamlRoot = App.MainWindow.Content.XamlRoot;   //Set XamlRoot
            var res = await dialog.ShowAsync(); //Show the dialog
            IsDialogOpen = false;    //Allow others Dialogs to show
            return res;              // Return the result
        }
        else
        {
            //We can't show a result as there's already a dialog open
            return ContentDialogResult.None;
        }
    }
}
