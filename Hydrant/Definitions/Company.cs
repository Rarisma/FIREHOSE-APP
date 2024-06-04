namespace HYDRANT.Definitions;

/// <summary>
/// Company information
/// </summary>
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
    public string Source;

    /// <summary>
    /// locates companies within text
    /// </summary>
    /// <returns></returns>
    public static string FindCompaniesInText(string text, List<Company> companies)
    {
        List<Company> Companies = new();

        foreach (var comp in companies)
        {
            if (comp.Name.Length > 3) // Company name must be over 3 chars
            {
                if (text.Contains(comp.Name)) { Companies.Add(comp); }
            }
            
            if (comp.Ticker.Length >= 2) // Check ticker is at least 2 char
            {
                if (text.Contains(comp.Ticker)) { Companies.Add(comp); }
            }
        }
        
        //Scan text
        string Found = "";

        //return results
        foreach (var Comp in Companies.ToList())
        {
            Found += $"{Comp.Ticker} ({Comp.Name}),";
        }

        return Found;
    }
}
