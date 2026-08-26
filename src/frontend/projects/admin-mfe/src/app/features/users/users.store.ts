import { Injectable, inject, signal } from '@angular/core';
import { RoleSummary, UserSummary } from './users.models';
import { UsersApi } from './users.api';

/** SDD CRM-035 — Feature-Based + Signals store. */
@Injectable({ providedIn: 'root' })
export class UsersStore {
  private readonly api = inject(UsersApi);

  readonly users = signal<UserSummary[]>([]);
  readonly roles = signal<RoleSummary[]>([]);
  readonly loading = signal(false);
  readonly error = signal('');
  readonly query = signal('');

  loadUsers(): void {
    this.loading.set(true);
    this.error.set('');
    this.api.search(this.query()).subscribe({
      next: (rows) => {
        this.users.set(rows);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load users.');
        this.loading.set(false);
      },
    });
  }

  loadRoles(): void {
    this.api.roles().subscribe({
      next: (rows) => this.roles.set(rows),
      error: () => this.error.set('Could not load roles.'),
    });
  }

  create(
    body: {
      email: string;
      displayName: string;
      password: string;
      role: string;
      departmentId?: string | null;
      branchId?: string | null;
    },
    onDone: () => void,
    onError?: (msg: string) => void,
  ): void {
    this.error.set('');
    this.api.create(body).subscribe({
      next: () => onDone(),
      error: (err) => {
        const msg = err?.error?.error ?? 'Create failed.';
        this.error.set(msg);
        onError?.(msg);
      },
    });
  }

  setRole(id: string, role: string): void {
    this.api.updateRole(id, role).subscribe({
      next: () => this.loadUsers(),
      error: (err) => this.error.set(err?.error?.error ?? 'Role update failed.'),
    });
  }

  deactivate(id: string): void {
    this.api.deactivate(id).subscribe({
      next: () => this.loadUsers(),
      error: (err) => this.error.set(err?.error?.error ?? 'Deactivate failed.'),
    });
  }
}
