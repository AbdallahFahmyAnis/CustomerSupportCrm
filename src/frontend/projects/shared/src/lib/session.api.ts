import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { tap } from 'rxjs';

export interface Session {
  authenticated: boolean;
  id?: string;
  name?: string;
  email?: string;
  role?: string;
}

@Injectable({ providedIn: 'root' })
export class SessionApi {
  private readonly http = inject(HttpClient);
  readonly session = signal<Session | null>(null);

  load() {
    return this.http.get<Session>('/api/session').pipe(
      tap((value) => this.session.set(value.authenticated ? value : null)),
    );
  }

  login(email: string, password: string) {
    return this.http.post<Session>('/login', { email, password }).pipe(
      tap((value) => this.session.set({ ...value, authenticated: true })),
    );
  }

  logout() {
    return this.http.post('/logout', {}).pipe(tap(() => this.session.set(null)));
  }
}
