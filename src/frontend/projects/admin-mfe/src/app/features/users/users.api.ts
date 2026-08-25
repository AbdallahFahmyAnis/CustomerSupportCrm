import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { RoleSummary, UserSummary } from './users.models';

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

  updateRolePermissions(roleName: string, permissions: string[]): Observable<RoleSummary> {
    return this.http.put<RoleSummary>(
      `/api/identity/roles/${encodeURIComponent(roleName)}/permissions`,
      { permissions },
    );
  }

  permissions(): Observable<{ permissions: string[] }> {
    return this.http.get<{ permissions: string[] }>('/api/identity/permissions');
  }

  createPermission(name: string, description?: string): Observable<{ name: string }> {
    return this.http.post<{ name: string }>('/api/identity/permissions', {
      name,
      description,
    });
  }

  updatePermission(
    currentName: string,
    name: string,
    description?: string,
  ): Observable<{ name: string }> {
    return this.http.put<{ name: string }>(
      `/api/identity/permissions/${encodeURIComponent(currentName)}`,
      { name, description },
    );
  }

  deletePermission(name: string): Observable<void> {
    return this.http.delete<void>(`/api/identity/permissions/${encodeURIComponent(name)}`);
  }
}
