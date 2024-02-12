using System.Text.Json.Serialization;

namespace HYDRANT.Definitions;

/// <summary>
/// Company information
/// </summary>
[JsonSerializable(typeof(List<Company>))]
[JsonSerializable(typeof(Company))]
public class Company
{
    /// <summary>
    /// Name of company
    /// i.e. Microsoft
    /// </summary>
    public string Name;

    /// <summary>
    /// Ticker name
    /// i.e. MSFT
    /// </summary>
    public string Ticker;

    /// <summary>
    /// List of Executives
    /// </summary>
    public string[] Excectives;

    /// <summary>
    /// Source of the ticker, FTSE, NASDAQ etc.
    /// </summary>
    public TickerSource Source;

    /// <summary>
    /// locates companies within text
    /// </summary>
    /// <returns></returns>
    public static string FindCompaniesInText(string text, List<Company> companies)
    {
        //Scan text
        var Companies = companies.Where(company => text.Contains(company.Name) || text.Contains(company.Ticker));
        string Found = "";

        //return results
        foreach (var Comp in Companies.ToList())
        {
            Found += $"{Comp.Ticker} ({Comp.Name}),";
        }

        return Found;
    }
}