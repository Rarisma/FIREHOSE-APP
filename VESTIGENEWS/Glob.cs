//I'VE GOT A RECKLESS TONGUE
namespace VESTIGENEWS;
public static class Glob
{
    public static Stack<object> NaviStack = new();
    public static Frame Frame = new();

    /// <summary>
    /// Either im stupid or I can't do regular frame navigation
    /// So this does the navigation for us.
    /// </summary>
    public static void DoNavi()
    {
        Frame.Content = NaviStack.First();
    }

    /// <summary>
    /// Get rid of the top item, then go back.
    /// </summary>
    public static void FuckGoBack()
    {
        NaviStack.Pop();
        DoNavi();
    }
}
