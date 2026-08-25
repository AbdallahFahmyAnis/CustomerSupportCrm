import { Injectable, Logger } from '@nestjs/common';
import {
  InboundWhatsAppPayload,
  OutboundWhatsAppPayload,
  WhatsAppProvider,
} from './whatsapp-provider';

/** SDD CRM-040 — local/dev WhatsApp provider: parse JSON inbound; log outbound. */
@Injectable()
export class DevWhatsAppProvider implements WhatsAppProvider {
  private readonly logger = new Logger(DevWhatsAppProvider.name);

  parseInbound(raw: unknown): InboundWhatsAppPayload {
    const body = (raw ?? {}) as Record<string, unknown>;
    return {
      from: String(body.from ?? body.phone ?? body.waId ?? ''),
      body: String(body.body ?? body.message ?? body.text ?? ''),
      name: body.name != null ? String(body.name) : undefined,
      subject: body.subject != null ? String(body.subject) : undefined,
    };
  }

  async sendOutbound(payload: OutboundWhatsAppPayload): Promise<void> {
    this.logger.log(
      `[dev-whatsapp] to=${payload.to} ticket=${payload.ticketId} chars=${payload.body.length}`,
    );
  }
}
