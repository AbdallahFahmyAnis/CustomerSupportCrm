import { Injectable, Logger } from '@nestjs/common';
import { channelsConfig } from '../../app/config';
import {
  InboundSmsPayload,
  OutboundSmsPayload,
  SmsProvider,
} from './sms-provider';
import { DevSmsProvider } from './dev-sms.provider';

/**
 * SDD CRM-040 — Twilio Messages API for SMS when TWILIO_* env is set.
 * Inbound parsing delegates to DevSmsProvider.
 */
@Injectable()
export class TwilioSmsProvider implements SmsProvider {
  private readonly logger = new Logger(TwilioSmsProvider.name);
  private readonly parser = new DevSmsProvider();

  parseInbound(raw: unknown): InboundSmsPayload {
    return this.parser.parseInbound(raw);
  }

  async sendOutbound(payload: OutboundSmsPayload): Promise<void> {
    const sid = channelsConfig.twilioAccountSid?.trim();
    const token = channelsConfig.twilioAuthToken?.trim();
    const from = channelsConfig.twilioSmsFrom?.trim();
    if (!sid || !token || !from) {
      throw new Error('Twilio SMS env (ACCOUNT_SID, AUTH_TOKEN, SMS_FROM) is not configured.');
    }

    const body = new URLSearchParams({
      To: payload.to,
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
      throw new Error(`Twilio SMS send failed (${res.status}): ${text}`);
    }

    this.logger.log(`[twilio-sms] sent to=${payload.to} ticket=${payload.ticketId}`);
  }
}
