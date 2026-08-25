/** SDD CRM-017 — SLA policy and evaluation DTOs. */
export interface SlaPolicy {
  priority: string;
  firstResponseMinutes: number;
  resolutionMinutes: number;
  updatedAt: string;
}

export interface SlaEvaluation {
  priority: string;
  firstResponseMinutes: number;
  resolutionMinutes: number;
  firstResponseDueAt: string;
  resolutionDueAt: string;
  firstResponseBreached: boolean;
  resolutionBreached: boolean;
  asOf: string;
}
