import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { RoleSummary, UserSummary } from './user.models';

/** SDD CRM-035 — identity admin API via gateway. */
@Injectable({ providedIn: 'root' })
export class UsersApi {
  private readonly http = inject(HttpClient);

  search(q = ''): Observable<UserSummary[]> {
    const query = q.trim() ? `?q=${encodeURIComponent(q.trim())}` : '';
    return this.http.get<UserSummary[]>(`/api/identity/users${query}`);
  }

  create(body: {
    email: string;
    displayName: string;
    password: string;
    role: string;
  }): Observable<UserSummary> {
    return this.http.post<UserSummary>('/api/identity/users', body);
  }

  updateRole(id: string, role: string): Observable<UserSummary> {
    return this.http.post<UserSummary>(`/api/identity/users/${id}/role`, { role });
  }

  deactivate(id: string): Observable<UserSummary> {
    return this.http.post<UserSummary>(`/api/identity/users/${id}/deactivate`, {});
  }

  roles(): Observable<RoleSummary[]> {
    return this.http.get<RoleSummary[]>('/api/identity/roles');
  }
}
