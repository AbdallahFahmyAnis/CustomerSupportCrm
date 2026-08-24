import { Injectable, inject, signal } from '@angular/core';
import { RoleSummary } from '../users/users.models';
import { UsersApi } from '../users/users.api';

/** SDD CRM-035 — roles feature store (signals). */
@Injectable({ providedIn: 'root' })
export class RolesStore {
  private readonly api = inject(UsersApi);
  readonly roles = signal<RoleSummary[]>([]);
  readonly error = signal('');

  load(): void {
    this.error.set('');
    this.api.roles().subscribe({
      next: (rows) => this.roles.set(rows),
      error: () => this.error.set('Could not load roles.'),
    });
  }
}
