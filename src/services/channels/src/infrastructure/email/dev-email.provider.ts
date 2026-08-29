import { Injectable, Logger } from '@nestjs/common';
import {
  EmailProvider,
  InboundEmailPayload,
  OutboundEmailPayload,
} from './email-provider';

/** SDD CRM-040 — local/dev provider: parse JSON inbound; log outbound. */
@Injectable()
export class DevEmailProvider implements EmailProvider {
  private readonly logger = new Logger(DevEmailProvider.name);

  parseInbound(raw: unknown): InboundEmailPayload {
    const body = (raw ?? {}) as Record<string, unknown>;
    return {
      from: String(body.from ?? body.email ?? ''),
      subject: String(body.subject ?? ''),
      body: String(body.body ?? body.message ?? ''),
      name: body.name != null ? String(body.name) : undefined,
    };
  }

  async sendOutbound(payload: OutboundEmailPayload): Promise<void> {
    this.logger.log(
      `[dev-email] to=${payload.to} ticket=${payload.ticketId ?? '-'} subject=${payload.subject}`,
    );
  }
}
