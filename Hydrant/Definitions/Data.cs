using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HYDRANT.Definitions;

public class Data
{
    /// <summary>
    /// Used to track if API is compatible.
    /// </summary>
    public const int ApiVersion = 5;

    /// <summary>
    /// Is the token valid
    /// </summary>
    public bool TokenValid { get; set; }
    
    /// <summary>
    /// Lowest Acceptable version
    /// </summary>
    public int MinimumVersion { get; set; }

    /// <summary>
    /// Is token allowed accessed to /Engine/
    /// </summary>
    public bool EngineAPI { get; set; }

    /// <summary>
    /// Available filters to user
    /// </summary>
    public List<Filter> Filters { get; set; }

    /// <summary>
    /// List of current Publications
    /// </summary>
    public List<Publication> Publications { get; set; }

}
