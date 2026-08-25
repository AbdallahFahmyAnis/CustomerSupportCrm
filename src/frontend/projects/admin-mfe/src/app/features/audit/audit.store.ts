import { Injectable, inject, signal } from '@angular/core';
import { AuditApi } from './audit.api';
import { AuditLogEntry } from './audit.models';

/** SDD CRM-036 — Feature-Based + Signals store. */
@Injectable({ providedIn: 'root' })
export class AuditStore {
  private readonly api = inject(AuditApi);

  readonly entries = signal<AuditLogEntry[]>([]);
  readonly loading = signal(false);
  readonly error = signal('');
  readonly query = signal('');

  load(): void {
    this.loading.set(true);
    this.error.set('');
    this.api.list(this.query()).subscribe({
      next: (rows) => {
        this.entries.set(rows);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load audit log (admin required).');
        this.loading.set(false);
      },
    });
  }
}
