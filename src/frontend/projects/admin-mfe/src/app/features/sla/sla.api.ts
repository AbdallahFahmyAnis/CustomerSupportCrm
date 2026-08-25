import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { SlaPolicy } from './sla.models';

/** SDD CRM-017 — SLA policies via gateway. */
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
}
