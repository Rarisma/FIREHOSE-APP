using HYDRANT.Serializers;
using System.Text.Json;
//D4RK SIDE
namespace HYDRANT.Definitions;
/// <summary>
/// Used to filter articles, populated from Filters.json
/// </summary>
public class Filter
{
    /// <summary>
    /// Name of the filter
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// LINQ that will be executed for filter
    /// </summary>
    public string LINQ { get; set; }

    /// <summary>
    /// Only returned to TestGroup users
    /// </summary>
    public bool Experimental { get; set; }
    
    /// <summary>
    /// Shown to nobody
    /// </summary>
    public bool Hidden { get; set; } 

    /// <summary>
    /// </summary>
    public bool Selected { get; set; }

    public static List<Filter> LoadFilters(string Path)
    {
        string content = File.ReadAllText(Path);
        return JsonSerializer.Deserialize(content, FilterSerializer.Default.ListFilter);
    }
}
