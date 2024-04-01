namespace VESTIGENEWS.Controls;
//
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

    public Article ItemSource
    {
        get { return (Article)GetValue(ItemSourceProperty); }
        set { SetValue(ItemSourceProperty, value); }
    }

    public String Publisher
    {
        get
        {
            try { return ItemSource.Publisher; }
            catch { return "Unknown Publisher"; }
        }
    }

    public String TimeFromPublication
    {
        get
        {
            try
            {
                //Calculate how long it's been since the article was published.
                TimeSpan Diff = DateTime.Now - ItemSource.PublishDate;

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

    public String PublisherIcon
    {
        get
        {
            try
            {
                return ItemSource.PublisherIcon;
            }
            catch (Exception e)
            {
                return "Assets/QuestionMark.png";
            }
        }
    } 

    public PublisherInformation() => InitializeComponent();
}
