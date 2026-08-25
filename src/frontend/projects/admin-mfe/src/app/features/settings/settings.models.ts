export interface SystemSettings {
  organizationName: string;
  supportEmail: string;
  defaultCulture: string;
  maxFailedLoginAttempts: number;
  lockoutMinutes: number;
  updatedAt: string;
  productTitle?: string;
  primaryColor?: string;
  logoUrl?: string;
  erpWebhookUrl?: string;
}
