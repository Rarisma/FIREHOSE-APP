using System;
using System.Text.Json.Nodes;
using HtmlAgilityPack;
using HYDRANT.Definitions;
using Newtonsoft.Json;

namespace FirehoseServer.Stocks;


internal class CompanyData
{
    public static List<Company> Tickers = new();

    private static readonly HttpClient client = new();

    public static async Task LoadTickers()
    {
        string[] Exchanges = ["NASDAQ", "NASDAQ_OTHER", "NYSE", "LSE"];
        string[] Indexes = ["SP500","DOW", "NIFTY50", "FTSE100", "FTSE250"];
        
        foreach (var index in Indexes)
        {
            //Get names and tickers of
            List<string> Names = JsonConvert.DeserializeObject<List<string>>(await client.GetStringAsync($"http://127.0.0.1:5002/index/{index}/names"));
            List<string> Ticker = JsonConvert.DeserializeObject<List<string>>(await client.GetStringAsync($"http://127.0.0.1:5002/index/{index}/tickers"));
            for (int i = 0; i < Names.Count; i++)
            {
                try
                {
                    // Check ticker isn't bad
                    if (string.IsNullOrWhiteSpace(Names[i]) || string.IsNullOrWhiteSpace(Ticker[i]))
                    {
                        continue;
                    }
                    
                    //Convert to ticker object
                    Tickers.Add(new()
                    {
                        Excectives = [],
                        Name = Names[i],
                        Ticker = Ticker[i],
                        Source = index
                    });
                }
                catch
                {
                    Console.WriteLine($"Failed to load index {index} at index {i}");
                }
            }
        }
        
        foreach (var Exchange in Exchanges)
        {
            //Get names and tickers of
            List<string> Names = JsonConvert.DeserializeObject<List<string>>(await client.GetStringAsync($"http://127.0.0.1:5002/exchange/{Exchange}/names"));
            List<string> Ticker = JsonConvert.DeserializeObject<List<string>>(await client.GetStringAsync($"http://127.0.0.1:5002/exchange/{Exchange}/tickers"));
            

            for (int i = 0; i <= Names.Count; i++)
            {
                try
                {
                    // Check ticker isn't bad
                    if (string.IsNullOrWhiteSpace(Names[i]) || string.IsNullOrWhiteSpace(Ticker[i])) { continue; }
                    Tickers.Add(new()
                    {
                        Excectives = [],
                        Name = Names[i].Replace("[", "").Replace("]", ""),
                        Ticker = Ticker[i].Replace("[", "").Replace("]", ""),
                        Source = Exchange
                    });
                }
                catch
                {
                    Console.WriteLine($"Failed to load exchange {Exchange} at index {i}");
                }

            }
        }
    }
}
