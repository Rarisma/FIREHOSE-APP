namespace Firehose2.Stocks;
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

    public static string FindCompaniesInText(string text, List<Company> companies)
    {
        var c = companies.Where(company => text.Contains(company.Name) || text.Contains(company.Ticker)).ToList();
        string R = "";
        foreach (var Comp in c)
        {
            R += $"{Comp.Ticker} ({Comp.Name}),";
        }

        return R;
    }
}
