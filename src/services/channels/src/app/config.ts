/** Channels service configuration — SDD CRM-012 / CRM-037 / CRM-040. */
export const channelsConfig = {
  port: Number(process.env.PORT ?? 5201),
  dataPath: process.env.CHANNELS_DATA_PATH,
  /** Postgres URL, e.g. postgres://crm:Crm_Local_Pg_2026!@localhost:5432/crm_channels */
  databaseUrl: process.env.CHANNELS_DATABASE_URL,
  customersUrl: (process.env.CUSTOMERS_URL ?? 'http://localhost:5102').replace(
    /\/$/,
    '',
  ),
  ticketsUrl: (process.env.TICKETS_URL ?? 'http://localhost:5103').replace(
    /\/$/,
    '',
  ),
  smtpHost: process.env.EMAIL_SMTP_HOST,
  smtpPort: Number(process.env.EMAIL_SMTP_PORT ?? 587),
  smtpUser: process.env.EMAIL_SMTP_USER,
  smtpPass: process.env.EMAIL_SMTP_PASS,
  smtpFrom: process.env.EMAIL_SMTP_FROM ?? 'crm@localhost',
  sendgridApiKey: process.env.SENDGRID_API_KEY,
  sendgridFrom: process.env.SENDGRID_FROM ?? process.env.EMAIL_SMTP_FROM ?? 'crm@localhost',
  twilioAccountSid: process.env.TWILIO_ACCOUNT_SID,
  twilioAuthToken: process.env.TWILIO_AUTH_TOKEN,
  twilioSmsFrom: process.env.TWILIO_SMS_FROM,
  twilioWhatsAppFrom: process.env.TWILIO_WHATSAPP_FROM,
  /** Public base URL Twilio calls (gateway). Used for signature validation. */
  publicUrl: (process.env.CHANNELS_PUBLIC_URL ?? 'http://localhost:5000').replace(
    /\/$/,
    '',
  ),
};

/** SDD CRM-040 — which email adapter the factory should bind. */
export function resolveEmailProviderKind(cfg = channelsConfig): 'sendgrid' | 'smtp' | 'dev' {
  if (cfg.sendgridApiKey?.trim()) {
    return 'sendgrid';
  }
  if (cfg.smtpHost?.trim()) {
    return 'smtp';
  }
  return 'dev';
}

/** SDD CRM-040 — SMS adapter selection. */
export function resolveSmsProviderKind(cfg = channelsConfig): 'twilio' | 'dev' {
  if (
    cfg.twilioAccountSid?.trim() &&
    cfg.twilioAuthToken?.trim() &&
    cfg.twilioSmsFrom?.trim()
  ) {
    return 'twilio';
  }
  return 'dev';
}

/** SDD CRM-040 — WhatsApp adapter selection. */
export function resolveWhatsAppProviderKind(cfg = channelsConfig): 'twilio' | 'dev' {
  if (
    cfg.twilioAccountSid?.trim() &&
    cfg.twilioAuthToken?.trim() &&
    cfg.twilioWhatsAppFrom?.trim()
  ) {
    return 'twilio';
  }
  return 'dev';
}
