import { Injectable, Logger } from '@nestjs/common';
import {
  InboundSmsPayload,
  OutboundSmsPayload,
  SmsProvider,
} from './sms-provider';

/** SDD CRM-040 — local/dev SMS provider: parse JSON inbound; log outbound. */
@Injectable()
export class DevSmsProvider implements SmsProvider {
  private readonly logger = new Logger(DevSmsProvider.name);

  parseInbound(raw: unknown): InboundSmsPayload {
    const body = (raw ?? {}) as Record<string, unknown>;
    return {
      from: String(body.from ?? body.phone ?? body.msisdn ?? ''),
      body: String(body.body ?? body.message ?? body.text ?? ''),
      name: body.name != null ? String(body.name) : undefined,
      subject: body.subject != null ? String(body.subject) : undefined,
    };
  }

  async sendOutbound(payload: OutboundSmsPayload): Promise<void> {
    this.logger.log(
      `[dev-sms] to=${payload.to} ticket=${payload.ticketId} chars=${payload.body.length}`,
    );
  }
}
