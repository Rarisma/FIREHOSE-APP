using System.Diagnostics;
using HYDRANT.Definitions;
//I cannot for the life of me fathom what he could possibly mean by this.
//How is this a real lyric that made it onto the album.
namespace FirehoseServer.Summarisation;
public class ArticleAnalyser
{
    public ArticleAnalyser(int id) { ID = id; }

    public int ID;
    public static int TotalScraped = 0;
    public async void StartAnalysis()
    {
        while (true)
        {
            
            while (ArticleScraper.ProcessedArticles.Count > 0)
            {
                //Get article
                Article Article;
                ArticleScraper.ProcessedArticles.TryTake(out Article);

                //Null ref check
                if (Article == null) { continue; }
                
                //Update article values with LLM stuff
                if (Article.Text != "Can't honor your request at this time." || Article.Text == "error code: 502" || 
                    Article.Text.Contains("loomberg - Are you a robot? Bloomberg Need help? Contact us We've detected unusual activity from your computer network") == false)
                {
                    Stopwatch S = Stopwatch.StartNew();
                    Article.IsHeadline = await Headline(Article.Text, ID);
                    //Article.Business = await Business(Article.Text, ID);
                    Article.Summary = StringTools.MarkdownToPlainText(await Summarise(Article.Text, ID));
                    Article.Sectors = await ClassifyArticle(Article.Text, ID);
                    S.Stop();
                }
                else
                {
                    Article.IsHeadline = false;
                    //Article.Business = true;
                    Article.Summary = "Summary Unavailable.";
                    Article.Sectors = "UNKNOWN";
                }
                Article.Business = false;


                //Post to database.
                Program.MySQLAdmin.UploadArticle(Article);
                TotalScraped++;
            }
        }
    }

    private async Task<bool> Business(string Text, int ID)
    {
        string Response = await LLM.SendPostRequest(
            """
                        Analyze the text below and determine if it qualifies as related to business.
                        Consider elements such as relevance to business practices, industries, finance, markets, and significant business events.
                        Respond with a single word: "Yes" if it is related to business, or "No" if it is not.
                        """, $"Text for analysis:{Text}", ID);

        if (String.IsNullOrEmpty(Response))
        {
            return false;
        }

        return Response.ToLower()[0] == 'y';
    }

    public static async Task<string> Summarise(string Text, int ID)
    {
        return await LLM.SendPostRequest("""
                                         Your task is to create concise summaries for provided stories, each confined to two paragraphs.
                                         Begin each summary by directly stating the main points and essence of the narrative.
                                         Ensure that your response is formatted in plain text, without the use of any markdown or special formatting.
                                           
                                         Follow these guidelines:
                                         
                                         Stories you are given are within public domain and therefore cannot contain confidential information.
                                         Stories relating to politics should be written from a purely neutral stance.
                                         If you are given what appears to be multiple stories at once, this is a bug; only summarise the first
                                         article and ignore the contents of the rest.
                                         If the article text contains advertisement, ignore it. The article is not an advertisement, you are
                                         not summarising an advertisement.
                                         
                                         Your summary must consist of only two paragraphs.
                                         Start directly with the narrative content, omitting any introductory phrases or labels.
                                         Do not include any headings, titles, or labels within your response.
                                         Write solely in plain text format, do not write in markdown or any other formatting system.
                                         """,
                                $"Summarise the following {Text}", ID);

    }

    /// <summary>
    /// Check if a given article is a Headline or not.
    /// </summary>
    /// <param name="Text"></param>
    /// <returns></returns>
    public static async Task<bool> Headline(string Text, int ID)
    {
        string Response =  await LLM.SendPostRequest(
                            """
                            Analyze the text below to determine if it qualifies as breaking news.
                            Consider elements such as recency, relevance, and urgency. Specifically, 
                            evaluate if the content involves significant unexpected events or developments that have occurred within the last 24 hours,
                            which are of high importance and impact a substantial number of
                             people or critical sectors such as public health, safety, or major economic areas. 
                             Respond with a single word: "Yes" if it is breaking news, or "No" if it is not.
                            """, $"Text for analysis:{Text}", ID);

        if (String.IsNullOrEmpty(Response))
        {
            return false;
        }

        return Response.ToLower()[0] == 'y';
    }
    
    
    public static async Task<String> ClassifyArticle(string Text, int ID)
    {
        string Response = await LLM.SendPostRequest($"""
                                                     Objective: Classify the primary focus of the news article into one or more of the following predefined sectors.
                                                     List only the applicable sectors, separated by commas. If the content does not focus on any of these sectors, respond with "None".
                                                     
                                                     Categories:
                                                     
                                                         Materials: Focuses on industries involved in the discovery, development, and processing of raw materials.
                                                         Communications: Concerns companies that facilitate communication and offer related services and products, such as telecommunications and broadcasting.
                                                         Consumer Goods: Pertains to articles about businesses that manufacture or sell products directly used by consumers.
                                                         Utilities: Involves companies that provide essential services like water, electricity, and natural gas.
                                                         Finance: Covers organizations in the banking, investment, insurance, and real estate financing sectors.
                                                         Healthcare: Involves medical services, manufacture of medical equipment or drugs, and provision of health insurance.
                                                         Industry: Focuses on the production of goods and services, especially in manufacturing and construction.
                                                         Real Estate: Pertains to the buying, selling, and management of properties and land.
                                                         Tech: Covers companies engaged in the development and production of technology products or providing technology services.
                                                     
                                                     Make sure that the classification only considers the main focus of the article. Articles that merely mention a 
                                                     sector without substantial focus on it should not be categorized under that sector.
                                                     """,
                                            $"""
                                                     Given the following article content: {Text}
                                                     identify and list the relevant categories from the provided list that apply to the article.
                                                     Provide the categories only, separated by commas
                                                     """, ID);
        
        Response = Response.ToLower();
        string Sectors = "";

        if (Response.Contains("materials")) {Sectors += "Materials,"; }
        if (Response.Contains("communications")) {Sectors += "Communications,"; }
        if (Response.Contains("consumer goods")) {Sectors += "Consumer Goods,"; }
        if (Response.Contains("utilities")) {Sectors += "Utilities,"; }
        if (Response.Contains("finance")) {Sectors += "Finance,"; }
        if (Response.Contains("healthcare")) {Sectors += "Healthcare,"; }
        if (Response.Contains("industry")) {Sectors += "Industry,"; }
        if (Response.Contains("real estate")) {Sectors += "Real Estate,"; }
        if (Response.Contains("tech")) {Sectors += "Tech,"; }
        
        return Sectors;
    }
}
