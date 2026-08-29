import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { AuditLogPage } from './audit.models';

/** SDD CRM-036 / specs/051 — identity audit API via gateway. */
@Injectable({ providedIn: 'root' })
export class AuditApi {
  private readonly http = inject(HttpClient);

  list(q = '', skip = 0, take = 25, service = ''): Observable<AuditLogPage> {
    const params = new URLSearchParams();
    if (q.trim()) {
      params.set('q', q.trim());
    }
    if (service.trim()) {
      params.set('service', service.trim());
    }
    params.set('skip', String(Math.max(0, skip)));
    params.set('take', String(take));
    return this.http.get<AuditLogPage>(`/api/identity/audit?${params.toString()}`);
  }
}
