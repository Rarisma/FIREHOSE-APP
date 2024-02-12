using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace FirehoseServer.Controllers;

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
            return Ok(JsonSerializer.Serialize<List<HYDRANT.Definitions.Publication>>(Program.Publications));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
