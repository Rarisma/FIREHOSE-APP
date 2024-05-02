//I'VE GOT A RECKLESS TONGUE
using Windows.UI.Core;

namespace VESTIGENEWS;
public static class Glob
{
    public static Stack<object> NaviStack = new();
    public static Frame Frame = new();

    /// <summary>
    /// List of publications from firehose
    /// </summary>
    public static List<Publication> Publications = new();


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

    public static PreferencesModel Model = new();

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
}
