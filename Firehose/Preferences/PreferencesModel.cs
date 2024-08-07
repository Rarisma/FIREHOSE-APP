using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using HYDRANT.Definitions;

namespace FirehoseApp.Preferences;
public class PreferencesModel : ObservableObject
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
        BlockedKeywords = new();
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
    /// Sources that contain these keywords will be blocked
    /// </summary>
    private ObservableCollection<string> blockedKeywords;
    public ObservableCollection<string> BlockedKeywords
    {
        get => blockedKeywords;
        set => SetProperty(ref blockedKeywords, value);
    }

    /// <summary>
    /// write this model
    /// </summary>
    public static void Save()
    {
        try
        {
            PreferencesModel Pref = Ioc.Default.GetRequiredService<PreferencesModel>();

            //Serialise PreferenceModel 
            string Result = JsonSerializer.Serialize(Pref);
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
    public static PreferencesModel Load()
    {
        PreferencesModel Pref = new();
        try
        {

            if (File.Exists(SavePath) == false)
            {
                Pref = new();
                return Pref;
            }

            var content = File.ReadAllText(SavePath);
            Pref = (PreferencesModel)JsonSerializer.Deserialize(content, typeof(PreferencesModel))!;

            if (Pref.ArticleFetchLimit <= 5)
            {
                Pref.ArticleFetchLimit = 20;
            }
        }
        catch (Exception Ex)
        {
            Glob.Log(Glob.LogLevel.Wrn, Ex.Message);
        }
        
        return Pref;
    }
}
