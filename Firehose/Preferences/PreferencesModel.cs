using System.Text.Json;
using HYDRANT.Definitions;

namespace Firehose.Preferences;
public class PreferencesModel
{
    
    static StorageFolder SaveFolder = ApplicationData.Current.LocalFolder;
    static string SavePath = Path.Combine(SaveFolder.Path, "prefs.json");
    public PreferencesModel()
    {
        Proxy = string.Empty;
        ArticleFetchLimit = 20;
        BookmarkedArticles = new();
    }

    /// <summary>
    /// Proxies allow users to bypass geo restrictions
    /// </summary>
    public string Proxy { get; set; }

    /// <summary>
    /// Don't use web view, jump to reader mode.
    /// </summary>
    public bool AlwaysOpenReader { get; set; }

    /// <summary>
    /// Article fetch limit
    /// </summary>
    public int ArticleFetchLimit { get; set; }

    /// <summary>
    /// Article Objects that the user has bookmarked
    /// </summary>
    public List<Article> BookmarkedArticles { get; set; }

    /// <summary>
    /// Tapping/clicking an article will open the summary
    /// instead of opening the article itself.
    /// </summary>
    public bool ArticleTapOpensSummary { get; set; }


    /// <summary>
    /// write prefs model
    /// </summary>
    public static void Save()
    {
        try
        {
            //Serialise PreferenceModel 
            string Result = JsonSerializer.Serialize(Glob.Model);
            File.WriteAllText(SavePath, Result);
        }
        catch (Exception Ex)
        {
            Glob.Log(Glob.LogLevel.Wrn, Ex.Message);
        }

    }

    /// <summary>
    /// Load prefs model.
    /// </summary>
    public static void Load() 
    {
        try
        {
            if (File.Exists(SavePath) == false)
            {
                Glob.Model = new();
                return;
            }

            var content = File.ReadAllText(SavePath);
            Glob.Model = (PreferencesModel)JsonSerializer.Deserialize(content, typeof(PreferencesModel))!;

            if (Glob.Model.ArticleFetchLimit <= 5)
            {
                Glob.Model.ArticleFetchLimit = 20;
            }
        }
        catch (Exception Ex)
        {
            Glob.Log(Glob.LogLevel.Wrn, Ex.Message);
            Glob.Model = new();
        }
    }
}
