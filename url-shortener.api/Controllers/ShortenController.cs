using MediatR;
using Microsoft.AspNetCore.Mvc;
using url_shortener.api.Features.ShortenedUrl;

namespace url_shortener.api.Controllers;

public class ShortenController(ISender sender) : ControllerBase
{
    [HttpGet("{shortCode}")]
    public async Task<ActionResult> Index(string shortCode)
    {
        var query = GetShortenedUrl.Query.Create(shortCode);
        var result = await sender.Send(query);
        if (!result.IsSuccess)
        {
            return BadRequest();
        }
        var visitCommand = AccessShortenedUrl.Command.Create(result.Value.Id, 
            Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");
        var visitResult = await sender.Send(visitCommand);
        return Redirect(result.Value.Url.ToString());
    }
}