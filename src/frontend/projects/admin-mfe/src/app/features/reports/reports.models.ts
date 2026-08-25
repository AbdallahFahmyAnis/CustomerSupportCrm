/** SDD CRM-031…034 — management reports. */
export interface ReportBucket {
  key: string;
  count: number;
}

export interface ReportAgentBucket {
  agentId?: string | null;
  agentName?: string | null;
  count: number;
}

export interface TicketReportSummary {
  from: string;
  to: string;
  created: number;
  open: number;
  resolvedOrClosed: number;
  escalated: number;
  byStatus: ReportBucket[];
  byCategory: ReportBucket[];
  byPriority: ReportBucket[];
  byAgent: ReportAgentBucket[];
}

export interface SlaAgentPerformance {
  agentId?: string | null;
  agentName?: string | null;
  ticketCount: number;
  resolutionBreached: number;
}

export interface SlaPerformanceReport {
  from: string;
  to: string;
  ticketCount: number;
  resolutionBreached: number;
  breachPercent: number;
  byAgent: SlaAgentPerformance[];
}

export interface CsatDistributionBucket {
  rating: number;
  count: number;
}

export interface CsatAgentBucket {
  agentId?: string | null;
  agentName?: string | null;
  count: number;
  averageRating: number;
}

export interface CsatReport {
  from: string;
  to: string;
  count: number;
  averageRating: number;
  distribution: CsatDistributionBucket[];
  byAgent: CsatAgentBucket[];
}
