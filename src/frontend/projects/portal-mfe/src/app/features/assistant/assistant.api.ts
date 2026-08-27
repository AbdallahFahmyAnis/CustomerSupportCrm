import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ChatResponse } from './assistant.models';

/** SDD CRM-026 */
@Injectable({ providedIn: 'root' })
export class AssistantApi {
  private readonly http = inject(HttpClient);

  chat(message: string, sessionId?: string): Observable<ChatResponse> {
    return this.http.post<ChatResponse>('/api/ai/chat', {
      message,
      sessionId: sessionId || undefined,
    });
  }
}
