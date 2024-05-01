namespace VESTIGENEWS.Controls;
//NO MORE FREESTYLE
public sealed partial class PublisherInformation : UserControl
{

    /// <summary>
    /// Set this to the Article Object you want to display publisher information for
    /// </summary>
    public static readonly DependencyProperty ItemSourceProperty =
        DependencyProperty.Register(
            "ItemSource", // Name of the property
            typeof(Article), // Type of the property
            typeof(PublisherInformation), // Owner of the property (this control)
            new PropertyMetadata(default(string)));

    /// <summary>
    /// Publication information for given article.
    /// </summary>
    public Article ItemSource
    {
        get { return ((Article)GetValue(ItemSourceProperty)); }
        set { SetValue(ItemSourceProperty, value); }
    }

    /// <summary>
    /// Publication information for given article.
    /// </summary>
    public Publication PublicationData
    {
        //Gets article's publication ID and finds publication info Publications list
        get { return Glob.Publications[ItemSource.PublisherID]; }
        set { SetValue(ItemSourceProperty, value); }
    }

    /// <summary>
    /// Name of the publication
    /// i.e. BBC News
    /// </summary>
    public String Publisher
    {
        get
        {
            try { return PublicationData.Name; }
            catch { return "Unknown Publisher"; }
        }
    }

    /// <summary>
    /// Gets the article was published.
    /// i.e. 19 Hours ago
    /// </summary>
    public String TimeFromPublication
    {
        get
        {
            try
            {
                //Calculate how long it's been since the article was published.
                TimeSpan Diff = DateTime.Now - ((Article)GetValue(ItemSourceProperty)).PublishDate;

                //If it's been published for less than an hour, show the article in Minutes
                if (Diff.TotalMinutes < 60)
                {
                    return Math.Round(Diff.TotalMinutes) + " Minutes ago";
                }

                //If it's older than 2 days, show the article in hours
                if (Diff.TotalHours <= 48)
                {
                    return Math.Round(Diff.TotalHours) + " Hours ago";
                }
                return Math.Round(Diff.TotalDays) + " Days ago";
            }
            catch
            {
                return "Unknown Publication Date";
            }
        }
    }

    /// <summary>
    /// Gets publication favicon
    /// i.e. https://edition.cnn.com/favicon.ico
    /// </summary>
    public String PublisherIcon
    {
        get
        {
            try
            {
                return PublicationData.Favicon;
            }
            catch (Exception e)
            {
                Glob.Log(Glob.LogLevel.Wrn,
                    $"Failed to get publisher icon for {PublicationData.Favicon} " +
                    $"Exception {e.Message}\n\n{e.StackTrace}\n\n{e.Source}");
                return "Assets/QuestionMark.png";
            }
        }
    } 

    public PublisherInformation() => InitializeComponent();
}
