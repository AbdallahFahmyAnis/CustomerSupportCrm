import { Injectable, Logger } from '@nestjs/common';
import { channelsConfig } from '../../app/config';
import {
  InboundWhatsAppPayload,
  OutboundWhatsAppPayload,
  WhatsAppProvider,
} from './whatsapp-provider';
import { DevWhatsAppProvider } from './dev-whatsapp.provider';

/**
 * SDD CRM-040 — Twilio WhatsApp send when TWILIO_WHATSAPP_FROM is set.
 * Inbound parsing delegates to DevWhatsAppProvider.
 */
@Injectable()
export class TwilioWhatsAppProvider implements WhatsAppProvider {
  private readonly logger = new Logger(TwilioWhatsAppProvider.name);
  private readonly parser = new DevWhatsAppProvider();

  parseInbound(raw: unknown): InboundWhatsAppPayload {
    return this.parser.parseInbound(raw);
  }

  async sendOutbound(payload: OutboundWhatsAppPayload): Promise<void> {
    const sid = channelsConfig.twilioAccountSid?.trim();
    const token = channelsConfig.twilioAuthToken?.trim();
    const from = channelsConfig.twilioWhatsAppFrom?.trim();
    if (!sid || !token || !from) {
      throw new Error(
        'Twilio WhatsApp env (ACCOUNT_SID, AUTH_TOKEN, WHATSAPP_FROM) is not configured.',
      );
    }

    const to = payload.to.startsWith('whatsapp:')
      ? payload.to
      : `whatsapp:${payload.to}`;

    const body = new URLSearchParams({
      To: to,
      From: from,
      Body: payload.body,
    });

    const auth = Buffer.from(`${sid}:${token}`).toString('base64');
    const res = await fetch(
      `https://api.twilio.com/2010-04-01/Accounts/${sid}/Messages.json`,
      {
        method: 'POST',
        headers: {
          Authorization: `Basic ${auth}`,
          'Content-Type': 'application/x-www-form-urlencoded',
        },
        body,
      },
    );

    if (!res.ok) {
      const text = await res.text();
      let hint = '';
      if (text.includes('63007')) {
        hint =
          ' Activate WhatsApp Sandbox in Twilio Console (Messaging → Try it out → WhatsApp), use Live Account SID/Auth Token (not Test), From must be whatsapp:+14155238886, and the recipient must join the sandbox.';
      }
      throw new Error(
        `Twilio WhatsApp send failed (${res.status}): ${text}.${hint}`,
      );
    }

    this.logger.log(
      `[twilio-whatsapp] sent to=${to} ticket=${payload.ticketId}`,
    );
  }
}
