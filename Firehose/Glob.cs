using Windows.UI.Core;
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
    
    private static bool _IsContentDialogOpen { get; set; }

    public static bool IsDialogOpen { get; private set; }
    /// <summary>
    /// Reference to currently open dialog.
    /// </summary>
    private static ContentDialog OpenDialog;

    /// <summary>
    /// This takes a ContentDialog and shows it to the user
    /// It will handle XAMLRoot and showing the dialog.
    /// </summary>
    /// <param name="Dialog">Content dialog to show</param>
    /// <param name="force">Force show content dialog, will close currently open dialog if one
    /// is already open
    /// </param>
    /// <returns>A ContentDialogResult enum value.</returns>
    public static async Task<ContentDialogResult> OpenContentDialog(ContentDialog Dialog,
        bool force = false)
    {
        //force close any other dialog if one is open
        //(if force is enabled)
        if (force && _IsContentDialogOpen && OpenDialog != null)
        {
            OpenDialog.Hide();

            //This will be unset eventually but because we have called
            //Hide() already we can unset this early.
            _IsContentDialogOpen = false;
        }

        //Checks a content dialog isn't already open
        if (!_IsContentDialogOpen)
        {
            OpenDialog = Dialog;

            //Set XAML root and correct theme.
            OpenDialog.XamlRoot = XamlRoot;

            _IsContentDialogOpen = true;

            //Show and log result.
            ContentDialogResult Result = await OpenDialog.ShowAsync();
            _IsContentDialogOpen = false;
            OpenDialog = null;
            return Result;
        }
        return ContentDialogResult.None;
    }
}
