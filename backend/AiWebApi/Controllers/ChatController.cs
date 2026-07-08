using AiWebApi.Models;
using AiWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

namespace AiWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("VueCors")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
    }

    [HttpPost("stream")]
    public async Task GetChatStream([FromBody] ChatRequestDto request)
    {
        if (request?.Messages == null || !request.Messages.Any())
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            await Response.WriteAsync("data: Error: Empty message payload\n\n");
            return;
        }

        Response.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        Response.Headers.Connection = "keep-alive";

        try
        {
            var tokenStream = _chatService.StreamChatAsync(request.Messages);

            await foreach (var token in tokenStream)
            {
                await Response.WriteAsync($"data: {Uri.EscapeDataString(token)}\n\n");
                await Response.Body.FlushAsync();
            }
        }
        catch (Exception ex)
        {
            await Response.WriteAsync($"data: {Uri.EscapeDataString($"Internal Server Error: {ex.Message}")}\n\n");
            await Response.Body.FlushAsync();
        }
    }
}