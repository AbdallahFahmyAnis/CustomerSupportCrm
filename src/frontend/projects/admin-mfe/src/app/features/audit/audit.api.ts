import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { AuditLogEntry } from './audit.models';

/** SDD CRM-036 — identity audit API via gateway. */
@Injectable({ providedIn: 'root' })
export class AuditApi {
  private readonly http = inject(HttpClient);

  list(q = '', take = 100): Observable<AuditLogEntry[]> {
    const params = new URLSearchParams();
    if (q.trim()) {
      params.set('q', q.trim());
    }
    params.set('take', String(take));
    const query = params.toString();
    return this.http.get<AuditLogEntry[]>(`/api/identity/audit?${query}`);
  }
}
