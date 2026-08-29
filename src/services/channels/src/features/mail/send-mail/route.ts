import { Body, Controller, Inject, Post } from '@nestjs/common';
import { EMAIL_PROVIDER, EmailProvider } from '../../../infrastructure/email/email-provider';

/** SDD CRM-046 — transactional email (password reset, etc.). */
@Controller()
export class SendMailRoute {
  constructor(@Inject(EMAIL_PROVIDER) private readonly email: EmailProvider) {}

  @Post('api/channels/mail/send')
  async send(
    @Body() body: { to?: string; subject?: string; body?: string },
  ): Promise<{ ok: true }> {
    const to = (body?.to ?? '').trim();
    const subject = (body?.subject ?? '').trim();
    const text = (body?.body ?? '').trim();
    if (!to || !subject || !text) {
      return { ok: true };
    }
    await this.email.sendOutbound({ to, subject, body: text });
    return { ok: true };
  }
}
