using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HYDRANT;

namespace Firehose2;
internal class ArticleAnalyser
{
    public static async void StartAnalysis()
    {
        while (true)
        {
            
            while (ArticleScraper.ProcessedArticles.Count > 0)
            {
                //Get article
                Article Article = ArticleScraper.ProcessedArticles.First();

                //Null ref check
                if (Article == null) { continue; }
                
                ArticleScraper.ProcessedArticles.Remove(Article);
                
                //Update article values with LLM stuff
                Article.Headline = await Headline(Article.Content);
                Article.Summary = await Summarise(Article.Content);
                
                //Post to database.
                Article.Upload();
            }
        }
    }

    public static async Task<string> Summarise(string Text)
    {
        return await LLM.SendPostRequest("""
                                         Your task is to craft concise summaries for provided stories, each limited to a single paragraph.
                                         Start each summary with the word 'Summary:' followed by the key points and narrative essence of the story.
                                         Ensure you exclude any headings, titles, or introductory labels other than 'Summary:'.
                                         The objective is to succinctly capture the core details and relevant content of each story.

                                         Your output MUST be no more than a paragraph.
                                         Your output MUST just be the summary.
                                         Your output MUST NOT start with summary: or any other introduction to the summary.
                                         """,
                                $"Summarise the following {Text}" );

    }

    /// <summary>
    /// Check if a given article is a Headline or not.
    /// </summary>
    /// <param name="Text"></param>
    /// <returns></returns>
    private static async Task<bool> Headline(string Text)
    {
        string Response =  await LLM.SendPostRequest(
                        """
                        Analyze the text below and determine if it qualifies as breaking news. Consider elements such as recency, relevance, urgency, and the presence of significant events. Respond with a single word: "Yes" if it is breaking news, or "No" if it is not.
                        Text for analysis:Analyze the text below and determine if it qualifies as breaking news. Consider elements such as recency, relevance, urgency, and the presence of significant events. Respond with a single word: "Yes" if it is breaking news, or "No" if it is not.
                        """,
                        $"Text for analysis:{Text}"
            );


        return Response.ToLower() == "yes";
    }
}
