import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { PortalFaqDetail, PortalFaqSummary } from './faqs.models';

/** SDD CRM-029 — portal FAQs via gateway. */
@Injectable({ providedIn: 'root' })
export class FaqsApi {
  private readonly http = inject(HttpClient);

  list(q = ''): Observable<PortalFaqSummary[]> {
    const qs = q.trim() ? `?q=${encodeURIComponent(q.trim())}` : '';
    return this.http.get<PortalFaqSummary[]>(`/api/knowledge/portal/faqs${qs}`);
  }

  get(id: string): Observable<PortalFaqDetail> {
    return this.http.get<PortalFaqDetail>(`/api/knowledge/portal/faqs/${id}`);
  }
}
