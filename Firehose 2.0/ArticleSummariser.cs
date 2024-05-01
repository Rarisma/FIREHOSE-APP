using HYDRANT;

namespace Firehose2;
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
                Article.Headline = await Headline(Article.ArticleText, ID);
                Article.Summary = await Summarise(Article.ArticleText, ID);
                
                //Post to database.
                Article.Upload();
                TotalScraped++;
            }
        }
    }

    public static async Task<string> Summarise(string Text, int ID)
    {
        return await LLM.SendPostRequest("""
                                         Your task is to create concise summaries for provided stories, each confined to a single paragraph.
                                         Begin each summary by directly stating the main points and essence of the narrative.
                                         Ensure that your response is formatted in plain text, without the use of any markdown or special formatting.
                                         Follow these guidelines:
                                         
                                         Your summary must consist of only one paragraph.
                                         Start directly with the narrative content, omitting any introductory phrases or labels.
                                         Do not include any headings, titles, or labels within your response.
                                         Write solely in plain text format.
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
                        Analyze the text below and determine if it qualifies as breaking news. Consider elements such as recency, relevance, urgency, and the presence of significant events. Respond with a single word: "Yes" if it is breaking news, or "No" if it is not.
                        Text for analysis:Analyze the text below and determine if it qualifies as breaking news. Consider elements such as recency, relevance, urgency, and the presence of significant events. Respond with a single word: "Yes" if it is breaking news, or "No" if it is not.
                        """, $"Text for analysis:{Text}", ID);


        return Response.ToLower() == "yes";
    }
}
