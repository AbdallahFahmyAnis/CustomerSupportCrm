import { Injectable, Logger } from '@nestjs/common';
import * as nodemailer from 'nodemailer';
import { channelsConfig } from '../../app/config';
import {
  EmailProvider,
  InboundEmailPayload,
  OutboundEmailPayload,
} from './email-provider';
import { DevEmailProvider } from './dev-email.provider';

/**
 * SDD CRM-040 — SMTP send when EMAIL_SMTP_HOST is set.
 * Inbound parsing delegates to DevEmailProvider.
 */
@Injectable()
export class SmtpEmailProvider implements EmailProvider {
  private readonly logger = new Logger(SmtpEmailProvider.name);
  private readonly parser = new DevEmailProvider();

  parseInbound(raw: unknown): InboundEmailPayload {
    return this.parser.parseInbound(raw);
  }

  async sendOutbound(payload: OutboundEmailPayload): Promise<void> {
    const host = channelsConfig.smtpHost;
    if (!host) {
      throw new Error('EMAIL_SMTP_HOST is not configured.');
    }

    const port = channelsConfig.smtpPort;
    const transporter = nodemailer.createTransport({
      host,
      port,
      secure: port === 465,
      auth:
        channelsConfig.smtpUser && channelsConfig.smtpPass
          ? {
              user: channelsConfig.smtpUser,
              pass: channelsConfig.smtpPass,
            }
          : undefined,
    });

    await transporter.sendMail({
      from: channelsConfig.smtpFrom,
      to: payload.to,
      subject: payload.subject,
      text: payload.body,
    });
    this.logger.log(`[smtp] sent to=${payload.to} ticket=${payload.ticketId}`);
  }
}
