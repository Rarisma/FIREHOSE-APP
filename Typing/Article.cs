namespace REMNANT.Typing;

public class Article
{
    public string Url { get; set; }
    public string Title { get; set; }
    public string RssSummary { get; set; }
    public DateTime PublishDate { get; set; }
    public bool LowQuality { get; set; }
    public bool Paywall { get; set; }
    public string Summary { get; set; }
}