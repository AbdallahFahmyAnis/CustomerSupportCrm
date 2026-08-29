import { Injectable, Logger } from '@nestjs/common';
import { channelsConfig } from '../../app/config';
import {
  EmailProvider,
  InboundEmailPayload,
  OutboundEmailPayload,
} from './email-provider';
import { DevEmailProvider } from './dev-email.provider';

/**
 * SDD CRM-040 — Twilio SendGrid v3 mail send when SENDGRID_API_KEY
 * (or TWILIO_SENDGRID_API_KEY) is set. Inbound parsing delegates to DevEmailProvider.
 */
@Injectable()
export class SendGridEmailProvider implements EmailProvider {
  private readonly logger = new Logger(SendGridEmailProvider.name);
  private readonly parser = new DevEmailProvider();

  parseInbound(raw: unknown): InboundEmailPayload {
    return this.parser.parseInbound(raw);
  }

  async sendOutbound(payload: OutboundEmailPayload): Promise<void> {
    const apiKey = channelsConfig.sendgridApiKey?.trim();
    if (!apiKey) {
      throw new Error(
        'Twilio SendGrid is not configured. Set SENDGRID_API_KEY (or TWILIO_SENDGRID_API_KEY) and SENDGRID_FROM.',
      );
    }

    const from = channelsConfig.sendgridFrom?.trim();
    if (!from || from === 'crm@localhost' || !from.includes('@')) {
      throw new Error(
        'SENDGRID_FROM must be a verified sender in Twilio SendGrid (e.g. support@yourdomain.com).',
      );
    }

    const res = await fetch('https://api.sendgrid.com/v3/mail/send', {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${apiKey}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        personalizations: [{ to: [{ email: payload.to }] }],
        from: { email: from },
        subject: payload.subject,
        content: [{ type: 'text/plain', value: payload.body }],
      }),
    });

    if (!res.ok) {
      const text = await res.text();
      throw new Error(`Twilio SendGrid send failed (${res.status}): ${text}`);
    }

    this.logger.log(
      `[twilio-sendgrid] sent to=${payload.to} ticket=${payload.ticketId}`,
    );
  }
}
