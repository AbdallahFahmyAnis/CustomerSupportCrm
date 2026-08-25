export interface CrmChatMessage {
  id: string;
  body: string;
  timeLabel?: string;
  /** When true, aligns as outbound / “mine” bubble. */
  mine?: boolean;
  meta?: string;
}
