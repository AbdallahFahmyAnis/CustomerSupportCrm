import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { SubmitFeedbackBody, TicketFeedback } from './feedback.models';

/** SDD CRM-030 */
@Injectable({ providedIn: 'root' })
export class FeedbackApi {
  private readonly http = inject(HttpClient);

  submit(body: SubmitFeedbackBody): Observable<TicketFeedback> {
    return this.http.post<TicketFeedback>('/api/tickets/feedback', body);
  }
}
