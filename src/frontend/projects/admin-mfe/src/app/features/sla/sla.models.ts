/** SDD CRM-017 / CRM-018 / CRM-019 — SLA DTOs. */
export interface SlaPolicy {
  priority: string;
  firstResponseMinutes: number;
  resolutionMinutes: number;
  updatedAt: string;
}

export interface AutoAssignRule {
  id: string;
  category?: string | null;
  priority?: string | null;
  agentId: string;
  agentName: string;
  enabled: boolean;
}

export interface EscalationSettings {
  escalateOnFirstResponseBreach: boolean;
  escalateOnResolutionBreach: boolean;
  escalateUrgentAlways: boolean;
  assignToAgentId: string;
  assignToAgentName: string;
  updatedAt: string;
}
