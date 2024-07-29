using System.Text.Json;
using HYDRANT.Definitions;

namespace FirehoseApp.Preferences;
public class PreferencesModel
{
    /// <summary>
    /// the folder where the preferences are saved to
    /// </summary>
    static StorageFolder SaveFolder = ApplicationData.Current.LocalFolder;

    /// <summary>
    /// The file path to prefs.json
    /// </summary>
    static string SavePath = Path.Combine(SaveFolder.Path, "prefs.json");
    public PreferencesModel()
    {
        Proxy = string.Empty;
        ArticleFetchLimit = 50;
        BookmarkedArticles = new();
        OpenInMode = 0;
        UserGeneratedContent = false;
    }

    /// <summary>
    /// Proxies allow users to bypass geo restrictions
    /// </summary>
    public string Proxy { get; set; }

    /// <summary>
    /// Controls what articles are opened in
    /// 0 - Article WebView
    /// 1 - Reader Mode
    /// 2 - Default Web Browser
    /// </summary>
    public int OpenInMode { get; set; }

    /// <summary>
    /// Article fetch limit
    /// </summary>
    public int ArticleFetchLimit { get; set; }
    
    /// <summary>
    /// Article fetch limit
    /// </summary>
    public bool UserGeneratedContent { get; set; }

    /// <summary>
    /// Article Objects that the user has bookmarked
    /// </summary>
    public List<Article> BookmarkedArticles { get; set; }
    
    /// <summary>
    /// Allows users access to features.
    /// </summary>
    public string AccountToken { get; set; }


    /// <summary>
    /// write this model
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
    /// Load this model.
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
