using HYDRANT;
using HYDRANT.Definitions;

namespace FirehoseServer.Summarisation;
public class ArticleAnalyser
{
    public ArticleAnalyser(int id)
    {
        ID = id;
    }

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
                if (Article.Text != "Can't honor your request at this time." || Article.Text == "error code: 502")
                {
                    Article.IsHeadline = await Headline(Article.Text, ID);
                    Article.Business = await Business(Article.Text, ID);
                    Article.Summary = StringTools.MarkdownToPlainText(await Summarise(Article.Text, ID));
                }
                else
                {
                    Article.IsHeadline = false;
                    Article.Business = true;
                    Article.Summary = "Summary Unavailable.";
                }

                
                //Post to database.
				Program.MySQL.UploadArticle(Article);
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
                                         Your task is to create concise summaries for provided stories, each confined to three paragraph.
                                         Begin each summary by directly stating the main points and essence of the narrative.
                                         Ensure that your response is formatted in plain text, without the use of any markdown or special formatting.
                                           
                                         Follow these guidelines:
                                         
                                         Stories you are given are within public domain and therefore cannot contain confidential information.
                                         Stories relating to politics should be written from a purely neutral stance.
                                         
                                         Your summary must consist of only three paragraphs.
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
    private static async Task<bool> Headline(string Text, int ID)
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
}
