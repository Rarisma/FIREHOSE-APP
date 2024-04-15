using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

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

    /// <summary>
    /// Article Objects that the user has bookmarked
    /// </summary>
    public List<Article> bookmarkedArticles = new();


    /// <summary>
    /// write prefs model
    /// </summary>
    public static void Save()
    {
        //Serialise PreferenceModel 
        string result;
        using (MemoryStream memoryStream = new())
        {
            using (StreamWriter streamWriter = new(memoryStream))
            {
                JsonSerializer.Serialize(memoryStream, Glob.Model);
                streamWriter.Flush();  // Ensure all data is written to the MemoryStream
                memoryStream.Position = 0;  // Reset the position to read from the beginning

                // Read the stream into a string
                using (StreamReader reader = new(memoryStream))
                {
                    result = reader.ReadToEnd();
                }
            }
        }

        //Write model to settings value.
        ApplicationData.Current.LocalSettings.Values["Pref"] = result;
    }

    /// <summary>
    /// Load prefs model.
    /// </summary>
    public static void Load() 
    {
        try
        {

            // Assuming the XML string is saved in the application's local settings.
            string xmlData = (string)ApplicationData.Current.LocalSettings.Values["Pref"];
            if (xmlData == null)
            {
                Glob.Model = new();
                return;
            }

            //Write model back into memory
            using (MemoryStream memoryStream = new(Encoding.UTF8.GetBytes(xmlData)))
            {
                Glob.Model = (PreferencesModel)JsonSerializer.Deserialize(memoryStream, typeof(PreferencesModel));
            }
        }
        catch (Exception ex)
        {
            Glob.Log(Glob.LogLevel.Wrn, ex.Message);
            Glob.Model = new();
        }
    }
}
