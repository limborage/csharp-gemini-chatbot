namespace AiWebApi.Models;

public record ChatMessageDto(string Role, string Content);

public record ChatRequestDto(List<ChatMessageDto> Messages);