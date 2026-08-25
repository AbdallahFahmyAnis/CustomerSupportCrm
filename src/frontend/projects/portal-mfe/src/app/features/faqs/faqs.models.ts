/** SDD CRM-029 — portal FAQ models. */
export interface PortalFaqSummary {
  id: string;
  title: string;
  kind: string;
  status: string;
  updatedAt: string;
}

export interface PortalFaqDetail extends PortalFaqSummary {
  body: string;
  createdBy: string;
  createdAt: string;
}
