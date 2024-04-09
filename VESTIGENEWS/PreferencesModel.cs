namespace VESTIGENEWS;
public class PreferencesModel
{
    /// <summary>
    /// Proxies allow users to bypass geo restrictions
    /// </summary>
    public string Proxy = "";

    /// <summary>
    /// Don't use web view, jump to reader mode.
    /// </summary>
    public bool AlwaysOpenReader;

    /// <summary>
    /// Article fetch limit
    /// </summary>
    public int ArticleFetchLimit = 0;

    /// <summary>
    /// Always use HallonAPIServer instead of MySQL Calls.
    /// </summary>
    public bool ForceHallon;

    /// <summary>
    /// Force enable shuffling.
    /// </summary>
    public bool AlwaysShuffle;

    public List<Article> bookmarkedArticles = new();
}
