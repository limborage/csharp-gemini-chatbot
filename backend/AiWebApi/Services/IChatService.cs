using AiWebApi.Models;

namespace AiWebApi.Services;

public interface IChatService
{
	IAsyncEnumerable<string> StreamChatAsync(List<ChatMessageDto> conversationHistory);
}