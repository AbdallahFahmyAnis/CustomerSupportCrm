import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  PortalRequestSummary,
  SubmitRequestBody,
  SubmitRequestResult,
} from './request.models';

/** SDD CRM-012 / CRM-027 / CRM-028 — Channels portal API via gateway. */
@Injectable({ providedIn: 'root' })
export class RequestsApi {
  private readonly http = inject(HttpClient);

  submit(body: SubmitRequestBody): Observable<SubmitRequestResult> {
    return this.http.post<SubmitRequestResult>(
      '/api/channels/intake/web-form',
      body,
    );
  }

  track(email: string): Observable<PortalRequestSummary[]> {
    return this.http.get<PortalRequestSummary[]>(
      `/api/channels/portal/requests?email=${encodeURIComponent(email.trim())}`,
    );
  }
}
