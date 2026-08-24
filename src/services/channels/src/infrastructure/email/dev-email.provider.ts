import { Injectable } from '@nestjs/common';
import {
  EmailProvider,
  InboundEmailPayload,
} from './email-provider';

/** SDD CRM-040 — local/dev provider: accepts plain JSON { from, subject, body, name? }. */
@Injectable()
export class DevEmailProvider implements EmailProvider {
  parseInbound(raw: unknown): InboundEmailPayload {
    const body = (raw ?? {}) as Record<string, unknown>;
    return {
      from: String(body.from ?? body.email ?? ''),
      subject: String(body.subject ?? ''),
      body: String(body.body ?? body.message ?? ''),
      name: body.name != null ? String(body.name) : undefined,
    };
  }
}
