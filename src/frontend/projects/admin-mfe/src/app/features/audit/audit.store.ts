import { Injectable, inject, signal } from '@angular/core';
import { AuditApi } from './audit.api';
import { AuditLogDetail, AuditLogEntry } from './audit.models';

/** SDD CRM-036 / specs/051 — Feature-Based + Signals store. */
@Injectable({ providedIn: 'root' })
export class AuditStore {
  private readonly api = inject(AuditApi);

  readonly entries = signal<AuditLogEntry[]>([]);
  readonly total = signal(0);
  readonly skip = signal(0);
  readonly take = signal(25);
  readonly loading = signal(false);
  readonly error = signal('');
  readonly query = signal('');
  readonly service = signal('');

  load(): void {
    this.loading.set(true);
    this.error.set('');
    this.api.list(this.query(), this.skip(), this.take(), this.service()).subscribe({
      next: (page) => {
        this.entries.set(page.items ?? []);
        this.total.set(page.total ?? 0);
        this.skip.set(page.skip ?? 0);
        this.take.set(page.take ?? 25);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('auditLoadFailed');
        this.loading.set(false);
      },
    });
  }

  goToPage(pageIndex: number): void {
    const take = this.take();
    this.skip.set(Math.max(0, pageIndex) * take);
    this.load();
  }

  pageIndex(): number {
    const take = this.take() || 25;
    return Math.floor(this.skip() / take);
  }

  pageCount(): number {
    const take = this.take() || 25;
    return Math.max(1, Math.ceil(this.total() / take));
  }

  loadDetail(id: string, onError?: (msg: string) => void): void {
    this.loading.set(true);
    this.error.set('');
    this.api.get(id).subscribe({
      next: (row) => {
        this.selected.set(row);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('auditDetailLoadFailed');
        this.selected.set(null);
        this.loading.set(false);
        onError?.('auditDetailLoadFailed');
      },
    });
  }

  readonly selected = signal<AuditLogDetail | null>(null);
}
