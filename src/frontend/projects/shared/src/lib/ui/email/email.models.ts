export interface CrmEmailMessage {
  id: string;
  /** Display name in list + message card header. */
  fromName: string;
  /** Email / address line under the name. */
  fromMeta?: string;
  /** Short list preview (defaults to body snippet). */
  preview?: string;
  body: string;
  timeLabel?: string;
  /** Outbound / agent message. */
  mine?: boolean;
  avatarText?: string;
}
