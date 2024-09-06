using System.Drawing;
using System.Text.Json;
using HYDRANT.Serializers;

namespace HYDRANT.Definitions;

/// <summary>
/// Stores data about publishers.
/// </summary>
public class Publication
{
    #region Fields
    /// <summary>
    /// Numerical ID of article.
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Name of publication
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// unused
    /// </summary>
    public int Weighting { get; set; }


    /// <summary>
    /// Favicon representing publication
    /// </summary>
    public string Favicon { get; set; }

    /// <summary>
    /// Information about the publication
    /// </summary>
    public string Information { get; set; }

    /// <summary>
    /// RSS feed.URLs related to the publication
    /// </summary>
    public string[] Feeds { get; set; }

	/// <summary>
	/// Does the publication have a paywall?
	/// </summary>
	public bool Paywall { get; set; }

    /// <summary>
    /// Optional background override for the publication
    /// Some logos are hard to see as many are black or white,
    /// meaning they are hard to see in either light/dark mode.
    /// </summary>
    public string? BackgroundOverride { get; set; }

    /// <summary>
    /// Is this source disabled
    /// (Clientside)
    /// </summary>
    public bool Enabled { get; set; }
    #endregion

    /// <summary>
    /// Loads publications from a feeds.json
    /// </summary>
    /// <param name="File">Filepath</param>
    /// <returns>List of publications</returns>
    public static List<Publication>? LoadFromJSON(string File = "Data/Feeds.json")
    {
        //read file then deserialize
        string content = System.IO.File.ReadAllText(File);
        return JsonSerializer.Deserialize(content, PublicationSerialiser.Default.ListPublication);
    }
    
    public Color Background
    {
        get
        {
            if (string.IsNullOrEmpty(BackgroundOverride))
            {
                return new Color();
            }
            
            Color drawingColor = (Color)new ColorConverter().
                ConvertFromString(BackgroundOverride);
            
            // Convert to Windows.UI.Color
            Color uiColor = Color.FromArgb(drawingColor.A, drawingColor.R,
                drawingColor.G, drawingColor.B);
            
            // Create and return a SolidColorBrush with the specified color
            return uiColor;
        }
    }
}
