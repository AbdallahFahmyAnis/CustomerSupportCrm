import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

/** SDD CRM-020 — in-app notification DTO. */
export interface CrmNotification {
  id: string;
  userId: string;
  title: string;
  body: string;
  kind: string;
  href?: string;
  createdAt: string;
  readAt: string | null;
}

/** SDD CRM-020 — notifications via gateway. */
@Injectable({ providedIn: 'root' })
export class NotificationsApi {
  private readonly http = inject(HttpClient);

  list(): Observable<CrmNotification[]> {
    return this.http.get<CrmNotification[]>('/api/notifications');
  }

  unreadCount(): Observable<{ count: number }> {
    return this.http.get<{ count: number }>('/api/notifications/unread-count');
  }

  markRead(id: string): Observable<CrmNotification> {
    return this.http.post<CrmNotification>(`/api/notifications/${id}/read`, {});
  }
}
