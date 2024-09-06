using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HYDRANT.Definitions;

/// <summary>
/// News Article Object definition
/// (All fields are matched to the DB if you capitalize them and split words with _)
/// </summary>
[Table("ARTICLES_NEW")]
public class Article
{
    /// <summary>
    /// Article.URL
    /// </summary>
    [Key]
    [Column("URL", TypeName = "VARCHAR(512)")]
    public string URL { get; set; }
    
    /// <summary>
    /// Article Title
    /// </summary>
    [Column("TITLE", TypeName = "VARCHAR(512)")]
    public string Title { get; set; }
    
    /// <summary>
    /// Date Time of when Article was Published
    /// </summary>
    [Column("PUBLISHED", TypeName = "DATETIME")]
    public DateTime Published { get; set; }
    
    /// <summary>
    /// Summary of Article.
    /// </summary>
    [Column("SUMMARY", TypeName = "LONGTEXT")]
    public string Summary { get; set; }
    
    /// <summary>
    /// Image.URL
    /// </summary>
    [Column("IMAGE", TypeName = "VARCHAR(1024)")]
    public string Image { get; set; }
    
    /// <summary>
    /// Article Content
    /// </summary>
    [Column("CONTENT", TypeName = "LONGTEXT")]
    public string Content { get; set; }
    
    /// <summary>
    /// FHN Publisher ID
    /// </summary>
    [Column("PUBLISHER")]
    public int Publisher { get; set; }
    
    /// <summary>
    /// Author Name(s)
    /// </summary>
    [Column("AUTHOR", TypeName = "VARCHAR(1024)")]
    public string Author { get; set; }
    
    /// <summary>
    /// Amount of clickbait reports
    /// </summary>
    [Column("CLICKBAIT_COUNTER", TypeName = "INT")]
    public int ClickbaitCounter { get; set; } = 0;
    
    /// <summary>
    /// Amount of Summary Reports
    /// </summary>
    [Column("REPORT_COUNTER", TypeName = "INT")]
    public int ReportCounter { get; set; } = 0;
    
    /// <summary>
    /// Impact score
    /// 0 - irrelevant, 50 - Breaking News
    /// </summary>
    [Column("IMPACT", TypeName = "INT")]
    public int Impact { get; set; }

    /// <summary>
    /// Impact score
    /// 0 - irrelevant, 50 - Breaking News
    /// </summary>
    [Column("CATEGORY", TypeName = "VARCHAR(128)")]
    public string Category { get; set; }

    /// <summary>
    /// Time to read in minutes
    /// </summary>
    [Column("READ_TIME", TypeName = "INT")]
    public int ReadTime { get; set; } = 0;
    
    /// <summary>
    /// Amount of views all time
    /// </summary>
    [Column("VIEWSALL", TypeName = "INT")]
    public int ViewsAll { get; set; } = 0;
    
    /// <summary>
    /// Amount of views within the last 24hrs approximate.
    /// </summary>
    [Column("VIEWSDAY", TypeName = "INT")]
    public int ViewsDay { get; set; } = 0;
    
    public ArticleType Type
    {
        get
        {
            if (URL.Contains(".youtube.") || URL.Contains(".youtu.be"))
            {
                return ArticleType.Video;
            }
            else if (URL.Contains("http"))
            {
                return ArticleType.Article;
            }
            else if (URL.Contains(@"C:\"))
            {
                return ArticleType.File;
            }
            else
            {
                return ArticleType.Unknown;
            }
        }
    }
    
    /// <summary>
    /// Used for different kinds of articles
    /// </summary>
    public enum ArticleType
    {
        /// <summary>
        /// Article is a web article
        /// </summary>
        Article,
        
        /// <summary>
        /// Article is a video
        /// </summary>
        Video,
        
        /// <summary>
        /// Article is a PDF/DOCX/DOC file
        /// </summary>
        File,
        
        /// <summary>
        /// Type is unknown
        /// </summary>
        Unknown
    }
}
