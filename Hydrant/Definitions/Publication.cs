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
    /// Are all articles from this source jake flagged
    /// </summary>
    public bool AlwaysJakeFlag { get; set; }

    /// <summary>
    /// RSS feed URLs related to the publication
    /// </summary>
    public string[] Feeds { get; set; }

	/// <summary>
	/// Does the publication have a paywall?
	/// </summary>
	public bool Paywall { get; set; }
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
}
