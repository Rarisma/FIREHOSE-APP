using System.Text.Json;
using HYDRANT.Definitions;
using Microsoft.AspNetCore.Mvc;

namespace FirehoseServer.Controllers;

[ApiController]
[Route("[controller]")]
public class ArticlesController : ControllerBase
{

    [HttpGet("GetArticles")]
    public IActionResult GetArticles(int limit = 200, int Offset = 0, string Filter = "")
    {
        try
        {
			// ReSharper disable once RedundantTypeArgumentsOfMethod
			if (Filter == "")
			{
				return Ok(JsonSerializer.Serialize<List<Article>>(Program.MySQL.GetArticles(limit, Offset)));
			}
			else
			{
				return Ok(JsonSerializer.Serialize<List<Article>>(Program.MySQL.GetArticles(limit, Offset, Filter: Filter)));
			}
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
            return Ok(Program.MySQL.GetURLs());
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
