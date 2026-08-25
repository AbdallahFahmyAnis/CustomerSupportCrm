export type ChatSource = { id: string; title: string };

export type ChatResponse = {
  reply: string;
  sources: ChatSource[];
  sessionId: string | null;
};
