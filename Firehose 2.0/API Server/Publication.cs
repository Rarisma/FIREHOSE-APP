using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Firehose2.API_Server;

[ApiController]
[Route("[controller]")]
public class Publication : ControllerBase
{

    [HttpGet("GetPublicationData")]
    public IActionResult GetPublicationData()
    {

        try
        {
            // ReSharper disable once RedundantTypeArgumentsOfMethod
            return Ok(JsonSerializer.Serialize<List<HYDRANT.Publication>>(Program.Publications));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
