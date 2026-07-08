<script setup lang="ts">
import { ref, nextTick, watch } from 'vue';
import { useChat } from './composables/useChat';

const { messages, isLoading, runtimeError, postUserPrompt, flushChatHistory } = useChat();
const userInput = ref<string>('');
const scrollingPanel = ref<HTMLElement | null>(null);

const handleDispatch = async (): Promise<void> => {
  const messageText = userInput.value;
  if (!messageText.trim() || isLoading.value) return;

  userInput.value = '';
  await postUserPrompt(messageText);
};

watch(
  () => messages.value.length,
  async () => {
    await nextTick();
    if (scrollingPanel.value) {
      scrollingPanel.value.scrollTop = scrollingPanel.value.scrollHeight;
    }
  },
  { deep: true }
);
</script>

<template>
  <div class="workspace-wrapper">
    <header class="app-navbar">
      <div class="brand">
        <h2>Gemini AI Architect Pro</h2>
        <span class="pill">Vue 3 + TS</span>
      </div>
      <button class="btn-clear" @click="flushChatHistory" :disabled="isLoading">
        Reset Thread
      </button>
    </header>

    <div ref="scrollingPanel" class="conversation-panel">
      <div v-if="messages.length === 0" class="empty-state-notice">
        <h3>System Active</h3>
        <p>Injected Identity Profile: Pragmatic Senior Software Engineering Mentor</p>
      </div>

      <div 
        v-for="(msg, index) in messages" 
        :key="index" 
        :class="['message-row', msg.role]"
      >
        <div class="message-bubble">
          <header class="meta">
            {{ msg.role === 'user' ? 'Client Request' : 'Mentor System Output' }}
          </header>
          <p class="content-text">{{ msg.content || '▋' }}</p>
        </div>
      </div>
    </div>

    <footer class="input-container">
      <form @submit.prevent="handleDispatch" class="interactive-form">
        <input 
          v-model="userInput"
          type="text"
          placeholder="Query model or command software workflow executions..."
          :disabled="isLoading"
        />
        <button type="submit" :disabled="isLoading || !userInput.trim()">
          {{ isLoading ? 'Processing...' : 'Execute' }}
        </button>
      </form>
      <p v-if="runtimeError" class="alert-banner">{{ runtimeError }}</p>
    </footer>
  </div>
</template>

<style scoped>
.workspace-wrapper { display: flex; flex-direction: column; height: 100vh; max-width: 1000px; margin: 0 auto; padding: 1rem; box-sizing: border-box; background-color: #121212; color: #e0e0e0; font-family: ui-sans-serif, system-ui, sans-serif; }
.app-navbar { display: flex; justify-content: space-between; align-items: center; padding-bottom: 1rem; border-bottom: 1px solid #2d2d2d; }
.brand { display: flex; align-items: center; gap: 0.75rem; }
.brand h2 { margin: 0; font-size: 1.25rem; font-weight: 600; color: #fff; }
.pill { font-size: 0.7rem; background-color: #1e1e1e; padding: 0.25rem 0.5rem; border-radius: 4px; border: 1px solid #333; color: #888; }
.btn-clear { background: transparent; border: 1px solid #cc3333; color: #cc3333; padding: 0.4rem 0.8rem; border-radius: 4px; cursor: pointer; transition: all 0.2s; }
.btn-clear:hover:not(:disabled) { background: #cc3333; color: #fff; }
.btn-clear:disabled { opacity: 0.3; cursor: not-allowed; }
.conversation-panel { flex: 1; overflow-y: auto; margin: 1rem 0; padding: 1.5rem; background: #1a1a1a; border: 1px solid #2d2d2d; border-radius: 8px; display: flex; flex-direction: column; gap: 1.25rem; }
.empty-state-notice { text-align: center; margin-top: 5rem; color: #666; }
.empty-state-notice h3 { color: #444; margin-bottom: 0.5rem; }
.message-row { display: flex; width: 100%; }
.message-row.user { justify-content: flex-end; }
.message-bubble { max-width: 80%; padding: 0.85rem 1.1rem; background: #242424; border-radius: 8px; border: 1px solid #2d2d2d; }
.user .message-bubble { background: #005c99; border-color: #007acc; color: #fff; }
.meta { font-size: 0.7rem; font-weight: 700; text-transform: uppercase; letter-spacing: 0.05em; opacity: 0.6; margin-bottom: 0.35rem; }
.content-text { margin: 0; white-space: pre-wrap; line-height: 1.5; font-size: 0.95rem; }
.input-container { background: #121212; padding-top: 0.5rem; }
.interactive-form { display: flex; gap: 0.75rem; }
input { flex: 1; background: #1a1a1a; border: 1px solid #2d2d2d; border-radius: 6px; padding: 0.85rem; color: #fff; font-size: 0.95rem; transition: border 0.2s; }
input:focus { outline: none; border-color: #007acc; }
button[type="submit"] { background: #007acc; border: none; color: #fff; font-weight: 600; padding: 0 1.75rem; border-radius: 6px; cursor: pointer; transition: background 0.2s; }
button[type="submit"]:hover:not(:disabled) { background: #0099ff; }
button[type="submit"]:disabled { background: #242424; color: #555; cursor: not-allowed; }
.alert-banner { color: #ff3333; font-size: 0.85rem; margin-top: 0.5rem; text-align: center; }
</style>