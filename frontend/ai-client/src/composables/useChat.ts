import { ref, readonly } from 'vue';
import { streamChatCompletion, type ChatMessage } from '../services/chatClient';

export function useChat() {
  const _messages = ref<ChatMessage[]>([]);
  const _isLoading = ref<boolean>(false);
  const _runtimeError = ref<string | null>(null);

  async function postUserPrompt(promptText: string): Promise<void> {
    const sanitizedPrompt = promptText.trim();
    if (!sanitizedPrompt || _isLoading.value) return;

    _runtimeError.value = null;
    _messages.value.push({ role: 'user', content: sanitizedPrompt });
    
    const responseTrackerIndex = _messages.value.push({ role: 'assistant', content: '' }) - 1;
    _isLoading.value = true;

    const payload = {
      messages: [..._messages.value.slice(0, -1)]
    };

    await streamChatCompletion(
      payload,
      (incomingToken: string) => {
        console.log(incomingToken);
        _messages.value[responseTrackerIndex].content += incomingToken;
      },
      (error: any) => {
        console.error("Stream channel error:", error);
        _runtimeError.value = "An error occurred while streaming the response.";
        _messages.value[responseTrackerIndex].content = "⚠️ Engine failed to finalize response transmission.";
      }
    );

    _isLoading.value = false;
  }

  function flushChatHistory(): void {
    _messages.value = [];
    _runtimeError.value = null;
    _isLoading.value = false;
  }

  return {
    messages: readonly(_messages),
    isLoading: readonly(_isLoading),
    runtimeError: readonly(_runtimeError),
    postUserPrompt,
    flushChatHistory
  };
}