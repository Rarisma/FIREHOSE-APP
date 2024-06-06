using System.Text.Json;
using HYDRANT.Definitions;
using Microsoft.AspNetCore.Mvc;

namespace FirehoseServer.Controllers;

[ApiController]
[Route("[controller]")]
public class ArticlesController : ControllerBase
{

    [HttpGet("GetArticles")]
    public IActionResult GetArticles(int limit = 200, int Offset = 0, string Filter = "", bool Minimal = false)
    {
        try
        {
			// ReSharper disable once RedundantTypeArgumentsOfMethod
			if (Filter == "")
			{
				return Ok(JsonSerializer.Serialize<List<Article>>(Program.MySQLReadOnly.GetArticles(limit, Offset, Minimal:Minimal)));
			}
			else
			{
				return Ok(JsonSerializer.Serialize<List<Article>>(Program.MySQLReadOnly.GetArticles(limit, Offset, Filter: Filter, Minimal)));
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
            return Ok(Program.MySQLReadOnly.GetURLs());
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

    /// <summary>
    /// Report an article as clickbait.
    /// </summary>
    /// <param name="URL">URL to report summary for</param>
    [HttpGet("ReportAsClickbait")]
    public IActionResult ReportClickbait(string URL)
    {
        Program.MySQLAdmin.IncrementClickbaitCounter(URL);
        return StatusCode(200);
    }
}
