import { Injectable, inject, signal } from '@angular/core';
import { SettingsApi } from './settings.api';
import { ErpDelivery, SystemSettings } from './settings.models';

/** SDD CRM-037 — Feature-Based + Signals store. */
@Injectable({ providedIn: 'root' })
export class SettingsStore {
  private readonly api = inject(SettingsApi);

  readonly settings = signal<SystemSettings | null>(null);
  readonly erpDeliveries = signal<ErpDelivery[]>([]);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly error = signal('');
  readonly saved = signal(false);

  load(): void {
    this.loading.set(true);
    this.error.set('');
    this.saved.set(false);
    this.api.get().subscribe({
      next: (row) => {
        this.settings.set(row);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load settings (admin required).');
        this.loading.set(false);
      },
    });
    this.loadErpDeliveries();
  }

  /** SDD CRM-039 polish / 044 */
  loadErpDeliveries(): void {
    this.api.erpDeliveries(10).subscribe({
      next: (rows) => this.erpDeliveries.set(rows ?? []),
      error: () => this.erpDeliveries.set([]),
    });
  }

  save(body: Omit<SystemSettings, 'updatedAt'>): void {
    this.saving.set(true);
    this.error.set('');
    this.saved.set(false);
    this.api.update(body).subscribe({
      next: (row) => {
        this.settings.set(row);
        this.saving.set(false);
        this.saved.set(true);
      },
      error: (err) => {
        this.error.set(err?.error?.error ?? 'Save failed.');
        this.saving.set(false);
      },
    });
  }
}
