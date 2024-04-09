//I'VE GOT A RECKLESS TONGUE
using Windows.UI.Core;

namespace VESTIGENEWS;
public static class Glob
{
    public static Stack<object> NaviStack = new();
    public static Frame Frame = new();

    /// <summary>
    /// The built-in navigation within Frames doesn't work
    /// like I want it to work, so we persist the page items properly.
    /// </summary>
    public static void DoNavi()
    {
        Frame.Content = NaviStack.First();
    }

    /// <summary>
    /// Get rid of the top item, then go back.
    /// </summary>
    public static void GoBack()
    {
        NaviStack.Pop();
        DoNavi();
    }

    public static void OnBackRequested(object? sender, BackRequestedEventArgs e)
    {
        if (NaviStack.Count > 1) { GoBack(); }
    }

    public static PreferencesModel Model = new();
}
