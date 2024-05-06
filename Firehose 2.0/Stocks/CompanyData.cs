using HtmlAgilityPack;
namespace Firehose2.Stocks;

/// <summary>
/// Open Source Reimplementation of Ticker Name Service and Ticker lib
/// </summary>
internal class CompanyData
{
    public static List<Company> Tickers = new();

    private static readonly HttpClient client = new();

    public static async Task<Company[]> GetFtse100Tickers() => await GetTickers("https://en.wikipedia.org/wiki/FTSE_100_Index", "constituents", TickerSource.FTSE_100);

    /// <summary>
    /// Gets FTSE 250 Ticker Data.
    /// </summary>
    private static async Task<Company[]> GetFtse250Tickers() => await GetTickers("https://en.wikipedia.org/wiki/FTSE_250_Index", "constituents", TickerSource.FTSE_250);

    /// <summary>
    /// Gets Dow Jones Industrial Average Ticker Data
    /// </summary>
    private static async Task<Company[]> GetDowTickers() => await GetTickers("https://en.wikipedia.org/wiki/Dow_Jones_Industrial_Average", "constituents",TickerSource.DowJones);

    /// <summary>
    /// Gets Nifty 50 Ticker information
    /// </summary>
    private static async Task<Company[]> GetNifty50Tickers() => await GetTickers("https://en.wikipedia.org/wiki/NIFTY_50", "constituents",TickerSource.Nifty50);

    private static async Task<Company[]> GetSP500Tickers()
    {
        var url = "https://en.wikipedia.org/wiki/List_of_S%26P_500_companies";
        // This will need a different table ID or handling as the S&P 500 might have a different table structure.
        var tickers = await GetTickers(url, "constituents", TickerSource.FTSE_350);  // Check actual table ID on the webpage
        return tickers;
    }

    /// <summary>
    /// Scrapes Ticker Data from a URL and Table.
    /// </summary>
    /// <param name="url"></param>
    /// <param name="tableId"></param>
    /// <returns></returns>
    private static async Task<Company[]> GetTickers(string url, string tableId, TickerSource Source)
    {
        var response = await client.GetAsync(url);
        var pageContents = await response.Content.ReadAsStringAsync();

        HtmlDocument htmlDoc = new();
        htmlDoc.LoadHtml(pageContents);

        var table = htmlDoc.DocumentNode.SelectSingleNode($"//table[@id='{tableId}']");
        var rows = table.SelectNodes(".//tr");

        List<Company> tickers = new();

        for (int i = 1; i < rows.Count; i++)  // Start at 1 to skip the header
        {
            var cells = rows[i].SelectNodes(".//td");
            var tickerSuffix = cells[1].InnerText.Trim().EndsWith(".") ? "L" : ".L";
            tickers.Add(new()
            {
                Ticker = cells[1].InnerText.Trim() + tickerSuffix,
                Name = cells[0].InnerText.Trim(),
                Excectives = [],
                Source = Source
            });
        }

        return tickers.ToArray();
    }

    public static async void LoadTickers()
    {
        //Load FTSE100
        foreach (var ticker in await GetFtse100Tickers()) { Tickers.Add(ticker); }

        //Load FTSE250
        foreach (var ticker in await GetFtse250Tickers()) { Tickers.Add(ticker); }

        //Load Dow Jones
        foreach (var ticker in await GetDowTickers()) { Tickers.Add(ticker); }

        //Nifty 50
        foreach (var ticker in await GetNifty50Tickers()) { Tickers.Add(ticker); }
    }
}
