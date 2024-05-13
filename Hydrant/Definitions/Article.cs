using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
//Battle for everyone's source code.
[assembly: InternalsVisibleTo("FirehoseServer")]
namespace HYDRANT.Definitions;

/// <summary>
/// Article object, contains lots of information about articles.
/// </summary>
[JsonSerializable(typeof(List<Article>))]
[JsonSerializable(typeof(Article))]
public class Article
{
    #region Fields
    /// <summary>
    /// Title of article
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// URL of article
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Content of the article.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// The RSS Summary as provided by the author.
    /// This is stored for analytical purposes.
    /// </summary>
    public string RSSSummary { get; set; }

    /// <summary>
    /// DateTime object of the article was made public
    /// </summary>
    public DateTime PublishDate { get; set; }

    /// <summary>
    /// Is this a major story, i.e. something major is going on
    /// or is the article not a major event
    /// </summary>
    public bool IsHeadline { get; set; }

    /// <summary>
    /// is the article behind a paywall?
    /// </summary>
    public bool IsPaywall { get; set; }

    /// <summary>
    /// AI-Generated summary of the article
    /// </summary>
    public string Summary { get; set; }

    /// <summary>
    /// Image associated with URL
    /// </summary>
    public string ImageURL { get; set; }

    /// <summary>
    /// Firehose Publication ID
    /// </summary>
    public int PublisherID { get; set; }

    /// <summary>
    /// Does the article relate to business?
    /// </summary>
    public bool Business { get; set; }

    /// <summary>
    /// Company names/tickers mentioned within the article.
    /// </summary>
    public string CompaniesMentioned { get; set; }

	/// <summary>
	/// Author of the article.
	/// Will be unknown author if we can't find the author.
	/// If an article has multiple Authors, they will be separated by commas.
	/// </summary>
	public string Author { get; set; }

    /// <summary>
    /// Internal field, reserved.
    /// (Will always return false via FirehoseServer)
    /// </summary>
    public bool HasJakeFlag { get; set; }
    #endregion
}
