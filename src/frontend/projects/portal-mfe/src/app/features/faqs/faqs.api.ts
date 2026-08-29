import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { PortalFaqDetail, PortalFaqSummary } from './faqs.models';

/** SDD CRM-029 — portal FAQs via gateway. */
@Injectable({ providedIn: 'root' })
export class FaqsApi {
  private readonly http = inject(HttpClient);

  list(q = '', locale?: string): Observable<PortalFaqSummary[]> {
    const qs = new URLSearchParams();
    if (q.trim()) qs.set('q', q.trim());
    if (locale) qs.set('locale', locale);
    const suffix = qs.toString() ? `?${qs.toString()}` : '';
    return this.http.get<PortalFaqSummary[]>(`/api/knowledge/portal/faqs${suffix}`);
  }

  get(id: string): Observable<PortalFaqDetail> {
    return this.http.get<PortalFaqDetail>(`/api/knowledge/portal/faqs/${id}`);
  }
}
