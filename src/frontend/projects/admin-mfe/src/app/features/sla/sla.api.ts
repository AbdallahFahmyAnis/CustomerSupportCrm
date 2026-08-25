import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { AutoAssignRule, EscalationSettings, SlaPolicy } from './sla.models';

/** SDD CRM-017 / CRM-018 / CRM-019 — SLA via gateway. */
@Injectable({ providedIn: 'root' })
export class SlaApi {
  private readonly http = inject(HttpClient);

  list(): Observable<SlaPolicy[]> {
    return this.http.get<SlaPolicy[]>('/api/sla/policies');
  }

  update(priority: string, firstResponseMinutes: number, resolutionMinutes: number): Observable<SlaPolicy> {
    return this.http.put<SlaPolicy>(`/api/sla/policies/${encodeURIComponent(priority)}`, {
      firstResponseMinutes,
      resolutionMinutes,
    });
  }

  listAssignRules(): Observable<AutoAssignRule[]> {
    return this.http.get<AutoAssignRule[]>('/api/sla/assign-rules');
  }

  replaceAssignRules(rules: AutoAssignRule[]): Observable<AutoAssignRule[]> {
    return this.http.put<AutoAssignRule[]>('/api/sla/assign-rules', { rules });
  }

  getEscalationSettings(): Observable<EscalationSettings> {
    return this.http.get<EscalationSettings>('/api/sla/escalation-settings');
  }

  updateEscalationSettings(body: Omit<EscalationSettings, 'updatedAt'>): Observable<EscalationSettings> {
    return this.http.put<EscalationSettings>('/api/sla/escalation-settings', body);
  }
}
