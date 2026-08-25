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
  /** SDD CRM-039 deferred / 048 */
  erpWebhookAuthHeader?: string;
}

/** SDD CRM-039 polish / 044 */
export interface ErpDelivery {
  ticketId: string;
  status: string;
  at: string;
}
