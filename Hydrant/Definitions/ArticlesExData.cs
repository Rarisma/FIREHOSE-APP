using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HYDRANT.Definitions;

public class ArticlesExData
{
    /// <summary>
    /// Article URL (primary key)
    /// </summary>
    [Key]
    [Column("URL", TypeName = "VARCHAR(512)")]
    public string URL { get; set; }
    /// <summary>
    /// Article Embeddings
    /// </summary>
    [Column("EMBEDDINGS", TypeName = "BLOB")]
    public string? Embeddings { get; set; }
    /// <summary>
    /// Time the summary was generated
    /// </summary>
    [Column("GEN_TIME", TypeName = "DATETIME")]
    public DateTime GenTime { get; set; }
    /// <summary>
    /// Reader mode used
    /// </summary>
    [Column("READER_MODE", TypeName = "VARCHAR(32)")]
    public string? ReaderMode { get; set; }
    /// <summary>
    /// Model used to generate summary
    /// </summary>
    [Column("MODEL", TypeName = "VARCHAR(32)")]
    public string? Model { get; set; }
    /// <summary>
    /// Original headline if the headline has been rewritten
    /// </summary>
    [Column("OLD_HEADLINE", TypeName = "VARCHAR(512)")]
    public string? OldHeadline{ get; set; }
    /// <summary>
    /// Has the original headline been rewritten?
    /// </summary>
    [Column("HEADLINE_REWRITTEN", TypeName = "BOOLEAN")]
    public bool? HeadlineRewritten { get; set; }
}
