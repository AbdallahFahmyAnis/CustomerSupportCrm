import { Injectable, Logger } from '@nestjs/common';
import {
  ChatProvider,
  InboundChatPayload,
  OutboundChatPayload,
} from './chat-provider';

/** SDD CRM-010 — local/dev live chat provider: parse JSON inbound; log outbound. */
@Injectable()
export class DevChatProvider implements ChatProvider {
  private readonly logger = new Logger(DevChatProvider.name);

  parseInbound(raw: unknown): InboundChatPayload {
    const body = (raw ?? {}) as Record<string, unknown>;
    return {
      email: String(body.email ?? body.from ?? ''),
      body: String(body.body ?? body.message ?? body.text ?? ''),
      name: body.name != null ? String(body.name) : undefined,
      subject: body.subject != null ? String(body.subject) : undefined,
      ticketId: body.ticketId != null ? String(body.ticketId) : undefined,
      sessionId: body.sessionId != null ? String(body.sessionId) : undefined,
    };
  }

  async sendOutbound(payload: OutboundChatPayload): Promise<void> {
    this.logger.log(
      `[dev-chat] to=${payload.to} ticket=${payload.ticketId} chars=${payload.body.length}`,
    );
  }
}
