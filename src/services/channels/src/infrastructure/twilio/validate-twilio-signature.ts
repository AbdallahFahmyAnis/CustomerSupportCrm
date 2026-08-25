import { createHmac, timingSafeEqual } from 'crypto';

/**
 * SDD CRM-040 — Twilio request validation.
 * https://www.twilio.com/docs/usage/security#validating-requests
 */
export function validateTwilioSignature(options: {
  authToken?: string | null;
  url: string;
  params: Record<string, string>;
  signature?: string | null;
}): { ok: boolean; bypassed: boolean } {
  const token = options.authToken?.trim() ?? '';
  if (!token) {
    return { ok: true, bypassed: true };
  }

  const signature = options.signature?.trim() ?? '';
  if (!signature) {
    return { ok: false, bypassed: false };
  }

  const data =
    options.url +
    Object.keys(options.params)
      .sort()
      .map((key) => key + (options.params[key] ?? ''))
      .join('');

  const expected = createHmac('sha1', token).update(Buffer.from(data, 'utf8')).digest('base64');
  try {
    const a = Buffer.from(expected);
    const b = Buffer.from(signature);
    if (a.length !== b.length) {
      return { ok: false, bypassed: false };
    }
    return { ok: timingSafeEqual(a, b), bypassed: false };
  } catch {
    return { ok: false, bypassed: false };
  }
}

/** Build the exact URL Twilio signed (public gateway URL + path). */
export function buildTwilioWebhookUrl(publicBase: string, path: string): string {
  const base = publicBase.replace(/\/$/, '');
  const p = path.startsWith('/') ? path : `/${path}`;
  return `${base}${p}`;
}
