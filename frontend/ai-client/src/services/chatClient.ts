export interface ChatMessage {
  role: 'user' | 'assistant';
  content: string;
}

export interface ChatRequestPayload {
  messages: ChatMessage[];
}

const API_BASE_URL = 'http://localhost:5000/api/chat';

export async function streamChatCompletion(
  payload: ChatRequestPayload,
  onTokenReceived: (token: string) => void,
  onError: (error: any) => void
): Promise<void> {
  try {
    
    const response = await fetch(`${API_BASE_URL}/stream`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    });

    if (!response.ok) {
      throw new Error(`HTTP network failure status: ${response.status}`);
    }

    const reader = response.body?.getReader();
    const decoder = new TextDecoder('utf-8');

    if (!reader) {
      throw new Error('Response payload stream reader could not be instantiated.');
    }

    while (true) {
      const { value, done } = await reader.read();
      if (done) break;

      const chunkString = decoder.decode(value, { stream: true });
      const rawLines = chunkString.split('\n');

      for (const line of rawLines) {
        if (line.startsWith('data: ')) {
          const escapedToken = line.replace('data: ', '').trim();
          console.log(line);
          console.log(escapedToken);
          const cleanToken = decodeURIComponent(escapedToken);

          if (cleanToken) {
            onTokenReceived(cleanToken);
          }
        }
      }
    }
  } catch (err) {
    onError(err);
  }
}