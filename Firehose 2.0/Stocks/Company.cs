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
}
