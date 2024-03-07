using Microsoft.AspNetCore.Mvc;
using HYDRANT;
using System.Text.Json;
//GREENWALD API IS REAL 2401
namespace HallonAPIServer;

[ApiController]
[Route("[controller]")]
public class ArticlesController : ControllerBase
{

    [HttpGet("GetArticles")]
    public IActionResult GetArticles(int limit = 9999999)
    {
        try
        {
            return Ok(JsonSerializer.Serialize<List<Article>>(Article.FetchArticlesFromDB(limit)));
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

    // Assuming Article has properties for Upload method to work and it's part of Article class
    [HttpPost("Upload")]
    public IActionResult Upload([FromBody] Article article)
    {
        try
        {
            article.Upload();
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
