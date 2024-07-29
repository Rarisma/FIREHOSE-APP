using HYDRANT.Serializers;
using System.Text.Json;

namespace HYDRANT.Definitions;
public class Filter
{
 
    /// <summary>
    /// Name of the filter
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// SQL that will be executed for filter
    /// </summary>
    public string SQL { get; set; }

    /// <summary>
    /// Only returned to TestGroup users
    /// </summary>
    public bool Experimental { get; set; }
    
    /// <summary>
    /// Shown to nobody
    /// </summary>
    public bool Hidden;

    public static List<Filter> LoadFilters(string Path)
    {
        string content = File.ReadAllText(Path);
        return JsonSerializer.Deserialize(content, FilterSerializer.Default.ListFilter);
    }
}
