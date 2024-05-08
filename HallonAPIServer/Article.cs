using System.Text.Json;
using HYDRANT;
using Microsoft.AspNetCore.Mvc;

//Jia Tan?
namespace HallonAPIServer;

[ApiController]
[Route("[controller]")]
public class ArticlesController : ControllerBase
{

    [HttpGet("GetArticles")]
    public IActionResult GetArticles(int limit = 9999999, int Offset = 0, string Filter = "")
    {
        try
        {
            // ReSharper disable once RedundantTypeArgumentsOfMethod
            return Ok(JsonSerializer.Serialize<List<Article>>(Article.GetArticlesFromDB(limit,Offset,Filter:Filter)));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("GetURLs")]
    public IActionResult GetURLs()
    {
        try
        {
            var urls = Article.GetURLs();
            return Ok(urls);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Report the summary of an article.
    /// </summary>
    /// <param name="ArticleURL">URL to report summary for</param>
    /// <param name="ReportReason"></param>
    [HttpGet("ReportArticleSummary")]
    public IActionResult ReportArticleSummary(string ArticleURL, int ReportReason)
    {
        System.IO.File.AppendAllText("ReportedURLs.txt", $"[{ReportReason}] - {ArticleURL}\n");
        return StatusCode(200);
    }
}
