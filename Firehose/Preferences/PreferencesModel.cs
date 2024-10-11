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
        blockedSources = new();
        Proxy = string.Empty;
        ArticleFetchLimit = 50;
        BookmarkedArticles = new();
        OpenInMode = 0;
        UserGeneratedContent = false;
        LastOpened = DateTime.MinValue;
        DayInstalled = DateTime.Now;
        TimeSaved = 0;
        ArticlesOpened = 0;
        DaysUsed = 0;
    }

    /// <summary>
    /// Filter to apply to articles by default
    /// </summary>
    public string DefaultFilter { get; set; }

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
    /// Controls what happens when share is clicked
    /// 0 - Default Share Menu
    /// 1 - Copy link to Clipboard
    /// 2 - Copy Link and summary
    /// </summary>
    public int ShareMode { get; set; }

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
    /// Times summary has been tapped without opening the article
    /// </summary>
    public int TimeSaved { get; set; }
    
    /// <summary>
    /// Articles Opened
    /// </summary>
    public int ArticlesOpened { get; set; }

    /// <summary>
    /// How many days firehose has been used
    /// </summary>
    public int DaysUsed { get; set; }
    
    /// <summary>
    /// Day firehose was installed/initalised
    /// </summary>
    public DateTime DayInstalled { get; set; }

    /// <summary>
    /// Date firehose was last opened
    /// (Used to calculate DaysUsed)
    /// </summary>
    public DateTime LastOpened { get; set; }

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
    /// Articles from these sources will be blocked
    /// </summary>
    private ObservableCollection<int> blockedSources;
    public ObservableCollection<int> BlockedSources
    {
        get => blockedSources;
        set => SetProperty(ref blockedSources, value);
    }

    /// <summary>
    /// write this model
    /// </summary>
    public static void Save()
    {
        try
        {
            PreferencesModel Pref = Ioc.Default.GetRequiredService<PreferencesModel>();

            //Update days used count
            if (DateTime.Now.Date != Pref.LastOpened.Date)
            {
                Pref.DaysUsed++;
                Pref.LastOpened = DateTime.Now;
            }

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
            Pref.blockedSources = new();
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
