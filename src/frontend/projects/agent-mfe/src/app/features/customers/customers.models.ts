/** SDD CRM-001 / specs/002-customer-profiles */
export interface CustomerSummary {
  id: string;
  displayName: string;
  organization?: string | null;
  status: string;
  uniqueIdentifier: string;
}

export interface Contact {
  id: string;
  type: string;
  value: string;
  isPrimary: boolean;
  isActive: boolean;
}

export interface Note {
  id: string;
  body: string;
  authorName: string;
  createdAt: string;
}

export interface Attachment {
  id: string;
  fileName: string;
  contentType: string;
  sizeBytes: number;
  createdAt: string;
}

export interface TimelineItem {
  id: string;
  kind: string;
  summary: string;
  occurredAt: string;
}

export interface CustomerDetail extends CustomerSummary {
  contacts: Contact[];
  notes: Note[];
  attachments: Attachment[];
  timeline: TimelineItem[];
}

export interface DuplicateWarning {
  message: string;
  existingCustomerId: string;
}
