using AiWebApi.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AiWebApi.Services;

public class GeminiChatService : IChatService
{
    private readonly IChatCompletionService _chatCompletionService;
    private readonly Kernel _kernel;

    public GeminiChatService(IChatCompletionService chatCompletion, Kernel kernel)
    { 
        _chatCompletionService = chatCompletion ?? throw new ArgumentNullException(nameof(chatCompletion));
        _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
    }

    public async IAsyncEnumerable<string> StreamChatAsync(List<ChatMessageDto> conversationHistory)
    {
        var history = new ChatHistory();
        history.AddSystemMessage("You are a helpful, pragmatic senior developer mentor.");

        foreach (var msg in conversationHistory) { 
            if (msg.Role.Equals("user", StringComparison.OrdinalIgnoreCase))
            {
                history.AddUserMessage(msg.Content);
            }
            else if (msg.Role.Equals("assistant", StringComparison.OrdinalIgnoreCase))
            {
                history.AddAssistantMessage(msg.Content);
            }
        }

        var chunks = _chatCompletionService.GetStreamingChatMessageContentsAsync(history, kernel: _kernel);

        await foreach (var chunk in chunks)
        {
            if (!string.IsNullOrEmpty(chunk.Content))
            {
                yield return chunk.Content;
            }
        }
    }
}