import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CsatReport, SlaPerformanceReport, TicketReportSummary } from './reports.models';

/** SDD CRM-031…033 */
@Injectable({ providedIn: 'root' })
export class ReportsApi {
  private readonly http = inject(HttpClient);

  summary(from?: string, to?: string): Observable<TicketReportSummary> {
    return this.http.get<TicketReportSummary>(`/api/tickets/reports/summary${qs(from, to)}`);
  }

  slaPerformance(from?: string, to?: string): Observable<SlaPerformanceReport> {
    return this.http.get<SlaPerformanceReport>(`/api/tickets/reports/sla-performance${qs(from, to)}`);
  }

  csat(from?: string, to?: string): Observable<CsatReport> {
    return this.http.get<CsatReport>(`/api/tickets/reports/csat${qs(from, to)}`);
  }
}

function qs(from?: string, to?: string): string {
  const p = new URLSearchParams();
  if (from) p.set('from', from);
  if (to) p.set('to', to);
  const s = p.toString();
  return s ? `?${s}` : '';
}
