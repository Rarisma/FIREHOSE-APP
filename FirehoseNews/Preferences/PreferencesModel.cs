using System.Text;
using System.Text.Json;
using HYDRANT.Definitions;

namespace FirehoseNews.Preferences;
public class PreferencesModel
{
    public PreferencesModel()
    {
        Proxy = string.Empty;
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
        //WARNING: THE BELOW CODE DOES NOT WORK ON UNPACKAGED WINDOWS BUILDS.

        //Serialise PreferenceModel 
        string Result;
        using (MemoryStream MemoryStream = new())
        {
            using (StreamWriter StreamWriter = new(MemoryStream))
            {
                JsonSerializer.Serialize(MemoryStream, Glob.Model);
                StreamWriter.Flush();  // Ensure all data is written to the MemoryStream
                MemoryStream.Position = 0;  // Reset the position to read from the beginning

                // Read the stream into a string
                using (StreamReader Reader = new(MemoryStream))
                {
                    Result = Reader.ReadToEnd();
                }
            }
        }

        //Write model to settings value.
        ApplicationData.Current.LocalSettings.Values["Pref"] = Result;
    }

    /// <summary>
    /// Load prefs model.
    /// </summary>
    public static void Load() 
    {
        //WARNING: THE BELOW CODE DOES NOT WORK ON UNPACKAGED WINDOWS BUILDS.

        try
        {
            // Assuming the XML string is saved in the application's local settings.
            string XmlData = (string)ApplicationData.Current.LocalSettings.Values["Pref"];
            if (XmlData == null)
            {
                Glob.Model = new();
                return;
            }

            //Write model back into memory
            using (MemoryStream MemoryStream = new(Encoding.UTF8.GetBytes(XmlData)))
            {
                Glob.Model = ((PreferencesModel)JsonSerializer.Deserialize(MemoryStream, typeof(PreferencesModel))!);
            }

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
