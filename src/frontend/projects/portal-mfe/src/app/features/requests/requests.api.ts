import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  ChannelMessageDto,
  PortalRequestSummary,
  SubmitRequestBody,
  SubmitRequestResult,
} from './requests.models';

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

  /** SDD CRM-010 — start or continue a live chat (returns ticket ids). */
  chat(body: {
    name: string;
    email: string;
    body: string;
    ticketId?: string;
  }): Observable<SubmitRequestResult> {
    return this.http.post<SubmitRequestResult>('/api/channels/intake/chat', body);
  }

  /** SDD CRM-010 / CRM-012 — poll ticket thread so portal sees agent replies. */
  listMessages(ticketId: string): Observable<ChannelMessageDto[]> {
    return this.http.get<ChannelMessageDto[]>(
      `/api/channels/tickets/${encodeURIComponent(ticketId)}/messages`,
    );
  }

  track(email: string): Observable<PortalRequestSummary[]> {
    return this.http.get<PortalRequestSummary[]>(
      `/api/channels/portal/requests?email=${encodeURIComponent(email.trim())}`,
    );
  }
}
